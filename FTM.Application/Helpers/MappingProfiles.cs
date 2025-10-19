using AutoMapper;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.Applications;
using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {

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


            CreateMap<FTMemberFileRequest, FTMemberFile>().ReverseMap();
            CreateMap<MReligionDto, MReligion>().ReverseMap();
            CreateMap<MEthnicDto, MEthnic>().ReverseMap();
            CreateMap<MWardDto, MWard>().ReverseMap();
            CreateMap<MprovinceDto, Mprovince>().ReverseMap();
            CreateMap<MprovinceDto, Mprovince>().ReverseMap();
            CreateMap<FTMemberFileDto, FTMemberFile>().ReverseMap();
            CreateMap<UpsertFTRelationshipRequest, FTRelationship>().ReverseMap();

            CreateMap<FamilyTree, FamilyTreeDataTableDto>()
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.FTMembers.Any() ? src.FTMembers.Count : 0));
        }
    }
}
