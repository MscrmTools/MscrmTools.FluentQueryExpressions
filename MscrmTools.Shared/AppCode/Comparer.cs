using Microsoft.Xrm.Sdk.Query;
using MscrmTools.FluentQueryExpressions;

namespace MscrmTools.Shared.AppCode
{
    public class Comparer<T>
    {
        private readonly T _parent;

        public Comparer(T parent, string attributeName)
        {
            _parent = parent;

            AttributeName = attributeName;
        }

        public Comparer(T parent, string entityName, string attributeName)
        {
            _parent = parent;

            AttributeName = attributeName;
            EntityName = entityName;
        }

        internal string AttributeName { get; set; }
        internal string EntityName { get; set; }

#if CRMV9

        public T Equal(string attributeName)
        {
            var ce = new ConditionExpression(AttributeName, ConditionOperator.Equal, true, attributeName);
            if (!string.IsNullOrEmpty(EntityName)) ce.EntityName = EntityName;

            return AddCondition(ce);
        }

        public T NotEqual(string attributeName)
        {
            var ce = new ConditionExpression(AttributeName, ConditionOperator.NotEqual, true, attributeName);
            if (!string.IsNullOrEmpty(EntityName)) ce.EntityName = EntityName;

            return AddCondition(ce);
        }

        public T GreaterThan(string attributeName)
        {
            var ce = new ConditionExpression(AttributeName, ConditionOperator.GreaterThan, true, attributeName);
            if (!string.IsNullOrEmpty(EntityName)) ce.EntityName = EntityName;

            return AddCondition(ce);
        }

        public T GreaterOrEqualThan(string attributeName)
        {
            var ce = new ConditionExpression(AttributeName, ConditionOperator.GreaterEqual, true, attributeName);
            if (!string.IsNullOrEmpty(EntityName)) ce.EntityName = EntityName;

            return AddCondition(ce);
        }

        public T LessThan(string attributeName)
        {
            var ce = new ConditionExpression(AttributeName, ConditionOperator.LessThan, true, attributeName);
            if (!string.IsNullOrEmpty(EntityName)) ce.EntityName = EntityName;

            return AddCondition(ce);
        }

        public T LessOrEqualThan(string attributeName)
        {
            var ce = new ConditionExpression(AttributeName, ConditionOperator.LessEqual, true, attributeName);
            if (!string.IsNullOrEmpty(EntityName)) ce.EntityName = EntityName;

            return AddCondition(ce);
        }

        private T AddCondition(ConditionExpression ce)
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Query<>)
                || typeof(T) == typeof(Query))
            {
                ((dynamic)_parent).QueryExpression.Criteria.AddCondition(ce);
            }
            else if (typeof(T) == typeof(Filter))
                ((Filter)(object)_parent).InnerFilter.AddCondition(ce);
            else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Link<>)
                     || typeof(T) == typeof(Link))
                ((dynamic)_parent).InnerLinkEntity.LinkCriteria.AddCondition(ce);

            return _parent;
        }

#endif
    }
}