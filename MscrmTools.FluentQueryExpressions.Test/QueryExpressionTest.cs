using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MscrmTools.FluentQueryExpressions.Test.AppCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MscrmTools.FluentQueryExpressions.Test
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class QueryExpressionTest

    {
        #region Query

        private Guid item1Id = Guid.NewGuid();

        private Guid item2Id = Guid.NewGuid();

        [TestMethod]
        public void ShouldAddAllAttributes()
        {
            var query = new Query<Account>().Select(true);

            Assert.AreEqual(true, query.QueryExpression.ColumnSet.AllColumns);
        }

        [TestMethod]
        public void ShouldAddAttributesEarlyBound()
        {
            var query = new Query<Account>().Select(a => new { a.Name, a.AccountNumber });

            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.Name));
            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.AccountNumber));
        }

        [TestMethod]
        public void ShouldAddAttributesLateBound()
        {
            var query = new Query(Account.EntityLogicalName).Select(Account.Fields.Name, Account.Fields.AccountNumber);

            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.Name));
            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.AccountNumber));
        }

        [TestMethod]
        public void ShouldAddEarlyBoundFilter()
        {
            var query = new Query<Account>().AddFilter(new Filter<Account>(LogicalOperator.Or));

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.And);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters.First().FilterOperator, LogicalOperator.Or);
        }

        [TestMethod]
        public void ShouldAddEarlyBoundFilters()
        {
            var query = new Query<Account>().AddFilters(new Filter<Account>(LogicalOperator.Or), new Filter<Account>(LogicalOperator.And));

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.And);
            Assert.AreEqual(2, query.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters[0].FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters[1].FilterOperator, LogicalOperator.And);
        }

        [TestMethod]
        public void ShouldAddEarlyBoundFiltersWithAnonymous()
        {
            var query = new Query<Account>().AddFilters(f1 => f1, f2 => f2);

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.And);
            Assert.AreEqual(2, query.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters[0].FilterOperator, LogicalOperator.And);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters[1].FilterOperator, LogicalOperator.And);
        }

        [TestMethod]
        public void ShouldAddEarlyBoundFilterWithAnonymous()
        {
            var query = new Query<Account>().AddFilter(f => f);

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.And);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters.First().FilterOperator, LogicalOperator.And);
        }

        [TestMethod]
        public void ShouldAddLateBoundFilter()
        {
            var query = new Query(Account.EntityLogicalName).AddFilter(new Filter(LogicalOperator.Or));

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.And);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters.First().FilterOperator, LogicalOperator.Or);
        }

        [TestMethod]
        public void ShouldAddLateBoundFilters()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter(LogicalOperator.Or), new Filter(LogicalOperator.And));

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.And);
            Assert.AreEqual(2, query.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters[0].FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters[1].FilterOperator, LogicalOperator.And);
        }

        [TestMethod]
        public void ShouldAddLinkEarlyBound()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(
                    a => a.AccountId,
                    c => c.ParentCustomerId,
                    l => l.Select(c => new { c.FirstName, c.LastName }),
                    JoinOperator.LeftOuter
                );

            Assert.AreEqual(1, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities.First().LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, query.QueryExpression.LinkEntities.First().JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkEarlyBoundWithLink()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Account, Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                );

            Assert.AreEqual(1, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities.First().LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().EntityAlias);
            Assert.AreEqual(JoinOperator.Inner, query.QueryExpression.LinkEntities.First().JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkEarlyBoundWithOuterLink()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Account, Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                );

            Assert.AreEqual(1, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities.First().LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, query.QueryExpression.LinkEntities.First().JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkEarlyBoundWithoutReturnedLink()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(
                    a => a.AccountId,
                    c => c.ParentCustomerId,
                    JoinOperator.LeftOuter
                );

            Assert.AreEqual(1, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities.First().LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, query.QueryExpression.LinkEntities.First().JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkLateBoundWithLink()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(
                    new Link(Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                );

            Assert.AreEqual(1, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities.First().LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().EntityAlias);
            Assert.AreEqual(JoinOperator.Inner, query.QueryExpression.LinkEntities.First().JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkLateBoundWithOuterLink()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(
                    new Link(Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                );

            Assert.AreEqual(1, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities.First().LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, query.QueryExpression.LinkEntities.First().JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkLateBoundWithParameters()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(Account.Fields.AccountId, Contact.EntityLogicalName, Contact.Fields.ParentCustomerId);

            Assert.AreEqual(1, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities.First().LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().EntityAlias);
            Assert.AreEqual(JoinOperator.Inner, query.QueryExpression.LinkEntities.First().JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkLateBoundWithParametersOuter()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(Account.Fields.AccountId, Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, JoinOperator.LeftOuter);

            Assert.AreEqual(1, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities.First().LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities.First().LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities.First().EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, query.QueryExpression.LinkEntities.First().JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinks()
        {
            var query = new Query<Account>()
                 .AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId,
                     l => l.Select(c => new { c.FirstName, c.LastName }),
                     JoinOperator.LeftOuter
                 )
                 .AddLink<Task>(a => a.AccountId, t => t.RegardingObjectId,
                     l => l.Select(t => new { t.Subject }),
                     JoinOperator.LeftOuter
                 );

            Assert.AreEqual(2, query.QueryExpression.LinkEntities.Count);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities[0].LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, query.QueryExpression.LinkEntities[0].LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities[0].LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities[0].LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, query.QueryExpression.LinkEntities[0].EntityAlias);
            Assert.AreEqual(Task.EntityLogicalName, query.QueryExpression.LinkEntities[1].LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, query.QueryExpression.LinkEntities[1].LinkToAttributeName);
            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities[1].LinkFromAttributeName);
            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.LinkEntities[1].LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, query.QueryExpression.LinkEntities[1].EntityAlias);
        }

        [TestMethod]
        public void ShouldAddNoAttribute()
        {
            var query = new Query<Account>().Select(new string[0]);

            Assert.AreEqual(false, query.QueryExpression.ColumnSet.AllColumns);
            Assert.AreEqual(0, query.QueryExpression.ColumnSet.Columns.Count);
        }

        [TestMethod]
        public void ShouldAddNoAttributeEarlyBound()
        {
            var query = new Query<Account>().Select();

            Assert.AreEqual(false, query.QueryExpression.ColumnSet.AllColumns);
        }

        [TestMethod]
        public void ShouldAddNoAttributeLateBound()
        {
            var query = new Query(Account.EntityLogicalName).Select();

            Assert.AreEqual(false, query.QueryExpression.ColumnSet.AllColumns);
        }

        [TestMethod]
        public void ShouldAddOneAttributeEarlyBound()
        {
            var query = new Query<Account>().Select(a => a.Name);
            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.Name));

            var query2 = new Query<Account>().Select(a => a.AccountId);
            Assert.IsTrue(query2.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.AccountId));
        }

        [TestMethod]
        public void ShouldAddOneAttributeLateBound()
        {
            var query = new Query(Account.EntityLogicalName).Select(Account.Fields.Name);

            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.Name));
        }

        [TestMethod]
        public void ShouldAddOrderEarlyBound()
        {
            var query = new Query<Account>().OrderByDescending(a => a.Name);

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Orders.First().AttributeName);
            Assert.AreEqual(OrderType.Descending, query.QueryExpression.Orders.First().OrderType);

            var query2 = new Query<Account>().OrderBy(a => a.Name);

            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Orders.First().AttributeName);
            Assert.AreEqual(OrderType.Ascending, query2.QueryExpression.Orders.First().OrderType);
        }

        [TestMethod]
        public void ShouldAddOrderLateBound()
        {
            var query = new Query(Account.EntityLogicalName).OrderBy(Account.Fields.Name);

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Orders.First().AttributeName);
            Assert.AreEqual(OrderType.Ascending, query.QueryExpression.Orders.First().OrderType);

            var query2 = new Query(Account.EntityLogicalName).OrderByDescending(Account.Fields.Name);

            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Orders.First().AttributeName);
            Assert.AreEqual(OrderType.Descending, query2.QueryExpression.Orders.First().OrderType);
        }

        [TestMethod]
        public void ShouldAddPaging()
        {
            var query = new Query<Account>()
                .SetPagingInfo(1, 100, true);

            Assert.AreEqual(1, query.QueryExpression.PageInfo.PageNumber);
            Assert.AreEqual(100, query.QueryExpression.PageInfo.Count);
            Assert.AreEqual(true, query.QueryExpression.PageInfo.ReturnTotalRecordCount);
        }

        [TestMethod]
        public void ShouldBeAccountQueryExpression()
        {
            var query = new Query<Account>();

            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.EntityName);
        }

        [TestMethod]
        public void ShouldBeDistinct()
        {
            var query = new Query<Account>()
                .Distinct();

            Assert.AreEqual(true, query.QueryExpression.Distinct);
        }

        [TestMethod]
        public void ShouldCompareWhereEqualEarlyBound()
        {
            var query = new Query<Account>().Compare(a => a.NumberOfEmployees).Equal(a => a.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query<Account>().Compare(Account.EntityLogicalName, a => a.NumberOfEmployees).Equal(a => a.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereEqualLateBound()
        {
            var query = new Query(Account.EntityLogicalName).Compare(Account.Fields.NumberOfEmployees).Equal(Account.Fields.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName).Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).Equal(Account.Fields.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterOrEqualThanEarlyBound()
        {
            var query = new Query<Account>().Compare(a => a.NumberOfEmployees).GreaterOrEqualThan(a => a.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query<Account>().Compare(Account.EntityLogicalName, a => a.NumberOfEmployees).GreaterOrEqualThan(a => a.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterOrEqualThanLateBound()
        {
            var query = new Query(Account.EntityLogicalName).Compare(Account.Fields.NumberOfEmployees).GreaterOrEqualThan(Account.Fields.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).GreaterOrEqualThan(Account.Fields.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterThanEarlyBound()
        {
            var query = new Query<Account>().Compare(a => a.NumberOfEmployees).GreaterThan(a => a.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query<Account>().Compare(Account.EntityLogicalName, a => a.NumberOfEmployees).GreaterThan(a => a.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterThanLateBound()
        {
            var query = new Query(Account.EntityLogicalName).Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).GreaterThan(Account.Fields.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).GreaterThan(Account.Fields.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessOrEqualThanEarlyBound()
        {
            var query = new Query<Account>()
                .Compare(a => a.NumberOfEmployees).LessOrEqualThan(a => a.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, a => a.NumberOfEmployees).LessOrEqualThan(a => a.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessOrEqualThanLateBound()
        {
            var query = new Query(Account.EntityLogicalName).Compare(Account.Fields.NumberOfEmployees).LessOrEqualThan(Account.Fields.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName).Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).LessOrEqualThan(Account.Fields.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessThanEarlyBound()
        {
            var query = new Query<Account>()
                .Compare(a => a.NumberOfEmployees).LessThan(a => a.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, a => a.NumberOfEmployees).LessThan(a => a.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessThanLateBound()
        {
            var query = new Query(Account.EntityLogicalName)
                .Compare(Account.Fields.NumberOfEmployees).LessThan(Account.Fields.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).LessThan(Account.Fields.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereNotEqualEarlyBound()
        {
            var query = new Query<Account>()
                .Compare(a => a.NumberOfEmployees).NotEqual(a => a.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, a => a.NumberOfEmployees).NotEqual(a => a.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereNotEqualLateBound()
        {
            var query = new Query(Account.EntityLogicalName)
                .Compare(Account.Fields.NumberOfEmployees).NotEqual(Account.Fields.Revenue);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).NotEqual(Account.Fields.Revenue);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCreateLateBound()
        {
            var query = new Query(Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query.QueryExpression.EntityName);
        }

        [TestMethod]
        public void ShouldGetAll()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var records = new Query<Account>()
                    .GetAll(service);

                Assert.AreEqual(1, records.Count);
                Assert.AreEqual(item1Id, records.First().Id);
            }
        }

        [TestMethod]
        public void ShouldGetAllTopCount()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var records = new Query<Account>()
                    .Top(100)
                    .GetAll(service);

                Assert.AreEqual(1, records.Count);
                Assert.AreEqual(item1Id, records.First().Id);
            }
        }

        [TestMethod]
        public void ShouldGetAllWithExtension()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var records = service.RetrieveMultiple(new Query<Account>());

                Assert.AreEqual(1, records.Count);
                Assert.AreEqual(item1Id, records.First().Id);
            }
        }

        [TestMethod]
        public void ShouldGetAllWithoutPaging()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var records = new Query<Account>()
                    .GetAll(service);

                Assert.AreEqual(1, records.Count);
                Assert.AreEqual(item1Id, records.First().Id);
            }
        }

        [TestMethod]
        public void ShouldGetById()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var query = new Query<Account>();
                var record = query.GetById(Guid.NewGuid(), service);

                Assert.IsNotNull(record);
                Assert.AreEqual(Account.EntityLogicalName + "id", query.QueryExpression.Criteria.Conditions.First().AttributeName);
            }
        }

        [TestMethod]
        public void ShouldGetByIdForActivity()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var query = new Query<Task>();
                var record = query.GetById(Guid.NewGuid(), service, true);

                Assert.IsNotNull(record);
                Assert.AreEqual("activityid", query.QueryExpression.Criteria.Conditions.First().AttributeName);
            }
        }

        [TestMethod]
        public void ShouldGetByIdForActivityLateBound()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var query = new Query(Task.EntityLogicalName);
                var record = query.GetById(Guid.NewGuid(), service, true);

                Assert.IsNotNull(record);
                Assert.AreEqual("activityid", query.QueryExpression.Criteria.Conditions.First().AttributeName);
            }
        }

        [TestMethod]
        public void ShouldGetByIdLateBound()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var query = new Query(Contact.EntityLogicalName);
                var record = query.GetById(Guid.NewGuid(), service, false);

                Assert.IsNotNull(record);
                Assert.AreEqual(Contact.PrimaryIdAttribute, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            }
        }

        [TestMethod]
        public void ShouldGetFirst()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item2Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetFirst(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item1Id, record.Id);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldGetFirstNull()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName
                            };
                        }

                        return new EntityCollection();
                    }
                };

                new Query<Account>()
                    .GetFirst(service);
            }
        }

        [TestMethod]
        public void ShouldGetFirstOrDefault()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id =item2Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetFirstOrDefault(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item1Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldGetFirstOrDefaultIsNull()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetFirstOrDefault(service);

                Assert.IsNull(record);
            }
        }

        [TestMethod]
        public void ShouldGetFirstOrDefaultWithTop()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id =item2Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .Top(10)
                    .GetFirstOrDefault(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item1Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldGetFirstWithTop()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item2Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .Top(10)
                    .GetFirst(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item1Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldGetLast()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    // Data is inverted because GetLast change sort order
                                    new Entity(qe.EntityName)
                                    {
                                        Id =item2Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .OrderBy(a => a.CreatedOn)
                    .GetLast(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item2Id, record.Id);

                record = new Query<Account>()
                    .OrderByDescending(a => a.CreatedOn)
                    .GetLast(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item2Id, record.Id);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldGetLastNull()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName
                            };
                        }

                        return new EntityCollection();
                    }
                };

                new Query<Account>()
                    .OrderBy(a => a.CreatedOn)
                    .GetLast(service);
            }
        }

        [TestMethod]
        public void ShouldGetLastOrDefault()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                      new Entity(qe.EntityName)
                                    {
                                        Id =item2Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .OrderBy(a => a.CreatedOn)
                    .GetLastOrDefault(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item2Id, record.Id);

                record = new Query<Account>()
                   .OrderByDescending(a => a.CreatedOn)
                   .GetLastOrDefault(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item2Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldGetLastOrDefaultIsNull()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetLastOrDefault(service);

                Assert.IsNull(record);
            }
        }

        [TestMethod]
        public void ShouldGetLastOrDefaultWithTop()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id =item2Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .Top(10)
                    .GetLastOrDefault(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item2Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldGetLastWithTop()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id =item2Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .Top(10)
                    .GetLast(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item2Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldGetResults()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item2Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var records = new Query<Account>()
                    .GetResults(service);

                Assert.AreEqual(2, records.Entities.Count);
            }
        }

        [TestMethod]
        public void ShouldGetSingle()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetSingle(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item1Id, record.Id);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldGetSingleMany()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id =item2Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                new Query<Account>().GetSingle(service);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldGetSingleNull()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                            };
                        }

                        return new EntityCollection();
                    }
                };

                new Query<Account>()
                    .GetSingle(service);
            }
        }

        [TestMethod]
        public void ShouldGetSingleOrDefault()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetSingleOrDefault(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item1Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldGetSingleOrDefaultIsNull()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetSingleOrDefault(service);

                Assert.IsNull(record);
            }
        }

        [TestMethod]
        public void ShouldGetSingleOrDefaultIsNullWithTop()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .Top(10)
                    .GetSingleOrDefault(service);

                Assert.IsNull(record);
            }
        }

        [TestMethod]
        public void ShouldGetSingleWithTop()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .Top(10)
                    .GetSingle(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item1Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldHaveNoLock()
        {
            var query = new Query<Account>()
                .NoLock();

            Assert.AreEqual(true, query.QueryExpression.NoLock);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldNotGetLastOrDefaultWithException()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetLast(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item2Id, record.Id);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldNotGetLastWithException()
        {
            using (ShimsContext.Create())
            {
                var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService
                {
                    RetrieveMultipleQueryBase = queryBase =>
                    {
                        if (queryBase is QueryExpression qe)
                        {
                            return new EntityCollection
                            {
                                EntityName = qe.EntityName,
                                Entities =
                                {
                                    // Data is inverted because GetLast change sort order
                                    new Entity(qe.EntityName)
                                    {
                                        Id =item2Id
                                    },
                                    new Entity(qe.EntityName)
                                    {
                                        Id = item1Id
                                    }
                                }
                            };
                        }

                        return new EntityCollection();
                    }
                };

                var record = new Query<Account>()
                    .GetLast(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(item2Id, record.Id);
            }
        }

        [TestMethod]
        public void ShouldSetLogicalOperatorOr()
        {
            var query = new Query<Account>().SetLogicalOperator(LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.Or);

            var query2 = new Query(Account.EntityLogicalName).SetLogicalOperator(LogicalOperator.Or);
            Assert.AreEqual(query2.QueryExpression.Criteria.FilterOperator, LogicalOperator.Or);
        }

        [TestMethod]
        public void ShouldSetNextPage()
        {
            var query = new Query<Account>()
                .SetPagingInfo(1, 100, true)
                .NextPage("<fakePagingCookie>");

            Assert.AreEqual(2, query.QueryExpression.PageInfo.PageNumber);
            Assert.AreEqual(100, query.QueryExpression.PageInfo.Count);
            Assert.AreEqual(true, query.QueryExpression.PageInfo.ReturnTotalRecordCount);
            Assert.AreEqual("<fakePagingCookie>", query.QueryExpression.PageInfo.PagingCookie);
        }

        [TestMethod]
        public void ShouldSetTop()
        {
            var query = new Query<Account>().Top(100);

            Assert.AreEqual(100, query.QueryExpression.TopCount);
        }

        #endregion Query

        #region Conditions

        [TestMethod]
        public void ShouldSetWhere()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).Where(Account.Fields.AccountId, ConditionOperator.Above, guid);

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).Where(Account.EntityLogicalName, Account.Fields.AccountId, ConditionOperator.Above, guid);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().Where(a => a.AccountId, ConditionOperator.Above, guid);

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().Where<Contact>(Contact.EntityLogicalName, a => a.FirstName, ConditionOperator.Equal, "Tanguy");

            Assert.AreEqual(Contact.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Contact.Fields.FirstName, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("Tanguy", query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereAbove()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).WhereAbove(Account.Fields.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereAbove(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereAbove(a => a.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereAbove<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereAboveOrEqual()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).WhereAboveOrEqual(Account.Fields.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereAboveOrEqual(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereAboveOrEqual(a => a.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereAboveOrEqual<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereBeginsWith()
        {
            var query = new Query(Account.EntityLogicalName).WhereBeginsWith(Account.Fields.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereBeginsWith(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereBeginsWith(a => a.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereBeginsWith<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereBetween()
        {
            var query = new Query(Account.EntityLogicalName).WhereBetween(Account.Fields.NumberOfEmployees, 10, 50);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).WhereBetween(Account.Fields.NumberOfEmployees, 10, 50, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query2.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().WhereBetween(a => a.NumberOfEmployees, 10, 50);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query3.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query4 = new Query<Account>().WhereBetween<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10, 50);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query4.QueryExpression.Criteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereContains()
        {
            var query = new Query(Account.EntityLogicalName).WhereContains(Account.Fields.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereContains(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereContains(a => a.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereContains<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereContainValues()
        {
            var query = new Query(Account.EntityLogicalName).WhereContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query2 = new Query(Account.EntityLogicalName).WhereContainValues(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().WhereContainValues(a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().WhereContainValues<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotBeginWith()
        {
            var query = new Query(Account.EntityLogicalName).WhereDoesNotBeginWith(Account.Fields.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereDoesNotBeginWith(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereDoesNotBeginWith(a => a.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereDoesNotBeginWith<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContain()
        {
            var query = new Query(Account.EntityLogicalName).WhereDoesNotContain(Account.Fields.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereDoesNotContain(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereDoesNotContain(a => a.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereDoesNotContain<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContainValues()
        {
            var query = new Query(Account.EntityLogicalName).WhereDoesNotContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query2 = new Query(Account.EntityLogicalName).WhereDoesNotContainValues(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().WhereDoesNotContainValues(a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().WhereDoesNotContainValues<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotEndWith()
        {
            var query = new Query(Account.EntityLogicalName).WhereDoesNotEndWith(Account.Fields.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereDoesNotEndWith(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereDoesNotEndWith(a => a.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereDoesNotEndWith<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEndsWith()
        {
            var query = new Query(Account.EntityLogicalName).WhereEndsWith(Account.Fields.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereEndsWith(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereEndsWith(a => a.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereEndsWith<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEqual()
        {
            var query = new Query(Account.EntityLogicalName).WhereEqual(Account.Fields.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).WhereEqual(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().WhereEqual(a => a.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereEqual<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEqualBusinessId()
        {
            var query = new Query(Account.EntityLogicalName).WhereEqualBusinessId(Account.Fields.OwningBusinessUnit);

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereEqualBusinessId(Account.Fields.OwningBusinessUnit, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereEqualBusinessId(a => a.OwningBusinessUnit);

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereEqualBusinessId<Account>(Account.EntityLogicalName, a => a.OwningBusinessUnit);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserId()
        {
            var query = new Query(Account.EntityLogicalName).WhereEqualUserId(Account.Fields.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereEqualUserId(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereEqualUserId(a => a.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereEqualUserId<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserLanguage()
        {
            var query = new Query(Account.EntityLogicalName).WhereEqualUserLanguage("no_language_attribute");

            Assert.AreEqual("no_language_attribute", query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereEqualUserLanguage("no_language_attribute", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual("no_language_attribute", query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereEqualUserLanguage(a => a.NumberOfEmployees);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereEqualUserLanguage<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchy()
        {
            var query = new Query(Account.EntityLogicalName).WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereEqualUserOrUserHierarchy(a => a.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereEqualUserOrUserHierarchy<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchyAndTeams()
        {
            var query = new Query(Account.EntityLogicalName).WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereEqualUserOrUserHierarchyAndTeams(a => a.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereEqualUserOrUserHierarchyAndTeams<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserTeams()
        {
            var query = new Query(Account.EntityLogicalName).WhereEqualUserOrUserTeams(Account.Fields.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereEqualUserOrUserTeams(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereEqualUserOrUserTeams(a => a.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereEqualUserOrUserTeams<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserTeams()
        {
            var query = new Query(Account.EntityLogicalName).WhereEqualUserTeams(Account.Fields.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereEqualUserTeams(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereEqualUserTeams(a => a.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereEqualUserTeams<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereGreaterEqual()
        {
            var query = new Query(Account.EntityLogicalName).WhereGreaterEqual(Account.Fields.NumberOfEmployees, 10);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereGreaterEqual(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereGreaterEqual(a => a.NumberOfEmployees, 10);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereGreaterEqual<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereGreaterThan()
        {
            var query = new Query(Account.EntityLogicalName).WhereGreaterThan(Account.Fields.NumberOfEmployees, 10);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereGreaterThan(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereGreaterThan(a => a.NumberOfEmployees, 10);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.Single());

            var query4 = new Query<Account>().WhereGreaterThan<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereIn()
        {
            var query = new Query(Account.EntityLogicalName).WhereIn(Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query2 = new Query(Account.EntityLogicalName).WhereIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().WhereIn(a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().WhereIn(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).WhereInFiscalPeriod(Account.Fields.CreatedOn, 1);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereInFiscalPeriod(Account.Fields.CreatedOn, 1, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereInFiscalPeriod(a => a.CreatedOn, 1);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereInFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).WhereInFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).WhereInFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query2.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().WhereInFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query4 = new Query<Account>().WhereInFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query4.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query4.QueryExpression.Criteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).WhereInFiscalYear(Account.Fields.CreatedOn, 2018);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(2018, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereInFiscalYear(Account.Fields.CreatedOn, 2018, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(2018, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereInFiscalYear(a => a.CreatedOn, 2018);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(2018, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereInFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 2018);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(2018, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereInList()
        {
            var query = new Query(Account.EntityLogicalName).WhereIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query2 = new Query(Account.EntityLogicalName).WhereIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query3 = new Query<Account>().WhereIn(a => a.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query4 = new Query<Account>().WhereIn<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereInOrAfterFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).WhereInOrAfterFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).WhereInOrAfterFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query2.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().WhereInOrAfterFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query4 = new Query<Account>().WhereInOrAfterFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query4.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query4.QueryExpression.Criteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereInOrBeforeFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).WhereInOrBeforeFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).WhereInOrBeforeFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query2.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().WhereInOrBeforeFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query4 = new Query<Account>().WhereInOrBeforeFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(1, query4.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query4.QueryExpression.Criteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereLast7Days()
        {
            var query = new Query(Account.EntityLogicalName).WhereLast7Days(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereLast7Days(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereLast7Days(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereLast7Days<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastFiscalPeriod(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereLastFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereLastFiscalPeriod(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereLastFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastFiscalYear(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereLastFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereLastFiscalYear(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereLastFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastMonth()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastMonth(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereLastMonth(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereLastMonth(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereLastMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastWeek()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastWeek(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereLastWeek(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereLastWeek(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereLastWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastXDays()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastXDays(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLastXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLastXDays(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLastXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalPeriods()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastXFiscalPeriods(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLastXFiscalPeriods(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLastXFiscalPeriods(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLastXFiscalPeriods<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalYears()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastXFiscalYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLastXFiscalYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLastXFiscalYears(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLastXFiscalYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXHours()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastXHours(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLastXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLastXHours(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLastXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXMonths()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastXMonths(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLastXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLastXMonths(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLastXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXWeeks()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastXWeeks(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLastXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLastXWeeks(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLastXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXYears()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastXYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLastXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLastXYears(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLastXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastYear()
        {
            var query = new Query(Account.EntityLogicalName).WhereLastYear(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereLastYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereLastYear(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereLastYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLessEqual()
        {
            var query = new Query(Account.EntityLogicalName).WhereLessEqual(Account.Fields.NumberOfEmployees, 10);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLessEqual(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLessEqual(a => a.NumberOfEmployees, 10);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLessEqual<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLessThan()
        {
            var query = new Query(Account.EntityLogicalName).WhereLessThan(Account.Fields.NumberOfEmployees, 10);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereLessThan(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLessThan(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLessThan<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLike()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereLike(Account.Fields.Name, "%test%");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereLike(Account.Fields.Name, "%test%", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereLike(a => a.Name, "%test%");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereLike<Account>(Account.EntityLogicalName, a => a.Name, "%test%");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereMask()
        {
            var obj = new object();
            var query = new Query(Account.EntityLogicalName)
                .WhereMask(Account.Fields.Name, obj);

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereMask(Account.Fields.Name, obj, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereMask(a => a.Name, obj);

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereMask<Account>(Account.EntityLogicalName, a => a.Name, obj);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNext7Days()
        {
            var query = new Query(Account.EntityLogicalName).WhereNext7Days(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereNext7Days(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNext7Days(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNext7Days<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).WhereNextFiscalPeriod(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereNextFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNextFiscalPeriod(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNextFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).WhereNextFiscalYear(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereNextFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNextFiscalYear(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNextFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextMonth()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNextMonth(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNextMonth(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNextMonth(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNextMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextWeek()
        {
            var query = new Query(Account.EntityLogicalName).WhereNextWeek(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).WhereNextWeek(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNextWeek(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNextWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextXDays()
        {
            var query = new Query(Account.EntityLogicalName).WhereNextXDays(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereNextXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNextXDays(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNextXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalPeriods()
        {
            var query = new Query(Account.EntityLogicalName).WhereNextXFiscalPeriods(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).WhereNextXFiscalPeriods(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNextXFiscalPeriods(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNextXFiscalPeriods<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalYears()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNextXFiscalYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNextXFiscalYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNextXFiscalYears(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNextXFiscalYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXHours()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNextXHours(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNextXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNextXHours(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNextXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXMonths()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNextXMonths(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNextXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNextXMonths(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNextXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXWeeks()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNextXWeeks(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNextXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNextXWeeks(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNextXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXYears()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNextXYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNextXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNextXYears(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNextXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextYear()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNextYear(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNextYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNextYear(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNextYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotBetween()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNotBetween(Account.Fields.NumberOfEmployees, 10, 50);

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotBetween(Account.Fields.NumberOfEmployees, 10, 50, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query2.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().WhereNotBetween(a => a.CreatedOn, 10, 50);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query3.QueryExpression.Criteria.Conditions.First().Values[1]);

            var query4 = new Query<Account>().WhereNotBetween<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10, 50);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query4.QueryExpression.Criteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqual()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNotEqual(Account.Fields.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotEqual(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNotEqual(a => a.Name, "test");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNotEqual<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualBusinessId()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit);

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNotEqualBusinessId(a => a.OwningBusinessUnit);

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNotEqualBusinessId<Account>(Account.EntityLogicalName, a => a.OwningBusinessUnit);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualUserId()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNotEqualUserId(Account.Fields.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotEqualUserId(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNotEqualUserId(a => a.OwnerId);

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNotEqualUserId<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotIn()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNotIn(Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().WhereNotIn(a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().WhereNotIn(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereNotInList()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNotIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query3 = new Query<Account>().WhereNotIn(a => a.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query4 = new Query<Account>().WhereNotIn(Account.EntityLogicalName, a => a.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereNotLike()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNotLike(Account.Fields.Name, "%test%");

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotLike(Account.Fields.Name, "%test%", Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNotLike(a => a.Name, "%test%");

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNotLike<Account>(Account.EntityLogicalName, a => a.Name, "%test%");

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotMask()
        {
            var obj = new object();
            var query = new Query(Account.EntityLogicalName)
                .WhereNotMask(Account.Fields.Name, obj);

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                 .WhereNotMask(Account.Fields.Name, obj, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNotMask(a => a.Name, obj);

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNotMask<Account>(Account.EntityLogicalName, a => a.Name, obj);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotNull()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNotNull(Account.Fields.Name);

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotNull(Account.Fields.Name, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNotNull(a => a.Name);

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNotNull<Account>(Account.EntityLogicalName, a => a.Name);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotOn()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName)
                .WhereNotOn(Account.Fields.CreatedOn, date);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotOn(Account.Fields.CreatedOn, date, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNotOn(a => a.CreatedOn, date);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNotOn<Account>(Account.EntityLogicalName, a => a.CreatedOn, date);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotUnder()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName)
                .WhereNotUnder(Account.Fields.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNotUnder(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereNotUnder(a => a.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereNotUnder<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNull()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereNull(Account.Fields.Name);

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereNull(Account.Fields.Name, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereNull(a => a.Name);

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereNull<Account>(Account.EntityLogicalName, a => a.Name);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXDays()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereOlderThanXDays(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOlderThanXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOlderThanXDays(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOlderThanXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXHours()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereOlderThanXHours(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOlderThanXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOlderThanXHours(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOlderThanXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMinutes()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereOlderThanXMinutes(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOlderThanXMinutes(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOlderThanXMinutes(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOlderThanXMinutes<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMonths()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereOlderThanXMonths(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOlderThanXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOlderThanXMonths(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOlderThanXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXWeeks()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereOlderThanXWeeks(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOlderThanXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOlderThanXWeeks(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOlderThanXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXYears()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereOlderThanXYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOlderThanXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOlderThanXYears(a => a.CreatedOn, 10);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOlderThanXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOn()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName)
                .WhereOn(Account.Fields.CreatedOn, date);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOn(Account.Fields.CreatedOn, date, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOn(a => a.CreatedOn, date);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOn<Account>(Account.EntityLogicalName, a => a.CreatedOn, date);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOnOrAfter()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName)
                .WhereOnOrAfter(Account.Fields.CreatedOn, date);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOnOrAfter(Account.Fields.CreatedOn, date, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOnOrAfter(a => a.CreatedOn, date);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOnOrAfter<Account>(Account.EntityLogicalName, a => a.CreatedOn, date);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOnOrBefore()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName)
                .WhereOnOrBefore(Account.Fields.CreatedOn, date);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereOnOrBefore(Account.Fields.CreatedOn, date, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereOnOrBefore(a => a.CreatedOn, date);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereOnOrBefore<Account>(Account.EntityLogicalName, a => a.CreatedOn, date);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(date, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereThisFiscalPeriod(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereThisFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereThisFiscalPeriod(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereThisFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereThisFiscalYear(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereThisFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereThisFiscalYear(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereThisFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisMonth()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereThisMonth(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereThisMonth(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereThisMonth(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereThisMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisWeek()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereThisWeek(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereThisWeek(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereThisWeek(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereThisWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisYear()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereThisYear(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereThisYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereThisYear(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereThisYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereToday()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereToday(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereToday(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereToday(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereToday<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereTomorrow()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereTomorrow(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereTomorrow(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereTomorrow(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereTomorrow<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereUnder()
        {
            Guid guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName)
                .WhereUnder(Account.Fields.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereUnder(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereUnder(a => a.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereUnder<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereUnderOrEqual()
        {
            Guid guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName)
                .WhereUnderOrEqual(Account.Fields.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName)
                .WhereUnderOrEqual(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().WhereUnderOrEqual(a => a.AccountId, guid);

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Conditions.First().Values.First());

            var query4 = new Query<Account>().WhereUnderOrEqual<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereYesterday()
        {
            var query = new Query(Account.EntityLogicalName)
                .WhereYesterday(Account.Fields.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName)
                .WhereYesterday(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query2.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().WhereYesterday(a => a.CreatedOn);

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query3.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Conditions.First().Values.Count);

            var query4 = new Query<Account>().WhereYesterday<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query4.QueryExpression.Criteria.Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Conditions.First().Values.Count);
        }

        #endregion Conditions

        #region Global

        [TestMethod]
        public void ShouldWork()
        {
            var fluqry = new Query("account")
                .NoLock(true)
                .Distinct(true)
                .Select(
                    "name",
                    "primarycontactid",
                    "address1_city",
                    "address1_addresstypecode",
                    "address1_shippingmethodcode",
                    "telephone1",
                    "accountid"
                )
                .WhereNotNull("emailaddress1")
                .AddFilter(new Filter(LogicalOperator.And)
                    .WhereNotNull("address1_county")
                    .WhereNotNull("address1_city")
                )
                .AddFilter(new Filter(LogicalOperator.And)
                    .WhereNull("address1_line1")
                    .WhereEqual("statecode", 0)
                    .AddFilter(new Filter(LogicalOperator.Or)
                        .WhereNull("accountid")
                        .WhereNotEqual("fullname", "fdfdsds", "par")
                        .AddFilter(new Filter(LogicalOperator.And)
                            .WhereEqual("sic", "hgfhgf")
                            .WhereNull("slaid")
                        )
                    )
                )
                .OrderBy("name")
                .OrderByDescending("address1_fax")
                .AddLink(new Link("contact", "contactid", "primarycontactid", JoinOperator.LeftOuter)
                    .SetAlias("par")
                    .Select("emailaddress1")
                    .AddFilter(new Filter(LogicalOperator.And)
                        .WhereLike("address1_country", "%ede%")
                    )
                    .AddFilter(new Filter(LogicalOperator.Or)
                        .WhereEqual("address1_addresstypecode", 3)
                    )
                )
                .AddLink(new Link("contact", "parentcustomerid", "accountid", JoinOperator.LeftOuter)
                    .Select("firstname", "lastname")
                    .AddLink(new Link("systemuser", "systemuserid", "owninguser", JoinOperator.Inner)
                        .SetAlias("R")
                        .Select("accessmode")
                        .WhereNotNull("internalemailaddress")
                        .AddFilter(new Filter(LogicalOperator.And)
                            .WhereBetween("createdon", "2021-12-31T00:00:00+01:00", "2022-12-01T00:00:00+01:00")
                            .WhereLastYear("modifiedon")
                            .AddFilter(new Filter(LogicalOperator.And)
                                .WhereBetween("createdon", "2021-12-31T00:00:00+01:00", "2022-12-01T00:00:00+01:00")
                                .WhereLastYear("modifiedon")
                            )
                            .AddFilter(new Filter(LogicalOperator.And)
                                .WhereBetween("createdon", "2021-12-31T00:00:00+01:00", "2022-12-01T00:00:00+01:00")
                                .WhereLastYear("modifiedon")
                            )
                        )
                        .OrderBy("fullname")
                    )
                );

            Assert.IsTrue(true);
        }

        #endregion Global
    }
}