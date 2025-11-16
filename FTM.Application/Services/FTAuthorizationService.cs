using AutoMapper;
using AutoMapper.Execution;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
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
        private readonly IFTMemberRepository _fTMemberRepository;
        private readonly IFTAuthorizationRepository _fTAuthorizationRepository;
        private readonly IFTUserRepository _fTUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FTAuthorizationService(
            IGenericRepository<FamilyTree> familyTreeRepository,
            IFTMemberRepository fTMemberRepository,
            IFTAuthorizationRepository fTAuthorizationRepository,
            IFTUserRepository fTUserRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _familyTreeRepository = familyTreeRepository;
            _fTMemberRepository = fTMemberRepository;
            _fTAuthorizationRepository = fTAuthorizationRepository;
            _fTUserRepository = fTUserRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FTAuthorizationDto?> AddAsync(UpsertFTAuthorizationRequest request)
        {
            if (request.AuthorizationList == null || request.AuthorizationList.Count == 0)
                throw new ArgumentException("Quyền không hợp lệ");

            if ((await _familyTreeRepository.GetByIdAsync(request.FTId)) is null)
                throw new ArgumentException($"Không tồn tại gia phả với id {request.FTId}");

            if (!await _fTMemberRepository.IsExisted(request.FTId, request.FTMemberId))
                throw new ArgumentException($"Không tồn tại thành viên");

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
                                        .OrderBy(a => a.AuthorizedMember.Fullname)
                                        .GroupBy(a => a.AuthorizedMember)
                                        .Select(memberGroup => new KeyValueModel
                                        {
                                            Key = new {
                                                memberGroup.Key.Id,
                                                memberGroup.Key.Fullname,
                                                Avatar = memberGroup.Key.FTMemberFiles.FirstOrDefault()?.FilePath
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
                                        .Skip(specParams.Skip)
                                        .Take(specParams.Take)
                                        .ToList()
                                })
                                .FirstOrDefault();

            return result;
        }

        public async Task<int> CountAuthorizationListViewAsync(FTAuthorizationSpecParams specParams)
        {
            //var spec = new FTAuthorizationSpecificationForCount(specParams);
            //var authorList = await _fTAuthorizationRepository.ListAsync(spec);

            var spec = new FTAuthorizationSpecification(specParams);
            var authorList = await _fTAuthorizationRepository.ListAsync(spec);


            var result = authorList
                                .GroupBy(a => a.FTId)
                                .Select(ftGroup => new FTAuthorizationListViewDto
                                {
                                    FTId = ftGroup.Key,
                                    Datalist = ftGroup
                                        .OrderBy(a => a.AuthorizedMember.Fullname)
                                        .GroupBy(a => a.AuthorizedMember)
                                        .Select(memberGroup => new KeyValueModel
                                        {
                                            Key = new
                                            {
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
                                        .Skip(specParams.Skip)
                                        .Take(specParams.Take)
                                        .ToList()
                                })
                                .FirstOrDefault();

            if (result == null) return 0;

            return result.Datalist.Count;
        }

        public async Task<bool> HasPermissionAsync(Guid ftId, Guid userId, FeatureType feature, MethodType method)
        {
            return await _fTAuthorizationRepository.HasPermissionAsync(ftId, userId, feature, method);
        }

        public async Task<bool> IsAccessedToFamilyTreeAsync(Guid ftId, Guid userId)
        {
            return await _fTUserRepository.BelongedToAsync(ftId, userId);
        }

        public async Task<bool> IsOwnerAsync(Guid ftId, Guid userId)
        {
            return await _fTUserRepository.IsOwnerAsync(ftId, userId);  
        }

        public async Task SetMemberAuthorizationAsync(Guid ftId, Guid memberId)
        {
            var memberAuthor = new UpsertFTAuthorizationRequest
            {
                FTId = ftId,
                FTMemberId = memberId,
                AuthorizationList = new HashSet<AuthorizationModel>()
            };

            memberAuthor.AuthorizationList.Add(new AuthorizationModel
            {
                FeatureCode = FeatureType.MEMBER,
                MethodsList = new HashSet<MethodType>
                                   {
                                        MethodType.VIEW,
                                   }
            });

            memberAuthor.AuthorizationList.Add(new AuthorizationModel
            {
                FeatureCode = FeatureType.EVENT,
                MethodsList = new HashSet<MethodType>
                                   {
                                        MethodType.VIEW,
                                   }
            });

            memberAuthor.AuthorizationList.Add(new AuthorizationModel
            {
                FeatureCode = FeatureType.FUND,
                MethodsList = new HashSet<MethodType>
                                   {
                                        MethodType.VIEW,
                                   }
            });

            foreach (var authorization in memberAuthor.AuthorizationList)
            {
                foreach (var method in authorization.MethodsList)
                {
                    if (!await _fTAuthorizationRepository.IsAuthorizationExisting(memberAuthor.FTId, memberAuthor.FTMemberId,
                                                                                authorization.FeatureCode, method))
                    {
                        var newAuthorization = new FTAuthorization
                        {
                            FTId = memberAuthor.FTId,
                            FTMemberId = memberAuthor.FTMemberId,
                            FeatureCode = authorization.FeatureCode,
                            MethodCode = method
                        };

                        await _fTAuthorizationRepository.AddAsync(newAuthorization);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }
        }

        public async Task<FTAuthorizationDto?> UpdateAsync(UpsertFTAuthorizationRequest request)
        {
            await DeleteAuthorizationAsync(request.FTId, request.FTMemberId);
            return await AddAsync(request);
        }

        public async Task DeleteAuthorizationAsync(Guid ftId, Guid ftMemberId)
        {
            if ((await _familyTreeRepository.GetByIdAsync(ftId)) is null)
                throw new ArgumentException($"Không tồn tại gia phả với id {ftId}");

            if (!await _fTMemberRepository.IsExisted(ftId, ftMemberId))
                throw new ArgumentException($"Không tồn tại thành viên");

            // Delete old Authorization List
            var oldAuthorList = await _fTAuthorizationRepository.GetListAsync(ftId, ftMemberId);

            foreach (var oldAuthor in oldAuthorList)
            {
                _fTAuthorizationRepository.Delete(oldAuthor);
            }

            await _unitOfWork.CompleteAsync();
            // End delete old Authorization List
        }

        public async Task<bool> IsGuestAsync(Guid ftId, Guid userId)
        {
            return await _fTUserRepository.IsGuestAsync(ftId, userId);
        }

        public async Task<FTAuthorizationDto> GetAuthorizationAsync(Guid ftId, Guid ftMemberId)
        {
            var ftAuthorizationDto = await _fTAuthorizationRepository.GetAuthorizationAsync(ftId, ftMemberId);

            if(ftAuthorizationDto is null)
            {
                ftAuthorizationDto = new FTAuthorizationDto() { FTId = ftId, FTMemberId = ftMemberId };
            }


            return ftAuthorizationDto;
        }
    }
}
