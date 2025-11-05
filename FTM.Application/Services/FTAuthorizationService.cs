using AutoMapper;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Models;
using FTM.Domain.Specification;
using FTM.Domain.Specification.FTAuthorizations;
using FTM.Domain.Specification.FTMembers;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAct;

namespace FTM.Application.Services
{
    public class FTAuthorizationService : IFTAuthorizationService
    {
        private readonly IGenericRepository<FamilyTree> _familyTreeRepository;
        private readonly IFTMemberService _fTMemberRepository;
        private readonly IFTAuthorizationRepository _fTAuthorizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FTAuthorizationService(
            IGenericRepository<FamilyTree> familyTreeRepository,
            IFTMemberService fTMemberRepository,
            IFTAuthorizationRepository fTAuthorizationRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _familyTreeRepository = familyTreeRepository;
            _fTMemberRepository = fTMemberRepository;
            _fTAuthorizationRepository = fTAuthorizationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FTAuthorizationDto?> AddAsync(UpsertFTAuthorizationRequest request)
        {
            if (request.AuthorizationList == null || request.AuthorizationList.Count == 0)
                throw new ArgumentException("Quyền không hợp lệ");

            if ((await _familyTreeRepository.GetByIdAsync(request.FTId)) is null)
                throw new ArgumentException($"Không tồn tại gia phả với id {request.FTId}");

            await _fTMemberRepository.GetByMemberId(request.FTId, request.FTMemberId);

            foreach (var authorization in request.AuthorizationList)
            {
                foreach (var method in authorization.MethodsList)
                {
                    if (!await _fTAuthorizationRepository.IsAuthorizationExisting(request.FTId, request.FTMemberId,
                                                                                authorization.FeatureCode, method))
                    {
                        var newAuthorization = new FTAuthorization
                        {
                            FTId = request.FTId,
                            FTMemberId = request.FTMemberId,
                            FeatureCode = authorization.FeatureCode,
                            MethodCode = method
                        };
                        
                        await _fTAuthorizationRepository.AddAsync(newAuthorization);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }

            // Clean Authorization
            // End Clean Authorization
            var ftAuthorizationDto = await _fTAuthorizationRepository.GetAuthorizationAsync(request.FTId, request.FTMemberId);
            return ftAuthorizationDto;
        }

        public Task<bool> DeleteAsync(Guid authorizationId)
        {
            throw new NotImplementedException();
        }

        public async Task<FTAuthorizationListViewDto?> GetAuthorizationListViewAsync(FTAuthorizationSpecParams specParams)
        {
            var spec = new FTAuthorizationSpecification(specParams);
            var authorList = await _fTAuthorizationRepository.ListAsync(spec);
            var result = authorList
                                .GroupBy(a => a.FTId)
                                .Select(ftGroup => new FTAuthorizationListViewDto
                                {
                                    FTId = ftGroup.Key,
                                    Datalist = ftGroup
                                        .GroupBy(a => a.AuthorizedMember)
                                        .Select(memberGroup => new KeyValueModel
                                        {
                                            Key = new {
                                                memberGroup.Key.Id,
                                                memberGroup.Key.Fullname
                                            }, 
                                            Value = memberGroup
                                                .GroupBy(a => a.FeatureCode)
                                                .Select(featureGroup => new AuthorizationModel
                                                {
                                                    FeatureCode = featureGroup.Key,
                                                    MethodsList = featureGroup
                                                        .Select(x => x.MethodCode)
                                                        .Distinct()
                                                        .ToHashSet()
                                                })
                                                .ToList()
                                        })
                                        .ToList()
                                })
                                .FirstOrDefault();

            return result;
        }

        public async Task<FTAuthorizationDto?> UpdateAsync(UpsertFTAuthorizationRequest request)
        {
            if (request.AuthorizationList == null || request.AuthorizationList.Count == 0)
                throw new ArgumentException("Quyền không hợp lệ");

            if ((await _familyTreeRepository.GetByIdAsync(request.FTId)) is null)
                throw new ArgumentException($"Không tồn tại gia phả với id {request.FTId}");

            await _fTMemberRepository.GetByMemberId(request.FTId, request.FTMemberId);

            // Delete old Authorization List
            var oldAuthorList = await _fTAuthorizationRepository.GetListAsync(request.FTId, request.FTMemberId);
            
            foreach( var oldAuthor in oldAuthorList)
            {
                _fTAuthorizationRepository.Delete(oldAuthor);
            }
            // End delete old Authorization List

            return await AddAsync(request);
        }
    }
}
