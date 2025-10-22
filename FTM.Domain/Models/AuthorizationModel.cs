using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Models
{
    public class AuthorizationModel
    {
        [EnumDataType(typeof(FeatureType), ErrorMessage = "Vui lòng chọn đúng tính năng được thêm quyền.(MEMBER: 7001, EVENT: 7002, FUND: 7003, ALL: 7004)")]
        public FeatureType FeatureCode { get; set; }

        [ValidEnumList(typeof(MethodType), ErrorMessage = "Vui lòng chọn đúng quyền. (VIEW: 6001, ADD: 6002, UPDATE: 6003, DELETE: 6004, ALL: 6005)")]
        public HashSet<MethodType> MethodsList { get; set; }
    }
}
