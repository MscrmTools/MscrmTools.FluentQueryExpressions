using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MscrmTools.FluentQueryExpressions;
using MscrmTools.FluentQueryExpressions.Helpers;
using System;
using System.Linq.Expressions;

namespace MscrmTools.Shared.AppCode
{
    /// <summary>
    /// A class to start a column comparison in Dataverse
    /// </summary>
    /// <typeparam name="T"><see cref="Query"/>, <see cref="Link"/> or <see cref="Filter"/></typeparam>
    public class Comparer<T> : Comparer<T, Entity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="column">First column to compare.</param>
        public Comparer(T parent, string column) : base(parent, column)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="tableAlias">Alias of the related table.</param>
        /// <param name="column">First column to compare.</param>
        public Comparer(T parent, string tableAlias, string column) : base(parent, tableAlias, column)
        {
        }
    }

    /// <summary>
    /// A class to start a column comparison in Dataverse
    /// </summary>
    /// <typeparam name="T">Query, Link or Filter</typeparam>
    /// <typeparam name="U">Strongly typed Dataverse table class</typeparam>
    public class Comparer<T, U> where U : Entity
    {
        /// <summary>
        /// The parent (<see cref="Query"/>, <see cref="Link"/> or <see cref="Filter"/>)
        /// </summary>
        private readonly T _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer{T, U}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="column">First column to compare.</param>
        public Comparer(T parent, string column)
        {
            _parent = parent;

            Column = column;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer{T, U}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="tableAlias">Alias of the related table.</param>
        /// <param name="column">First column to compare.</param>
        public Comparer(T parent, string tableAlias, string column)
        {
            _parent = parent;

            Column = column;
            TableAlias = tableAlias;
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        internal string Column { get; set; }

        /// <summary>
        /// Gets or sets the table alias.
        /// </summary>
        /// <value>
        /// The table alias.
        /// </value>
        internal string TableAlias { get; set; }

#if CRMV9

        /// <summary>
        /// First column value is equal to <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T Equal(string column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.Equal, true, column);
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is equal to <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T Equal(Expression<Func<U, object>> column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.Equal, true, AnonymousTypeHelper.GetAttributeName(column));
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is not equal to <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T NotEqual(string column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.NotEqual, true, column);
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is not equal to <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T NotEqual(Expression<Func<U, object>> column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.NotEqual, true, AnonymousTypeHelper.GetAttributeName(column));
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is greater than <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T GreaterThan(string column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.GreaterThan, true, column);
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is greater than <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T GreaterThan(Expression<Func<U, object>> column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.GreaterThan, true, AnonymousTypeHelper.GetAttributeName(column));
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is greater or equal to <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T GreaterOrEqualThan(string column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.GreaterEqual, true, column);
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is greater or equal to <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T GreaterOrEqualThan(Expression<Func<U, object>> column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.GreaterEqual, true, AnonymousTypeHelper.GetAttributeName(column));
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is less than <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T LessThan(string column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.LessThan, true, column);
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is less than <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T LessThan(Expression<Func<U, object>> column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.LessThan, true, AnonymousTypeHelper.GetAttributeName(column));
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is less or equal to <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T LessOrEqualThan(string column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.LessEqual, true, column);
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// First column value is less or equal to <paramref name="column"/> value
        /// </summary>
        /// <param name="column">Second column to compare</param>
        /// <returns><typeparamref name="T"/> instance</returns>
        public T LessOrEqualThan(Expression<Func<U, object>> column)
        {
            var ce = new ConditionExpression(Column, ConditionOperator.LessEqual, true, AnonymousTypeHelper.GetAttributeName(column));
            if (!string.IsNullOrEmpty(TableAlias)) ce.EntityName = TableAlias;

            return AddCondition(ce);
        }

        /// <summary>
        /// Add the condition in the parent object
        /// </summary>
        /// <param name="ce">the <see cref="ConditionExpression"/></param>
        /// <returns><typeparamref name="T"/> instance</returns>
        private T AddCondition(ConditionExpression ce)
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Query<>) || typeof(T) == typeof(Query))
            {
                ((dynamic)_parent).QueryExpression.Criteria.AddCondition(ce);
            }
            else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Filter<>) || typeof(T) == typeof(Filter))
            {
                ((dynamic)_parent).InnerFilter.AddCondition(ce);
            }
            else
            {
                ((dynamic)_parent).InnerLinkEntity.LinkCriteria.AddCondition(ce);
            }

            return _parent;
        }

#endif
    }
}