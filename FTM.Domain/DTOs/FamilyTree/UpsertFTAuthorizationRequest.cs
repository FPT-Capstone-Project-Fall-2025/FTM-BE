using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Domain.Models;
using Newtonsoft.Json.Serialization;
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

        [Required(ErrorMessage = "Vui lòng nhập id của gia phả")]
        public Guid FTId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập id của thành viên")]
        public Guid FTMemberId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền")]
        public HashSet<AuthorizationModel> AuthorizationList { get; set; }
    }
}
