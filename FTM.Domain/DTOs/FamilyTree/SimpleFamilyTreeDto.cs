using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class SimpleFamilyTreeDto
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string? FilePath { get; set; }
    }
}
