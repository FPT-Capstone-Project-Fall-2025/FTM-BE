using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification.FTMembers
{
    public class FTMemberSpecification : BaseSpecifcation<FTMember>
    {
        public FTMemberSpecification()
        {

        }

        public FTMemberSpecification(Guid ftId, FTMemberSpecParams specParams) :
            base(x =>
                     (string.IsNullOrEmpty(specParams.Search) ||
                           x.Fullname.ToLower().Contains(specParams.Search.ToLower()) ||
                           (x.Email != null && x.Email.ToLower().Contains(specParams.Search.ToLower())) ||
                           (x.Address != null && x.Address.ToLower().Contains(specParams.Search.ToLower()))
                     ) && x.IsDeleted == false)
        {

            AddInclude(x => x.Ethnic);
            AddInclude(x => x.Religion);
            AddInclude(x => x.Ward);
            AddInclude(x => x.Province);
            AddInclude(x => x.BurialWard);
            AddInclude(x => x.BurialProvince);
            AddInclude(x => x.FTMemberFiles);

            ApplyPaging(specParams.Skip, specParams.Take);

            if (!string.IsNullOrEmpty(specParams.OrderBy))
                AddOrderBy(specParams.OrderBy);

            ApplyFilter(specParams.PropertyFilters);
        }
    }
}
