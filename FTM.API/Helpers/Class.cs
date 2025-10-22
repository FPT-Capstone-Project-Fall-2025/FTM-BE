using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FTM.API.Helpers
{
    public class Class
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập id của gia phả")]
        public Guid FTId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập id của thành viên")]
        public Guid FTMemberId { get; set; }

        [EnumDataType(typeof(FeatureType), ErrorMessage = "Invalid feature type.(MEMBER: 7001, EVENT: 7002, FUND: 7003, ALL: 7004)")]
        public FeatureType FeatureCode { get; set; }

        [ValidEnumList(typeof(MethodType), ErrorMessage = "Invalid method code. (VIEW: 6001, ADD: 6002, UPDATE: 6003, DELETE: 6004, ALL: 6005)")]
        public HashSet<MethodType> MethodsList { get; set; }
    }
}
