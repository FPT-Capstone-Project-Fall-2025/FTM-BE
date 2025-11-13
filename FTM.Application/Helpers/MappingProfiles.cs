using AutoMapper;
using AutoMapper.Execution;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.DTOs.Notifications;
using FTM.Domain.Entities.Applications;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Entities.Notifications;
using FTM.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FTM.Domain.Constants.Constants;

namespace FTM.Application.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<FTMember, FTMemberTreeDetailsDto>()
                .ForMember(dest => dest.Partners, opt => opt.MapFrom(src =>
                    src.FTRelationshipFrom
                            .Where(rFrom => rFrom.CategoryCode == FTRelationshipCategory.PARTNER)
                            .Select(rFrom => rFrom.ToFTMemberId)
                            .ToArray()))
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src =>
                    src.FTRelationshipFrom
                            .Where(rFrom => rFrom.CategoryCode == FTRelationshipCategory.CHILDREN && rFrom.FromFTMemberPartnerId != null)
                            .GroupBy(rFrom => rFrom.FromFTMemberPartnerId)
                            .Select(gr =>
                                new KeyValueModel
                                {
                                    Key = gr.Key,
                                    Value = gr.OrderBy(x => x.ToFTMember.Birthday).Select(rFrom => rFrom.ToFTMember.Id).ToArray()
                                })))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Fullname))

                .AfterMap((src, desc) =>
                {
                    if (desc.Partners.IsNullOrEmpty())
                    {
                        desc.Partners = src.FTRelationshipTo.Where(x => x.CategoryCode == FTRelationshipCategory.PARTNER && x.ToFTMemberId == src.Id).Select(x => x.FromFTMember.Id).ToList();
                    }

                    if (src.FTMemberFiles.Any(f => f.Title.Contains("Avatar")))
                    {
                        desc.Avatar = src.FTMemberFiles.FirstOrDefault(f => f.Title.Contains("Avatar"))?.FilePath;
                    }
                });

            CreateMap<UpsertFTMemberRequest, FTMember>()
                .ForMember(dest => dest.FTMemberFiles, opt => opt.Ignore())
                .ForMember(dest => dest.FTRelationshipFrom, opt => opt.Ignore())
                .ForMember(dest => dest.FTRelationshipFromPartner, opt => opt.Ignore())
                .ForMember(dest => dest.FTRelationshipTo, opt => opt.Ignore())
                .ForMember(dest => dest.FTAuthorizations, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<FTMember, FTMemberDetailsDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
                .ForMember(dest => dest.Religion, opt => opt.MapFrom(src => src.Religion))
                .ForMember(dest => dest.Ethnic, opt => opt.MapFrom(src => src.Ethnic))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Province))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Ward))
                .ForMember(dest => dest.BurialProvince, opt => opt.MapFrom(src => src.BurialProvince))
                .ForMember(dest => dest.BurialWard, opt => opt.MapFrom(src => src.BurialWard))
                .ForMember(dest => dest.FTMemberFiles, opt => opt.MapFrom(src => src.FTMemberFiles));

            CreateMap<FamilyTree, FamilyTreeDataTableDto>()
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.FTMembers.Any(m => m.IsDeleted == false) ? src.FTMembers.Count(m => m.IsDeleted == false) : 0));

            CreateMap<FTMember, FTMemberSimpleDto>()
                .AfterMap((src, desc) => {
                    if (!src.FTMemberFiles.IsNullOrEmpty())
                    {
                        desc.FilePath = src.FTMemberFiles.First().FilePath;
                    }
                });
    

            CreateMap<FTMemberFileRequest, FTMemberFile>().ReverseMap();
            CreateMap<MReligionDto, MReligion>().ReverseMap();
            CreateMap<MEthnicDto, MEthnic>().ReverseMap();
            CreateMap<MWardDto, MWard>().ReverseMap();
            CreateMap<MprovinceDto, Mprovince>().ReverseMap();
            CreateMap<MprovinceDto, Mprovince>().ReverseMap();
            CreateMap<FTMemberFileDto, FTMemberFile>().ReverseMap();
            CreateMap<UpsertFTRelationshipRequest, FTRelationship>().ReverseMap();
            CreateMap<UpsertFTAuthorizationRequest, FTAuthorizationDto>();
            CreateMap<FTAuthorization, FTAuthorizationDto>();
            CreateMap<FTInvitation, FTInvitationDto>();
            CreateMap<FTInvitation, SimpleFTInvitationDto>();
            CreateMap<FTNotification, FTNotificationDto>();
            CreateMap<FamilyTree, SimpleFamilyTreeDto>();
            CreateMap<FTUser, FTUserDto>();

            CreateMap<UpdateFTMemberRequest, FTMember>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FTId, opt => opt.Ignore())
                .ForMember(dest => dest.FT, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.FTRole, opt => opt.Ignore())
                .ForMember(dest => dest.StatusCode, opt => opt.Ignore())
                .ForMember(dest => dest.PrivacyData, opt => opt.Ignore())
                .ForMember(dest => dest.FTMemberFiles, opt => opt.Ignore())
                .ForMember(dest => dest.FTRelationshipFrom, opt => opt.Ignore())
                .ForMember(dest => dest.FTRelationshipFromPartner, opt => opt.Ignore())
                .ForMember(dest => dest.FTRelationshipTo, opt => opt.Ignore())
                .ForMember(dest => dest.FTAuthorizations, opt => opt.Ignore())
                // chỉ map khi source != null
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<FTMember, MemberRelationshipDto>()
                .ForMember(dest => dest.HasFather, opt => opt.MapFrom(src =>
                                                                           src.FTRelationshipTo != null
                                                                           && src.FTRelationshipTo.Any(r =>
                                                                                                          r.CategoryCode == FTRelationshipCategory.CHILDREN
                                                                                                          && (r.FromFTMember.Gender == 0 || (r.FromFTMemberPartner.Gender == 0 && r.FromFTMemberPartner.StatusCode != FTMemberStatus.UNDEFINED)))))
                .ForMember(dest => dest.HasMother, opt => opt.MapFrom(src =>
                                                                           src.FTRelationshipTo != null
                                                                           && src.FTRelationshipTo.Any(r =>
                                                                                                          r.CategoryCode == FTRelationshipCategory.CHILDREN
                                                                                                          && (r.FromFTMember.Gender == 1 || (r.FromFTMemberPartner.Gender == 1 && r.FromFTMemberPartner.StatusCode != FTMemberStatus.UNDEFINED)))))
                .ForMember(dest => dest.HasSiblings, opt => opt.MapFrom(src =>
                                                                           src.FTRelationshipTo != null
                                                                           && src.FTRelationshipTo.Any(r => 
                                                                                                         r.CategoryCode == FTRelationshipCategory.CHILDREN
                                                                                                         && ((r.FromFTMember.FTRelationshipFrom.Count(fr => fr.CategoryCode == FTRelationshipCategory.CHILDREN) > 1)
                                                                                                            ||(r.FromFTMemberPartner.FTRelationshipFrom.Count(fr => fr.CategoryCode == FTRelationshipCategory.CHILDREN ) > 1)
                                                                                                            ||(r.FromFTMember.FTRelationshipFrom.Count(fr => fr.CategoryCode == FTRelationshipCategory.CHILDREN) + r.FromFTMemberPartner.FTRelationshipFrom.Count(fr => fr.CategoryCode == FTRelationshipCategory.CHILDREN) > 1)))))
                .ForMember(dest => dest.HasPartner, opt => opt.MapFrom(src =>
                                                                           (src.FTRelationshipFrom != null
                                                                                        && src.FTRelationshipFrom.Any(r =>
                                                                                                          r.CategoryCode == FTRelationshipCategory.PARTNER
                                                                                                          && r.ToFTMember.StatusCode != FTMemberStatus.UNDEFINED
                                                                                                          && !r.ToFTMember.IsDivorced))
                                                                           || (src.FTRelationshipTo != null
                                                                                        && src.FTRelationshipTo.Any(r =>
                                                                                                          r.CategoryCode == FTRelationshipCategory.PARTNER
                                                                                                          && r.FromFTMember.StatusCode != FTMemberStatus.UNDEFINED
                                                                                                          && !r.FromFTMember.IsDivorced))))
                .ForMember(dest => dest.HasChildren, opt => opt.MapFrom(src =>
                    (src.FTRelationshipFrom != null && src.FTRelationshipFrom.Any(r => r.CategoryCode == FTRelationshipCategory.CHILDREN))
                    || (src.FTRelationshipFromPartner != null && src.FTRelationshipFromPartner.Any(r => r.CategoryCode == FTRelationshipCategory.CHILDREN))
                ));

            // Campaign Donation mappings
            CreateMap<FTCampaignDonation, FTCampaignDonationResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CampaignId, opt => opt.MapFrom(src => src.CampaignId))
                .ForMember(dest => dest.CampaignName, opt => opt.MapFrom(src => src.Campaign != null ? src.Campaign.CampaignName : null))
                .ForMember(dest => dest.DonorId, opt => opt.MapFrom(src => src.FTMemberId))
                .ForMember(dest => dest.DonorName, opt => opt.MapFrom(src => src.DonorName))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.DonationAmount))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.DonorNotes))
                .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.IsAnonymous))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PayOSOrderCode, opt => opt.MapFrom(src => src.PayOSOrderCode != null ? src.PayOSOrderCode.ToString() : string.Empty))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.PaymentTransactionId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn.DateTime))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.ConfirmedOn.HasValue ? src.ConfirmedOn.Value.DateTime : (DateTime?)null))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedOn.DateTime));

            // Campaign Expense mappings
            CreateMap<FTCampaignExpense, FTCampaignExpenseResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CampaignId, opt => opt.MapFrom(src => src.CampaignId))
                .ForMember(dest => dest.CampaignName, opt => opt.MapFrom(src => src.Campaign != null ? src.Campaign.CampaignName : null))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ExpenseTitle))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ExpenseDescription))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.ExpenseAmount))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.ExpenseDate, opt => opt.MapFrom(src => src.ExpenseDate.DateTime))
                .ForMember(dest => dest.ReceiptUrl, opt => opt.MapFrom(src => src.ReceiptImages))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ApprovalStatus))
                .ForMember(dest => dest.RequestedById, opt => opt.MapFrom(src => src.AuthorizedBy))
                .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src => src.Authorizer != null ? src.Authorizer.Fullname : null))
                .ForMember(dest => dest.ApprovedById, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.Approver != null ? src.Approver.Fullname : null))
                .ForMember(dest => dest.ApprovedAt, opt => opt.MapFrom(src => src.ApprovedOn.HasValue ? src.ApprovedOn.Value.DateTime : (DateTime?)null))
                .ForMember(dest => dest.ApprovalNotes, opt => opt.MapFrom(src => src.ApprovalNotes))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn.DateTime))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedOn.DateTime));

            // Campaign mappings
            CreateMap<FTFundCampaign, FTCampaignResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CampaignName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.CampaignDescription))
                .ForMember(dest => dest.FamilyTreeId, opt => opt.MapFrom(src => src.FTId))
                .ForMember(dest => dest.FamilyTreeName, opt => opt.MapFrom(src => src.FamilyTree != null ? src.FamilyTree.Name : null))
                .ForMember(dest => dest.CampaignManagerId, opt => opt.MapFrom(src => src.CampaignManagerId))
                .ForMember(dest => dest.CampaignManagerName, opt => opt.MapFrom(src => src.CampaignManager != null ? src.CampaignManager.Fullname : null))
                .ForMember(dest => dest.TargetAmount, opt => opt.MapFrom(src => src.FundGoal))
                .ForMember(dest => dest.CurrentAmount, opt => opt.MapFrom(src => src.CurrentBalance))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.DateTime))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.DateTime))
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.BeneficiaryInfo, opt => opt.MapFrom(src => src.AccountHolderName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn.DateTime))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedOn.DateTime))
                .ForMember(dest => dest.TotalDonations, opt => opt.MapFrom(src => src.Donations != null ? src.Donations.Count : 0))
                .ForMember(dest => dest.TotalDonors, opt => opt.MapFrom(src => src.Donations != null ? src.Donations.Select(d => d.FTMemberId).Distinct().Count() : 0));

        }
    }
}
