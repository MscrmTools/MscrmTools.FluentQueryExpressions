using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MscrmTools.FluentQueryExpressions.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MscrmTools.FluentQueryExpressions
{
    public class Query : Query<Entity>
    {
        public Query(string entityLogicalName) : base(entityLogicalName)
        {
        }
    }

    public class Query<T> where T : Entity
    {
        public Query()
        {
            string entityLogicalName = typeof(T).GetField("EntityLogicalName").GetRawConstantValue().ToString();
            QueryExpression = new QueryExpression(entityLogicalName);
        }

        protected Query(string entityLogicalName)
        {
            QueryExpression = new QueryExpression(entityLogicalName);
        }

        public QueryExpression QueryExpression { get; protected set; }

        #region QueryExpression

        public Query<T> Distinct()
        {
            QueryExpression.Distinct = true;

            return this;
        }

        public Query<T> NoLock()
        {
            QueryExpression.NoLock = true;

            return this;
        }

        public Query<T> Top(int top)
        {
            QueryExpression.TopCount = top;

            return this;
        }

        #endregion QueryExpression

        #region Attributes

        public Query<T> Select(bool allColumns)
        {
            QueryExpression.ColumnSet = new ColumnSet(allColumns);

            return this;
        }

        public Query<T> Select(params string[] attributes)
        {
            if (attributes.Length == 0)
            {
                QueryExpression.ColumnSet = new ColumnSet();
            }

            QueryExpression.ColumnSet.AddColumns(attributes);

            return this;
        }

        public Query<T> Select(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.ColumnSet.AddColumns(AnonymousTypeHelper.GetAttributeNamesArray(anonymousTypeInitializer));
            return this;
        }

        #endregion Attributes

        #region Filters

        public Query<T> AddFilters(params Filter[] filters)
        {
            QueryExpression.Criteria.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        public Query<T> AddFilters(LogicalOperator logicalOperator, params Filter[] filters)
        {
            QueryExpression.Criteria.FilterOperator = logicalOperator;
            QueryExpression.Criteria.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        public Query<T> SetDefaultFilterOperator(LogicalOperator logicalOperator)
        {
            QueryExpression.Criteria.FilterOperator = logicalOperator;

            return this;
        }

        #endregion Filters

        #region Columns Comparer

#if CRMV9
        public Shared.AppCode.Comparer<Query<T>> Compare(string attributeName)
        {
            return new Shared.AppCode.Comparer<Query<T>>(this, attributeName);
        }

        public Shared.AppCode.Comparer<Query<T>> Compare(string entityName, string attributeName)
        {
            return new Shared.AppCode.Comparer<Query<T>>(this, entityName, attributeName);
        }
#endif

        #endregion Columns Comparer

        #region Link Entities

        public Query<T> AddLink<TU>(Link<TU> link) where TU : Entity
        {
            QueryExpression.LinkEntities.Add(link.InnerLinkEntity);
            link.InnerLinkEntity.LinkFromEntityName = QueryExpression.EntityName;
            return this;
        }

        #endregion Link Entities

        #region Order

        public Query<T> Order(string attribute, OrderType order)
        {
            QueryExpression.AddOrder(attribute, order);
            return this;
        }

        #endregion Order

        #region PagingInfo

        public Query<T> NextPage(string pagingCookie)
        {
            QueryExpression.PageInfo.PageNumber++;
            QueryExpression.PageInfo.PagingCookie = pagingCookie;

            return this;
        }

        public Query<T> SetPagingInfo(int pageNumber, int count, bool returnTotalCount = false)
        {
            QueryExpression.PageInfo = new PagingInfo
            {
                Count = count,
                PageNumber = pageNumber,
                ReturnTotalRecordCount = returnTotalCount
            };

            return this;
        }

        #endregion PagingInfo

        #region Conditions

        public Query<T> Where(string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(attributeName, conditionOperator, values);

            return this;
        }

        public Query<T> Where(Expression<Func<T, object>> anonymousTypeInitializer, ConditionOperator conditionOperator, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName<T>(anonymousTypeInitializer), conditionOperator, values);

            return this;
        }

        public Query<T> Where(string entityName, string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(entityName, attributeName, conditionOperator, values);

            return this;
        }

        public Query<T> Where<U>(string entityName, Expression<Func<U, object>> anonymousTypeInitializer, ConditionOperator conditionOperator, params object[] values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityName, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), conditionOperator, values);

            return this;
        }

        public Query<T> WhereAbove(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Above, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Above, value);
            }

            return this;
        }

        public Query<T> WhereAbove(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Above, value);

            return this;
        }

        public Query<T> WhereAbove<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Above, value);

            return this;
        }

        public Query<T> WhereAboveOrEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.AboveOrEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.AboveOrEqual, value);
            }

            return this;
        }

        public Query<T> WhereAboveOrEqual(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.AboveOrEqual, value);

            return this;
        }

        public Query<T> WhereAboveOrEqual<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.AboveOrEqual, value);

            return this;
        }

        public Query<T> WhereBeginsWith(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.BeginsWith, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.BeginsWith, value);
            }

            return this;
        }

        public Query<T> WhereBeginsWith(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.BeginsWith, value);

            return this;
        }

        public Query<T> WhereBeginsWith<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.BeginsWith, value);

            return this;
        }

        public Query<T> WhereBetween(string attributeName, object value1, object value2, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Between, value1, value2);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Between, value1, value2);
            }

            return this;
        }

        public Query<T> WhereBetween(Expression<Func<T, object>> anonymousTypeInitializer, object value1, object value2)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Between, value1, value2);

            return this;
        }

        public Query<T> WhereBetween<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value1, object value2) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Between, value1, value2);

            return this;
        }

        public Query<T> WhereChildOf(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.ChildOf, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.ChildOf, value);
            }

            return this;
        }

        public Query<T> WhereChildOf(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ChildOf, value);

            return this;
        }

        public Query<T> WhereChildOf<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ChildOf, value);

            return this;
        }

        public Query<T> WhereContains(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Contains, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Contains, value);
            }

            return this;
        }

        public Query<T> WhereContains(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Contains, value);

            return this;
        }

        public Query<T> WhereContains<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Contains, value);

            return this;
        }

#if CRMV9

        public Query<T> WhereContainValues(string attributeName, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.ContainValues, values);

            return this;
        }

        public Query<T> WhereContainValues(string entityname, string attributeName, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.ContainValues, values);

            return this;
        }

        public Query<T> WhereContainValues(Expression<Func<T, object>> anonymousTypeInitializer, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ContainValues, values);

            return this;
        }

        public Query<T> WhereContainValues<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, params object[] values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ContainValues, values);

            return this;
        }

#endif

        public Query<T> WhereDoesNotBeginWith(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.DoesNotBeginWith, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.DoesNotBeginWith, value);
            }

            return this;
        }

        public Query<T> WhereDoesNotBeginWith(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.DoesNotBeginWith, value);

            return this;
        }

        public Query<T> WhereDoesNotBeginWith<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.DoesNotBeginWith, value);

            return this;
        }

        public Query<T> WhereDoesNotContain(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.DoesNotContain, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.DoesNotContain, value);
            }

            return this;
        }

        public Query<T> WhereDoesNotContain(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.DoesNotContain, value);

            return this;
        }

        public Query<T> WhereDoesNotContain<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.DoesNotContain, value);

            return this;
        }

#if CRMV9
        public Query<T> WhereDoesNotContainValues(string attributeName, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.DoesNotContainValues, values);

            return this;
        }

        public Query<T> WhereDoesNotContainValues(string entityname, string attributeName, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.DoesNotContainValues, values);

            return this;
        }

        public Query<T> WhereDoesNotContainValues(Expression<Func<T, object>> anonymousTypeInitializer, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.DoesNotContainValues, values);

            return this;
        }

        public Query<T> WhereDoesNotContainValues<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, params object[] values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.DoesNotContainValues, values);

            return this;
        }
#endif
        public Query<T> WhereDoesNotEndWith(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.DoesNotEndWith, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.DoesNotEndWith, value);
            }

            return this;
        }

        public Query<T> WhereDoesNotEndWith(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.DoesNotEndWith, value);

            return this;
        }

        public Query<T> WhereDoesNotEndWith<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.DoesNotEndWith, value);

            return this;
        }

        public Query<T> WhereEndsWith(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.EndsWith, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.EndsWith, value);
            }

            return this;
        }

        public Query<T> WhereEndsWith(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EndsWith, value);

            return this;
        }

        public Query<T> WhereEndsWith<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EndsWith, value);

            return this;
        }

        public Query<T> WhereEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Equal, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Equal, value);
            }

            return this;
        }

        public Query<T> WhereEqual(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Equal, value);

            return this;
        }

        public Query<T> WhereEqual<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Equal, value);

            return this;
        }

        public Query<T> WhereEqualBusinessId(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.EqualBusinessId);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.EqualBusinessId);
            }

            return this;
        }

        public Query<T> WhereEqualBusinessId(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualBusinessId);

            return this;
        }

        public Query<T> WhereEqualBusinessId<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualBusinessId);

            return this;
        }

        public Query<T> WhereEqualUserId(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.EqualUserId);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.EqualUserId);
            }

            return this;
        }

        public Query<T> WhereEqualUserId(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserId);

            return this;
        }

        public Query<T> WhereEqualUserId<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserId);

            return this;
        }

        public Query<T> WhereEqualUserLanguage(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.EqualUserLanguage);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.EqualUserLanguage);
            }

            return this;
        }

        public Query<T> WhereEqualUserLanguage(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserLanguage);

            return this;
        }

        public Query<T> WhereEqualUserLanguage<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserLanguage);

            return this;
        }

        public Query<T> WhereEqualUserOrUserHierarchy(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.EqualUserOrUserHierarchy);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.EqualUserOrUserHierarchy);
            }

            return this;
        }

        public Query<T> WhereEqualUserOrUserHierarchy(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserOrUserHierarchy);

            return this;
        }

        public Query<T> WhereEqualUserOrUserHierarchy<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserOrUserHierarchy);

            return this;
        }

        public Query<T> WhereEqualUserOrUserHierarchyAndTeams(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName,
                    ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            }

            return this;
        }

        public Query<T> WhereEqualUserOrUserHierarchyAndTeams(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserOrUserHierarchyAndTeams);

            return this;
        }

        public Query<T> WhereEqualUserOrUserHierarchyAndTeams<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserOrUserHierarchyAndTeams);

            return this;
        }

        public Query<T> WhereEqualUserOrUserTeams(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.EqualUserOrUserTeams);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.EqualUserOrUserTeams);
            }

            return this;
        }

        public Query<T> WhereEqualUserOrUserTeams(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserOrUserTeams);

            return this;
        }

        public Query<T> WhereEqualUserOrUserTeams<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserOrUserTeams);

            return this;
        }

        public Query<T> WhereEqualUserTeams(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.EqualUserTeams);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.EqualUserTeams);
            }

            return this;
        }

        public Query<T> WhereEqualUserTeams(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserTeams);

            return this;
        }

        public Query<T> WhereEqualUserTeams<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.EqualUserTeams);

            return this;
        }

        public Query<T> WhereGreaterEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.GreaterEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.GreaterEqual, value);
            }

            return this;
        }

        public Query<T> WhereGreaterEqual(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.GreaterEqual, value);

            return this;
        }

        public Query<T> WhereGreaterEqual<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.GreaterEqual, value);

            return this;
        }

        public Query<T> WhereGreaterThan(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.GreaterThan, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.GreaterThan, value);
            }

            return this;
        }

        public Query<T> WhereGreaterThan(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.GreaterThan, value);

            return this;
        }

        public Query<T> WhereGreaterThan<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.GreaterThan, value);

            return this;
        }

        public Query<T> WhereIn(string attributeName, IList value)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(attributeName, ConditionOperator.In, value));

            return this;
        }

        public Query<T> WhereIn(string entityname, string attributeName, IList value)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(entityname, attributeName, ConditionOperator.In, value));

            return this;
        }

        public Query<T> WhereIn(Expression<Func<T, object>> anonymousTypeInitializer, IList value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.In, value);

            return this;
        }

        public Query<T> WhereIn<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, IList value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.In, value);

            return this;
        }

        public Query<T> WhereIn(string attributeName, params object[] values)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(attributeName, ConditionOperator.In, values));

            return this;
        }

        public Query<T> WhereIn(string entityname, string attributeName, params object[] values)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(entityname, attributeName, ConditionOperator.In, values));

            return this;
        }

        public Query<T> WhereIn(Expression<Func<T, object>> anonymousTypeInitializer, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.In, values);

            return this;
        }

        public Query<T> WhereIn<U>(string entityname, Expression<Func<T, object>> anonymousTypeInitializer, params object[] values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.In, values);

            return this;
        }

        public Query<T> WhereInFiscalPeriod(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.InFiscalPeriod, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.InFiscalPeriod, value);
            }

            return this;
        }

        public Query<T> WhereInFiscalPeriod(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InFiscalPeriod, value);

            return this;
        }

        public Query<T> WhereInFiscalPeriod<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InFiscalPeriod, value);

            return this;
        }

        public Query<T> WhereInFiscalPeriodAndYear(string attributeName, int period, int year, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.InFiscalPeriodAndYear, period, year);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.InFiscalPeriodAndYear, period, year);
            }

            return this;
        }

        public Query<T> WhereInFiscalPeriodAndYear(Expression<Func<T, object>> anonymousTypeInitializer, int period, int year)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InFiscalPeriodAndYear, period, year);

            return this;
        }

        public Query<T> WhereInFiscalPeriodAndYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int period, int year) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InFiscalPeriodAndYear, period, year);

            return this;
        }

        public Query<T> WhereInFiscalYear(string attributeName, int year, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.InFiscalYear, year);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.InFiscalYear, year);
            }

            return this;
        }

        public Query<T> WhereInFiscalYear(Expression<Func<T, object>> anonymousTypeInitializer, int year)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InFiscalYear, year);

            return this;
        }

        public Query<T> WhereInFiscalYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int year) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InFiscalYear, year);

            return this;
        }

        public Query<T> WhereInOrAfterFiscalPeriodAndYear(string attributeName, int period, int year, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.InOrAfterFiscalPeriodAndYear,
                    period, year);
            }

            return this;
        }

        public Query<T> WhereInOrAfterFiscalPeriodAndYear(Expression<Func<T, object>> anonymousTypeInitializer, int period, int year)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);

            return this;
        }

        public Query<T> WhereInOrAfterFiscalPeriodAndYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int period, int year) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);

            return this;
        }

        public Query<T> WhereInOrBeforeFiscalPeriodAndYear(string attributeName, int period, int year, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.InOrBeforeFiscalPeriodAndYear,
                    period, year);
            }

            return this;
        }

        public Query<T> WhereInOrBeforeFiscalPeriodAndYear(Expression<Func<T, object>> anonymousTypeInitializer, int period, int year)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);

            return this;
        }

        public Query<T> WhereInOrBeforeFiscalPeriodAndYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int period, int year) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);

            return this;
        }

        public Query<T> WhereLast7Days(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Last7Days);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Last7Days);
            }

            return this;
        }

        public Query<T> WhereLast7Days(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Last7Days);

            return this;
        }

        public Query<T> WhereLast7Days<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Last7Days);

            return this;
        }

        public Query<T> WhereLastFiscalPeriod(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastFiscalPeriod);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastFiscalPeriod);
            }

            return this;
        }

        public Query<T> WhereLastFiscalPeriod(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastFiscalPeriod);

            return this;
        }

        public Query<T> WhereLastFiscalPeriod<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastFiscalPeriod);

            return this;
        }

        public Query<T> WhereLastFiscalYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastFiscalYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastFiscalYear);
            }

            return this;
        }

        public Query<T> WhereLastFiscalYear(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastFiscalYear);

            return this;
        }

        public Query<T> WhereLastFiscalYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastFiscalYear);

            return this;
        }

        public Query<T> WhereLastMonth(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastMonth);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastMonth);
            }

            return this;
        }

        public Query<T> WhereLastMonth(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastMonth);

            return this;
        }

        public Query<T> WhereLastMonth<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastMonth);

            return this;
        }

        public Query<T> WhereLastWeek(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastWeek);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastWeek);
            }

            return this;
        }

        public Query<T> WhereLastWeek(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastWeek);

            return this;
        }

        public Query<T> WhereLastWeek<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastWeek);

            return this;
        }

        public Query<T> WhereLastXDays(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastXDays, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastXDays, value);
            }

            return this;
        }

        public Query<T> WhereLastXDays(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXDays, value);

            return this;
        }

        public Query<T> WhereLastXDays<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXDays, value);

            return this;
        }

        public Query<T> WhereLastXFiscalPeriods(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastXFiscalPeriods, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastXFiscalPeriods, value);
            }

            return this;
        }
        public Query<T> WhereLastXFiscalPeriods(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXFiscalPeriods, value);

            return this;
        }

        public Query<T> WhereLastXFiscalPeriods<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXFiscalPeriods, value);

            return this;
        }

        public Query<T> WhereLastXFiscalYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastXFiscalYears, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastXFiscalYears, value);
            }

            return this;
        }

        public Query<T> WhereLastXFiscalYears(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXFiscalYears, value);

            return this;
        }

        public Query<T> WhereLastXFiscalYears<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXFiscalYears, value);

            return this;
        }

        public Query<T> WhereLastXHours(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastXHours, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastXHours, value);
            }

            return this;
        }

        public Query<T> WhereLastXHours(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXHours, value);

            return this;
        }

        public Query<T> WhereLastXHours<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXHours, value);

            return this;
        }

        public Query<T> WhereLastXMonths(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastXMonths, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastXMonths, value);
            }

            return this;
        }

        public Query<T> WhereLastXMonths(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXMonths, value);

            return this;
        }

        public Query<T> WhereLastXMonths<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXMonths, value);

            return this;
        }

        public Query<T> WhereLastXWeeks(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastXWeeks, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastXWeeks, value);
            }

            return this;
        }

        public Query<T> WhereLastXWeeks(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXWeeks, value);

            return this;
        }

        public Query<T> WhereLastXWeeks<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXWeeks, value);

            return this;
        }

        public Query<T> WhereLastXYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastXYears, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastXYears, value);
            }

            return this;
        }

        public Query<T> WhereLastXYears(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXYears, value);

            return this;
        }

        public Query<T> WhereLastXYears<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastXYears, value);

            return this;
        }

        public Query<T> WhereLastYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LastYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LastYear);
            }

            return this;
        }

        public Query<T> WhereLastYear(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastYear);

            return this;
        }

        public Query<T> WhereLastYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LastYear);

            return this;
        }

        public Query<T> WhereLessEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LessEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LessEqual, value);
            }

            return this;
        }

        public Query<T> WhereLessEqual(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LessEqual, value);

            return this;
        }

        public Query<T> WhereLessEqual<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LessEqual, value);

            return this;
        }

        public Query<T> WhereLessThan(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.LessThan, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.LessThan, value);
            }

            return this;
        }

        public Query<T> WhereLessThan(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LessThan, value);

            return this;
        }

        public Query<T> WhereLessThan<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.LessThan, value);

            return this;
        }

        public Query<T> WhereLike(string attributeName, string value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Like, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Like, value);
            }

            return this;
        }

        public Query<T> WhereLike(Expression<Func<T, object>> anonymousTypeInitializer, string value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Like, value);

            return this;
        }

        public Query<T> WhereLike<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, string value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Like, value);

            return this;
        }

        public Query<T> WhereMask(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Mask, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Mask, value);
            }

            return this;
        }

        public Query<T> WhereMask(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Mask, value);

            return this;
        }

        public Query<T> WhereMask<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Mask, value);

            return this;
        }

        public Query<T> WhereMasksSelect(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.MasksSelect, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.MasksSelect, value);
            }

            return this;
        }

        public Query<T> WhereMasksSelect(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.MasksSelect, value);

            return this;
        }

        public Query<T> WhereMasksSelect<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.MasksSelect, value);

            return this;
        }

        public Query<T> WhereNext7Days(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Next7Days);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Next7Days);
            }

            return this;
        }

        public Query<T> WhereNext7Days(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Next7Days);

            return this;
        }

        public Query<T> WhereNext7Days<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Next7Days);

            return this;
        }

        public Query<T> WhereNextFiscalPeriod(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextFiscalPeriod);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextFiscalPeriod);
            }

            return this;
        }

        public Query<T> WhereNextFiscalPeriod(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextFiscalPeriod);

            return this;
        }

        public Query<T> WhereNextFiscalPeriod<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextFiscalPeriod);

            return this;
        }

        public Query<T> WhereNextFiscalYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextFiscalYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextFiscalYear);
            }

            return this;
        }

        public Query<T> WhereNextFiscalYear(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextFiscalYear);

            return this;
        }

        public Query<T> WhereNextFiscalYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextFiscalYear);

            return this;
        }

        public Query<T> WhereNextMonth(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextMonth);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextMonth);
            }

            return this;
        }

        public Query<T> WhereNextMonth(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextMonth);

            return this;
        }

        public Query<T> WhereNextMonth<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextMonth);

            return this;
        }

        public Query<T> WhereNextWeek(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextWeek);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextWeek);
            }

            return this;
        }

        public Query<T> WhereNextWeek(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextWeek);

            return this;
        }

        public Query<T> WhereNextWeek<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextWeek);

            return this;
        }

        public Query<T> WhereNextXDays(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextXDays, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextXDays, value);
            }

            return this;
        }

        public Query<T> WhereNextXDays(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXDays, value);

            return this;
        }

        public Query<T> WhereNextXDays<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXDays, value);

            return this;
        }

        public Query<T> WhereNextXFiscalPeriods(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextXFiscalPeriods, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextXFiscalPeriods, value);
            }

            return this;
        }

        public Query<T> WhereNextXFiscalPeriods(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXFiscalPeriods, value);

            return this;
        }

        public Query<T> WhereNextXFiscalPeriods<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXFiscalPeriods, value);

            return this;
        }

        public Query<T> WhereNextXFiscalYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextXFiscalYears, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextXFiscalYears, value);
            }

            return this;
        }

        public Query<T> WhereNextXFiscalYears(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXFiscalYears, value);

            return this;
        }

        public Query<T> WhereNextXFiscalYears<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXFiscalYears, value);

            return this;
        }

        public Query<T> WhereNextXHours(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextXHours, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextXHours, value);
            }

            return this;
        }

        public Query<T> WhereNextXHours(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXHours, value);

            return this;
        }

        public Query<T> WhereNextXHours<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXHours, value);

            return this;
        }

        public Query<T> WhereNextXMonths(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextXMonths, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextXMonths, value);
            }

            return this;
        }

        public Query<T> WhereNextXMonths(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXMonths, value);

            return this;
        }

        public Query<T> WhereNextXMonths<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXMonths, value);

            return this;
        }

        public Query<T> WhereNextXWeeks(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextXWeeks, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextXWeeks, value);
            }

            return this;
        }

        public Query<T> WhereNextXWeeks(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXWeeks, value);

            return this;
        }

        public Query<T> WhereNextXWeeks<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXWeeks, value);

            return this;
        }

        public Query<T> WhereNextXYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextXYears, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextXYears, value);
            }

            return this;
        }

        public Query<T> WhereNextXYears(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXYears, value);

            return this;
        }

        public Query<T> WhereNextXYears<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextXYears, value);

            return this;
        }

        public Query<T> WhereNextYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NextYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NextYear);
            }

            return this;
        }

        public Query<T> WhereNextYear(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextYear);

            return this;
        }

        public Query<T> WhereNextYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NextYear);

            return this;
        }

        public Query<T> WhereNotBetween(string attributeName, object value1, object value2, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotBetween, value1, value2);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotBetween, value1, value2);
            }

            return this;
        }

        public Query<T> WhereNotBetween(Expression<Func<T, object>> anonymousTypeInitializer, object value1, object value2)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotBetween, value1, value2);

            return this;
        }

        public Query<T> WhereNotBetween<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value1, object value2) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotBetween, value1, value2);

            return this;
        }

        public Query<T> WhereNotEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotEqual, value);
            }

            return this;
        }

        public Query<T> WhereNotEqual(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotEqual, value);

            return this;
        }

        public Query<T> WhereNotEqual<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotEqual, value);

            return this;
        }

        public Query<T> WhereNotEqualBusinessId(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotEqualBusinessId);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotEqualBusinessId);
            }

            return this;
        }

        public Query<T> WhereNotEqualBusinessId(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotEqualBusinessId);

            return this;
        }

        public Query<T> WhereNotEqualBusinessId<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotEqualBusinessId);

            return this;
        }

        public Query<T> WhereNotEqualUserId(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotEqualUserId);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotEqualUserId);
            }

            return this;
        }

        public Query<T> WhereNotEqualUserId(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotEqualUserId);

            return this;
        }

        public Query<T> WhereNotEqualUserId<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotEqualUserId);

            return this;
        }

        public Query<T> WhereNotIn(string entityname, string attributeName, IList value)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(entityname, attributeName, ConditionOperator.NotIn, value));

            return this;
        }

        public Query<T> WhereNotIn(string attributeName, IList value)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(attributeName, ConditionOperator.NotIn, value));

            return this;
        }

        public Query<T> WhereNotIn(Expression<Func<T, object>> anonymousTypeInitializer, IList value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotIn, value);

            return this;
        }

        public Query<T> WhereNotIn<U>(string entityname, Expression<Func<T, object>> anonymousTypeInitializer, IList value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotIn, value);

            return this;
        }

        public Query<T> WhereNotIn(string attributeName, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotIn, values);

            return this;
        }

        public Query<T> WhereNotIn(string entityname, string attributeName, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotIn, values);

            return this;
        }

        public Query<T> WhereNotIn(Expression<Func<T, object>> anonymousTypeInitializer, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotIn, values);

            return this;
        }

        public Query<T> WhereNotIn<U>(string entityname, Expression<Func<T, object>> anonymousTypeInitializer, params object[] values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotIn, values);

            return this;
        }

        public Query<T> WhereNotLike(string attributeName, string value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotLike, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotLike, value);
            }

            return this;
        }

        public Query<T> WhereNotLike(Expression<Func<T, object>> anonymousTypeInitializer, string value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotLike, value);

            return this;
        }

        public Query<T> WhereNotLike<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, string value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotLike, value);

            return this;
        }

        public Query<T> WhereNotMask(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotMask, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotMask, value);
            }

            return this;
        }

        public Query<T> WhereNotMask(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotMask, value);

            return this;
        }

        public Query<T> WhereNotMask<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotMask, value);

            return this;
        }

        public Query<T> WhereNotNull(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotNull);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotNull);
            }

            return this;
        }

        public Query<T> WhereNotNull(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotNull);

            return this;
        }

        public Query<T> WhereNotNull<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotNull);

            return this;
        }

        public Query<T> WhereNotOn(string attributeName, DateTime value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotOn, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotOn, value);
            }

            return this;
        }

        public Query<T> WhereNotOn(Expression<Func<T, object>> anonymousTypeInitializer, DateTime value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotOn, value);

            return this;
        }

        public Query<T> WhereNotOn<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, DateTime value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotOn, value);

            return this;
        }

        public Query<T> WhereNotUnder(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.NotUnder, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.NotUnder, value);
            }

            return this;
        }

        public Query<T> WhereNotUnder(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotUnder, value);

            return this;
        }

        public Query<T> WhereNotUnder<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.NotUnder, value);

            return this;
        }

        public Query<T> WhereNull(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Null);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Null);
            }

            return this;
        }

        public Query<T> WhereNull(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Null);

            return this;
        }

        public Query<T> WhereNull<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Null);

            return this;
        }

        public Query<T> WhereOlderThanXDays(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXDays, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.OlderThanXDays, value);
            }

            return this;
        }

        public Query<T> WhereOlderThanXDays(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXDays, value);

            return this;
        }

        public Query<T> WhereOlderThanXDays<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXDays, value);

            return this;
        }

        public Query<T> WhereOlderThanXHours(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXHours, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.OlderThanXHours, value);
            }

            return this;
        }

        public Query<T> WhereOlderThanXHours(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXHours, value);

            return this;
        }

        public Query<T> WhereOlderThanXHours<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXHours, value);

            return this;
        }

        public Query<T> WhereOlderThanXMinutes(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXMinutes, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.OlderThanXMinutes, value);
            }

            return this;
        }

        public Query<T> WhereOlderThanXMinutes(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXMinutes, value);

            return this;
        }

        public Query<T> WhereOlderThanXMinutes<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXMinutes, value);

            return this;
        }

        public Query<T> WhereOlderThanXMonths(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXMonths, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.OlderThanXMonths, value);
            }

            return this;
        }

        public Query<T> WhereOlderThanXMonths(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXMonths, value);

            return this;
        }

        public Query<T> WhereOlderThanXMonths<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXMonths, value);

            return this;
        }

        public Query<T> WhereOlderThanXWeeks(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXWeeks, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.OlderThanXWeeks, value);
            }

            return this;
        }

        public Query<T> WhereOlderThanXWeeks(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXWeeks, value);

            return this;
        }

        public Query<T> WhereOlderThanXWeeks<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXWeeks, value);

            return this;
        }

        public Query<T> WhereOlderThanXYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXYears, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.OlderThanXYears, value);
            }

            return this;
        }

        public Query<T> WhereOlderThanXYears(Expression<Func<T, object>> anonymousTypeInitializer, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXYears, value);

            return this;
        }

        public Query<T> WhereOlderThanXYears<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OlderThanXYears, value);

            return this;
        }

        public Query<T> WhereOn(string attributeName, DateTime value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.On, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.On, value);
            }

            return this;
        }

        public Query<T> WhereOn(Expression<Func<T, object>> anonymousTypeInitializer, DateTime value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.On, value);

            return this;
        }

        public Query<T> WhereOn<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, DateTime value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.On, value);

            return this;
        }

        public Query<T> WhereOnOrAfter(string attributeName, DateTime value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.OnOrAfter, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.OnOrAfter, value);
            }

            return this;
        }

        public Query<T> WhereOnOrAfter(Expression<Func<T, object>> anonymousTypeInitializer, DateTime value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OnOrAfter, value);

            return this;
        }

        public Query<T> WhereOnOrAfter<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, DateTime value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OnOrAfter, value);

            return this;
        }

        public Query<T> WhereOnOrBefore(string attributeName, DateTime value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.OnOrBefore, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.OnOrBefore, value);
            }

            return this;
        }

        public Query<T> WhereOnOrBefore(Expression<Func<T, object>> anonymousTypeInitializer, DateTime value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OnOrBefore, value);

            return this;
        }

        public Query<T> WhereOnOrBefore<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, DateTime value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.OnOrBefore, value);

            return this;
        }

        public Query<T> WhereThisFiscalPeriod(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.ThisFiscalPeriod);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.ThisFiscalPeriod);
            }

            return this;
        }

        public Query<T> WhereThisFiscalPeriod(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisFiscalPeriod);

            return this;
        }

        public Query<T> WhereThisFiscalPeriod<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisFiscalPeriod);

            return this;
        }

        public Query<T> WhereThisFiscalYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.ThisFiscalYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.ThisFiscalYear);
            }

            return this;
        }

        public Query<T> WhereThisFiscalYear(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisFiscalYear);

            return this;
        }

        public Query<T> WhereThisFiscalYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisFiscalYear);

            return this;
        }

        public Query<T> WhereThisMonth(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.ThisMonth);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.ThisMonth);
            }

            return this;
        }

        public Query<T> WhereThisMonth(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisMonth);

            return this;
        }

        public Query<T> WhereThisMonth<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisMonth);

            return this;
        }

        public Query<T> WhereThisWeek(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.ThisWeek);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.ThisWeek);
            }

            return this;
        }

        public Query<T> WhereThisWeek(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisWeek);

            return this;
        }

        public Query<T> WhereThisWeek<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisWeek);

            return this;
        }

        public Query<T> WhereThisYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.ThisYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.ThisYear);
            }

            return this;
        }

        public Query<T> WhereThisYear(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisYear);

            return this;
        }

        public Query<T> WhereThisYear<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.ThisYear);

            return this;
        }

        public Query<T> WhereToday(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Today);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Today);
            }

            return this;
        }

        public Query<T> WhereToday(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Today);

            return this;
        }

        public Query<T> WhereToday<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Today);

            return this;
        }

        public Query<T> WhereTomorrow(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Tomorrow);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Tomorrow);
            }

            return this;
        }

        public Query<T> WhereTomorrow(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Tomorrow);

            return this;
        }

        public Query<T> WhereTomorrow<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Tomorrow);

            return this;
        }

        public Query<T> WhereUnder(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Under, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Under, value);
            }

            return this;
        }

        public Query<T> WhereUnder(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Under, value);

            return this;
        }

        public Query<T> WhereUnder<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Under, value);

            return this;
        }

        public Query<T> WhereUnderOrEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.UnderOrEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.UnderOrEqual, value);
            }

            return this;
        }

        public Query<T> WhereUnderOrEqual(Expression<Func<T, object>> anonymousTypeInitializer, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.UnderOrEqual, value);

            return this;
        }

        public Query<T> WhereUnderOrEqual<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.UnderOrEqual, value);

            return this;
        }

        public Query<T> WhereYesterday(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.Yesterday);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.Yesterday);
            }

            return this;
        }

        public Query<T> WhereYesterday(Expression<Func<T, object>> anonymousTypeInitializer)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Yesterday);

            return this;
        }

        public Query<T> WhereYesterday<U>(string entityname, Expression<Func<U, object>> anonymousTypeInitializer) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(entityname, AnonymousTypeHelper.GetAttributeName(anonymousTypeInitializer), ConditionOperator.Yesterday);

            return this;
        }

        #endregion Conditions

        #region Service calls

        public List<T> GetAll(IOrganizationService service)
        {
            if (QueryExpression.TopCount.HasValue)
            {
                return service.RetrieveMultiple(QueryExpression)
                    .Entities
                    .Select(e => e.ToEntity<T>())
                    .ToList();
            }

            EntityCollection ec;
            var list = new List<T>();
            do
            {
                ec = service.RetrieveMultiple(QueryExpression);

                list.AddRange(ec.Entities.Select(e => e.ToEntity<T>()));

                NextPage(ec.PagingCookie);
            } while (ec.MoreRecords);

            return list;
        }

        public T GetFirst(IOrganizationService service)
        {
            return service.RetrieveMultiple(QueryExpression).Entities.First().ToEntity<T>();
        }

        public T GetFirstOrDefault(IOrganizationService service)
        {
            return service.RetrieveMultiple(QueryExpression).Entities.FirstOrDefault()?.ToEntity<T>();
        }

        public T GetLast(IOrganizationService service)
        {
            return service.RetrieveMultiple(QueryExpression).Entities.Last().ToEntity<T>();
        }

        public T GetLastOrDefault(IOrganizationService service)
        {
            return service.RetrieveMultiple(QueryExpression).Entities.LastOrDefault()?.ToEntity<T>();
        }

        public EntityCollection GetResults(IOrganizationService service)
        {
            return service.RetrieveMultiple(QueryExpression);
        }

        public T GetSingle(IOrganizationService service)
        {
            return service.RetrieveMultiple(QueryExpression).Entities.Single().ToEntity<T>();
        }

        public T GetSingleOrDefault(IOrganizationService service)
        {
            return service.RetrieveMultiple(QueryExpression).Entities.SingleOrDefault()?.ToEntity<T>();
        }

        public T GetById(Guid id, IOrganizationService service, bool isActivityEntity = false)
        {
            QueryExpression.Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(isActivityEntity ? "activityid" : QueryExpression.EntityName + "id", ConditionOperator.Equal, id)
                }
            };

            return GetFirstOrDefault(service);
        }

        #endregion Service calls
    }
}