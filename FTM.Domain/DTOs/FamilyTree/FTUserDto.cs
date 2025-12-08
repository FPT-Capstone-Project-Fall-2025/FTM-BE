using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTUserDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public FTMRole FTRole { get; set; }
        public SimpleFamilyTreeDto FT { get; set; }
    }
}
