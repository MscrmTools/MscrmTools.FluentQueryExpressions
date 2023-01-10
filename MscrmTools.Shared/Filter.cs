using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MscrmTools.FluentQueryExpressions.Helpers;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace MscrmTools.FluentQueryExpressions
{
    /// <summary>
    /// A filter in the query
    /// </summary>
    public class Filter : Filter<Entity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Filter"/> class.
        /// </summary>
        /// <param name="logicalOperator">Logical operator for this filter</param>
        public Filter(LogicalOperator logicalOperator = LogicalOperator.And) : base(logicalOperator)
        {
        }

        #region Filters

        /// <summary>Adds filter(s).</summary>
        /// <param name="filters">The filters.</param>
        /// <returns>This <see cref="Filter"></see></returns>
        public Filter AddFilters(params Filter[] filters)
        {
            InnerFilter.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        /// <summary>Adds a filter.</summary>
        /// <param name="filter">The filter.</param>
        /// <returns>This <see cref="Filter{T}"></see></returns>
        public Filter AddFilter(Filter filter)
        {
            InnerFilter.Filters.Add(filter.InnerFilter);

            return this;
        }

        #endregion Filters

        #region Columns Comparer

#if CRMV9
        /// <summary>
        /// Starts a column comparison
        /// </summary>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Filter> Compare(string column)
        {
            return new Shared.AppCode.Comparer<Filter>(this, column);
        }

        /// <summary>
        /// Starts a column comparison
        /// </summary>
        /// <param name="alias">Alias of the related table in relationship</param>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Filter> Compare(string alias, string column)
        {
            return new Shared.AppCode.Comparer<Filter>(this, alias, column);
        }

#endif

        #endregion Columns Comparer

        #region Conditions

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter Where(string column, ConditionOperator conditionOperator, params object[] values)
        {
            InnerFilter.AddCondition(column, conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter Where(string tableAlias, string column, ConditionOperator conditionOperator, params object[] values)
        {
            InnerFilter.AddCondition(tableAlias, column, conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereAbove(string column, Guid value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Above, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Above, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereAboveOrEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.AboveOrEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.AboveOrEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value begins with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereBeginsWith(string column, string value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.BeginsWith, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.BeginsWith, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereBetween(string column, object value1, object value2, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Between, value1, value2);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Between, value1, value2);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value contains <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereContains(string column, string value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Contains, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Contains, value);
            }

            return this;
        }

#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereContainValues(string column, params object[] values)
        {
            InnerFilter.AddCondition(column, ConditionOperator.ContainValues, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereContainValues(string tableAlias, string column, params object[] values)
        {
            InnerFilter.AddCondition(tableAlias, column, ConditionOperator.ContainValues, values);

            return this;
        }

#endif

        /// <summary>Adds a condition where <paramref name="column"/> value does not begin with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereDoesNotBeginWith(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.DoesNotBeginWith, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.DoesNotBeginWith, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not contain <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereDoesNotContain(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.DoesNotContain, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.DoesNotContain, value);
            }

            return this;
        }

#if CRMV9
        /// <summary>Adds a condition where <paramref name="column"/> does not contain <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereDoesNotContainValues(string column, params object[] values)
        {
            InnerFilter.AddCondition(column, ConditionOperator.DoesNotContainValues, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereDoesNotContainValues(string tableAlias, string column, params object[] values)
        {
            InnerFilter.AddCondition(tableAlias, column, ConditionOperator.DoesNotContainValues, values);

            return this;
        }

#endif
        /// <summary>Adds a condition where <paramref name="column"/> value does not end with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereDoesNotEndWith(string column, string value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.DoesNotEndWith, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.DoesNotEndWith, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value ends with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEndsWith(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.EndsWith, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.EndsWith, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value equals <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Equal, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Equal, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user business unit</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEqualBusinessId(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.EqualBusinessId);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.EqualBusinessId);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user unique identifier</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEqualUserId(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.EqualUserId);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.EqualUserId);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user language</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEqualUserLanguage(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.EqualUserLanguage);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.EqualUserLanguage);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or their reporting hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEqualUserOrUserHierarchy(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.EqualUserOrUserHierarchy);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.EqualUserOrUserHierarchy);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user and his teams or their reporting hierarchy and their teams</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEqualUserOrUserHierarchyAndTeams(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            }
            else
            {
                InnerFilter.AddCondition(column,
                    ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or a team the current user is member of</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEqualUserOrUserTeams(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.EqualUserOrUserTeams);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.EqualUserOrUserTeams);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals a team the current user is member of</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereEqualUserTeams(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.EqualUserTeams);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.EqualUserTeams);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater or equal to <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereGreaterEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.GreaterEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.GreaterEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater than <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereGreaterThan(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.GreaterThan, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.GreaterThan, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereIn(string column, IList values)
        {
            InnerFilter.Conditions.Add(new ConditionExpression(column, ConditionOperator.In, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereIn(string tableAlias, string column, IList values)
        {
            InnerFilter.Conditions.Add(new ConditionExpression(tableAlias, column, ConditionOperator.In, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereIn(string column, params object[] values)
        {
            InnerFilter.Conditions.Add(new ConditionExpression(column, ConditionOperator.In, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereIn(string tableAlias, string column, params object[] values)
        {
            InnerFilter.Conditions.Add(new ConditionExpression(tableAlias, column, ConditionOperator.In, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereInFiscalPeriod(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.InFiscalPeriod, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.InFiscalPeriod, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereInFiscalPeriodAndYear(string column, int period, int year, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.InFiscalPeriodAndYear, period, year);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.InFiscalPeriodAndYear, period, year);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereInFiscalYear(string column, int year, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.InFiscalYear, year);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.InFiscalYear, year);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or after the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereInOrAfterFiscalPeriodAndYear(string column, int period, int year, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.InOrAfterFiscalPeriodAndYear,
                    period, year);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or before the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereInOrBeforeFiscalPeriodAndYear(string column, int period, int year, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.InOrBeforeFiscalPeriodAndYear,
                    period, year);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on last 7 days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLast7Days(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Last7Days);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Last7Days);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastFiscalPeriod(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastFiscalPeriod);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastFiscalPeriod);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastFiscalYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastFiscalYear);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastFiscalYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastMonth(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastMonth);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastMonth);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastWeek(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastWeek);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastWeek);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastXDays(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastXDays, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastXDays, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal periods</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal periods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastXFiscalPeriods(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastXFiscalPeriods, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastXFiscalPeriods, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastXFiscalYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastXFiscalYears, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastXFiscalYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastXHours(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastXHours, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastXHours, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastXMonths(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastXMonths, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastXMonths, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastXWeeks(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastXWeeks, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastXWeeks, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastXYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastXYears, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastXYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLastYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LastYear);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LastYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less or equal to <paramref name="value"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLessEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LessEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LessEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less than <paramref name="value"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLessThan(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.LessThan, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.LessThan, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value matches the specifed <paramref name="pattern"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereLike(string column, string pattern, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Like, pattern);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Like, pattern);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereMask(string column, object bitmask, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Mask, bitmask);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Mask, bitmask);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next 7 days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNext7Days(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Next7Days);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Next7Days);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextFiscalPeriod(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextFiscalPeriod);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextFiscalPeriod);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextFiscalYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextFiscalYear);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextFiscalYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextMonth(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextMonth);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextMonth);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextWeek(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextWeek);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextWeek);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextXDays(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextXDays, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextXDays, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal perdiods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextXFiscalPeriods(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextXFiscalPeriods, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextXFiscalPeriods, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextXFiscalYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextXFiscalYears, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextXFiscalYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextXHours(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextXHours, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextXHours, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextXMonths(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextXMonths, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextXMonths, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextXWeeks(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextXWeeks, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextXWeeks, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextXYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextXYears, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextXYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNextYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NextYear);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NextYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotBetween(string column, object value1, object value2, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotBetween, value1, value2);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotBetween, value1, value2);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not equal <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user business unit</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotEqualBusinessId(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotEqualBusinessId);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotEqualBusinessId);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user unique identifier</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotEqualUserId(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotEqualUserId);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotEqualUserId);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotIn(string tableAlias, string column, IList values)
        {
            InnerFilter.Conditions.Add(new ConditionExpression(tableAlias, column, ConditionOperator.NotIn, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotIn(string column, IList values)
        {
            InnerFilter.Conditions.Add(new ConditionExpression(column, ConditionOperator.NotIn, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotIn(string column, params object[] values)
        {
            InnerFilter.AddCondition(column, ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotIn(string tableAlias, string column, params object[] values)
        {
            InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not match the specifed <paramref name="pattern"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotLike(string column, string pattern, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotLike, pattern);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotLike, pattern);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotMask(string column, object bitmask, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotMask, bitmask);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotMask, bitmask);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not null</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotNull(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotNull);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotNull);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not on the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotOn(string column, DateTime datetime, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotOn, datetime);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotOn, datetime);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is above <paramref name="value"/> in the hierarchy </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNotUnder(string column, Guid value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.NotUnder, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.NotUnder, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain data</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereNull(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Null);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Null);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOlderThanXDays(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.OlderThanXDays, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.OlderThanXDays, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOlderThanXHours(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.OlderThanXHours, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.OlderThanXHours, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> minutes</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of minutes to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOlderThanXMinutes(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.OlderThanXMinutes, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.OlderThanXMinutes, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOlderThanXMonths(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.OlderThanXMonths, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.OlderThanXMonths, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOlderThanXWeeks(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.OlderThanXWeeks, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.OlderThanXWeeks, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOlderThanXYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.OlderThanXYears, x);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.OlderThanXYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOn(string column, DateTime datetime, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.On, datetime);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.On, datetime);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or after the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOnOrAfter(string column, DateTime datetime, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.OnOrAfter, datetime);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.OnOrAfter, datetime);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or before the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereOnOrBefore(string column, DateTime datetime, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.OnOrBefore, datetime);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.OnOrBefore, datetime);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereThisFiscalPeriod(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.ThisFiscalPeriod);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.ThisFiscalPeriod);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereThisFiscalYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.ThisFiscalYear);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.ThisFiscalYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereThisMonth(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.ThisMonth);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.ThisMonth);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereThisWeek(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.ThisWeek);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.ThisWeek);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereThisYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.ThisYear);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.ThisYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current day</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereToday(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Today);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Today);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is tomorrow</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereTomorrow(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Tomorrow);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Tomorrow);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereUnder(string column, Guid value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Under, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Under, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereUnderOrEqual(string column, Guid value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.UnderOrEqual, value);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.UnderOrEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is yesterday</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter WhereYesterday(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                InnerFilter.AddCondition(tableAlias, column, ConditionOperator.Yesterday);
            }
            else
            {
                InnerFilter.AddCondition(column, ConditionOperator.Yesterday);
            }

            return this;
        }

        #endregion Conditions
    }

    /// <summary>
    /// A strongly typed filter in the query
    /// </summary>
    public class Filter<T> where T : Entity
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Filter{T}" /> class.</summary>
        /// <param name="logicalOperator">The logical operator.</param>
        public Filter(LogicalOperator logicalOperator = LogicalOperator.And)
        {
            InnerFilter = new FilterExpression(logicalOperator);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the <see cref="FilterExpression"/> representation of this <see cref="Filter{T}"/>
        /// </summary>
        internal FilterExpression InnerFilter { get; }

        #endregion Properties

        #region Filters

        /// <summary>Adds filter(s).</summary>
        /// <param name="filters">The filters.</param>
        /// <returns>This <see cref="Filter{T}"></see></returns>
        public Filter<T> AddFilters(params Filter<T>[] filters)
        {
            InnerFilter.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        /// <summary>Adds filter(s).</summary>
        /// <param name="logicalOperator">The logicala operator.</param>
        /// <param name="filters">The filters.</param>
        /// <returns>This <see cref="Filter{T}"></see></returns>
        public Filter<T> AddFilters(LogicalOperator logicalOperator, params Filter<T>[] filters)
        {
            InnerFilter.FilterOperator = logicalOperator;
            InnerFilter.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        /// <summary>Sets the logical operator for the this filter</summary>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <returns>This <see cref="Filter{T}"></see></returns>
        public Filter<T> SetLogicalOperator(LogicalOperator logicalOperator)
        {
            InnerFilter.FilterOperator = logicalOperator;

            return this;
        }

        /// <summary>Adds a filter.</summary>
        /// <param name="filter">The filter.</param>
        /// <returns>This <see cref="Filter{T}"></see></returns>
        public Filter<T> AddFilter(Func<Filter<T>, Filter<T>> filter)
        {
            var fe = new Filter<T>();
            filter(fe);

            InnerFilter.Filters.AddRange(fe.InnerFilter);

            return this;
        }

        /// <summary>Adds a filter.</summary>
        /// <param name="logicalOperator">The logical operator for the new filter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>This <see cref="Filter{T}"></see></returns>
        public Filter<T> AddFilter(LogicalOperator logicalOperator, Func<Filter<T>, Filter<T>> filter)
        {
            var fe = new Filter<T>(logicalOperator);
            filter(fe);

            InnerFilter.Filters.AddRange(fe.InnerFilter);

            return this;
        }

        #endregion Filters

        #region Columns Comparer

#if CRMV9

        /// <summary>
        /// Starts a column comparison
        /// </summary>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Filter<T>, T> Compare(Expression<Func<T, object>> column)
        {
            return new Shared.AppCode.Comparer<Filter<T>, T>(this, AnonymousTypeHelper.GetAttributeName<T>(column));
        }

        /// <summary>
        /// Starts a column comparison
        /// </summary>
        /// <param name="alias">Alias of the related table in relationship</param>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Filter<T>, T> Compare(string alias, Expression<Func<T, object>> column)
        {
            return new Shared.AppCode.Comparer<Filter<T>, T>(this, alias, AnonymousTypeHelper.GetAttributeName<T>(column));
        }
#endif

        #endregion Columns Comparer

        #region Conditions

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> Where(Expression<Func<T, object>> column, ConditionOperator conditionOperator, params object[] values)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName<T>(column), conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> Where<U>(string tableAlias, Expression<Func<U, object>> column, ConditionOperator conditionOperator, params object[] values) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereAbove(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Above, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereAbove<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Above, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereAboveOrEqual(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.AboveOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereAboveOrEqual<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.AboveOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value begins with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereBeginsWith(Expression<Func<T, object>> column, string value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.BeginsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value begins with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereBeginsWith<U>(string tableAlias, Expression<Func<U, object>> column, string value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.BeginsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereBetween(Expression<Func<T, object>> column, object value1, object value2)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Between, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereBetween<U>(string tableAlias, Expression<Func<U, object>> column, object value1, object value2) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Between, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value contains <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereContains(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Contains, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value contains <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereContains<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Contains, value);

            return this;
        }

#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereContainValues(Expression<Func<T, object>> column, params object[] values)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ContainValues, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereContainValues<U>(string tableAlias, Expression<Func<U, object>> column, params object[] values) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ContainValues, values);

            return this;
        }

#endif

        /// <summary>Adds a condition where <paramref name="column"/> value does not begin with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereDoesNotBeginWith(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotBeginWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not begin with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereDoesNotBeginWith<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotBeginWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not contain <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereDoesNotContain(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContain, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not contain <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereDoesNotContain<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContain, value);

            return this;
        }

#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> contdoes not containains <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereDoesNotContainValues(Expression<Func<T, object>> column, params object[] values)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContainValues, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereDoesNotContainValues<U>(string tableAlias, Expression<Func<U, object>> column, params object[] values) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContainValues, values);

            return this;
        }
#endif

        /// <summary>Adds a condition where <paramref name="column"/> value does not end with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereDoesNotEndWith(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotEndWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not end with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereDoesNotEndWith<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotEndWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value ends with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEndsWith(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EndsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value ends with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEndsWith<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EndsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value equals <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqual(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Equal, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value equals <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqual<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Equal, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user business unit</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualBusinessId(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user business unit</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualBusinessId<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user unique identifier</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserId(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user unique identifier</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserId<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user language</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserLanguage(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserLanguage);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user language</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserLanguage<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserLanguage);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or their reporting hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserOrUserHierarchy(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchy);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or their reporting hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserOrUserHierarchy<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchy);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user and his teams or their reporting hierarchy and their teams</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserOrUserHierarchyAndTeams(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchyAndTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user and his teams or their reporting hierarchy and their teams</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserOrUserHierarchyAndTeams<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchyAndTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or a team the current user is member of</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserOrUserTeams(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or a team the current user is member of</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserOrUserTeams<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals a team the current user is member of</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserTeams(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals a team the current user is member of</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereEqualUserTeams<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater or equal to <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereGreaterEqual(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater or equal to <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereGreaterEqual<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater than <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereGreaterThan(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater than <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereGreaterThan<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereIn(Expression<Func<T, object>> column, IList values)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereIn<U>(string tableAlias, Expression<Func<U, object>> column, IList values) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereIn(Expression<Func<T, object>> column, params object[] values)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereIn(string tableAlias, Expression<Func<T, object>> column, params object[] values)
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInFiscalPeriod(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriod, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInFiscalPeriod<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriod, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInFiscalPeriodAndYear(Expression<Func<T, object>> column, int period, int year)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInFiscalPeriodAndYear<U>(string tableAlias, Expression<Func<U, object>> column, int period, int year) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInFiscalYear(Expression<Func<T, object>> column, int year)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalYear, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInFiscalYear<U>(string tableAlias, Expression<Func<U, object>> column, int year) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalYear, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or after the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInOrAfterFiscalPeriodAndYear(Expression<Func<T, object>> column, int period, int year)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or after the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInOrAfterFiscalPeriodAndYear<U>(string tableAlias, Expression<Func<U, object>> column, int period, int year) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or before the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInOrBeforeFiscalPeriodAndYear(Expression<Func<T, object>> column, int period, int year)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or before the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereInOrBeforeFiscalPeriodAndYear<U>(string tableAlias, Expression<Func<U, object>> column, int period, int year) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on last 7 days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLast7Days(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Last7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on last 7 days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLast7Days<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Last7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastFiscalPeriod(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastFiscalPeriod<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastFiscalYear(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastFiscalYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastMonth(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastMonth<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastWeek(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastWeek<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXDays(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXDays<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal periods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXFiscalPeriods(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal periods</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal periods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXFiscalPeriods<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXFiscalYears(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXFiscalYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXHours(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXHours<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXMonths(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXMonths<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXWeeks(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXWeeks<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXYears(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastXYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastYear(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLastYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less or equal to <paramref name="value"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLessEqual(Expression<Func<T, object>> column, int value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less or equal to <paramref name="value"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLessEqual<U>(string tableAlias, Expression<Func<U, object>> column, int value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less than <paramref name="value"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLessThan(Expression<Func<T, object>> column, int value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less than <paramref name="value"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLessThan<U>(string tableAlias, Expression<Func<U, object>> column, int value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value matches the specifed <paramref name="pattern"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLike(Expression<Func<T, object>> column, string pattern)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Like, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value matches the specifed <paramref name="pattern"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereLike<U>(string tableAlias, Expression<Func<U, object>> column, string pattern) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Like, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereMask(Expression<Func<T, object>> column, object bitmask)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Mask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereMask<U>(string tableAlias, Expression<Func<U, object>> column, object bitmask) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Mask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next 7 days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNext7Days(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Next7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next 7 days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNext7Days<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Next7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextFiscalPeriod(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextFiscalPeriod<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextFiscalYear(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextFiscalYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextMonth(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextMonth<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextWeek(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextWeek<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXDays(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXDays<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal perdiods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXFiscalPeriods(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal perdiods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXFiscalPeriods<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXFiscalYears(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXFiscalYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXHours(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXHours<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXMonths(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXMonths<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXWeeks(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXWeeks<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXYears(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextXYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextYear(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNextYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotBetween(Expression<Func<T, object>> column, object value1, object value2)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotBetween, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotBetween<U>(string tableAlias, Expression<Func<U, object>> column, object value1, object value2) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotBetween, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not equal <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotEqual(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not equal <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotEqual<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user business unit</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotEqualBusinessId(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user business unit</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotEqualBusinessId<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user unique identifier</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotEqualUserId(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user unique identifier</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotEqualUserId<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotIn(Expression<Func<T, object>> column, IList values)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotIn(string tableAlias, Expression<Func<T, object>> column, IList values)
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotIn(Expression<Func<T, object>> column, params object[] values)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotIn(string tableAlias, Expression<Func<T, object>> column, params object[] values)
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not match the specifed <paramref name="pattern"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotLike(Expression<Func<T, object>> column, string pattern)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotLike, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not match the specifed <paramref name="pattern"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotLike<U>(string tableAlias, Expression<Func<U, object>> column, string pattern) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotLike, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotMask(Expression<Func<T, object>> column, object bitmask)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotMask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotMask<U>(string tableAlias, Expression<Func<U, object>> column, object bitmask) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotMask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not null</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotNull(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotNull);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not null</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotNull<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotNull);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not on the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotOn(Expression<Func<T, object>> column, DateTime datetime)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotOn, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not on the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotOn<U>(string tableAlias, Expression<Func<U, object>> column, DateTime datetime) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotOn, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is above <paramref name="value"/> in the hierarchy </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotUnder(Expression<Func<T, object>> column, object value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotUnder, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is above <paramref name="value"/> in the hierarchy </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNotUnder<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotUnder, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain data</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNull(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Null);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain data</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereNull<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Null);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXDays(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXDays<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXHours(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXHours<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> minutes</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of minutes to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXMinutes(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMinutes, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> minutes</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of minutes to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXMinutes<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMinutes, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXMonths(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXMonths<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXWeeks(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXWeeks<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXYears(Expression<Func<T, object>> column, int x)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOlderThanXYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOn(Expression<Func<T, object>> column, DateTime datetime)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.On, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOn<U>(string tableAlias, Expression<Func<U, object>> column, DateTime datetime) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.On, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or after the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOnOrAfter(Expression<Func<T, object>> column, DateTime datetime)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrAfter, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or after the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOnOrAfter<U>(string tableAlias, Expression<Func<U, object>> column, DateTime datetime) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrAfter, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or before the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOnOrBefore(Expression<Func<T, object>> column, DateTime datetime)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrBefore, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or before the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereOnOrBefore<U>(string tableAlias, Expression<Func<U, object>> column, DateTime datetime) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrBefore, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisFiscalPeriod(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisFiscalPeriod<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisFiscalYear(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisFiscalYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisMonth(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisMonth<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisWeek(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisWeek<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisYear(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereThisYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current day</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereToday(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Today);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current day</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereToday<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Today);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is tomorrow</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereTomorrow(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Tomorrow);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is tomorrow</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereTomorrow<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Tomorrow);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereUnder(Expression<Func<T, object>> column, Guid value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Under, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereUnder<U>(string tableAlias, Expression<Func<U, object>> column, Guid value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Under, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereUnderOrEqual(Expression<Func<T, object>> column, Guid value)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.UnderOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereUnderOrEqual<U>(string tableAlias, Expression<Func<U, object>> column, Guid value) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.UnderOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is yesterday</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereYesterday(Expression<Func<T, object>> column)
        {
            InnerFilter.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Yesterday);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is yesterday</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Filter<T> WhereYesterday<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            InnerFilter.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Yesterday);

            return this;
        }

        #endregion Conditions
    }
}