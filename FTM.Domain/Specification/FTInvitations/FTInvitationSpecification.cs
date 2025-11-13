using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Specification.FTMembers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification.FTInvitations
{
    public class FTInvitationSpecification : BaseSpecifcation<FTInvitation>
    {
        public FTInvitationSpecification()
        {

        }

        public FTInvitationSpecification(FTInvitationSpecParams specParams) :
            base(x =>
                     (string.IsNullOrEmpty(specParams.Search) ||
                           x.InvitedName.ToLower().Contains(specParams.Search.ToLower()) ||
                           (x.InviterName != null && x.InviterName.ToLower().Contains(specParams.Search.ToLower())) ||
                           (x.FTMemberName != null && x.FTMemberName.ToLower().Contains(specParams.Search.ToLower()))
                     ))
        {

            ApplyPaging(specParams.Skip, specParams.Take);

            if (!string.IsNullOrEmpty(specParams.OrderBy))
                AddOrderBy(specParams.OrderBy);

            ApplyFilter(specParams.PropertyFilters);
        }
    }
}
