using FTM.Domain.Enums;
using FTM.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTAuthorizationDto
    {
        public Guid FTId { get; set; }
        public Guid FTMemberId { get; set; }
        public HashSet<AuthorizationModel> AuthorizationList { get; set; }
    }
}
