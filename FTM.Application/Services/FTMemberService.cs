﻿using AutoMapper;
using AutoMapper.Execution;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FTM.Domain.Constants.Constants;

namespace FTM.Application.Services
{
    public class FTMemberService : IFTMemberService
    {
        private readonly IGenericRepository<FamilyTree> _familyTreeRepository;
        private readonly IFTMemberRepository _fTMemberRepository;
        private readonly IGenericRepository<FTRelationship> _fTRelationshipRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FTMemberService(
            IGenericRepository<FamilyTree> familyTreeRepository,
            IFTMemberRepository FTMemberRepository,
            IGenericRepository<FTRelationship> FTRelationshipRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _familyTreeRepository = familyTreeRepository;
            _fTMemberRepository = FTMemberRepository;
            _fTRelationshipRepository = FTRelationshipRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<FTMemberDetailsDto> Add(Guid FTId, UpsertFTMemberRequest request)
        {
            FTMember newFMember = new FTMember();
            var familyTree = await _familyTreeRepository.GetByIdAsync(FTId);

            FTMember rootOld = null;
            if (request.RootId != null)
            {
                rootOld = await _fTMemberRepository.GetByIdAsync(request.RootId.GetValueOrDefault());
                if (rootOld == null)
                {
                    throw new ArgumentException("Không tìm thấy thành viên được thêm mối quan hệ.");
                }
            }

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();
            var ftMember = _mapper.Map<FTMember>(request);
            ftMember.FTRole = FTMRole.FTMember;
            // Dummy For member file
            //ftMember.FTMemberFiles.First().FilePath = "dummyFilePath";

            await executionStrategy.ExecuteAsync(
                   async () =>
                   {
                       using (var transaction = await _unitOfWork.BeginTransactionAsync())
                       {
                           try
                           {
                               if (!familyTree.FTMembers.Any())
                               {
                                   ftMember.IsRoot = true;
                               }

                               switch (request.CategoryCode)
                               {
                                   case FTRelationshipCategory.PARENT:
                                       await AddParentMember(rootOld, ftMember, request);
                                       break;
                                   case FTRelationshipCategory.SIBLING:
                                       await AddSiblingMember(rootOld, ftMember, request);
                                       break;
                                   case FTRelationshipCategory.PARTNER:
                                       await AddPartnerMember(rootOld, ftMember, request);
                                       break;
                                   case FTRelationshipCategory.CHILDREN:
                                       await AddChildrenMember(rootOld, ftMember, request);
                                       break;
                                   default:
                                       await _fTMemberRepository.AddAsync(ftMember);
                                       break;
                               }

                               //----------------Handle MemberFile Entity---------------------
                               //----------------End Handle MemberFile Entity---------------------
                               await _unitOfWork.CompleteAsync();
                               newFMember = await _fTMemberRepository.GetDetaildedById(ftMember.Id);
                               await transaction.CommitAsync();
                           }
                           catch (Exception ex)
                           {
                               await transaction.RollbackAsync();
                               throw;
                           }
                       }
                   }
                );
            return _mapper.Map<FTMemberDetailsDto>(newFMember);
        }

        private async Task AddChildrenMember(FTMember? rootOld, FTMember ftMember, UpsertFTMemberRequest request)
        {
            await _fTMemberRepository.AddAsync(ftMember);

            var relationShipChild = new FTRelationship()
            {
                FromFTMemberId = request.FromFTMemberId.Value,
                FromFTMemberPartnerId = request.FromFTMemberPartnerId,
                ToFTMemberId = ftMember.Id,
                CategoryCode = FTRelationshipCategory.CHILDREN
            };

            if (relationShipChild.FromFTMemberId == null)
            {
                throw new ArgumentException("Cha hoặc mẹ là thành viên bắt buộc.");
            }

            if (relationShipChild.FromFTMemberPartnerId == null)
            {
                var partnerUndefined = new FTMember()
                {
                    StatusCode = FTMemberStatus.UNDEFINED,
                    FTId = request.FTId,
                    Fullname = $"Bố hoặc mẹ của {request.Fullname}",
                    IsRoot = false,
                    FTRole = FTMRole.FTMember,
                    Gender = request.Gender == 1 ? 0 : 1,
                };

                await _fTMemberRepository.AddAsync(partnerUndefined);

                var relationshipPartnerUndefined = new FTRelationship()
                {
                    FromFTMemberId = relationShipChild.FromFTMemberId,
                    ToFTMemberId = partnerUndefined.Id,
                    CategoryCode = FTRelationshipCategory.PARTNER,
                };

                relationShipChild.FromFTMemberPartnerId = partnerUndefined.Id;

                await _fTRelationshipRepository.AddAsync(relationshipPartnerUndefined);
            }
            await _fTRelationshipRepository.AddAsync(relationShipChild);
        }

        private async Task AddPartnerMember(FTMember? rootOld, FTMember ftMember, UpsertFTMemberRequest request)
        {
            var firstPartner = await _fTRelationshipRepository.GetQuery()
                                        .Include(x => x.ToFTMember)
                                        .FirstOrDefaultAsync(x => x.FromFTMemberId == request.RootId 
                                                            && x.CategoryCode == FTRelationshipCategory.PARTNER);

            if (firstPartner != null && firstPartner.ToFTMember.StatusCode == FTMemberStatus.UNDEFINED)
            {
                var partnerToUpdate = await _fTMemberRepository.GetByIdAsync(firstPartner.ToFTMemberId);
                //request.Id = partnerToUpdate.Id;
                partnerToUpdate = _mapper.Map(request, partnerToUpdate);
                partnerToUpdate.StatusCode = 0;

                _fTMemberRepository.Update(partnerToUpdate);
            }
            else
            {
                await _fTMemberRepository.AddAsync(ftMember);

                var relationshipPartner = new FTRelationship()
                {
                    FromFTMemberId = request.RootId.Value,
                    ToFTMemberId = ftMember.Id,
                    CategoryCode = FTRelationshipCategory.PARTNER,
                };
                await _fTRelationshipRepository.AddAsync(relationshipPartner);
            }

        }

        private async Task AddSiblingMember(FTMember? rootOld, FTMember ftMember, UpsertFTMemberRequest request)
        {
            var parentExisting = await _fTRelationshipRepository.GetQuery()
                                        .Include(x => x.FromFTMemberPartner)
                                        .FirstOrDefaultAsync(x => x.ToFTMemberId == request.RootId
                                                            && x.CategoryCode == FTRelationshipCategory.CHILDREN);

            if (parentExisting == null)
            {
                throw new ArgumentException("Số mối quan hệ cha mẹ không hợp lệ");
            }

            await _fTMemberRepository.AddAsync(ftMember);

            // Add a relationship for the children
            var relationShipOfParentExisting = new FTRelationship()
            {
                FromFTMemberId = parentExisting.FromFTMemberId,
                FromFTMemberPartnerId = parentExisting.FromFTMemberPartnerId,
                ToFTMemberId = (Guid)ftMember.Id,
                CategoryCode = FTRelationshipCategory.CHILDREN
            };
            await _fTRelationshipRepository.AddAsync(relationShipOfParentExisting);
        }

        private async Task AddParentMember(FTMember rootOld, FTMember ftMember, UpsertFTMemberRequest request)
        {
            var parent = await _fTRelationshipRepository.GetQuery()
                                        .Include(x => x.FromFTMemberPartner)
                                        .FirstOrDefaultAsync(x => x.ToFTMemberId == request.RootId
                                                            && x.CategoryCode == FTRelationshipCategory.CHILDREN);

            if (parent != null && parent.FromFTMemberPartner.StatusCode != FTMemberStatus.UNDEFINED)
            {
                throw new ArgumentException("Số mối quan hệ cha mẹ không hợp lệ");
            }
            else if (parent == null)
            {
                ftMember.IsRoot = true;
                await _fTMemberRepository.AddAsync(ftMember);

                // Create a new partner
                var partnerOfParentUndefined = new FTMember()
                {
                    StatusCode = FTMemberStatus.UNDEFINED,
                    FTId = request.FTId,
                    Fullname = $"Người đồng hành với {request.Fullname}",
                    IsRoot = false,
                    FTRole = FTMRole.FTMember,
                    Gender = request.Gender == 1 ? 0 : 1,
                };

                await _fTMemberRepository.AddAsync(partnerOfParentUndefined);

                // Add a relationship for the partner
                var relationshipPartnerOfParentUndefined = new FTRelationship()
                {
                    FromFTMemberId = ftMember.Id,
                    ToFTMemberId = partnerOfParentUndefined.Id,
                    CategoryCode = FTRelationshipCategory.PARTNER,
                };

                await _fTRelationshipRepository.AddAsync(relationshipPartnerOfParentUndefined);

                // Add a relationship for the children
                var relationShipChildAndParent = new FTRelationship()
                {
                    FromFTMemberId = ftMember.Id,
                    FromFTMemberPartnerId = partnerOfParentUndefined.Id,
                    ToFTMemberId = (Guid)request.RootId,
                    CategoryCode = FTRelationshipCategory.CHILDREN
                };

                await _fTRelationshipRepository.AddAsync(relationShipChildAndParent);

                // Change root of the family tree
                if (rootOld != null)
                {
                    rootOld.IsRoot = false;
                    _fTMemberRepository.Update(rootOld);
                }
            }
            else
            {
                var partnerToUpdate = await _fTMemberRepository.GetByIdAsync(parent.FromFTMemberPartnerId.Value);
                //request.Id = partnerToUpdate.Id;
                partnerToUpdate = _mapper.Map(request, partnerToUpdate);
                partnerToUpdate.StatusCode = 0;

                _fTMemberRepository.Update(partnerToUpdate);
            }
        }

    }
}
