using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.Events;
using FTM.Domain.Specification;
using System.Linq;
using System.Linq.Expressions;

namespace FTM.Domain.Specification.FamilyTrees
{
    public class FTFamilyEventSpecification : BaseSpecifcation<FTFamilyEvent>
    {
        public FTFamilyEventSpecification()
        {
            AddInclude(e => e.EventMembers);
            AddInclude(e => e.EventFTs);
            AddOrderBy(e => e.StartTime);
        }
    }

    public class FTFamilyEventByIdSpecification : BaseSpecifcation<FTFamilyEvent>
    {
        public FTFamilyEventByIdSpecification(Guid eventId)
            : base(e => e.Id == eventId && e.IsDeleted == false)
        {
            AddInclude(e => e.FT);
            AddInclude(e => e.EventMembers);
            AddInclude(e => e.EventFTs);
        }
    }

    public class FTFamilyEventByFTIdSpecification : BaseSpecifcation<FTFamilyEvent>
    {
        public FTFamilyEventByFTIdSpecification(Guid ftId, int skip = 0, int take = 20)
            : base(e => e.FTId == ftId && e.IsDeleted == false)
        {
            AddInclude(e => e.EventMembers);
            AddOrderBy(e => e.StartTime);
            ApplyPaging(skip, take);
        }
    }

    public class FTFamilyEventByDateRangeSpecification : BaseSpecifcation<FTFamilyEvent>
    {
        public FTFamilyEventByDateRangeSpecification(Guid ftId, DateTimeOffset startDate, DateTimeOffset endDate)
            : base(e => e.FTId == ftId && e.IsDeleted == false
                     && e.StartTime >= startDate && e.StartTime <= endDate)
        {
            AddInclude(e => e.EventMembers);
            AddOrderBy(e => e.StartTime);
        }
    }

    public class FTFamilyEventUpcomingSpecification : BaseSpecifcation<FTFamilyEvent>
    {
        public FTFamilyEventUpcomingSpecification(Guid ftId, DateTimeOffset now, DateTimeOffset endDate)
            : base(e => e.FTId == ftId && e.IsDeleted == false
                     && e.StartTime >= now && e.StartTime <= endDate)
        {
            AddInclude(e => e.EventMembers);
            AddOrderBy(e => e.StartTime);
        }
    }

    public class FTFamilyEventByMemberSpecification : BaseSpecifcation<FTFamilyEvent>
    {
        public FTFamilyEventByMemberSpecification(Guid memberId, int skip = 0, int take = 20)
            : base(e => e.EventMembers.Any(em => em.FTMemberId == memberId && em.IsDeleted == false)
                         && e.IsDeleted == false)
        {
            AddInclude(e => e.EventMembers);
            AddOrderBy(e => e.StartTime);
            ApplyPaging(skip, take);
        }
    }

    public class FTFamilyEventFilterSpecification : BaseSpecifcation<FTFamilyEvent>
    {
        public FTFamilyEventFilterSpecification(EventFilterRequest request)
            : base(BuildCriteria(request))
        {
            AddInclude(e => e.EventMembers);
            AddOrderBy(e => e.StartTime);
            ApplyPaging(request.Skip ?? 0, request.Take ?? 20);
        }

        private static Expression<Func<FTFamilyEvent, bool>> BuildCriteria(EventFilterRequest request)
        {
            Expression<Func<FTFamilyEvent, bool>> criteria = e => e.FTId == request.FTId && e.IsDeleted == false;

            if (request.StartDate.HasValue)
            {
                var startDateCriteria = (Expression<Func<FTFamilyEvent, bool>>)(e => e.StartTime >= request.StartDate.Value);
                criteria = CombineExpressions(criteria, startDateCriteria);
            }

            if (request.EndDate.HasValue)
            {
                var endDateCriteria = (Expression<Func<FTFamilyEvent, bool>>)(e => e.StartTime <= request.EndDate.Value);
                criteria = CombineExpressions(criteria, endDateCriteria);
            }

            if (!string.IsNullOrEmpty(request.EventType))
            {
                var eventType = Enum.Parse<FTM.Domain.Enums.EventType>(request.EventType);
                var eventTypeCriteria = (Expression<Func<FTFamilyEvent, bool>>)(e => e.EventType == eventType);
                criteria = CombineExpressions(criteria, eventTypeCriteria);
            }

            if (request.IsLunar.HasValue)
            {
                var isLunarCriteria = (Expression<Func<FTFamilyEvent, bool>>)(e => e.IsLunar == request.IsLunar.Value);
                criteria = CombineExpressions(criteria, isLunarCriteria);
            }

            return criteria;
        }

        private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }
    }

    public class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}