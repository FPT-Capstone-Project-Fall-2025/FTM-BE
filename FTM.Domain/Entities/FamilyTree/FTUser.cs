using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.FamilyTree
{
    public class FTUser : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public Guid FTId { get; set; }
        public FTMRole FTRole { get; set; }
        public virtual FamilyTree FT { get; set; }
    }
}
