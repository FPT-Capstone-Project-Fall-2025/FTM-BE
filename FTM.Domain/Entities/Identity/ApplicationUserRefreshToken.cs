using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.Identity
{
    [Table("UserRefreshTokens")]
    public class ApplicationUserRefreshToken : BaseEntity
    {
        [StringLength(450)]
        public string? Token { get; set; }

        public DateTimeOffset ExpiredAt { get; set; }

        [StringLength(450)] 
        public Guid ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
