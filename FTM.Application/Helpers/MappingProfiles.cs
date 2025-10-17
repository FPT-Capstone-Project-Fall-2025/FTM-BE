using AutoMapper;
using FTM.Domain.DTOs.FamilyTree;
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

            CreateMap<FTMemberFileRequest, FTMemberFile>().ReverseMap();

            CreateMap<UpsertFTRelationshipRequest, FTRelationship>().ReverseMap();
        }
    }
}
