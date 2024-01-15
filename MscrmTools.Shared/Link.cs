using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using MscrmTools.FluentQueryExpressions.Helpers;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MscrmTools.FluentQueryExpressions
{
    /// <summary>
    /// A link between two Dataverse table for a query
    /// </summary>
    public class Link : Link<Entity, Entity>
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="Link"/> class
        /// </summary>
        /// <param name="toEntity">Target table logical name</param>
        /// <param name="toAttribute">Target table column logical name</param>
        /// <param name="fromAttribute">Source table column logical name</param>
        /// <param name="joinOperator">Relationship join operator</param>
        public Link(string toEntity, string toAttribute, string fromAttribute, JoinOperator joinOperator = JoinOperator.Inner) :
            base(toEntity, toAttribute, fromAttribute, joinOperator)
        {
            ToEntity = toEntity;
        }

        #region Filters

        /// <summary>
        /// Adds a filter for the target table
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>The link</returns>
        public Link AddFilter(Filter filter)
        {
            InnerLinkEntity.LinkCriteria.Filters.Add(filter.InnerFilter);

            return this;
        }

        /// <summary>
        /// Adds filters for the target table
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <returns>The link</returns>
        public Link AddFilters(params Filter[] filters)
        {
            InnerLinkEntity.LinkCriteria.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        #endregion Filters

        #region Link Entities

        /// <summary>Adds a related table as a link.</summary>
        /// <param name="link">The link.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link AddLink(Link link)
        {
            InnerLinkEntity.LinkEntities.Add(link.InnerLinkEntity);
            link.InnerLinkEntity.LinkFromEntityName = InnerLinkEntity.LinkToEntityName;
            return this;
        }
        /// <summary>Adds a related table as a link.</summary>
        /// <param name="sourceColumn">Logical name of the source column in the relationship</param>
        /// <param name="targetTable">Logical name of the target table</param>
        /// <param name="targetColumn">Logical name of the target column in the relationship</param>
        /// <param name="jo">The join operator.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link AddLink(string sourceColumn, string targetTable, string targetColumn, JoinOperator jo = JoinOperator.Inner)
        {
            string fromEntity = InnerLinkEntity.LinkToEntityName;

            var link = new LinkEntity
            {
                LinkFromEntityName = fromEntity,
                LinkFromAttributeName = sourceColumn,
                LinkToAttributeName = targetColumn,
                LinkToEntityName = targetTable,
                JoinOperator = jo,
                EntityAlias = targetTable
            };

            InnerLinkEntity.LinkEntities.Add(link);
            return this;
        }

        #endregion Link Entities

        #region Columns Comparer

#if CRMV9

        /// <summary>
        /// Starts a column comparison
        /// </summary>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Link> Compare(string column)
        {
            return new Shared.AppCode.Comparer<Link>(this, column);
        }

#endif

        #endregion Columns Comparer
    }

    /// <summary>
    /// A link between two Dataverse table for a query
    /// </summary>
    /// <typeparam name="T">Strongly typed source table in the relationship</typeparam>
    /// <typeparam name="U">Strongly typed target table in the relationship</typeparam>
    public class Link<T, U>
        where T : Entity
        where U : Entity
    {
        /// <summary>
        /// Target table
        /// </summary>
        protected string ToEntity;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Link{T, U}" /> class.</summary>
        /// <param name="toAttribute">Logical name of the target table column in the relationship</param>
        /// <param name="fromAttribute">Logical name of the source table column in the relationship</param>
        /// <param name="joinOperator">The join operator.</param>
        public Link(string toAttribute, string fromAttribute, JoinOperator joinOperator = JoinOperator.Inner)
        {
            string toEntity = typeof(U).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;
            ToEntity = toEntity;
            InnerLinkEntity = new LinkEntity(null, toEntity, fromAttribute, toAttribute, joinOperator)
            { EntityAlias = toEntity };
        }

        /// <summary>Initializes a new instance of the <see cref="Link{T, U}" /> class.</summary>
        /// <param name="toAttribute">Logical name of the target table column in the relationship</param>
        /// <param name="fromAttribute">Logical name of the source table column in the relationship</param>
        /// <param name="joinOperator">The join operator.</param>
        public Link(Expression<Func<T, object>> fromAttribute, Expression<Func<U, object>> toAttribute, JoinOperator joinOperator = JoinOperator.Inner)
        {
            string fromEntity = typeof(T).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;
            string toEntity = typeof(U).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;
            ToEntity = toEntity;
            InnerLinkEntity = new LinkEntity(fromEntity, toEntity, AnonymousTypeHelper.GetAttributeName(fromAttribute), AnonymousTypeHelper.GetAttributeName(toAttribute), joinOperator)
            { EntityAlias = toEntity };
        }

        /// <summary>Initializes a new instance of the <see cref="Link{T, U}" /> class.</summary>
        /// <param name="toEntity">Logical name of the target table</param>
        /// <param name="toAttribute">Logical name of the target table column in the relationship</param>
        /// <param name="fromAttribute">Logical name of the source table column in the relationship</param>
        /// <param name="joinOperator">The join operator.</param>
        protected Link(string toEntity, string toAttribute, string fromAttribute, JoinOperator joinOperator = JoinOperator.Inner)
        {
            InnerLinkEntity = new LinkEntity(null, toEntity, fromAttribute, toAttribute, joinOperator)
            { EntityAlias = toEntity };
        }

        /// <summary>Initializes a new instance of the <see cref="Link{T, U}" /> class.</summary>
        /// <param name="fromEntity">Logical name of the source table</param>
        /// <param name="toEntity">Logical name of the target table</param>
        /// <param name="toAttribute">Logical name of the target table column in the relationship</param>
        /// <param name="fromAttribute">Logical name of the source table column in the relationship</param>
        /// <param name="joinOperator">The join operator.</param>
        public Link(string fromEntity, string toEntity, string fromAttribute, string toAttribute, JoinOperator joinOperator = JoinOperator.Inner)
        {
            InnerLinkEntity = new LinkEntity(fromEntity, toEntity, fromAttribute, toAttribute, joinOperator)
            { EntityAlias = toEntity };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the inner <see cref="LinkEntity"/> of this link
        /// </summary>
        public LinkEntity InnerLinkEntity { get; }

        #endregion Properties

        #region Link

        /// <summary>Set the column for the relationship from the source table</summary>
        /// <param name="column">The column</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> From(Expression<Func<T, object>> column)
        {
            InnerLinkEntity.LinkFromAttributeName = AnonymousTypeHelper.GetAttributeName(column);

            return this;
        }

        /// <summary>Set the column for the relationship from the target table</summary>
        /// <param name="column">The column</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> To(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkToAttributeName = AnonymousTypeHelper.GetAttributeName(column);

            return this;
        }

        /// <summary>Set the join operator for the relationship</summary>
        /// <param name="joinOperator">The join operator</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> SetJoin(JoinOperator joinOperator)
        {
            InnerLinkEntity.JoinOperator = joinOperator;

            return this;
        }

        /// <summary>Set the alias for the relationship</summary>
        /// <param name="alias">The alias</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> SetAlias(string alias)
        {
            InnerLinkEntity.EntityAlias = alias;

            return this;
        }

        #endregion Link

        #region Attributes

        /// <summary>Selects all columns of the target table</summary>
        /// <param name="allColumns">if set to <c>true</c>, retrieves all columns, else retrieves no column.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> Select(bool allColumns = false)
        {
            InnerLinkEntity.Columns = new ColumnSet(allColumns);

            return this;
        }

        /// <summary>Selects the specified columns from the target table.
        /// <example>
        /// For one column:
        /// <code>
        /// link.Select(a => a.AccountId);
        /// </code>
        /// For multiple columns:
        /// <code>
        /// link.Select(a => new { a.AccountId, a.Name});
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="column">The column(s).</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> Select(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.Columns.AddColumns(AnonymousTypeHelper.GetAttributeNamesArray(column));
            return this;
        }

        /// <summary>Selects the specified columns from the target table.</summary>
        /// <param name="columns">The colums.</param>
        /// <returns>This <see cref=""></see></returns>
        public Link<T, U> Select(params string[] columns)
        {
            InnerLinkEntity.Columns.AddColumns(columns);

            return this;
        }

        #endregion Attributes

        #region Filters

        /// <summary>Adds a filter.</summary>
        /// <param name="filter">The filter.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> AddFilter(Filter<U> filter)
        {
            InnerLinkEntity.LinkCriteria.Filters.Add(filter.InnerFilter);

            return this;
        }

        /// <summary>Adds a filter.</summary>
        /// <param name="filter">The filter as a lambda expression</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> AddFilter(Func<Filter<U>, Filter<U>> filter)
        {
            var fe = new Filter<U>();
            filter(fe);

            InnerLinkEntity.LinkCriteria.Filters.Add(fe.InnerFilter);

            return this;
        }

        /// <summary>Adds multiple filters.</summary>
        /// <param name="filters">The filters.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> AddFilters(params Filter<U>[] filters)
        {
            InnerLinkEntity.LinkCriteria.Filters.AddRange(filters.Select(f => f.InnerFilter));

            return this;
        }

        /// <summary>Adds multiple filters.</summary>
        /// <param name="filters">The filters.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> AddFilters(params Func<Filter<U>, Filter<U>>[] filters)
        {
            foreach (var filter in filters)
            {
                var fe = new Filter<U>();
                filter(fe);

                InnerLinkEntity.LinkCriteria.Filters.Add(fe.InnerFilter);
            }

            return this;
        }

        /// <summary>Sets the logical operator for the link root filter</summary>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> SetLogicalOperator(LogicalOperator logicalOperator)
        {
            InnerLinkEntity.LinkCriteria.FilterOperator = logicalOperator;

            return this;
        }

        #endregion Filters

        #region Link Entities

        /// <summary>Adds a related table as a link.</summary>
        /// <param name="link">The link.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> AddLink<V>(Link<U, V> link) where V : Entity
        {
            string fromEntity = typeof(U).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;

            InnerLinkEntity.LinkEntities.Add(link.InnerLinkEntity);

            link.InnerLinkEntity.LinkFromEntityName = fromEntity;

            return this;
        }

        /// <summary>Adds a related table as a link.</summary>
        /// <param name="sourceColumn">Column from the <typeparamref name="T"/> table</param>
        /// <param name="targetColumn">Column from the <typeparamref name="U"/> table</param>
        /// /// <param name="link">The link as lambda expression</param>
        /// <param name="jo">The join operator.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> AddLink<V>(Expression<Func<U, object>> sourceColumn, Expression<Func<V, object>> targetColumn, Func<Link<U, V>, Link<U, V>> link, JoinOperator jo = JoinOperator.Inner) where V : Entity
        {
            string fromEntity = typeof(U).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;
            string fromAttr = AnonymousTypeHelper.GetAttributeName(sourceColumn);
            string toAttr = AnonymousTypeHelper.GetAttributeName(targetColumn);
            string toEntity = typeof(V).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;

            var le = new Link<U, V>(fromEntity, toEntity, fromAttr, toAttr, jo);

            link(le);

            InnerLinkEntity.LinkEntities.Add(le.InnerLinkEntity);

            return this;
        }

        /// <summary>Adds a related table as a link.</summary>
        /// <param name="link">The link as lambda expression</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> AddLink<V>(Func<Link<U, V>, Link<U, V>> link) where V : Entity
        {
            string fromEntity = typeof(U).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;
            string toEntity = typeof(V).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;

            var le = new Link<U, V>(fromEntity, toEntity, null, null, JoinOperator.Inner);

            link(le);

            InnerLinkEntity.LinkEntities.Add(le.InnerLinkEntity);

            return this;
        }

        /// <summary>Adds a related table as a link.</summary>
        /// <param name="sourceColumn">Column from the <typeparamref name="T"/> table</param>
        /// <param name="targetColumn">Column from the <typeparamref name="U"/> table</param>
        /// <param name="jo">The join operator.</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> AddLink<V>(Expression<Func<U, object>> sourceColumn, Expression<Func<V, object>> targetColumn, JoinOperator jo = JoinOperator.Inner) where V : Entity
        {
            string fromEntity = typeof(U).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;
            string fromAttr = AnonymousTypeHelper.GetAttributeName(sourceColumn);
            string toAttr = AnonymousTypeHelper.GetAttributeName(targetColumn);
            string toEntity = typeof(V).GetCustomAttribute<EntityLogicalNameAttribute>(true).LogicalName;

            var le = new Link<U, V>(fromEntity, toEntity, fromAttr, toAttr, jo);

            InnerLinkEntity.LinkEntities.Add(le.InnerLinkEntity);

            return this;
        }

        #endregion Link Entities

        #region Order

        /// <summary>
        /// Order the result of the query by the <paramref name="column"/>
        /// </summary>
        /// <param name="column">Column to use to sort results of the query</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> OrderBy(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.Orders.Add(new OrderExpression(AnonymousTypeHelper.GetAttributeName(column), OrderType.Ascending));

            return this;
        }

        /// <summary>
        /// Order the result of the query by the <paramref name="column"/>
        /// </summary>
        /// <param name="column">Column to use to sort results of the query</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> OrderBy(string column)
        {
            InnerLinkEntity.Orders.Add(new OrderExpression(column, OrderType.Ascending));

            return this;
        }

        /// <summary>
        /// Order the result of the query by the <paramref name="column"/> descending
        /// </summary>
        /// <param name="column">Column to use to sort results of the query</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> OrderByDescending(string column)
        {
            InnerLinkEntity.Orders.Add(new OrderExpression(column, OrderType.Descending));

            return this;
        }

        /// <summary>
        /// Order the result of the query by the <paramref name="column"/> descending
        /// </summary>
        /// <param name="column">Column to use to sort results of the query</param>
        /// <returns>This <see cref="Link{T, U}"></see></returns>
        public Link<T, U> OrderByDescending(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.Orders.Add(new OrderExpression(AnonymousTypeHelper.GetAttributeName(column), OrderType.Descending));

            return this;
        }

        #endregion Order

        #region Columns Comparer

#if CRMV9

        /// <summary>
        /// Starts a column comparison
        /// </summary>
        /// <param name="column">First column to compare</param>
        /// <returns>The Comparer</returns>
        public Shared.AppCode.Comparer<Link<T, U>, U> Compare(Expression<Func<U, object>> column)
        {
            return new Shared.AppCode.Comparer<Link<T, U>, U>(this, AnonymousTypeHelper.GetAttributeName(column));
        }

#endif

        #endregion Columns Comparer

        #region Conditions

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> Where(Expression<Func<U, object>> column, ConditionOperator conditionOperator, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereAbove(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Above, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereAboveOrEqual(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.AboveOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value begins with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereBeginsWith(Expression<Func<U, object>> column, string value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.BeginsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereBetween(Expression<Func<U, object>> column, object value1, object value2)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Between, value1, value2);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value contains <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereContains(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Contains, value);

            return this;
        }

#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereContainValues(Expression<Func<U, object>> column, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ContainValues, values);

            return this;
        }

#endif
        /// <summary>Adds a condition where <paramref name="column"/> value does not begin with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereDoesNotBeginWith(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotBeginWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not contain <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereDoesNotContain(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContain, value);

            return this;
        }

#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> does not contain <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereDoesNotContainValues(Expression<Func<U, object>> column, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotContainValues, values);

            return this;
        }

#endif

        /// <summary>Adds a condition where <paramref name="column"/> value does not end with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereDoesNotEndWith(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.DoesNotEndWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value ends with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEndsWith(Expression<Func<U, object>> column, string value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EndsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value equals <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqual(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Equal, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user business unit</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualBusinessId(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user unique identifier</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserId(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user language</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserLanguage(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserLanguage);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or their reporting hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserOrUserHierarchy(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchy);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user and his teams or their reporting hierarchy and their teams</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserOrUserHierarchyAndTeams(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserHierarchyAndTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or a team the current user is member of</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserOrUserTeams(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserOrUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals a team the current user is member of</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserTeams(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.EqualUserTeams);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater or equal to <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereGreaterEqual(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is greater than <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereGreaterThan(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.GreaterThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereIn(Expression<Func<U, object>> column, IList values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereIn(Expression<Func<U, object>> column, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.In, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInFiscalPeriod(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriod, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInFiscalPeriodAndYear(Expression<Func<U, object>> column, int period, int year)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInFiscalYear(Expression<Func<U, object>> column, int year)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InFiscalYear, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or after the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInOrAfterFiscalPeriodAndYear(Expression<Func<U, object>> column, int period, int year)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrAfterFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or before the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInOrBeforeFiscalPeriodAndYear(Expression<Func<U, object>> column, int period, int year)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.InOrBeforeFiscalPeriodAndYear, period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on last 7 days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLast7Days(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Last7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastFiscalPeriod(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastFiscalYear(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastMonth(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastWeek(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXDays(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal periods to evaluate</param>
        public Link<T, U> WhereLastXFiscalPeriods(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXFiscalYears(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXHours(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXMonths(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXWeeks(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXYears(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastYear(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LastYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less or equal to <paramref name="value"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLessEqual(Expression<Func<U, object>> column, int value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less than <paramref name="value"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLessThan(Expression<Func<U, object>> column, int value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.LessThan, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value matches the specifed <paramref name="pattern"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLike(Expression<Func<U, object>> column, string pattern)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Like, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereMask(Expression<Func<U, object>> column, object bitmask)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Mask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next 7 days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNext7Days(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Next7Days);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextFiscalPeriod(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextFiscalYear(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextMonth(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on next week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextWeek(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXDays(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal perdiods to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXFiscalPeriods(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalPeriods, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXFiscalYears(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXHours(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXMonths(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXWeeks(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXYears(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextYear(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NextYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotBetween(Expression<Func<U, object>> column, object value1, object value2)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotBetween, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not equal <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotEqual(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqual, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user business unit</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotEqualBusinessId(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user unique identifier</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotEqualUserId(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotEqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotIn(Expression<Func<U, object>> column, IList values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotIn(Expression<Func<U, object>> column, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not match the specifed <paramref name="pattern"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        public Link<T, U> WhereNotLike(string column, string pattern)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotLike, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not match the specifed <paramref name="pattern"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        public Link<T, U> WhereNotLike(Expression<Func<U, object>> column, string pattern)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotLike, pattern);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotMask(Expression<Func<U, object>> column, object bitmask)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotMask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not null</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotNull(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotNull);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not on the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotOn(Expression<Func<U, object>> column, DateTime datetime)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotOn, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is above <paramref name="value"/> in the hierarchy </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotUnder(Expression<Func<U, object>> column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.NotUnder, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not contain data</summary>
        /// <param name="column">Logical name of the column</param>
        public Link<T, U> WhereNull(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Null);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXDays(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXHours(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> minutes</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of minutes to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXMinutes(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMinutes, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXMonths(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXMonths, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXWeeks(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXWeeks, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXYears(Expression<Func<U, object>> column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OlderThanXYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOn(Expression<Func<U, object>> column, DateTime datetime)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.On, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or after the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOnOrAfter(Expression<Func<U, object>> column, DateTime datetime)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrAfter, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or before the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOnOrBefore(Expression<Func<U, object>> column, DateTime datetime)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.OnOrBefore, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisFiscalPeriod(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalPeriod);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisFiscalYear(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisMonth(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisMonth);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisWeek(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisWeek);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisYear(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.ThisYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in the current day</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereToday(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Today);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is tomorrow</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereTomorrow(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Tomorrow);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereUnder(Expression<Func<U, object>> column, Guid value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Under, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereUnderOrEqual(Expression<Func<U, object>> column, Guid value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.UnderOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is yesterday</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereYesterday(Expression<Func<U, object>> column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(AnonymousTypeHelper.GetAttributeName(column), ConditionOperator.Yesterday);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> respects the <paramref name="conditionOperator"/> and the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="conditionOperator">The condition operator.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="Link{T, U}"/></returns>
        public Link<T, U> Where(string column, ConditionOperator conditionOperator, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, conditionOperator, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is above <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereAbove(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Above, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> is above or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereAboveOrEqual(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.AboveOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value begins with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereBeginsWith(string column, string value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.BeginsWith, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereBetween(string column, object value1, object value2)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Between, value1, value2);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value contains <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereContains(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Contains, value);

            return this;
        }
#if CRMV9

        /// <summary>Adds a condition where <paramref name="column"/> contains <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereContainValues(string column, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.ContainValues, values);

            return this;
        }

#endif

        /// <summary>Adds a condition where <paramref name="column"/> value does not begin with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereDoesNotBeginWith(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.DoesNotBeginWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value does not contain <paramref name="value"/></summary>
        /// <remarks>You must use the Contains operator for only those attributes that are enabled for full-text indexing.
        /// Otherwise, you will receive a generic SQL error message while retrieving data. In a Microsoft Dynamics 365 default
        /// installation, only the attributes of the KBArticle (article) entity are enabled for full-text indexing.</remarks>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">Value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereDoesNotContain(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.DoesNotContain, value);

            return this;
        }

#if CRMV9
        /// <summary>Adds a condition where <paramref name="column"/> does not contain <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">Values to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereDoesNotContainValues(string column, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.DoesNotContainValues, values);

            return this;
        }

#endif
        /// <summary>Adds a condition where <paramref name="column"/> value does not end with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereDoesNotEndWith(string column, string value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.DoesNotEndWith, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value ends with <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEndsWith(string column, string value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.EndsWith, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value equals <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqual(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Equal, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user business unit</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualBusinessId(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.EqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user unique identifier</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserId(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.EqualUserId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user language</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserLanguage(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.EqualUserLanguage);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> equals current user or their reporting hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserOrUserHierarchy(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.EqualUserOrUserHierarchy);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> equals current user and his teams or their reporting hierarchy and their teams</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserOrUserHierarchyAndTeams(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.EqualUserOrUserHierarchyAndTeams);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> equals current user or a team the current user is member of</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserOrUserTeams(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.EqualUserOrUserTeams);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> equals a team the current user is member of</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereEqualUserTeams(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.EqualUserTeams);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> is greater or equal to <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereGreaterEqual(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.GreaterEqual, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> is greater than <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereGreaterThan(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.GreaterThan, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereIn(string column, IList values)
        {
            InnerLinkEntity.LinkCriteria.Conditions.Add(new ConditionExpression(column, ConditionOperator.In, values));

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereIn(string column, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.Conditions.Add(new ConditionExpression(column, ConditionOperator.In, values));

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInFiscalPeriod(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.InFiscalPeriod, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInFiscalPeriodAndYear(string column, int period, int year)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.InFiscalPeriodAndYear, period, year);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInFiscalYear(string column, int year)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.InFiscalYear, year);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in or after the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInOrAfterFiscalPeriodAndYear(string column, int period, int year)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.InOrAfterFiscalPeriodAndYear,
                period, year);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is in or before the curent fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="period">The period</param>
        /// <param name="year">The year</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereInOrBeforeFiscalPeriodAndYear(string column, int period, int year)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.InOrBeforeFiscalPeriodAndYear,
                period, year);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on last 7 days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLast7Days(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Last7Days);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastFiscalPeriod(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastFiscalPeriod);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastFiscalYear(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastFiscalYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the last month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastMonth(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastMonth);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastWeek(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastWeek);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXDays(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastXDays, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal periods to evaluate</param>
        public Link<T, U> WhereLastXFiscalPeriods(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastXFiscalPeriods, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> fiscal years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXFiscalYears(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastXFiscalYears, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXHours(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastXHours, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXMonths(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastXMonths, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXWeeks(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastXWeeks, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastXYears(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastXYears, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the last year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLastYear(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LastYear);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is less or equal to <paramref name="value"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLessEqual(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LessEqual, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is less than <paramref name="value"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The value</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLessThan(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.LessThan, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value matches the specifed <paramref name="pattern"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereLike(string column, string pattern)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Like, pattern);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereMask(string column, object bitmask)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Mask, bitmask);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on next 7 days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNext7Days(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Next7Days);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextFiscalPeriod(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextFiscalPeriod);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on next fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextFiscalYear(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextFiscalYear);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on next month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextMonth(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextMonth);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on next week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextWeek(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextWeek);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXDays(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextXDays, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal periods</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal perdiods to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXFiscalPeriods(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextXFiscalPeriods, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> fiscal years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of fiscal years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXFiscalYears(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextXFiscalYears, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXHours(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextXHours, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXMonths(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextXMonths, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXWeeks(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextXWeeks, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the next <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextXYears(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextXYears, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the next year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNextYear(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NextYear);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is not between <paramref name="value1"/> and <paramref name="value2"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotBetween(string column, object value1, object value2)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotBetween, value1, value2);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value does not equal <paramref name="value"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotEqual(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user business unit</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotEqualBusinessId(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotEqualBusinessId);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> does not equal current user unique identifier</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotEqualUserId(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotEqualUserId);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotIn(string column, IList values)
        {
            InnerLinkEntity.LinkCriteria.Conditions.Add(new ConditionExpression(column, ConditionOperator.NotIn, values));

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is not in the specified <paramref name="values"/></summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="values">The values</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotIn(string column, params object[] values)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotIn, values);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not found in the specifed <paramref name="bitmask"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="bitmask">The bitmask</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotMask(string column, object bitmask)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotMask, bitmask);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not null</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotNull(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotNull);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is not on the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotOn(string column, DateTime datetime)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotOn, datetime);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is above <paramref name="value"/> in the hierarchy </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereNotUnder(string column, object value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.NotUnder, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> does not contain data</summary>
        /// <param name="column">Logical name of the column</param>
        public Link<T, U> WhereNull(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Null);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> days</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of days to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXDays(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.OlderThanXDays, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> hours</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of hours to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXHours(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.OlderThanXHours, x);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> minutes</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of minutes to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXMinutes(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.OlderThanXMinutes, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> months</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of months to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXMonths(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.OlderThanXMonths, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> weeks</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of weeks to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXWeeks(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.OlderThanXWeeks, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is older than <paramref name="x"/> years</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="x">Number of years to evaluate</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOlderThanXYears(string column, int x)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.OlderThanXYears, x);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is on the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOn(string column, DateTime datetime)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.On, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or after the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOnOrAfter(string column, DateTime datetime)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.OnOrAfter, datetime);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is on or before the specifed <paramref name="datetime"/> </summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="datetime">The date</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereOnOrBefore(string column, DateTime datetime)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.OnOrBefore, datetime);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal period</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisFiscalPeriod(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.ThisFiscalPeriod);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the current fiscal year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisFiscalYear(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.ThisFiscalYear);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the current month</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisMonth(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.ThisMonth);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the current week</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisWeek(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.ThisWeek);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the current year</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereThisYear(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.ThisYear);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> value is in the current day</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereToday(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Today);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is tomorrow</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereTomorrow(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Tomorrow);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> is under <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereUnder(string column, Guid value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Under, value);

            return this;
        }
        /// <summary>Adds a condition where <paramref name="column"/> is under or equals <paramref name="value"/> in the hierarchy</summary>
        /// <param name="column">Logical name of the column</param>
        /// <param name="value">The values.</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereUnderOrEqual(string column, Guid value)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.UnderOrEqual, value);

            return this;
        }

        /// <summary>Adds a condition where <paramref name="column"/> value is yesterday</summary>
        /// <param name="column">Logical name of the column</param>
        /// <returns>The <see cref="Link{T,U}"/></returns>
        public Link<T, U> WhereYesterday(string column)
        {
            InnerLinkEntity.LinkCriteria.AddCondition(column, ConditionOperator.Yesterday);

            return this;
        }

        #endregion Conditions
    }
}