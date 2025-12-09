using FTM.Domain.DTOs.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Models
{
    public class KeyValueModel
    {
        public object Key { get; set; }
        public object Value { get; set; }
    }

    public class KeyValueFTModel
    {
        public Guid Key { get; set; }
        public FTMemberTreeDetailsDto Value { get; set; }
    }
    public class KeyValueChildrenModel
    {
        public Guid Key { get; set; }
        public Guid[] Value { get; set; }
    }
}
