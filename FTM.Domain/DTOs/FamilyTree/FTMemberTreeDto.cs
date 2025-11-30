using FTM.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTMemberTreeDto
    {
        public Guid? Root { get; set; }
        public List<KeyValueModel> Datalist { get; set; } = new List<KeyValueModel>();
    }
}
