using AutoMapper;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Models;
using FTM.Domain.Specification;
using FTM.Domain.Specification.FTAuthorizations;
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

        public async Task<FTAuthorizationDto> AddAsync(UpsertFTAuthorizationRequest request)
        {
            FTAuthorizationDto result = new FTAuthorizationDto()
            {
                FTId = request.FTId,
                FTMemberId = request.FTMemberId,
                AuthorizationList = new HashSet<AuthorizationModel>(),
            };

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

            return _mapper.Map<FTAuthorizationDto>(result);
        }

        public Task<bool> DeleteAsync(Guid authorizationId)
        {
            throw new NotImplementedException();
        }

        public Task<FTAuthorizationListViewDto> GetAuthorizationListViewAsync(FTAuthorizationSpecParams specParams)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<FTAuthorizationDto>> GetListDetailsWithSpecificationAsync(FTAuthorizationSpecParams specParams)
        {
            throw new NotImplementedException();
        }

        public Task<FTAuthorizationDto> UpdateAsync(UpsertFTAuthorizationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
