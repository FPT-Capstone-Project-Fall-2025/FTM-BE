using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class UpsertFTAuthorizationRequest
    {
        public Guid? Id { get; set; }
        public Guid FTId { get; set; }
        public Guid FTMemberId { get; set; }

        [EnumDataType(typeof(MethodType), ErrorMessage = "Invalid method code.")]
        public MethodType MethodCode { get; set; }
        public FeatureType FeatureCode { get; set; }
    }
}
