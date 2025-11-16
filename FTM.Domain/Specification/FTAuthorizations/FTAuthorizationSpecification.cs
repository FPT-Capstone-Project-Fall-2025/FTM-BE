using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification.FTAuthorizations
{
    public class FTAuthorizationSpecification : BaseSpecifcation<FTAuthorization>
    {
        public FTAuthorizationSpecification()
        {
            
        }

        public FTAuthorizationSpecification(FTAuthorizationSpecParams specParams)
            : base(x =>
                (string.IsNullOrEmpty(specParams.Search) ||
                           x.AuthorizedMember.Fullname.ToLower().Contains(specParams.Search.ToLower()) ||
                           (x.AuthorizedMember.Email != null && x.AuthorizedMember.Email.ToLower().Contains(specParams.Search.ToLower())) ||
                           (x.FamilyTree.Name != null && x.FamilyTree.Name.ToLower().Contains(specParams.Search.ToLower()))
                     ) && x.IsDeleted == false)

        {
            AddInclude(x => x.FamilyTree);
            AddInclude(x => x.AuthorizedMember.FTMemberFiles);

            //ApplyPaging(specParams.Skip, specParams.Take);

            if (!string.IsNullOrEmpty(specParams.OrderBy))
                AddOrderBy(specParams.OrderBy);

            ApplyFilter(specParams.PropertyFilters);
        }
    }
}
