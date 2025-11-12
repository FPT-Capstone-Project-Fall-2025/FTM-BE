using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification.FTInvitations
{
    public class FTInvitationForCountSpecification : BaseSpecifcation<FTInvitation>
    {
        public FTInvitationForCountSpecification()
        {

        }

        public FTInvitationForCountSpecification(FTInvitationSpecParams specParams) :
            base(x =>
                     (string.IsNullOrEmpty(specParams.Search) ||
                           x.InvitedName.ToLower().Contains(specParams.Search.ToLower()) ||
                           (x.InviterName != null && x.InviterName.ToLower().Contains(specParams.Search.ToLower())) ||
                           (x.FTMemberName != null && x.FTMemberName.ToLower().Contains(specParams.Search.ToLower()))
                     ) && x.IsDeleted == false)
        {

            ApplyFilter(specParams.PropertyFilters);
        }
    }
}
