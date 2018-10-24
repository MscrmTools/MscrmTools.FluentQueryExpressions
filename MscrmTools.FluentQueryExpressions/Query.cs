using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Query<T> Where(string entityName, string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(entityName, attributeName, conditionOperator, values);

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

        public Query<T> WhereIn(string attributeName, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(attributeName, ConditionOperator.In, values);

            return this;
        }

        public Query<T> WhereIn(string entityname, string attributeName, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(entityname, attributeName, ConditionOperator.In, values);

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

        public Query<T> WhereLike(string attributeName, object value, string entityname = null)
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

        public Query<T> WhereNotLike(string attributeName, object value, string entityname = null)
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

        #endregion Service calls
    }
}