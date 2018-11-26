using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;

namespace MscrmTools.FluentQueryExpressions
{
    public class Filter
    {
        public Filter(LogicalOperator logicalOperator = LogicalOperator.And)
        {
            InnerFilter = new FilterExpression(logicalOperator);
        }

        internal FilterExpression InnerFilter { get; }

        #region Conditions

        public Filter Where(string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            InnerFilter.AddCondition(attributeName, conditionOperator, values);

            return this;
        }

        public Filter Where(string entityName, string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            InnerFilter.AddCondition(entityName, attributeName, conditionOperator, values);

            return this;
        }

        public Filter WhereAbove(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Above, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Above, value);
            }

            return this;
        }

        public Filter WhereAboveOrEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.AboveOrEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.AboveOrEqual, value);
            }

            return this;
        }

        public Filter WhereBeginsWith(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.BeginsWith, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.BeginsWith, value);
            }

            return this;
        }

        public Filter WhereBetween(string attributeName, object value1, object value2, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Between, value1, value2);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Between, value1, value2);
            }

            return this;
        }

        public Filter WhereChildOf(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.ChildOf, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.ChildOf, value);
            }

            return this;
        }

        public Filter WhereContains(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Contains, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Contains, value);
            }

            return this;
        }
#if CRMV9
        public Filter WhereContainValues(string attributeName, params object[] values)
        {
            InnerFilter.AddCondition(attributeName, ConditionOperator.ContainValues, values);

            return this;
        }

        public Filter WhereContainValues(string entityname, string attributeName, params object[] values)
        {
            InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.ContainValues, values);

            return this;
        }
#endif

        public Filter WhereDoesNotBeginWith(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.DoesNotBeginWith, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.DoesNotBeginWith, value);
            }

            return this;
        }

        public Filter WhereDoesNotContain(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.DoesNotContain, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.DoesNotContain, value);
            }

            return this;
        }
#if CRMV9
        public Filter WhereDoesNotContainValues(string attributeName, params object[] values)
        {
            InnerFilter.AddCondition(attributeName, ConditionOperator.DoesNotContainValues, values);

            return this;
        }

        public Filter WhereDoesNotContainValues(string entityname, string attributeName, params object[] values)
        {
            InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.DoesNotContainValues, values);

            return this;
        }
#endif
        public Filter WhereDoesNotEndWith(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.DoesNotEndWith, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.DoesNotEndWith, value);
            }

            return this;
        }

        public Filter WhereEndsWith(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.EndsWith, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.EndsWith, value);
            }

            return this;
        }

        public Filter WhereEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Equal, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Equal, value);
            }

            return this;
        }

        public Filter WhereEqualBusinessId(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.EqualBusinessId);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.EqualBusinessId);
            }

            return this;
        }

        public Filter WhereEqualUserId(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.EqualUserId);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.EqualUserId);
            }

            return this;
        }

        public Filter WhereEqualUserLanguage(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.EqualUserLanguage);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.EqualUserLanguage);
            }

            return this;
        }

        public Filter WhereEqualUserOrUserHierarchy(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.EqualUserOrUserHierarchy);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.EqualUserOrUserHierarchy);
            }

            return this;
        }

        public Filter WhereEqualUserOrUserHierarchyAndTeams(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            }
            else
            {
                InnerFilter.AddCondition(attributeName,
                    ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            }

            return this;
        }

        public Filter WhereEqualUserOrUserTeams(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.EqualUserOrUserTeams);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.EqualUserOrUserTeams);
            }

            return this;
        }

        public Filter WhereEqualUserTeams(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.EqualUserTeams);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.EqualUserTeams);
            }

            return this;
        }

        public Filter WhereGreaterEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.GreaterEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.GreaterEqual, value);
            }

            return this;
        }

        public Filter WhereGreaterThan(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.GreaterThan, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.GreaterThan, value);
            }

            return this;
        }

        public Filter WhereIn(string attributeName, params object[] values)
        {
            InnerFilter.AddCondition(attributeName, ConditionOperator.In, values);

            return this;
        }

        public Filter WhereIn(string entityname, string attributeName, params object[] values)
        {
            InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.In, values);

            return this;
        }

        public Filter WhereIn(string attributeName, IList value)
        {
            InnerFilter.Conditions.Add(new ConditionExpression(attributeName, ConditionOperator.In, value));

            return this;
        }

        public Filter WhereIn(string entityname, string attributeName, IList value)
        {
            InnerFilter.Conditions.Add(new ConditionExpression(entityname, attributeName, ConditionOperator.In, value));

            return this;
        }

        public Filter WhereInFiscalPeriod(string attributeName, int period, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.InFiscalPeriod, period);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.InFiscalPeriod, period);
            }

            return this;
        }

        public Filter WhereInFiscalPeriodAndYear(string attributeName, int period, int year, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.InFiscalPeriodAndYear, period, year);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.InFiscalPeriodAndYear, period, year);
            }

            return this;
        }

        public Filter WhereInFiscalYear(string attributeName, int year, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.InFiscalYear, year);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.InFiscalYear, year);
            }

            return this;
        }

        public Filter WhereInOrAfterFiscalPeriodAndYear(string attributeName, int period, int year, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.InOrAfterFiscalPeriodAndYear,
                    period, year);
            }

            return this;
        }

        public Filter WhereInOrBeforeFiscalPeriodAndYear(string attributeName, int period, int year, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.InOrBeforeFiscalPeriodAndYear,
                    period, year);
            }

            return this;
        }

        public Filter WhereLast7Days(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Last7Days);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Last7Days);
            }

            return this;
        }

        public Filter WhereLastFiscalPeriod(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastFiscalPeriod);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastFiscalPeriod);
            }

            return this;
        }

        public Filter WhereLastFiscalYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastFiscalYear);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastFiscalYear);
            }

            return this;
        }

        public Filter WhereLastMonth(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastMonth);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastMonth);
            }

            return this;
        }

        public Filter WhereLastWeek(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastWeek);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastWeek);
            }

            return this;
        }

        public Filter WhereLastXDays(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastXDays, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastXDays, value);
            }

            return this;
        }

        public Filter WhereLastXFiscalPeriods(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastXFiscalPeriods, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastXFiscalPeriods, value);
            }

            return this;
        }

        public Filter WhereLastXFiscalYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastXFiscalYears, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastXFiscalYears, value);
            }

            return this;
        }

        public Filter WhereLastXHours(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastXHours, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastXHours, value);
            }

            return this;
        }

        public Filter WhereLastXMonths(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastXMonths, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastXMonths, value);
            }

            return this;
        }

        public Filter WhereLastXWeeks(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastXWeeks, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastXWeeks, value);
            }

            return this;
        }

        public Filter WhereLastXYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastXYears, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastXYears, value);
            }

            return this;
        }

        public Filter WhereLastYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LastYear);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LastYear);
            }

            return this;
        }

        public Filter WhereLessEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LessEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LessEqual, value);
            }

            return this;
        }

        public Filter WhereLessThan(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.LessThan, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.LessThan, value);
            }

            return this;
        }

        public Filter WhereLike(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Like, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Like, value);
            }

            return this;
        }

        public Filter WhereMask(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Mask, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Mask, value);
            }

            return this;
        }

        public Filter WhereMasksSelect(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.MasksSelect, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.MasksSelect, value);
            }

            return this;
        }

        public Filter WhereNext7Days(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Next7Days);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Next7Days);
            }

            return this;
        }

        public Filter WhereNextFiscalPeriod(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextFiscalPeriod);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextFiscalPeriod);
            }

            return this;
        }

        public Filter WhereNextFiscalYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextFiscalYear);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextFiscalYear);
            }

            return this;
        }

        public Filter WhereNextMonth(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextMonth);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextMonth);
            }

            return this;
        }

        public Filter WhereNextWeek(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextWeek);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextWeek);
            }

            return this;
        }

        public Filter WhereNextXDays(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextXDays, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextXDays, value);
            }

            return this;
        }

        public Filter WhereNextXFiscalPeriods(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextXFiscalPeriods, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextXFiscalPeriods, value);
            }

            return this;
        }

        public Filter WhereNextXFiscalYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextXFiscalYears, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextXFiscalYears, value);
            }

            return this;
        }

        public Filter WhereNextXHours(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextXHours, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextXHours, value);
            }

            return this;
        }

        public Filter WhereNextXMonths(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextXMonths, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextXMonths, value);
            }

            return this;
        }

        public Filter WhereNextXWeeks(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextXWeeks, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextXWeeks, value);
            }

            return this;
        }

        public Filter WhereNextXYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextXYears, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextXYears, value);
            }

            return this;
        }

        public Filter WhereNextYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NextYear);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NextYear);
            }

            return this;
        }

        public Filter WhereNotBetween(string attributeName, object value1, object value2, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotBetween, value1, value2);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotBetween, value1, value2);
            }

            return this;
        }

        public Filter WhereNotEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotEqual, value);
            }

            return this;
        }

        public Filter WhereNotEqualBusinessId(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotEqualBusinessId);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotEqualBusinessId);
            }

            return this;
        }

        public Filter WhereNotEqualUserId(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotEqualUserId);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotEqualUserId);
            }

            return this;
        }

        public Filter WhereNotIn(string attributeName, params object[] values)
        {
            InnerFilter.AddCondition(attributeName, ConditionOperator.NotIn, values);

            return this;
        }

        public Filter WhereNotIn(string entityname, string attributeName, params object[] values)
        {
            InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotIn, values);

            return this;
        }

        public Filter WhereNotIn(string attributeName, IList value)
        {
            InnerFilter.AddCondition(attributeName, ConditionOperator.NotIn, value);

            return this;
        }

        public Filter WhereNotIn(string entityname, string attributeName, IList value)
        {
            InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotIn, value);

            return this;
        }

        public Filter WhereNotLike(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotLike, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotLike, value);
            }

            return this;
        }

        public Filter WhereNotMask(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotMask, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotMask, value);
            }

            return this;
        }

        public Filter WhereNotNull(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotNull);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotNull);
            }

            return this;
        }

        public Filter WhereNotOn(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotOn, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotOn, value);
            }

            return this;
        }

        public Filter WhereNotUnder(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.NotUnder, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.NotUnder, value);
            }

            return this;
        }

        public Filter WhereNull(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Null);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Null);
            }

            return this;
        }

        public Filter WhereOlderThanXDays(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXDays, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.OlderThanXDays, value);
            }

            return this;
        }

        public Filter WhereOlderThanXHours(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXHours, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.OlderThanXHours, value);
            }

            return this;
        }

        public Filter WhereOlderThanXMinutes(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXMinutes, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.OlderThanXMinutes, value);
            }

            return this;
        }

        public Filter WhereOlderThanXMonths(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXMonths, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.OlderThanXMonths, value);
            }

            return this;
        }

        public Filter WhereOlderThanXWeeks(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXWeeks, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.OlderThanXWeeks, value);
            }

            return this;
        }

        public Filter WhereOlderThanXYears(string attributeName, int value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.OlderThanXYears, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.OlderThanXYears, value);
            }

            return this;
        }

        public Filter WhereOn(string attributeName, DateTime value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.On, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.On, value);
            }

            return this;
        }

        public Filter WhereOnOrAfter(string attributeName, DateTime value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.OnOrAfter, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.OnOrAfter, value);
            }

            return this;
        }

        public Filter WhereOnOrBefore(string attributeName, DateTime value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.OnOrBefore, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.OnOrBefore, value);
            }

            return this;
        }

        public Filter WhereThisFiscalPeriod(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.ThisFiscalPeriod);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.ThisFiscalPeriod);
            }

            return this;
        }

        public Filter WhereThisFiscalYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.ThisFiscalYear);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.ThisFiscalYear);
            }

            return this;
        }

        public Filter WhereThisMonth(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.ThisMonth);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.ThisMonth);
            }

            return this;
        }

        public Filter WhereThisWeek(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.ThisWeek);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.ThisWeek);
            }

            return this;
        }

        public Filter WhereThisYear(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.ThisYear);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.ThisYear);
            }

            return this;
        }

        public Filter WhereToday(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Today);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Today);
            }

            return this;
        }

        public Filter WhereTomorrow(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Tomorrow);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Tomorrow);
            }

            return this;
        }

        public Filter WhereUnder(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Under, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Under, value);
            }

            return this;
        }

        public Filter WhereUnderOrEqual(string attributeName, object value, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.UnderOrEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.UnderOrEqual, value);
            }

            return this;
        }

        public Filter WhereYesterday(string attributeName, string entityname = null)
        {
            if (entityname != null)
            {
                InnerFilter.AddCondition(entityname, attributeName, ConditionOperator.Yesterday);
            }
            else
            {
                InnerFilter.AddCondition(attributeName, ConditionOperator.Yesterday);
            }

            return this;
        }

        #endregion Conditions
    }
}