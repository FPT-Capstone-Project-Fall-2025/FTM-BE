using FTM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace FTM.Domain.Specification
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.PropertyFilters != null && spec.PropertyFilters.Any())
            {
                var queries = string.Empty;

                foreach (var filter in spec.PropertyFilters)
                {
                    switch (filter.Operation.ToUpper())
                    {
                        case "EQUAL":
                            queries = $"{filter.Name} = @0";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "NOTEQUAL":
                            queries = $"{filter.Name} != @0";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "GREATER":
                            queries = $"{filter.Name} > @0";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "GREATEREQUAL":
                            queries = $"{filter.Name} >= @0";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "LESS":
                            queries = $"{filter.Name} < @0";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "LESSEQUAL":
                            queries = $"{filter.Name} <= @0";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "STARTSWITH":
                            queries = $"{filter.Name}.StartsWith(@0)";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "ENDSWITH":
                            queries = $"{filter.Name}.EndsWith(@0)";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "CONTAIN":
                            queries = $"{filter.Name}.Contains(@0)";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "NOTCONTAIN":
                            queries = $"!{filter.Name}.Contains(@0)";
                            query = query.Where(queries, filter.Value);
                            break;
                        case "DATEIN":
                            var date = DateTime.ParseExact(filter.Value.ToString(), "yyyy-M-d", null);
                            var today = date.Date;
                            var nextDay = today.AddDays(1);
                            queries = $"@0 <= {filter.Name} && {filter.Name} < @1";
                            query = query.Where(queries, today, nextDay);
                            break;
                        default:
                            throw new Exception($"{filter.Operation} is invalid filter.");
                    }
                }
            }
            if (spec.OrderByExpression != null)
            {
                query = query.OrderBy(spec.OrderByExpression);
            }

            if (spec.OrderByDescendingExpression != null)
            {
                query = query.OrderByDescending(spec.OrderByDescendingExpression);
            }

            if (!string.IsNullOrEmpty(spec.OrderBy))
            {
                query = query.OrderBy(spec.OrderBy);
            }

            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
