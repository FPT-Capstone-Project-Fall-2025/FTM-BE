using AutoMapper;
using AutoMapper.Execution;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Entities.Posts;
using FTM.Domain.Enums;
using FTM.Domain.Models;
using FTM.Domain.Specification;
using FTM.Domain.Specification.FamilyTrees;
using FTM.Domain.Specification.FTMembers;
using FTM.Domain.Specification.FTUsers;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAct;
using XAct.Users;
using static FTM.Domain.Constants.Constants;

namespace FTM.Application.Services
{
    public class FTMemberService : IFTMemberService
    {
        private readonly IFTInvitationService _fTInvitationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<FamilyTree> _familyTreeRepository;
        private readonly IFTMemberRepository _fTMemberRepository;
        private readonly IGenericRepository<FTRelationship> _fTRelationshipRepository;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IFTAuthorizationService _fTAuthorizationService;
        private readonly IFTMemberFileRepository _fTMemberFileRepository;
        private readonly IFTUserRepository _fTUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public FTMemberService(
            IFTInvitationService fTInvitationService,
            UserManager<ApplicationUser> userManager,
            IGenericRepository<FamilyTree> familyTreeRepository,
            IFTMemberRepository FTMemberRepository,
            IGenericRepository<FTRelationship> FTRelationshipRepository,
            ICurrentUserResolver CurrentUserResolver,
            IBlobStorageService blobStorageService,
            IFTAuthorizationService fTAuthorizationService,
            IFTMemberFileRepository fTMemberFileRepository,
            IFTUserRepository fTUserRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _fTInvitationService = fTInvitationService;
            _userManager = userManager;
            _familyTreeRepository = familyTreeRepository;
            _fTMemberRepository = FTMemberRepository;
            _fTRelationshipRepository = FTRelationshipRepository;
            _currentUserResolver = CurrentUserResolver;
            _blobStorageService = blobStorageService;
            _fTAuthorizationService = fTAuthorizationService;
            _fTMemberFileRepository = fTMemberFileRepository;
            _fTUserRepository = fTUserRepository;
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

                               //----------------Handle Avatar---------------------
                               if (!request.File.IsNull())
                               {
                                   string filePath = await _blobStorageService.UploadFileAsync(request.File, "ftmembers", $"Avatar{ftMember.Id}");
                                   ftMember.FTMemberFiles = new List<FTMemberFile>()
                                                           {
                                                               new FTMemberFile()
                                                               {
                                                                   FTMemberId = ftMember.Id,
                                                                   Title = $"Avatar{ftMember.Id}",
                                                                   FilePath = filePath,
                                                                   FileType = "1",
                                                                   Content = "",
                                                                   Description = "",
                                                                   Thumbnail = ""
                                                               }
                                                           };

                               }
                               //----------------End Handle Avatar---------------------

                               //----------------Handle Invitation---------------------
                               if (request.UserId != null && request.UserId != Guid.Empty)
                               {
                                   var invitedUser = await _userManager.FindByIdAsync(request.UserId.ToString());
                                   if (invitedUser == null)
                                       throw new ArgumentException("Người được mời không tồn tại trong hệ thống.");

                                   var isConnectedToMember = await _fTMemberRepository.IsConnectedTo(FTId, invitedUser.Id);

                                   if (isConnectedToMember)
                                   {
                                       throw new ArgumentException("Nguời dùng đã được liên kết với một thành viên khác trong gia phả");
                                   }

                                   var ftInvitation = new FTInvitation
                                   {
                                       FTId = request.FTId.Value,
                                       FTName = familyTree.Name,
                                       FTMemberId = ftMember.Id,
                                       FTMemberName = ftMember.Fullname,
                                       InviterUserId = _currentUserResolver.UserId,
                                       InviterName = _currentUserResolver.Name,
                                       Email = invitedUser.Email,
                                       InvitedUserId = invitedUser.Id,
                                       InvitedName = invitedUser.Name,
                                       Token = Guid.NewGuid().ToString(),
                                       ExpirationDate = DateTime.UtcNow.AddDays(3),
                                       Status = InvitationStatus.PENDING
                                   };

                                   await _fTInvitationService.AddAsync(ftInvitation);
                                   await _fTInvitationService.SendAsync(ftInvitation);
                               }
                               //----------------End Handle Invitation---------------------

                               //----------------Handle Authorization---------------------
                               if (request.UserId != null)
                               {
                                   var isOwner = await _fTAuthorizationService.IsOwnerAsync(request.FTId.Value, request.UserId.Value);
                                   if (!isOwner)
                                   {
                                       await _fTAuthorizationService.SetMemberAuthorizationAsync(request.FTId.Value, ftMember.Id);
                                   }
                               }
                               else
                               {
                                   await _fTAuthorizationService.SetMemberAuthorizationAsync(request.FTId.Value, ftMember.Id);

                               }
                               //----------------End Handle Authorization---------------------

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
                    FTId = request.FTId.Value,
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

            if (firstPartner != null && !firstPartner.ToFTMember.IsDivorced) throw new ArgumentException("Mỗi người chồng chỉ được có một người vợ.");

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
                var definedPosition = (request.Gender) == 0 ? "Vợ của" : "Chồng của";
                var partnerOfParentUndefined = new FTMember()
                {
                    StatusCode = FTMemberStatus.UNDEFINED,
                    FTId = request.FTId.Value,
                    Fullname = $"{definedPosition} {request.Fullname}",
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

        public async Task<FTMemberDetailsDto> GetByUserId(Guid FTId, Guid userId)
        {
            return await GetById(FTId, userId, isMemberId: false);
        }

        public async Task<FTMemberTreeDto> GetMembersTree(Guid ftId)
        {
            var members = await _fTMemberRepository.GetMembersTree(ftId);

            if (!members.Any(m => m.IsRoot == true)) return new FTMemberTreeDto();

            var membersTreeDetails = members.Select(_mapper.Map<FTMemberTreeDetailsDto>);
            var partnerIds = membersTreeDetails.SelectMany(m => m.Partners).ToList();

            return new FTMemberTreeDto()
            {
                Root = members.First(m => m.IsRoot == true).Id,
                Datalist = membersTreeDetails.Select(m =>
                {
                    m.IsPartner = partnerIds.Any(p => p == m.Id);
                    return new KeyValueModel
                    {
                        Key = m.Id,
                        Value = m
                    };
                }).ToList()
            };
        }

        public async Task<FTMemberDetailsDto> GetByMemberId(Guid ftid, Guid memberId)
        {
            return await GetById(ftid, memberId, isMemberId: true);
        }

        private async Task<FTMemberDetailsDto> GetById(Guid ftid, Guid id, bool isMemberId)
        {
            PropertyFilter[] propertyFilters = new PropertyFilter[]
            {
                new PropertyFilter
                {
                    Name = "FTId",
                    Operation = "EQUAL",
                    Value = ftid
                },
                new PropertyFilter
                {
                    Name = isMemberId? "Id": "UserId",
                    Operation = "EQUAL",
                    Value = id
                },
                new PropertyFilter
                {
                    Name = "IsDeleted",
                    Operation = "EQUAL",
                    Value = false
                }
            };

            var serializedFilters = JsonConvert.SerializeObject(propertyFilters);

            FTMemberSpecParams specParams = new FTMemberSpecParams
            {
                PropertyFilters = serializedFilters
            };

            var spec = new FTMemberSpecification(ftid, specParams);
            var members = await _fTMemberRepository.ListAsync(spec);

            if (members.Count() == 0)
            {
                throw new ArgumentException("Không tìm thấy thành viên gia phả.");
            }

            return _mapper.Map<FTMemberDetailsDto>(members.First());
        }

        public async Task<IReadOnlyList<FTMemberSimpleDto>> GetListOfMembers(FTMemberSpecParams specParams)
        {
            var spec = new FTMemberSimpleSpecification(specParams);
            var ftms = await _fTMemberRepository.ListAsync(spec);

            return _mapper.Map<IReadOnlyList<FTMemberSimpleDto>>(ftms);
        }
        public async Task<IReadOnlyList<FTUserDto>> GetListOfFTUsers(FTUserSpecParams specParams)
        {
            var spec = new FTUserSpecification(specParams);
            var ftUsers = await _fTUserRepository.ListAsync(spec);

            return _mapper.Map<IReadOnlyList<FTUserDto>>(ftUsers);
        }

        public async Task<int> CountMembers(FTMemberSpecParams specParams)
        {
            var spec = new FTMemberForCountSpecification(specParams);
            return await _fTMemberRepository.CountAsync(spec);
        }

        public async Task<FTMemberDetailsDto> UpdateDetailsByMemberId(Guid ftId, UpdateFTMemberRequest request)
        {
            var existingMember = await _fTMemberRepository.GetByIdAsync(request.ftMemberId);
            if (existingMember == null) throw new ArgumentException("Thành viên không tồn tại trong gia phả");
            _mapper.Map(request, existingMember);

            //----------------Handle Invitation---------------------
            if ((existingMember.UserId.HasValue && request.UserId.HasValue && existingMember.UserId != request.UserId.Value)
                || (!existingMember.UserId.HasValue && request.UserId.HasValue))
            {
                var invitedUser = await _userManager.FindByIdAsync(request.UserId.ToString());

                if (invitedUser == null)
                    throw new ArgumentException("Người được mời không tồn tại trong hệ thống.");

                var isConnectedToMember = await _fTMemberRepository.IsConnectedTo(ftId, invitedUser.Id);

                if (isConnectedToMember)
                {
                    throw new ArgumentException("Nguời dùng đã được liên kết với một thành viên khác trong gia phả");
                }

                var ftInvitation = new FTInvitation
                {
                    FTId = existingMember.FTId,
                    FTMemberId = existingMember.Id,
                    Email = invitedUser.Email,
                    InviterUserId = _currentUserResolver.UserId,
                    InvitedUserId = invitedUser.Id,
                    Token = Guid.NewGuid().ToString(),
                    ExpirationDate = DateTime.UtcNow.AddDays(3),
                    Status = InvitationStatus.PENDING
                };

                await _fTInvitationService.AddAsync(ftInvitation);
                await _fTInvitationService.SendAsync(ftInvitation);
            }
            //----------------End Handle Invitation---------------------

            //----------------Handle Avatar---------------------
            if (!request.File.IsNull())
            {
                var avatar = await _fTMemberFileRepository.FindAvatarAsync(existingMember.Id);
                string filePath = await _blobStorageService.UploadFileAsync(request.File, "ftmembers", $"Avatar{existingMember.Id}");
                if (avatar == null)
                {
                    await _fTMemberFileRepository.AddAsync(new FTMemberFile()
                    {
                        FTMemberId = existingMember.Id,
                        Title = $"Avatar{existingMember.Id}",
                        FilePath = filePath,
                        FileType = "1",
                        Content = "",
                        Description = "",
                        Thumbnail = ""
                    });
                }
                else
                {
                    await _blobStorageService.DeleteFileAsync("ftmembers", $"Avatar{existingMember.Id}");
                    avatar.FilePath = filePath;
                    _fTMemberFileRepository.Update(avatar);
                }
            }
            //----------------End Handle Avatar---------------------


            _fTMemberRepository.Update(existingMember);
            await _unitOfWork.CompleteAsync();

            return await GetByMemberId(ftId, request.ftMemberId);
        }

        public async Task Delete(Guid ftMemberId)
        {
            var member = await _fTMemberRepository.GetMemberById(ftMemberId);

            // <----------------------Relationship Validation---------------------->
            if (member == null)
                throw new ArgumentException("Không tìm thấy thành viên trong cây gia phả.");

            if (member.FTRelationshipFrom
                      .Count(x => x.CategoryCode == FTRelationshipCategory.CHILDREN) > 1)
                throw new ArgumentException(
                    $"Thành viên {member.Fullname} có nhiều con trong cây gia phả, nên không thể xóa."
                );

            if (member.FTRelationshipFrom
                      .Any(x => x.CategoryCode == FTRelationshipCategory.PARTNER &&
                                x.ToFTMember.StatusCode != FTMemberStatus.UNDEFINED))
                throw new ArgumentException(
                    $"Không thể xóa thành viên {member.Fullname} vì họ vẫn còn mối quan hệ vợ/chồng(Người thật) trong gia phả. " +
                    $"Vui lòng xóa hoặc vô hiệu hóa mối quan hệ đó trước."
                );

            if (member.FTRelationshipFrom.Any(x => x.CategoryCode == FTRelationshipCategory.CHILDREN) &&
                member.FTRelationshipTo.Any(x => x.CategoryCode == FTRelationshipCategory.CHILDREN))
                throw new ArgumentException(
                    $"Không thể xóa thành viên {member.Fullname} vì họ vừa là mối quan hệ cha/mẹ và con trong gia phả. " +
                    $"Vui lòng xóa hoặc vô hiệu hóa các mối quan hệ đó trước."
                );

            // <----------------------Handle Logic---------------------->
            // Step 1: If the deleted member is partner and have the CHILDREN relationship => Soft Delete 
            if (member.FTRelationshipTo.Any(x => x.FromFTMember.FTRelationshipFrom.Any(
                                y => y.CategoryCode == FTRelationshipCategory.CHILDREN && y.FromFTMemberPartnerId == member.Id)))
            {
                member.Fullname = "Thành viên đã bị xóa";
                member.StatusCode = FTMemberStatus.UNDEFINED;
                _fTMemberRepository.Update(member);
            }
            else // Member is parent or child => Hard Delete
            {
                _fTMemberRepository.Delete(member);
                _fTMemberRepository.Delete(member.FTRelationshipFrom.Select(x => x.ToFTMember).Where(m => m.StatusCode == FTMemberStatus.UNDEFINED).ToList());
            }

            // Step 2: Check if the member is a parent, signing root to the member's child
            var child = member.FTRelationshipFrom.FirstOrDefault(x => x.CategoryCode == FTRelationshipCategory.CHILDREN);
            if (child != null)
            {
                var childMember = child.ToFTMember;
                childMember.IsRoot = true;
                _fTMemberRepository.Update(childMember);
            }

            // Step 3: If deleting a child, check whether the partner of parent is underfine and the parent has a child, if it is true, remove the partner.
            var parent = member.FTRelationshipTo.FirstOrDefault(x => x.CategoryCode == FTRelationshipCategory.CHILDREN && x.ToFTMemberId == member.Id);
            if (parent != null && parent.FromFTMember.FTRelationshipFrom.Count(x => x.CategoryCode == FTRelationshipCategory.CHILDREN) == 1
                && parent.FromFTMemberPartner.StatusCode == FTMemberStatus.UNDEFINED)
            {
                _fTMemberRepository.Delete(parent.FromFTMemberPartner);
            }

            // Step 4: Check if the member is connected to a user, change user's role to GUEST
            if (member.UserId.HasValue)
            {
                var connectedUser = _userManager.FindByIdAsync(member.UserId.ToString());
                if (connectedUser != null)
                {
                    var ftUser = await _fTUserRepository.FindAsync(member.FTId, member.UserId.Value);
                    if (ftUser != null && ftUser.FTRole == FTMRole.FTMember)
                    {
                        ftUser.FTRole = FTMRole.FTGuest;
                        await _fTAuthorizationService.DeleteAuthorizationAsync(member.FTId, member.Id);
                    }
                    member.UserId = Guid.Empty;
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<MemberRelationshipDto> CheckRelationship(Guid ftMemberId)
        {
            var _ftMember = await _fTMemberRepository.GetMemberById(ftMemberId);

            if (_ftMember == null) throw new ArgumentException("Thành viên không tồn tại");

            return _mapper.Map<MemberRelationshipDto>(_ftMember);
        }

        public async Task<IReadOnlyList<FTMemberSimpleDto>> GetListOfMembersWithoutUser(Guid ftId)
        {
            var memberWithoutUserList = await _fTMemberRepository.GetMembersWithoutUserAsync(ftId);
            return _mapper.Map<IReadOnlyList<FTMemberSimpleDto>>(memberWithoutUserList);
        }

        public async Task<int> CountFTUsers(FTUserSpecParams specParams)
        {
            var spec = new FTUserForCountSpecification(specParams);
            return await _fTUserRepository.CountAsync(spec);
        }
    }
}
