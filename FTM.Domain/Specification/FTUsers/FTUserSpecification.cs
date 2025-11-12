using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Specification.FTMembers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification.FTUsers
{
    public class FTUserSpecification : BaseSpecifcation<FTUser>
    {

        public FTUserSpecification()
        {

        }

        public FTUserSpecification(FTUserSpecParams specParams) :
            base(x =>
                     (string.IsNullOrEmpty(specParams.Search) ||
                           x.Name.ToLower().Contains(specParams.Search.ToLower()) ||
                           (x.Username != null && x.Username.ToLower().Contains(specParams.Search.ToLower()))
                     ) && x.IsDeleted == false)
        {
            AddInclude(x => x.FT);

            ApplyPaging(specParams.Skip, specParams.Take);

            if (!string.IsNullOrEmpty(specParams.OrderBy))
                AddOrderBy(specParams.OrderBy);

            ApplyFilter(specParams.PropertyFilters);
        }
    }
}
