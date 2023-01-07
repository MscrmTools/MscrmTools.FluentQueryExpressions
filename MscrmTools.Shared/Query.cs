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
    /// <summary>
    /// A query to request Dataverse data
    /// </summary>
    public class Query : Query<Entity>
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of class <see cref="Query"/>
        /// </summary>
        /// <param name="table">Logical name of the table to query</param>
        public Query(string table) : base()
        {
            QueryExpression = new QueryExpression(table);
        }

        #endregion Constructors

        #region Attributes

        /// <summary>
        /// Specifies the list of columns the query should return
        /// </summary>
        /// <param name="columns">Columns to return</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query Select(params string[] columns)
        {
            if (columns.Length == 0)
            {
                QueryExpression.ColumnSet = new ColumnSet();
            }

            QueryExpression.ColumnSet.AddColumns(columns);

            return this;
        }

        #endregion Attributes

        #region Filters

        /// <summary>
        /// Add a filter under the default filter for this query
        /// </summary>
        /// <param name="filter">Filter to add</param>
        /// <returns>The <see cref="Query"/></returns>
        public Query AddFilter(Filter filter)
        {
            QueryExpression.Criteria.Filters.Add(filter.InnerFilter);

            return this;
        }

        /// <summary>
        /// Add filters under the default filter for this query
        /// </summary>
        /// <param name="filters">List of filters</param>
        /// <returns>The <see cref="Query"/></returns>
        public Query AddFilters(params Filter[] filters)
        {
            QueryExpression.Criteria.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        /// <summary>
        /// Add multiple filters under the default filter for this query
        /// </summary>
        /// <param name="logicalOperator">Logical operator for the default filter of this query</param>
        /// <param name="filters">Filters to add</param>
        /// <returns>The <see cref="Query"/></returns>
        [Obsolete("Use method AddFilters without LogicalOperator argument and use SetFilterOperator method to change the QueryExpression default filter logical operator")]
        public Query AddFilters(LogicalOperator logicalOperator, params Filter[] filters)
        {
            QueryExpression.Criteria.FilterOperator = logicalOperator;
            QueryExpression.Criteria.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        /// <summary>
        /// Define the filter operator for the default filter of this query
        /// </summary>
        /// <param name="logicalOperator">Logical operator</param>
        /// <returns>The <see cref="Query"/></returns>
        public new Query SetLogicalOperator(LogicalOperator logicalOperator)
        {
            QueryExpression.Criteria.FilterOperator = logicalOperator;

            return this;
        }

        #endregion Filters

        #region Links

        /// <summary>
        /// Add a related table in the query
        /// </summary>
        /// <param name="link">Link to the related table</param>
        /// <returns>The <see cref="Query"/></returns>
        public Query AddLink(Link link)
        {
            QueryExpression.LinkEntities.Add(link.InnerLinkEntity);
            link.InnerLinkEntity.LinkFromEntityName = QueryExpression.EntityName;
            return this;
        }

        /// <summary>
        /// Add a related table in the query
        /// </summary>
        /// <param name="fromAttributeName">Column from the source table</param>
        /// <param name="toEntityName">Related table to query</param>
        /// <param name="toAttributeName">Column from the related table</param>
        /// <param name="jo">Join operator for the link</param>
        /// <returns>The <see cref="Query"/></returns>
        public Query AddLink(string fromAttributeName, string toEntityName, string toAttributeName, JoinOperator jo = JoinOperator.Inner)
        {
            var link = new LinkEntity
            {
                LinkFromEntityName = QueryExpression.EntityName,
                LinkFromAttributeName = fromAttributeName,
                LinkToAttributeName = toAttributeName,
                LinkToEntityName = toEntityName,
                JoinOperator = jo,
                EntityAlias = toEntityName
            };

            QueryExpression.LinkEntities.Add(link);
            return this;
        }

        #endregion Links

        #region Columns Comparer

#if CRMV9

        /// <summary>
        /// Starts a column comparison
        /// </summary>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Query> Compare(string column)
        {
            return new Shared.AppCode.Comparer<Query>(this, column);
        }

        /// <summary>
        /// Starts a column comparison for a related table
        /// </summary>
        /// <param name="tableAlias">Alias of the linked table where to compare columns</param>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Query> Compare(string tableAlias, string column)
        {
            return new Shared.AppCode.Comparer<Query>(this, tableAlias, column);
        }

#endif

        #endregion Columns Comparer

        #region Order

        /// <summary>
        /// Order the result of the query by the <paramref name="column"/>
        /// </summary>
        /// <param name="column">Column to use to sort results of the query</param>
        /// <returns>The <see cref="Query"/></returns>
        public Query OrderBy(string column)
        {
            QueryExpression.AddOrder(column, OrderType.Ascending);
            return this;
        }

        /// <summary>
        /// Order the result of the query by the <paramref name="column"/> descending
        /// </summary>
        /// <param name="column">Column to use to sort results of the query</param>
        /// <returns>The <see cref="Query"/></returns>
        public Query OrderByDescending(string column)
        {
            QueryExpression.AddOrder(column, OrderType.Descending);
            return this;
        }

        #endregion Order

        #region Conditions

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query Where(string column, ConditionOperator conditionOperator, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(column, conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query Where(string tableAlias, string column, ConditionOperator conditionOperator, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(tableAlias, column, conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereAbove(string column, Guid value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Above, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Above, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereAboveOrEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.AboveOrEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.AboveOrEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value begins with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereBeginsWith(string column, string value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.BeginsWith, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.BeginsWith, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereBetween(string column, object value1, object value2, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Between, value1, value2);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Between, value1, value2);
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
        public Query WhereContains(string column, string value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Contains, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Contains, value);
            }

            return this;
        }

#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereContainValues(string column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(column, ConditionOperator.ContainValues, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereContainValues(string tableAlias, string column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.ContainValues, values);

            return this;
        }

#endif

        /// <summary>Adds a condition where <paramref name="column"/> value does not begin with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereDoesNotBeginWith(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.DoesNotBeginWith, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.DoesNotBeginWith, value);
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
        public Query WhereDoesNotContain(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.DoesNotContain, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.DoesNotContain, value);
            }

            return this;
        }

#if CRMV9
        /// <summary>Adds a condition where <paramref name="column"/> does not contain <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereDoesNotContainValues(string column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(column, ConditionOperator.DoesNotContainValues, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereDoesNotContainValues(string tableAlias, string column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.DoesNotContainValues, values);

            return this;
        }

#endif
        /// <summary>Adds a condition where <paramref name="column"/> value does not end with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereDoesNotEndWith(string column, string value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.DoesNotEndWith, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.DoesNotEndWith, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value ends with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEndsWith(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.EndsWith, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.EndsWith, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value equals <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Equal, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Equal, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user business unit</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEqualBusinessId(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.EqualBusinessId);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.EqualBusinessId);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user unique identifier</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEqualUserId(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.EqualUserId);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.EqualUserId);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user language</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEqualUserLanguage(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.EqualUserLanguage);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.EqualUserLanguage);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or their reporting hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEqualUserOrUserHierarchy(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.EqualUserOrUserHierarchy);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.EqualUserOrUserHierarchy);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user and his teams or their reporting hierarchy and their teams</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEqualUserOrUserHierarchyAndTeams(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column,
                    ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or a team the current user is member of</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEqualUserOrUserTeams(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.EqualUserOrUserTeams);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.EqualUserOrUserTeams);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals a team the current user is member of</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereEqualUserTeams(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.EqualUserTeams);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.EqualUserTeams);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater or equal to <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereGreaterEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.GreaterEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.GreaterEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater than <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereGreaterThan(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.GreaterThan, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.GreaterThan, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereIn(string column, IList values)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(column, ConditionOperator.In, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereIn(string tableAlias, string column, IList values)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(tableAlias, column, ConditionOperator.In, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereIn(string column, params object[] values)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(column, ConditionOperator.In, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereIn(string tableAlias, string column, params object[] values)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(tableAlias, column, ConditionOperator.In, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereInFiscalPeriod(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.InFiscalPeriod, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.InFiscalPeriod, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereInFiscalPeriodAndYear(string column, int period, int year, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.InFiscalPeriodAndYear, period, year);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.InFiscalPeriodAndYear, period, year);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereInFiscalYear(string column, int year, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.InFiscalYear, year);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.InFiscalYear, year);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or after the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereInOrAfterFiscalPeriodAndYear(string column, int period, int year, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.InOrAfterFiscalPeriodAndYear,
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
        public Query WhereInOrBeforeFiscalPeriodAndYear(string column, int period, int year, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.InOrBeforeFiscalPeriodAndYear,
                    period, year);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on last 7 days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLast7Days(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Last7Days);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Last7Days);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastFiscalPeriod(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastFiscalPeriod);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastFiscalPeriod);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastFiscalYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastFiscalYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastFiscalYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastMonth(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastMonth);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastMonth);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastWeek(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastWeek);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastWeek);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastXDays(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastXDays, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastXDays, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal periods</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal periods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastXFiscalPeriods(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastXFiscalPeriods, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastXFiscalPeriods, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastXFiscalYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastXFiscalYears, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastXFiscalYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastXHours(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastXHours, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastXHours, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastXMonths(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastXMonths, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastXMonths, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastXWeeks(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastXWeeks, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastXWeeks, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastXYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastXYears, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastXYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLastYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LastYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LastYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less or equal to <paramref name="value"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLessEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LessEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LessEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less than <paramref name="value"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLessThan(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.LessThan, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.LessThan, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value matches the specifed <paramref name="pattern"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereLike(string column, string pattern, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Like, pattern);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Like, pattern);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereMask(string column, object bitmask, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Mask, bitmask);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Mask, bitmask);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next 7 days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNext7Days(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Next7Days);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Next7Days);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextFiscalPeriod(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextFiscalPeriod);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextFiscalPeriod);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextFiscalYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextFiscalYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextFiscalYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextMonth(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextMonth);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextMonth);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextWeek(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextWeek);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextWeek);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextXDays(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextXDays, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextXDays, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal perdiods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextXFiscalPeriods(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextXFiscalPeriods, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextXFiscalPeriods, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextXFiscalYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextXFiscalYears, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextXFiscalYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextXHours(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextXHours, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextXHours, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextXMonths(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextXMonths, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextXMonths, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextXWeeks(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextXWeeks, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextXWeeks, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextXYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextXYears, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextXYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNextYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NextYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NextYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotBetween(string column, object value1, object value2, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotBetween, value1, value2);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotBetween, value1, value2);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not equal <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotEqual(string column, object value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user business unit</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotEqualBusinessId(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotEqualBusinessId);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotEqualBusinessId);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user unique identifier</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotEqualUserId(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotEqualUserId);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotEqualUserId);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotIn(string tableAlias, string column, IList values)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(tableAlias, column, ConditionOperator.NotIn, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotIn(string column, IList values)
        {
            QueryExpression.Criteria.Conditions.Add(new ConditionExpression(column, ConditionOperator.NotIn, values));

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotIn(string column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotIn(string tableAlias, string column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not match the specifed <paramref name="pattern"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotLike(string column, string pattern, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotLike, pattern);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotLike, pattern);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotMask(string column, object bitmask, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotMask, bitmask);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotMask, bitmask);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not null</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotNull(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotNull);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotNull);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not on the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotOn(string column, DateTime datetime, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotOn, datetime);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotOn, datetime);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is above <paramref name="value"/> in the hierarchy </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNotUnder(string column, Guid value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.NotUnder, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.NotUnder, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain data</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereNull(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Null);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Null);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOlderThanXDays(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.OlderThanXDays, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.OlderThanXDays, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOlderThanXHours(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.OlderThanXHours, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.OlderThanXHours, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> minutes</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of minutes to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOlderThanXMinutes(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.OlderThanXMinutes, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.OlderThanXMinutes, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOlderThanXMonths(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.OlderThanXMonths, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.OlderThanXMonths, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOlderThanXWeeks(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.OlderThanXWeeks, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.OlderThanXWeeks, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOlderThanXYears(string column, int x, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.OlderThanXYears, x);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.OlderThanXYears, x);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOn(string column, DateTime datetime, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.On, datetime);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.On, datetime);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or after the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOnOrAfter(string column, DateTime datetime, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.OnOrAfter, datetime);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.OnOrAfter, datetime);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or before the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereOnOrBefore(string column, DateTime datetime, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.OnOrBefore, datetime);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.OnOrBefore, datetime);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereThisFiscalPeriod(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.ThisFiscalPeriod);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.ThisFiscalPeriod);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereThisFiscalYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.ThisFiscalYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.ThisFiscalYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereThisMonth(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.ThisMonth);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.ThisMonth);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereThisWeek(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.ThisWeek);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.ThisWeek);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereThisYear(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.ThisYear);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.ThisYear);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current day</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereToday(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Today);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Today);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is tomorrow</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereTomorrow(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Tomorrow);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Tomorrow);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereUnder(string column, Guid value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Under, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Under, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereUnderOrEqual(string column, Guid value, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.UnderOrEqual, value);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.UnderOrEqual, value);
            }

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is yesterday</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query WhereYesterday(string column, string tableAlias = null)
        {
            if (tableAlias != null)
            {
                QueryExpression.Criteria.AddCondition(tableAlias, column, ConditionOperator.Yesterday);
            }
            else
            {
                QueryExpression.Criteria.AddCondition(column, ConditionOperator.Yesterday);
            }

            return this;
        }

        #endregion Conditions
    }

    /// <summary>
    /// A typed query to request Dataverse data
    /// </summary>
    /// <typeparam name="T">Table to query</typeparam>
    public class Query<T> where T : Entity
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of class <see cref="Query{T}"/>
        /// </summary>
        public Query()
        {
            string entityLogicalName = typeof(T).GetField("EntityLogicalName")?.GetRawConstantValue().ToString();
            QueryExpression = new QueryExpression(entityLogicalName);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the <c>QueryExpression</c> representation of this <see cref="Query{T}"/>
        /// </summary>
        public QueryExpression QueryExpression { get; protected set; }

        #endregion Properties

        #region QueryExpression

        /// <summary>
        /// Specifies that the <c>QueryExpression</c> should retrieve distinct data
        /// </summary>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> Distinct()
        {
            QueryExpression.Distinct = true;

            return this;
        }

        /// <summary>
        /// Specifies that the <c>QueryExpression</c> should not lock the table when retrieving data
        /// </summary>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> NoLock()
        {
            QueryExpression.NoLock = true;

            return this;
        }

        /// <summary>
        /// Specifies the number of records the <c>QueryExpression</c> should retrieve
        /// </summary>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> Top(int top)
        {
            QueryExpression.TopCount = top;

            return this;
        }

        #endregion QueryExpression

        #region Attributes

        /// <summary>
        /// Specifies that the query should return all columns of the queried table
        /// </summary>
        /// <param name="allColumns">Boolean that indicates if all columns should be returned</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> Select(bool allColumns = false)
        {
            QueryExpression.ColumnSet = new ColumnSet(allColumns);

            return this;
        }

        /// <summary>
        /// Specifies the list of columns the query should return
        /// <example>
        /// For one column:
        /// <code>
        /// query.Select(a => a.AccountId);
        /// </code>
        /// For multiple columns:
        /// <code>
        /// query.Select(a => new { a.AccountId, a.Name});
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="columns">Columns to return</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> Select(Expression<Func<T, object>> columns)
        {
            QueryExpression.ColumnSet.AddColumns(AnonymousTypeHelper.GetAttributeNamesArray(columns));
            return this;
        }

        /// <summary>
        /// Specifies the list of columns the query should return
        /// </summary>
        /// <param name="columns">Columns to return</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> Select(params string[] columns)
        {
            if (columns.Length == 0)
            {
                QueryExpression.ColumnSet = new ColumnSet();
            }

            QueryExpression.ColumnSet.AddColumns(columns);

            return this;
        }

        #endregion Attributes

        #region Filters

        /// <summary>
        /// Add a filter under the default filter for this query
        /// </summary>
        /// <param name="filter">Filter to add</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> AddFilter(Filter<T> filter)
        {
            QueryExpression.Criteria.Filters.Add(filter.InnerFilter);

            return this;
        }

        /// <summary>
        /// Add a filter under the default filter for this query
        /// </summary>
        /// <param name="filter">Filter to add</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> AddFilter(Func<Filter<T>, Filter<T>> filter)
        {
            var fe = new Filter<T>();
            filter(fe);

            QueryExpression.Criteria.Filters.Add(fe.InnerFilter);

            return this;
        }

        /// <summary>
        /// Add multiple filters under the default filter for this query
        /// </summary>
        /// <param name="filters">Filters to add</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> AddFilters(params Filter<T>[] filters)
        {
            QueryExpression.Criteria.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        /// <summary>
        /// Add multiple filters under the default filter for this query
        /// </summary>
        /// <param name="filters">Filters to add</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> AddFilters(params Func<Filter<T>, Filter<T>>[] filters)
        {
            foreach (var filter in filters)
            {
                var fe = new Filter<T>();
                filter(fe);

                QueryExpression.Criteria.Filters.Add(fe.InnerFilter);
            }

            return this;
        }

        /// <summary>
        /// Define the filter operator for the default filter of this query
        /// </summary>
        /// <param name="logicalOperator">Logical operator</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> SetLogicalOperator(LogicalOperator logicalOperator)
        {
            QueryExpression.Criteria.FilterOperator = logicalOperator;

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
        public Shared.AppCode.Comparer<Query<T>, T> Compare(Expression<Func<T, object>> column)
        {
            return new Shared.AppCode.Comparer<Query<T>, T>(this, AnonymousTypeHelper.GetAttributeName<T>(column));
        }

        /// <summary>
        /// Starts a column comparison for a related table
        /// </summary>
        /// <param name="tableAlias">Alias of the linked table where to compare columns</param>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Query<T>, T> Compare(string tableAlias, Expression<Func<T, object>> column)
        {
            return new Shared.AppCode.Comparer<Query<T>, T>(this, tableAlias, AnonymousTypeHelper.GetAttributeName<T>(column));
        }
#endif

        #endregion Columns Comparer

        #region Link Entities

        /// <summary>
        /// Add a related table in the query
        /// </summary>
        /// <param name="link">Link to the related table</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> AddLink<U>(Link<T, U> link) where U : Entity
        {
            QueryExpression.LinkEntities.Add(link.InnerLinkEntity);
            link.InnerLinkEntity.LinkFromEntityName = QueryExpression.EntityName;
            return this;
        }

        /// <summary>
        /// Add a related table in the query
        /// </summary>
        /// <typeparam name="U">Type of the related table</typeparam>
        /// <param name="fromColumn">Column from the <typeparamref name="T"/> table</param>
        /// <param name="toColumn">Column from the <typeparamref name="U"/> table</param>
        /// <param name="link">Link to the related table</param>
        /// <param name="jo">Join operator for the link</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> AddLink<U>(Expression<Func<T, object>> fromColumn, Expression<Func<U, object>> toColumn, Func<Link<T, U>, Link<T, U>> link, JoinOperator jo = JoinOperator.Inner)
            where U : Entity
        {
            string fromEntity = typeof(T).GetField("EntityLogicalName").GetRawConstantValue().ToString();
            string fromAttr = AnonymousTypeHelper.GetAttributeName(fromColumn);
            string toAttr = AnonymousTypeHelper.GetAttributeName(toColumn);
            string toEntity = typeof(U).GetField("EntityLogicalName").GetRawConstantValue().ToString();

            var le = new Link<T, U>(fromEntity, toEntity, fromAttr, toAttr, jo);

            link(le);

            QueryExpression.LinkEntities.Add(le.InnerLinkEntity);

            return this;
        }

        /// <summary>
        /// Add a related table in the query
        /// </summary>
        /// <typeparam name="U">Type of the related table</typeparam>
        /// <param name="link">Link to the related table</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> AddLink<U>(Func<Link<T, U>, Link<T, U>> link)
           where U : Entity
        {
            string fromEntity = typeof(T).GetField("EntityLogicalName").GetRawConstantValue().ToString();
            string toEntity = typeof(U).GetField("EntityLogicalName").GetRawConstantValue().ToString();

            var le = new Link<T, U>(fromEntity, toEntity, null, null, JoinOperator.Inner);

            link(le);

            QueryExpression.LinkEntities.Add(le.InnerLinkEntity);

            return this;
        }

        /// <summary>
        /// Add a related table in the query
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="fromColumn">Column from the <typeparamref name="T"/> table</param>
        /// <param name="toColumn">Column from the <typeparamref name="U"/> table</param>
        /// <param name="jo">Join operator for the link</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> AddLink<U>(Expression<Func<T, object>> fromColumn, Expression<Func<U, object>> toColumn, JoinOperator jo = JoinOperator.Inner)
            where U : Entity

        {
            string fromEntity = typeof(T).GetField("EntityLogicalName").GetRawConstantValue().ToString();
            string fromAttr = AnonymousTypeHelper.GetAttributeName(fromColumn);
            string toAttr = AnonymousTypeHelper.GetAttributeName(toColumn);
            string toEntity = typeof(U).GetField("EntityLogicalName").GetRawConstantValue().ToString();

            var le = new Link<T, U>(fromEntity, toEntity, fromAttr, toAttr, jo);

            QueryExpression.LinkEntities.Add(le.InnerLinkEntity);

            return this;
        }

        #endregion Link Entities

        #region Order

        /// <summary>
        /// Order the result of the query by the <paramref name="column"/>
        /// </summary>
        /// <param name="column">Column to use to sort results of the query</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Query<T> OrderBy(Expression<Func<T, object>> column)
        {
            QueryExpression.AddOrder(AnonymousTypeHelper.GetAttributeName(column), OrderType.Ascending);
            return this;
        }

        /// <summary>
        /// Order the result of the query by the <paramref name="column"/> descending
        /// </summary>
        /// <param name="column">Column to use to sort results of the query</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Query<T> OrderByDescending(Expression<Func<T, object>> column)
        {
            QueryExpression.AddOrder(AnonymousTypeHelper.GetAttributeName(column), OrderType.Descending);
            return this;
        }

        #endregion Order

        #region PagingInfo

        /// <summary>
        /// Set next page number in paging info
        /// </summary>
        /// <param name="pagingCookie">Paging cookie to find the next page</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> NextPage(string pagingCookie)
        {
            QueryExpression.PageInfo.PageNumber++;
            QueryExpression.PageInfo.PagingCookie = pagingCookie;

            return this;
        }

        /// <summary>
        /// Set configuration for query paging
        /// </summary>
        /// <param name="pageNumber">Number of the page to return</param>
        /// <param name="count">Number of items to return per page</param>
        /// <param name="returnTotalCount">Indicates if total count should be returned</param>
        /// <returns>The <see cref="Query{T}"/></returns>
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

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> Where(Expression<Func<T, object>> column, ConditionOperator conditionOperator, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName<T>(column), conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> Where<U>(string tableAlias, Expression<Func<U, object>> column, ConditionOperator conditionOperator, params object[] values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereAbove(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Above, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereAbove<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Above, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereAboveOrEqual(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.AboveOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereAboveOrEqual<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.AboveOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value begins with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereBeginsWith(Expression<Func<T, object>> column, string value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.BeginsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value begins with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereBeginsWith<U>(string tableAlias, Expression<Func<U, object>> column, string value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.BeginsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereBetween(Expression<Func<T, object>> column, object value1, object value2)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Between, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereBetween<U>(string tableAlias, Expression<Func<U, object>> column, object value1, object value2) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Between, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value contains <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereContains(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Contains, value);

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
        public Query<T> WhereContains<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Contains, value);

            return this;
        }

#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereContainValues(Expression<Func<T, object>> column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ContainValues, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereContainValues<U>(string tableAlias, Expression<Func<U, object>> column, params object[] values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ContainValues, values);

            return this;
        }

#endif

        /// <summary>Adds a condition where <paramref name="column"/> value does not begin with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereDoesNotBeginWith(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotBeginWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not begin with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereDoesNotBeginWith<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotBeginWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not contain <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereDoesNotContain(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContain, value);

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
        public Query<T> WhereDoesNotContain<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContain, value);

            return this;
        }

#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> contdoes not containains <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereDoesNotContainValues(Expression<Func<T, object>> column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContainValues, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereDoesNotContainValues<U>(string tableAlias, Expression<Func<U, object>> column, params object[] values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContainValues, values);

            return this;
        }
#endif

        /// <summary>Adds a condition where <paramref name="column"/> value does not end with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereDoesNotEndWith(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotEndWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not end with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereDoesNotEndWith<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotEndWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value ends with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEndsWith(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EndsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value ends with <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEndsWith<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EndsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value equals <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqual(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Equal, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value equals <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqual<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Equal, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user business unit</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualBusinessId(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user business unit</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualBusinessId<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user unique identifier</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserId(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user unique identifier</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserId<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user language</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserLanguage(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserLanguage);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user language</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserLanguage<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserLanguage);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or their reporting hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserOrUserHierarchy(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchy);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or their reporting hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserOrUserHierarchy<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchy);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user and his teams or their reporting hierarchy and their teams</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserOrUserHierarchyAndTeams(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchyAndTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user and his teams or their reporting hierarchy and their teams</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserOrUserHierarchyAndTeams<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchyAndTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or a team the current user is member of</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserOrUserTeams(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or a team the current user is member of</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserOrUserTeams<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals a team the current user is member of</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserTeams(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals a team the current user is member of</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereEqualUserTeams<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater or equal to <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereGreaterEqual(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater or equal to <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereGreaterEqual<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater than <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereGreaterThan(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater than <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereGreaterThan<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereIn(Expression<Func<T, object>> column, IList values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereIn<U>(string tableAlias, Expression<Func<U, object>> column, IList values) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereIn(Expression<Func<T, object>> column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereIn(string tableAlias, Expression<Func<T, object>> column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInFiscalPeriod(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriod, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInFiscalPeriod<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriod, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInFiscalPeriodAndYear(Expression<Func<T, object>> column, int period, int year)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInFiscalPeriodAndYear<U>(string tableAlias, Expression<Func<U, object>> column, int period, int year) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInFiscalYear(Expression<Func<T, object>> column, int year)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalYear, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInFiscalYear<U>(string tableAlias, Expression<Func<U, object>> column, int year) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalYear, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or after the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInOrAfterFiscalPeriodAndYear(Expression<Func<T, object>> column, int period, int year)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or after the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInOrAfterFiscalPeriodAndYear<U>(string tableAlias, Expression<Func<U, object>> column, int period, int year) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or before the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInOrBeforeFiscalPeriodAndYear(Expression<Func<T, object>> column, int period, int year)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or before the curent fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereInOrBeforeFiscalPeriodAndYear<U>(string tableAlias, Expression<Func<U, object>> column, int period, int year) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on last 7 days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLast7Days(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Last7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on last 7 days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLast7Days<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Last7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastFiscalPeriod(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastFiscalPeriod<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastFiscalYear(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastFiscalYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastMonth(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastMonth<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastWeek(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastWeek<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXDays(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXDays<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal periods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXFiscalPeriods(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal periods</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal periods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXFiscalPeriods<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXFiscalYears(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXFiscalYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXHours(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXHours<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXMonths(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXMonths<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXWeeks(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXWeeks<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXYears(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastXYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastYear(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLastYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less or equal to <paramref name="value"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLessEqual(Expression<Func<T, object>> column, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less or equal to <paramref name="value"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLessEqual<U>(string tableAlias, Expression<Func<U, object>> column, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less than <paramref name="value"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLessThan(Expression<Func<T, object>> column, int value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less than <paramref name="value"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLessThan<U>(string tableAlias, Expression<Func<U, object>> column, int value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value matches the specifed <paramref name="pattern"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLike(Expression<Func<T, object>> column, string pattern)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Like, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value matches the specifed <paramref name="pattern"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereLike<U>(string tableAlias, Expression<Func<U, object>> column, string pattern) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Like, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereMask(Expression<Func<T, object>> column, object bitmask)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Mask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereMask<U>(string tableAlias, Expression<Func<U, object>> column, object bitmask) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Mask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next 7 days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNext7Days(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Next7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next 7 days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNext7Days<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Next7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextFiscalPeriod(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextFiscalPeriod<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextFiscalYear(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextFiscalYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextMonth(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextMonth<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextWeek(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextWeek<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXDays(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXDays<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal perdiods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXFiscalPeriods(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal perdiods to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXFiscalPeriods<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXFiscalYears(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXFiscalYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXHours(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXHours<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXMonths(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXMonths<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXWeeks(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXWeeks<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXYears(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextXYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextYear(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNextYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotBetween(Expression<Func<T, object>> column, object value1, object value2)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotBetween, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotBetween<U>(string tableAlias, Expression<Func<U, object>> column, object value1, object value2) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotBetween, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not equal <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotEqual(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not equal <paramref name="value"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotEqual<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user business unit</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotEqualBusinessId(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user business unit</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotEqualBusinessId<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user unique identifier</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotEqualUserId(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user unique identifier</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotEqualUserId<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotIn(Expression<Func<T, object>> column, IList values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotIn(string tableAlias, Expression<Func<T, object>> column, IList values)
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotIn(Expression<Func<T, object>> column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotIn(string tableAlias, Expression<Func<T, object>> column, params object[] values)
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not match the specifed <paramref name="pattern"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotLike(Expression<Func<T, object>> column, string pattern)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotLike, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not match the specifed <paramref name="pattern"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotLike<U>(string tableAlias, Expression<Func<U, object>> column, string pattern) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotLike, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotMask(Expression<Func<T, object>> column, object bitmask)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotMask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotMask<U>(string tableAlias, Expression<Func<U, object>> column, object bitmask) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotMask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not null</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotNull(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotNull);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not null</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotNull<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotNull);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not on the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotOn(Expression<Func<T, object>> column, DateTime datetime)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotOn, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not on the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotOn<U>(string tableAlias, Expression<Func<U, object>> column, DateTime datetime) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotOn, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is above <paramref name="value"/> in the hierarchy </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotUnder(Expression<Func<T, object>> column, object value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotUnder, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is above <paramref name="value"/> in the hierarchy </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNotUnder<U>(string tableAlias, Expression<Func<U, object>> column, object value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotUnder, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain data</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNull(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Null);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain data</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereNull<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Null);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXDays(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> days</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXDays<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXHours(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> hours</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXHours<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> minutes</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of minutes to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXMinutes(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMinutes, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> minutes</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of minutes to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXMinutes<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMinutes, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXMonths(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> months</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXMonths<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXWeeks(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> weeks</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXWeeks<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXYears(Expression<Func<T, object>> column, int x)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> years</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOlderThanXYears<U>(string tableAlias, Expression<Func<U, object>> column, int x) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOn(Expression<Func<T, object>> column, DateTime datetime)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.On, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOn<U>(string tableAlias, Expression<Func<U, object>> column, DateTime datetime) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.On, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or after the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOnOrAfter(Expression<Func<T, object>> column, DateTime datetime)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrAfter, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or after the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOnOrAfter<U>(string tableAlias, Expression<Func<U, object>> column, DateTime datetime) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrAfter, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or before the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOnOrBefore(Expression<Func<T, object>> column, DateTime datetime)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrBefore, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or before the specifed <paramref name="datetime"/> </summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereOnOrBefore<U>(string tableAlias, Expression<Func<U, object>> column, DateTime datetime) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrBefore, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisFiscalPeriod(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal period</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisFiscalPeriod<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisFiscalYear(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisFiscalYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisMonth(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current month</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisMonth<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisWeek(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current week</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisWeek<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisYear(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current year</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereThisYear<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current day</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereToday(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Today);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current day</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereToday<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Today);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is tomorrow</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereTomorrow(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Tomorrow);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is tomorrow</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereTomorrow<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Tomorrow);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereUnder(Expression<Func<T, object>> column, Guid value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Under, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereUnder<U>(string tableAlias, Expression<Func<U, object>> column, Guid value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Under, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereUnderOrEqual(Expression<Func<T, object>> column, Guid value)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.UnderOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereUnderOrEqual<U>(string tableAlias, Expression<Func<U, object>> column, Guid value) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.UnderOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is yesterday</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereYesterday(Expression<Func<T, object>> column)
        {
            QueryExpression.Criteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Yesterday);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is yesterday</summary>
        /// <param name="tableAlias">Logical name of the related table where to apply the condition</param>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Query{T}"/></returns>
        public Query<T> WhereYesterday<U>(string tableAlias, Expression<Func<U, object>> column) where U : Entity
        {
            QueryExpression.Criteria.AddCondition(tableAlias, AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Yesterday);

            return this;
        }

        #endregion Conditions

        #region Service calls

        /// <summary>
        /// Retrieve all records for the entity
        /// </summary>
        /// <param name="service">Dataverse service</param>
        /// <returns>List of <typeparamref name="T"/> records</returns>
        public List<T> GetAll(IOrganizationService service)
        {
            if (QueryExpression.TopCount.HasValue)
            {
                return service.RetrieveMultiple(QueryExpression)
                    .Entities
                    .Select(e => e.ToEntity<T>())
                    .ToList();
            }

            var tmpPagingNumber = QueryExpression.PageInfo.PageNumber;
            var tmpPagingCookie = QueryExpression.PageInfo.PagingCookie;
            EntityCollection ec;
            var list = new List<T>();
            do
            {
                ec = service.RetrieveMultiple(QueryExpression);

                list.AddRange(ec.Entities.Select(e => e.ToEntity<T>()));

                NextPage(ec.PagingCookie);
            } while (ec.MoreRecords);

            QueryExpression.PageInfo.PageNumber = tmpPagingNumber;
            QueryExpression.PageInfo.PagingCookie = tmpPagingCookie;
            return list;
        }

        /// <summary>
        /// Retrieve the first item from the query
        /// </summary>
        /// <param name="service">Dataverse service</param>
        /// <returns>First <typeparamref name="T"/> record</returns>
        public T GetFirst(IOrganizationService service)
        {
            var topCount = 0;
            if (!QueryExpression.TopCount.HasValue)
            {
                QueryExpression.TopCount = 1;
            }
            else
            {
                topCount = QueryExpression.TopCount.Value;
            }

            var record = service.RetrieveMultiple(QueryExpression).Entities.First().ToEntity<T>();

            if (topCount == 0)
            {
                QueryExpression.TopCount = null;
            }
            else
            {
                QueryExpression.TopCount = topCount;
            }

            return record;
        }

        /// <summary>
        /// Retrieve the first item from the query or null if no records match the query
        /// </summary>
        /// <param name="service">Dataverse service</param>
        /// <returns>First <typeparamref name="T"/> record</returns>
        public T GetFirstOrDefault(IOrganizationService service)
        {
            var topCount = 0;
            if (!QueryExpression.TopCount.HasValue)
            {
                QueryExpression.TopCount = 1;
            }
            else
            {
                topCount = QueryExpression.TopCount.Value;
            }

            var record = service.RetrieveMultiple(QueryExpression).Entities.FirstOrDefault()?.ToEntity<T>();

            if (topCount == 0)
            {
                QueryExpression.TopCount = null;
            }
            else
            {
                QueryExpression.TopCount = topCount;
            }

            return record;
        }

        /// <summary>
        /// Retrieve the last item from the query
        /// </summary>
        /// <param name="service">Dataverse service</param>
        /// <returns>First <typeparamref name="T"/> record</returns>
        public T GetLast(IOrganizationService service)
        {
            if (QueryExpression.Orders.Count == 0)
            {
                throw new InvalidOperationException("Please provide a sorting criteria before using GetLast method");
            }

            foreach (var order in QueryExpression.Orders)
            {
                order.OrderType = (order.OrderType == OrderType.Ascending) ? OrderType.Descending : OrderType.Ascending;
            }

            var results = GetFirst(service);

            foreach (var order in QueryExpression.Orders)
            {
                order.OrderType = (order.OrderType == OrderType.Ascending) ? OrderType.Descending : OrderType.Ascending;
            }

            return results;
        }

        /// <summary>
        /// Retrieve the last item from the query or null if no records match the query
        /// </summary>
        /// <param name="service">Dataverse service</param>
        /// <returns>First <typeparamref name="T"/> record</returns>
        public T GetLastOrDefault(IOrganizationService service)
        {
            if (QueryExpression.Orders.Count == 0)
            {
                throw new InvalidOperationException("Please provide a sorting criteria before using GetLast method");
            }

            foreach (var order in QueryExpression.Orders)
            {
                order.OrderType = (order.OrderType == OrderType.Ascending) ? OrderType.Descending : OrderType.Ascending;
            }

            var results = GetFirstOrDefault(service);

            foreach (var order in QueryExpression.Orders)
            {
                order.OrderType = (order.OrderType == OrderType.Ascending) ? OrderType.Descending : OrderType.Ascending;
            }

            return results;
        }

        /// <summary>
        /// Retrieves current page of records for the query
        /// </summary>
        /// <param name="service">Dataverse service</param>
        /// <returns>List of records as <see cref="EntityCollection"/></returns>
        public EntityCollection GetResults(IOrganizationService service)
        {
            return service.RetrieveMultiple(QueryExpression);
        }

        /// <summary>
        /// Returns the only record of the query result and throw an exception if there is not exactly one record in the result
        /// </summary>
        /// <param name="service">Dataverse service</param>
        /// <returns>The single <typeparamref name="T"/> record</returns>
        public T GetSingle(IOrganizationService service)
        {
            var topCount = 0;
            if (!QueryExpression.TopCount.HasValue)
            {
                QueryExpression.TopCount = 2;
            }
            else
            {
                topCount = QueryExpression.TopCount.Value;
            }

            var record = service.RetrieveMultiple(QueryExpression).Entities.Single().ToEntity<T>();

            if (topCount == 0)
            {
                QueryExpression.TopCount = null;
            }
            else
            {
                QueryExpression.TopCount = topCount;
            }

            return record;
        }

        /// <summary>
        /// Returns the only record of the query result or null if no record and throw an exception if there is more than one record in the result
        /// </summary>
        /// <param name="service">Dataverse service</param>
        /// <returns>The single <typeparamref name="T"/> record</returns>
        public T GetSingleOrDefault(IOrganizationService service)
        {
            var topCount = 0;
            if (!QueryExpression.TopCount.HasValue)
            {
                QueryExpression.TopCount = 2;
            }
            else
            {
                topCount = QueryExpression.TopCount.Value;
            }

            var record = service.RetrieveMultiple(QueryExpression).Entities.SingleOrDefault()?.ToEntity<T>();

            if (topCount == 0)
            {
                QueryExpression.TopCount = null;
            }
            else
            {
                QueryExpression.TopCount = topCount;
            }

            return record;
        }

        /// <summary>
        /// Returns a record from its unique identifier or null if the record does not exist
        /// </summary>
        /// <param name="id">Unique identifier of the record</param>
        /// <param name="service">Dataverse service</param>
        /// <param name="isActivityEntity">Indicates if the table queried is an activity table (optional if <typeparamref name="T"/> is not <c>Entity</c>)</param>
        /// <returns></returns>
        public T GetById(Guid id, IOrganizationService service, bool isActivityEntity = false)
        {
            string primaryKey = typeof(T) != typeof(Entity) ? typeof(T).GetField("PrimaryIdAttribute").GetRawConstantValue().ToString() : null;
            if (string.IsNullOrEmpty(primaryKey))
            {
                primaryKey = isActivityEntity ? "activityid" : $"{QueryExpression.EntityName}id";
            }

            QueryExpression.Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(primaryKey, ConditionOperator.Equal, id)
                }
            };

            return GetFirstOrDefault(service);
        }

        #endregion Service calls
    }
}