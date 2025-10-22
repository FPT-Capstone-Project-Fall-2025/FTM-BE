using AutoMapper;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.Applications;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Models;
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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Fullname));

            CreateMap<UpsertFTMemberRequest, FTMember>()
                .ForMember(dest => dest.FTMemberFiles, opt => opt.MapFrom(src => src.FTMemberFiles))
                // Ignore navigation collections that shouldn’t be updated automatically
                .ForMember(dest => dest.FTRelationshipFrom, opt => opt.Ignore())
                .ForMember(dest => dest.FTRelationshipFromPartner, opt => opt.Ignore())
                .ForMember(dest => dest.FTRelationshipTo, opt => opt.Ignore())
                .ForMember(dest => dest.FTAuthorizations, opt => opt.Ignore());

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
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.FTMembers.Any() ? src.FTMembers.Count : 0));

            CreateMap<FTMember, FTMemberSimpleDto>();
            CreateMap<FTMemberFileRequest, FTMemberFile>().ReverseMap();
            CreateMap<MReligionDto, MReligion>().ReverseMap();
            CreateMap<MEthnicDto, MEthnic>().ReverseMap();
            CreateMap<MWardDto, MWard>().ReverseMap();
            CreateMap<MprovinceDto, Mprovince>().ReverseMap();
            CreateMap<MprovinceDto, Mprovince>().ReverseMap();
            CreateMap<FTMemberFileDto, FTMemberFile>().ReverseMap();
            CreateMap<UpsertFTRelationshipRequest, FTRelationship>().ReverseMap();
            CreateMap<UpsertFTAuthorizationRequest, FTAuthorization>();
            CreateMap<FTAuthorization, FTAuthorizationDto>();
           
        }
    }
}
