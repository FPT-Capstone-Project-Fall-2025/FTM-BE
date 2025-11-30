using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification.FamilyTrees
{
    public class MyFamilyTreeForCountSpecification : BaseSpecifcation<FamilyTree>
    {
        public MyFamilyTreeForCountSpecification()
        {
            
        }

        public MyFamilyTreeForCountSpecification(Guid userId, FamilyTreeSpecParams specParams) :
            base(x =>
                     (string.IsNullOrEmpty(specParams.Search) ||
                           x.Name.ToLower().Contains(specParams.Search.ToLower()) ||
                           (x.Description != null && x.Description.ToLower().Contains(specParams.Search.ToLower())) ||
                           (x.Owner != null && x.Owner.ToLower().Contains(specParams.Search.ToLower()))
                     ) && ((x.OwnerId == userId) || (x.FTMembers.Any(m => m.UserId == userId)))
                       && x.IsDeleted == false)
        {
            ApplyFilter(specParams.PropertyFilters);
        }
    }
}
