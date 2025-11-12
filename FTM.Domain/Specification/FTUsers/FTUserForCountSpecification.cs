using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification.FTUsers
{
    public class FTUserForCountSpecification : BaseSpecifcation<FTUser>
    {

        public FTUserForCountSpecification()
        {

        }

        public FTUserForCountSpecification(FTUserSpecParams specParams) :
            base(x =>
                     (string.IsNullOrEmpty(specParams.Search) ||
                           x.Name.ToLower().Contains(specParams.Search.ToLower()) ||
                           (x.Username != null && x.Username.ToLower().Contains(specParams.Search.ToLower()))
                     ) && x.IsDeleted == false)
        {
            ApplyFilter(specParams.PropertyFilters);
        }
    }
}
