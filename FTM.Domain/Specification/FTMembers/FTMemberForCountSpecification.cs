using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification.FTMembers
{
    public class FTMemberForCountSpecification : BaseSpecifcation<FTMember>
    {
        public FTMemberForCountSpecification()
        {
            
        }

        public FTMemberForCountSpecification(FTMemberSpecParams specParams) :
            base(x =>
                     (string.IsNullOrEmpty(specParams.Search) ||
                           x.Fullname.ToLower().Contains(specParams.Search.ToLower()) ||
                           (x.Email != null && x.Email.ToLower().Contains(specParams.Search.ToLower())) ||
                           (x.Address != null && x.Address.ToLower().Contains(specParams.Search.ToLower()))
                     ) && x.IsDeleted == false)
        {

            ApplyFilter(specParams.PropertyFilters);
        }
    }
}
