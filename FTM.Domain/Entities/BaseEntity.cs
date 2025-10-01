using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset LastModifiedOn { get; set; } = DateTimeOffset.Now;
        public string LastModifiedBy { get; set; } = String.Empty;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public string CreatedBy { get; set; } = String.Empty;
        public bool? IsDeleted { get; set; } = false;
        public Guid CreatedByUserId { get; set; } = Guid.NewGuid();
    }
}
