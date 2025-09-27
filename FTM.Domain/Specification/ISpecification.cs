using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderByExpression { get; }
        Expression<Func<T, object>> OrderByDescendingExpression { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
        PropertyFilter[] PropertyFilters { get; }
        string OrderBy { get; }
    }
}
