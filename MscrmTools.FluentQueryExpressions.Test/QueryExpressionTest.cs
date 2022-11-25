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
        [TestMethod]
        public void ShouldAddAllAttributes()
        {
            var query = new Query<Account>()
                .Select(true);

            Assert.AreEqual(query.QueryExpression.ColumnSet.AllColumns, true);
        }

        [TestMethod]
        public void ShouldAddAttributesWithAnonymousType()
        {
            var query = new Query<Account>()
                .Select(a => new { a.Name, a.AccountNumber });

            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.Name));
            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.AccountNumber));
        }

        [TestMethod]
        public void ShouldAddFilter()
        {
            var query = new Query<Account>()
                .AddFilters(LogicalOperator.Or, new Filter(LogicalOperator.Or));

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters.Count, 1);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters.First().FilterOperator, LogicalOperator.Or);
        }

        [TestMethod]
        public void ShouldAddFilterWithoutOperator()
        {
            var query = new Query<Account>()
                .AddFilters(new Filter(LogicalOperator.Or));

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.And);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters.Count, 1);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters.First().FilterOperator, LogicalOperator.Or);
        }

        [TestMethod]
        public void ShouldAddLink()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId));

            Assert.AreEqual(query.QueryExpression.LinkEntities.Count, 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToEntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToAttributeName, Contact.Fields.ParentCustomerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromAttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromEntityName, Account.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().EntityAlias, Contact.EntityLogicalName);
        }

        [TestMethod]
        public void ShouldAddLinks()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId))
                .AddLink(new Link<Task>(Task.Fields.RegardingObjectId, Account.Fields.AccountId));

            Assert.AreEqual(query.QueryExpression.LinkEntities.Count, 2);
            Assert.AreEqual(query.QueryExpression.LinkEntities[0].LinkToEntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities[0].LinkToAttributeName, Contact.Fields.ParentCustomerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities[0].LinkFromAttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities[0].LinkFromEntityName, Account.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities[0].EntityAlias, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities[1].LinkToEntityName, Task.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities[1].LinkToAttributeName, Task.Fields.RegardingObjectId);
            Assert.AreEqual(query.QueryExpression.LinkEntities[1].LinkFromAttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities[1].LinkFromEntityName, Account.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities[1].EntityAlias, Task.EntityLogicalName);
        }

        [TestMethod]
        public void ShouldAddNoAttribute()
        {
            var query = new Query<Account>()
                .Select();

            Assert.AreEqual(query.QueryExpression.ColumnSet.AllColumns, false);
        }

        [TestMethod]
        public void ShouldAddOneAttribute()
        {
            var query = new Query<Account>()
                .Select(Account.Fields.Name);

            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.Name));
        }

        [TestMethod]
        public void ShouldAddOrder()
        {
            var query = new Query<Account>()
                .Order(Account.Fields.Name, OrderType.Ascending);

            Assert.AreEqual(query.QueryExpression.Orders.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Orders.First().OrderType, OrderType.Ascending);
        }

        [TestMethod]
        public void ShouldAddPaging()
        {
            var query = new Query<Account>()
                .SetPagingInfo(1, 100, true);

            Assert.AreEqual(query.QueryExpression.PageInfo.PageNumber, 1);
            Assert.AreEqual(query.QueryExpression.PageInfo.Count, 100);
            Assert.AreEqual(query.QueryExpression.PageInfo.ReturnTotalRecordCount, true);
        }

        [TestMethod]
        public void ShouldAddTwoAttributes()
        {
            var query = new Query<Account>()
                .Select(Account.Fields.Name)
                .Select(Account.Fields.AccountNumber);

            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.Name));
            Assert.IsTrue(query.QueryExpression.ColumnSet.Columns.Contains(Account.Fields.AccountNumber));
        }

        [TestMethod]
        public void ShouldAddTwoFilters()
        {
            var query = new Query<Account>()
                .AddFilters(LogicalOperator.Or,
                    new Filter(LogicalOperator.Or),
                    new Filter());

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters.Count, 2);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters[0].FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.Criteria.Filters[1].FilterOperator, LogicalOperator.And);
        }

        [TestMethod]
        public void ShouldBeAccountQueryExpression()
        {
            var query = new Query<Account>();

            Assert.AreEqual(query.QueryExpression.EntityName, Account.EntityLogicalName);
        }

        [TestMethod]
        public void ShouldBeDistinct()
        {
            var query = new Query<Account>()
                .Distinct();

            Assert.AreEqual(query.QueryExpression.Distinct, true);
        }

        [TestMethod]
        public void ShouldCreateLateBound()
        {
            var query = new Query(Account.EntityLogicalName);

            Assert.AreEqual(query.QueryExpression.EntityName, Account.EntityLogicalName);
        }

        [TestMethod]
        public void ShouldHaveNoLock()
        {
            var query = new Query<Account>()
                .NoLock();

            Assert.AreEqual(query.QueryExpression.NoLock, true);
        }

        [TestMethod]
        public void ShouldSetLogicalOperatorOr()
        {
            var query = new Query<Account>()
                .SetDefaultFilterOperator(LogicalOperator.Or);

            Assert.AreEqual(query.QueryExpression.Criteria.FilterOperator, LogicalOperator.Or);
        }

        [TestMethod]
        public void ShouldSetNextPage()
        {
            var query = new Query<Account>()
                .SetPagingInfo(1, 100, true)
                .NextPage("<fakePagingCookie>");

            Assert.AreEqual(query.QueryExpression.PageInfo.PageNumber, 2);
            Assert.AreEqual(query.QueryExpression.PageInfo.Count, 100);
            Assert.AreEqual(query.QueryExpression.PageInfo.ReturnTotalRecordCount, true);
            Assert.AreEqual(query.QueryExpression.PageInfo.PagingCookie, "<fakePagingCookie>");
        }

        [TestMethod]
        public void ShouldSetTop()
        {
            var query = new Query<Account>()
                .Top(100);

            Assert.AreEqual(query.QueryExpression.TopCount, 100);
        }

        #region Conditions

        [TestMethod]
        public void ShouldCompareWhereEqual()
        {
            var query = new Query<Account>()
                .Compare(Account.Fields.NumberOfEmployees).Equal(Account.Fields.Revenue);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).Equal(Account.Fields.Revenue);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterOrEqualThan()
        {
            var query = new Query<Account>()
                .Compare(Account.Fields.NumberOfEmployees).GreaterOrEqualThan(Account.Fields.Revenue);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).GreaterOrEqualThan(Account.Fields.Revenue);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterThan()
        {
            var query = new Query<Account>()
                .Compare(Account.Fields.NumberOfEmployees).GreaterThan(Account.Fields.Revenue);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).GreaterThan(Account.Fields.Revenue);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereLessOrEqualThan()
        {
            var query = new Query<Account>()
                .Compare(Account.Fields.NumberOfEmployees).LessOrEqualThan(Account.Fields.Revenue);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).LessOrEqualThan(Account.Fields.Revenue);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereLessThan()
        {
            var query = new Query<Account>()
                .Compare(Account.Fields.NumberOfEmployees).LessThan(Account.Fields.Revenue);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).LessThan(Account.Fields.Revenue);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereNotEqual()
        {
            var query = new Query<Account>()
                .Compare(Account.Fields.NumberOfEmployees).NotEqual(Account.Fields.Revenue);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).NotEqual(Account.Fields.Revenue);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), Account.Fields.Revenue);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldSetWhere()
        {
            var guid = Guid.NewGuid();
            var query = new Query<Account>().Where(Account.Fields.AccountId, ConditionOperator.Above, guid);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query2 = new Query<Account>().Where(Account.EntityLogicalName, Account.Fields.AccountId, ConditionOperator.Above, guid);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query3 = new Query<Account>().Where(a => a.AccountId, ConditionOperator.Above, guid);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query4 = new Query<Account>().Where<Contact>(Contact.EntityLogicalName, a => a.FirstName, ConditionOperator.Equal, "Tanguy");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Contact.Fields.FirstName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), "Tanguy");
        }

        [TestMethod]
        public void ShouldSetWhereAbove()
        {
            var guid = Guid.NewGuid();
            var query = new Query<Account>().WhereAbove(Account.Fields.AccountId, guid);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query2 = new Query<Account>().WhereAbove(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query3 = new Query<Account>().WhereAbove(a => a.AccountId, guid);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query4 = new Query<Account>().WhereAbove<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereAboveOrEqual()
        {
            var guid = Guid.NewGuid();
            var query = new Query<Account>().WhereAboveOrEqual(Account.Fields.AccountId, guid);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.AboveOrEqual);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query2 = new Query<Account>().WhereAboveOrEqual(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.AboveOrEqual);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query3 = new Query<Account>().WhereAboveOrEqual(a => a.AccountId, guid);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.AboveOrEqual);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query4 = new Query<Account>().WhereAboveOrEqual<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.AboveOrEqual);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereBeginsWith()
        {
            var query = new Query<Account>().WhereBeginsWith(Account.Fields.Name, "test");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.BeginsWith);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>().WhereBeginsWith(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.BeginsWith);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query3 = new Query<Account>().WhereBeginsWith(a => a.Name, "test");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.BeginsWith);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query4 = new Query<Account>().WhereBeginsWith<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.BeginsWith);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereBetween()
        {
            var query = new Query<Account>().WhereBetween(Account.Fields.NumberOfEmployees, 10, 50);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Between);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[1], 50);

            var query2 = new Query<Account>().WhereBetween(Account.Fields.NumberOfEmployees, 10, 50, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Between);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[1], 50);

            var query3 = new Query<Account>().WhereBetween(a => a.NumberOfEmployees, 10, 50);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Between);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[1], 50);

            var query4 = new Query<Account>().WhereBetween<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10, 50);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Between);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[1], 50);
        }

        [TestMethod]
        public void ShouldSetWhereChildOf()
        {
            var guid = Guid.NewGuid();
            var query = new Query<Account>().WhereChildOf(Account.Fields.AccountId, guid);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ChildOf);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query2 = new Query<Account>().WhereChildOf(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ChildOf);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query3 = new Query<Account>().WhereChildOf(a => a.AccountId, guid);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ChildOf);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);

            var query4 = new Query<Account>().WhereChildOf<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ChildOf);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereContains()
        {
            var query = new Query<Account>().WhereContains(Account.Fields.Name, "test");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Contains);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>().WhereContains(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Contains);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query3 = new Query<Account>().WhereContains(a => a.Name, "test");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Contains);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query4 = new Query<Account>().WhereContains<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Contains);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereContainValues()
        {
            var query = new Query<Account>().WhereContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ContainValues);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query2 = new Query<Account>().WhereContainValues(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ContainValues);
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().WhereContainValues(a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ContainValues);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().WhereContainValues<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ContainValues);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotBeginWith()
        {
            var query = new Query<Account>().WhereDoesNotBeginWith(Account.Fields.Name, "test");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotBeginWith);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>().WhereDoesNotBeginWith(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotBeginWith);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query3 = new Query<Account>().WhereDoesNotBeginWith(a => a.Name, "test");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotBeginWith);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query4 = new Query<Account>().WhereDoesNotBeginWith<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotBeginWith);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContain()
        {
            var query = new Query<Account>().WhereDoesNotContain(Account.Fields.Name, "test");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotContain);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>().WhereDoesNotContain(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotContain);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query3 = new Query<Account>().WhereDoesNotContain(a => a.Name, "test");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotContain);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query4 = new Query<Account>().WhereDoesNotContain<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotContain);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContainValues()
        {
            var query = new Query<Account>().WhereDoesNotContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotContainValues);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query2 = new Query<Account>().WhereDoesNotContainValues(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotContainValues);
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().WhereDoesNotContainValues(a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotContainValues);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().WhereDoesNotContainValues<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotContainValues);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotEndWith()
        {
            var query = new Query<Account>().WhereDoesNotEndWith(Account.Fields.Name, "test");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotEndWith);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>().WhereDoesNotEndWith(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotEndWith);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query3 = new Query<Account>().WhereDoesNotEndWith(a => a.Name, "test");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotEndWith);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query4 = new Query<Account>().WhereDoesNotEndWith<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.DoesNotEndWith);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereEndsWith()
        {
            var query = new Query<Account>().WhereEndsWith(Account.Fields.Name, "test");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EndsWith);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>().WhereEndsWith(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EndsWith);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query3 = new Query<Account>().WhereEndsWith(a => a.Name, "test");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EndsWith);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query4 = new Query<Account>().WhereEndsWith<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EndsWith);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereEqual()
        {
            var query = new Query<Account>().WhereEqual(Account.Fields.Name, "test");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>().WhereEqual(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query3 = new Query<Account>().WhereEqual(a => a.Name, "test");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");

            var query4 = new Query<Account>().WhereEqual<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereEqualBusinessId()
        {
            var query = new Query<Account>().WhereEqualBusinessId(Account.Fields.OwningBusinessUnit);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualBusinessId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereEqualBusinessId(Account.Fields.OwningBusinessUnit, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualBusinessId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereEqualBusinessId(a => a.OwningBusinessUnit);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualBusinessId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereEqualBusinessId<Account>(Account.EntityLogicalName, a => a.OwningBusinessUnit);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualBusinessId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserId()
        {
            var query = new Query<Account>().WhereEqualUserId(Account.Fields.OwnerId);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereEqualUserId(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereEqualUserId(a => a.OwnerId);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereEqualUserId<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserLanguage()
        {
            var query = new Query<Account>().WhereEqualUserLanguage("no_language_attribute");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, "no_language_attribute");
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserLanguage);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereEqualUserLanguage("no_language_attribute", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, "no_language_attribute");
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserLanguage);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereEqualUserLanguage(a => a.NumberOfEmployees);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserLanguage);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereEqualUserLanguage<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserLanguage);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchy()
        {
            var query = new Query<Account>().WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchy);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchy);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereEqualUserOrUserHierarchy(a => a.OwnerId);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchy);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereEqualUserOrUserHierarchy<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchy);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchyAndTeams()
        {
            var query = new Query<Account>().WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereEqualUserOrUserHierarchyAndTeams(a => a.OwnerId);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereEqualUserOrUserHierarchyAndTeams<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserTeams()
        {
            var query = new Query<Account>().WhereEqualUserOrUserTeams(Account.Fields.OwnerId);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserTeams);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereEqualUserOrUserTeams(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserTeams);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereEqualUserOrUserTeams(a => a.OwnerId);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserTeams);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereEqualUserOrUserTeams<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserTeams);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserTeams()
        {
            var query = new Query<Account>().WhereEqualUserTeams(Account.Fields.OwnerId);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserTeams);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereEqualUserTeams(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserTeams);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereEqualUserTeams(a => a.OwnerId);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserTeams);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereEqualUserTeams<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.EqualUserTeams);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereGreaterEqual()
        {
            var query = new Query<Account>().WhereGreaterEqual(Account.Fields.NumberOfEmployees, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereGreaterEqual(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereGreaterEqual(a => a.NumberOfEmployees, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), 10);

            var query4 = new Query<Account>().WhereGreaterEqual<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereGreaterThan()
        {
            var query = new Query<Account>().WhereGreaterThan(Account.Fields.NumberOfEmployees, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereGreaterThan(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereGreaterThan(a => a.NumberOfEmployees, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Single(), 10);

            var query4 = new Query<Account>().WhereGreaterThan<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Single(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereIn()
        {
            var query = new Query<Account>().WhereIn(Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query2 = new Query<Account>().WhereIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().WhereIn(a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().WhereIn<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriod()
        {
            var query = new Query<Account>().WhereInFiscalPeriod(Account.Fields.CreatedOn, 1);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriod);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 1);

            var query2 = new Query<Account>().WhereInFiscalPeriod(Account.Fields.CreatedOn, 1, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriod);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 1);

            var query3 = new Query<Account>().WhereInFiscalPeriod(a => a.CreatedOn, 1);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriod);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 1);

            var query4 = new Query<Account>().WhereInFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriod);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 1);
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriodAndYear()
        {
            var query = new Query<Account>()
                .WhereInFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriodAndYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query2 = new Query<Account>()
                .WhereInFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriodAndYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query3 = new Query<Account>().WhereInFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriodAndYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query4 = new Query<Account>().WhereInFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriodAndYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[1], 2018);
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalYear()
        {
            var query = new Query<Account>().WhereInFiscalYear(Account.Fields.CreatedOn, 2018);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 2018);

            var query2 = new Query<Account>().WhereInFiscalYear(Account.Fields.CreatedOn, 2018, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 2018);

            var query3 = new Query<Account>().WhereInFiscalYear(a => a.CreatedOn, 2018);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 2018);

            var query4 = new Query<Account>().WhereInFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 2018);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InFiscalYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 2018);
        }

        [TestMethod]
        public void ShouldSetWhereInList()
        {
            var query = new Query<Account>().WhereIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query2 = new Query<Account>().WhereIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query3 = new Query<Account>().WhereIn(a => a.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query4 = new Query<Account>().WhereIn<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereInOrAfterFiscalPeriodAndYear()
        {
            var query = new Query<Account>().WhereInOrAfterFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InOrAfterFiscalPeriodAndYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query2 = new Query<Account>().WhereInOrAfterFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InOrAfterFiscalPeriodAndYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query3 = new Query<Account>().WhereInOrAfterFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InOrAfterFiscalPeriodAndYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query4 = new Query<Account>().WhereInOrAfterFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InOrAfterFiscalPeriodAndYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[1], 2018);
        }

        [TestMethod]
        public void ShouldSetWhereInOrBeforeFiscalPeriodAndYear()
        {
            var query = new Query<Account>().WhereInOrBeforeFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query2 = new Query<Account>().WhereInOrBeforeFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query3 = new Query<Account>().WhereInOrBeforeFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[1], 2018);

            var query4 = new Query<Account>().WhereInOrBeforeFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[1], 2018);
        }

        [TestMethod]
        public void ShouldSetWhereLast7Days()
        {
            var query = new Query<Account>().WhereLast7Days(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Last7Days);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereLast7Days(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Last7Days);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereLast7Days(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Last7Days);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereLast7Days<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Last7Days);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalPeriod()
        {
            var query = new Query<Account>().WhereLastFiscalPeriod(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastFiscalPeriod);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereLastFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastFiscalPeriod);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereLastFiscalPeriod(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastFiscalPeriod);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereLastFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastFiscalPeriod);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalYear()
        {
            var query = new Query<Account>().WhereLastFiscalYear(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastFiscalYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereLastFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastFiscalYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereLastFiscalYear(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastFiscalYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereLastFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastFiscalYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastMonth()
        {
            var query = new Query<Account>().WhereLastMonth(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastMonth);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereLastMonth(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastMonth);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereLastMonth(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastMonth);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereLastMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastMonth);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastWeek()
        {
            var query = new Query<Account>().WhereLastWeek(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastWeek);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereLastWeek(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastWeek);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereLastWeek(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastWeek);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereLastWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastWeek);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastXDays()
        {
            var query = new Query<Account>().WhereLastXDays(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXDays);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLastXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXDays);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLastXDays(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXDays);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLastXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXDays);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalPeriods()
        {
            var query = new Query<Account>().WhereLastXFiscalPeriods(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXFiscalPeriods);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLastXFiscalPeriods(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXFiscalPeriods);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLastXFiscalPeriods(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXFiscalPeriods);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLastXFiscalPeriods<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXFiscalPeriods);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalYears()
        {
            var query = new Query<Account>().WhereLastXFiscalYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXFiscalYears);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLastXFiscalYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXFiscalYears);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLastXFiscalYears(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXFiscalYears);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLastXFiscalYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXFiscalYears);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXHours()
        {
            var query = new Query<Account>().WhereLastXHours(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXHours);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLastXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXHours);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLastXHours(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXHours);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLastXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXHours);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXMonths()
        {
            var query = new Query<Account>().WhereLastXMonths(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXMonths);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLastXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXMonths);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLastXMonths(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXMonths);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLastXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXMonths);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXWeeks()
        {
            var query = new Query<Account>().WhereLastXWeeks(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXWeeks);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLastXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXWeeks);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLastXWeeks(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXWeeks);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLastXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXWeeks);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXYears()
        {
            var query = new Query<Account>().WhereLastXYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXYears);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLastXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXYears);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLastXYears(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXYears);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLastXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastXYears);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastYear()
        {
            var query = new Query<Account>().WhereLastYear(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereLastYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereLastYear(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereLastYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LastYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLessEqual()
        {
            var query = new Query<Account>().WhereLessEqual(Account.Fields.NumberOfEmployees, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLessEqual(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLessEqual(a => a.NumberOfEmployees, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLessEqual<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLessThan()
        {
            var query = new Query<Account>().WhereLessThan(Account.Fields.NumberOfEmployees, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereLessThan(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereLessThan(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereLessThan<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLike()
        {
            var query = new Query<Account>()
                .WhereLike(Account.Fields.Name, "%test%");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Like);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), "%test%");

            var query2 = new Query<Account>()
                .WhereLike(Account.Fields.Name, "%test%", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Like);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), "%test%");

            var query3 = new Query<Account>().WhereLike(a => a.Name, "%test%");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Like);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), "%test%");

            var query4 = new Query<Account>().WhereLike<Account>(Account.EntityLogicalName, a => a.Name, "%test%");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Like);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), "%test%");
        }

        [TestMethod]
        public void ShouldSetWhereMask()
        {
            var obj = new object();
            var query = new Query<Account>()
                .WhereMask(Account.Fields.Name, obj);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Mask);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query2 = new Query<Account>()
                .WhereMask(Account.Fields.Name, obj, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Mask);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query3 = new Query<Account>().WhereMask(a => a.Name, obj);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Mask);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query4 = new Query<Account>().WhereMask<Account>(Account.EntityLogicalName, a => a.Name, obj);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Mask);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), obj);
        }

        [TestMethod]
        public void ShouldSetWhereMasksSelect()
        {
            var obj = new object();
            var query = new Query<Account>().WhereMasksSelect(Account.Fields.Name, obj);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.MasksSelect);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query2 = new Query<Account>().WhereMasksSelect(Account.Fields.Name, obj, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.MasksSelect);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query3 = new Query<Account>().WhereMasksSelect(a => a.Name, obj);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.MasksSelect);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query4 = new Query<Account>().WhereMasksSelect<Account>(Account.EntityLogicalName, a => a.Name, obj);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.MasksSelect);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), obj);
        }

        [TestMethod]
        public void ShouldSetWhereNext7Days()
        {
            var query = new Query<Account>().WhereNext7Days(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Next7Days);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereNext7Days(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Next7Days);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNext7Days(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Next7Days);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNext7Days<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Next7Days);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalPeriod()
        {
            var query = new Query<Account>().WhereNextFiscalPeriod(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextFiscalPeriod);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereNextFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextFiscalPeriod);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNextFiscalPeriod(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextFiscalPeriod);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNextFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextFiscalPeriod);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalYear()
        {
            var query = new Query<Account>().WhereNextFiscalYear(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextFiscalYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereNextFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextFiscalYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNextFiscalYear(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextFiscalYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNextFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextFiscalYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextMonth()
        {
            var query = new Query<Account>()
                .WhereNextMonth(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextMonth);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereNextMonth(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextMonth);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNextMonth(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextMonth);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNextMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextMonth);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextWeek()
        {
            var query = new Query<Account>().WhereNextWeek(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextWeek);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>().WhereNextWeek(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextWeek);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNextWeek(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextWeek);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNextWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextWeek);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextXDays()
        {
            var query = new Query<Account>().WhereNextXDays(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXDays);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereNextXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXDays);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereNextXDays(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXDays);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereNextXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXDays);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalPeriods()
        {
            var query = new Query<Account>().WhereNextXFiscalPeriods(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXFiscalPeriods);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>().WhereNextXFiscalPeriods(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXFiscalPeriods);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereNextXFiscalPeriods(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXFiscalPeriods);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereNextXFiscalPeriods<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXFiscalPeriods);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalYears()
        {
            var query = new Query<Account>()
                .WhereNextXFiscalYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXFiscalYears);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereNextXFiscalYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXFiscalYears);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereNextXFiscalYears(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXFiscalYears);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereNextXFiscalYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXFiscalYears);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXHours()
        {
            var query = new Query<Account>()
                .WhereNextXHours(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXHours);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereNextXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXHours);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereNextXHours(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXHours);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereNextXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXHours);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXMonths()
        {
            var query = new Query<Account>()
                .WhereNextXMonths(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXMonths);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereNextXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXMonths);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereNextXMonths(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXMonths);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereNextXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXMonths);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXWeeks()
        {
            var query = new Query<Account>()
                .WhereNextXWeeks(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXWeeks);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereNextXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXWeeks);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereNextXWeeks(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXWeeks);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereNextXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXWeeks);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXYears()
        {
            var query = new Query<Account>()
                .WhereNextXYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXYears);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereNextXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXYears);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereNextXYears(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXYears);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereNextXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextXYears);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextYear()
        {
            var query = new Query<Account>()
                .WhereNextYear(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereNextYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNextYear(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNextYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NextYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNotBetween()
        {
            var query = new Query<Account>()
                .WhereNotBetween(Account.Fields.NumberOfEmployees, 10, 50);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotBetween);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values[1], 50);

            var query2 = new Query<Account>()
                .WhereNotBetween(Account.Fields.NumberOfEmployees, 10, 50, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotBetween);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values[1], 50);

            var query3 = new Query<Account>().WhereNotBetween(a => a.CreatedOn, 10, 50);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotBetween);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values[1], 50);

            var query4 = new Query<Account>().WhereNotBetween<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10, 50);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotBetween);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values[1], 50);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqual()
        {
            var query = new Query<Account>()
                .WhereNotEqual(Account.Fields.Name, "test");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), "test");

            var query2 = new Query<Account>()
                .WhereNotEqual(Account.Fields.Name, "test", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), "test");

            var query3 = new Query<Account>().WhereNotEqual(a => a.Name, "test");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), "test");

            var query4 = new Query<Account>().WhereNotEqual<Account>(Account.EntityLogicalName, a => a.Name, "test");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualBusinessId()
        {
            var query = new Query<Account>()
                .WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqualBusinessId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqualBusinessId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNotEqualBusinessId(a => a.OwningBusinessUnit);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqualBusinessId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNotEqualBusinessId<Account>(Account.EntityLogicalName, a => a.OwningBusinessUnit);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqualBusinessId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualUserId()
        {
            var query = new Query<Account>()
                .WhereNotEqualUserId(Account.Fields.OwnerId);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqualUserId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereNotEqualUserId(Account.Fields.OwnerId, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqualUserId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNotEqualUserId(a => a.OwnerId);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqualUserId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNotEqualUserId<Account>(Account.EntityLogicalName, a => a.OwnerId);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotEqualUserId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNotIn()
        {
            var query = new Query<Account>()
                .WhereNotIn(Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query2 = new Query<Account>()
                .WhereNotIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().WhereNotIn(a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().WhereNotIn<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereNotInList()
        {
            var query = new Query<Account>()
                .WhereNotIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query2 = new Query<Account>()
                .WhereNotIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query3 = new Query<Account>().WhereNotIn(a => a.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query3.QueryExpression.Criteria.Conditions.First().Values is IList);

            var query4 = new Query<Account>().WhereNotIn<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, new List<int> { 1, 2, 3 });

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query4.QueryExpression.Criteria.Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereNotLike()
        {
            var query = new Query<Account>()
                .WhereNotLike(Account.Fields.Name, "%test%");

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotLike);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), "%test%");

            var query2 = new Query<Account>()
                .WhereNotLike(Account.Fields.Name, "%test%", Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotLike);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), "%test%");

            var query3 = new Query<Account>().WhereNotLike(a => a.Name, "%test%");

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotLike);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), "%test%");

            var query4 = new Query<Account>().WhereNotLike<Account>(Account.EntityLogicalName, a => a.Name, "%test%");

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotLike);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), "%test%");
        }

        [TestMethod]
        public void ShouldSetWhereNotMask()
        {
            var obj = new object();
            var query = new Query<Account>()
                .WhereNotMask(Account.Fields.Name, obj);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotMask);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query2 = new Query<Account>()
                 .WhereNotMask(Account.Fields.Name, obj, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotMask);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query3 = new Query<Account>().WhereNotMask(a => a.Name, obj);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotMask);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), obj);

            var query4 = new Query<Account>().WhereNotMask<Account>(Account.EntityLogicalName, a => a.Name, obj);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotMask);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), obj);
        }

        [TestMethod]
        public void ShouldSetWhereNotNull()
        {
            var query = new Query<Account>()
                .WhereNotNull(Account.Fields.Name);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotNull);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereNotNull(Account.Fields.Name, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotNull);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNotNull(a => a.Name);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotNull);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNotNull<Account>(Account.EntityLogicalName, a => a.Name);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotNull);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNotOn()
        {
            var date = new DateTime();
            var query = new Query<Account>()
                .WhereNotOn(Account.Fields.CreatedOn, date);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query2 = new Query<Account>()
                .WhereNotOn(Account.Fields.CreatedOn, date, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query3 = new Query<Account>().WhereNotOn(a => a.CreatedOn, date);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query4 = new Query<Account>().WhereNotOn<Account>(Account.EntityLogicalName, a => a.CreatedOn, date);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), date);
        }

        [TestMethod]
        public void ShouldSetWhereNotUnder()
        {
            var guid = new Guid();
            var query = new Query<Account>()
                .WhereNotUnder(Account.Fields.AccountId, guid);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotUnder);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query2 = new Query<Account>()
                .WhereNotUnder(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotUnder);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query3 = new Query<Account>().WhereNotUnder(a => a.AccountId, guid);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotUnder);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query4 = new Query<Account>().WhereNotUnder<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.NotUnder);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereNull()
        {
            var query = new Query<Account>()
                .WhereNull(Account.Fields.Name);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Null);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereNull(Account.Fields.Name, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Null);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereNull(a => a.Name);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Null);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereNull<Account>(Account.EntityLogicalName, a => a.Name);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Null);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXDays()
        {
            var query = new Query<Account>()
                .WhereOlderThanXDays(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXDays);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereOlderThanXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXDays);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereOlderThanXDays(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXDays);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereOlderThanXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXDays);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXHours()
        {
            var query = new Query<Account>()
                .WhereOlderThanXHours(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXHours);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereOlderThanXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXHours);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereOlderThanXHours(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXHours);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereOlderThanXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXHours);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMinutes()
        {
            var query = new Query<Account>()
                .WhereOlderThanXMinutes(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXMinutes);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereOlderThanXMinutes(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXMinutes);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereOlderThanXMinutes(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXMinutes);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereOlderThanXMinutes<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXMinutes);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMonths()
        {
            var query = new Query<Account>()
                .WhereOlderThanXMonths(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXMonths);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereOlderThanXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXMonths);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereOlderThanXMonths(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXMonths);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereOlderThanXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXMonths);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXWeeks()
        {
            var query = new Query<Account>()
                .WhereOlderThanXWeeks(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXWeeks);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereOlderThanXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXWeeks);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereOlderThanXWeeks(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXWeeks);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereOlderThanXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXWeeks);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXYears()
        {
            var query = new Query<Account>()
                .WhereOlderThanXYears(Account.Fields.CreatedOn, 10);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXYears);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .WhereOlderThanXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXYears);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query3 = new Query<Account>().WhereOlderThanXYears(a => a.CreatedOn, 10);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXYears);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), 10);

            var query4 = new Query<Account>().WhereOlderThanXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OlderThanXYears);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOn()
        {
            var date = new DateTime();
            var query = new Query<Account>()
                .WhereOn(Account.Fields.CreatedOn, date);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.On);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query2 = new Query<Account>()
                .WhereOn(Account.Fields.CreatedOn, date, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.On);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query3 = new Query<Account>().WhereOn(a => a.CreatedOn, date);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.On);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query4 = new Query<Account>().WhereOn<Account>(Account.EntityLogicalName, a => a.CreatedOn, date);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.On);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), date);
        }

        [TestMethod]
        public void ShouldSetWhereOnOrAfter()
        {
            var date = new DateTime();
            var query = new Query<Account>()
                .WhereOnOrAfter(Account.Fields.CreatedOn, date);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OnOrAfter);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query2 = new Query<Account>()
                .WhereOnOrAfter(Account.Fields.CreatedOn, date, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OnOrAfter);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query3 = new Query<Account>().WhereOnOrAfter(a => a.CreatedOn, date);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OnOrAfter);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query4 = new Query<Account>().WhereOnOrAfter<Account>(Account.EntityLogicalName, a => a.CreatedOn, date);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OnOrAfter);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), date);
        }

        [TestMethod]
        public void ShouldSetWhereOnOrBefore()
        {
            var date = new DateTime();
            var query = new Query<Account>()
                .WhereOnOrBefore(Account.Fields.CreatedOn, date);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OnOrBefore);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query2 = new Query<Account>()
                .WhereOnOrBefore(Account.Fields.CreatedOn, date, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OnOrBefore);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query3 = new Query<Account>().WhereOnOrBefore(a => a.CreatedOn, date);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OnOrBefore);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), date);

            var query4 = new Query<Account>().WhereOnOrBefore<Account>(Account.EntityLogicalName, a => a.CreatedOn, date);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.OnOrBefore);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), date);
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalPeriod()
        {
            var query = new Query<Account>()
                .WhereThisFiscalPeriod(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisFiscalPeriod);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereThisFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisFiscalPeriod);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereThisFiscalPeriod(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisFiscalPeriod);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereThisFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisFiscalPeriod);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalYear()
        {
            var query = new Query<Account>()
                .WhereThisFiscalYear(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisFiscalYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereThisFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisFiscalYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereThisFiscalYear(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisFiscalYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereThisFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisFiscalYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereThisMonth()
        {
            var query = new Query<Account>()
                .WhereThisMonth(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisMonth);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereThisMonth(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisMonth);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereThisMonth(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisMonth);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereThisMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisMonth);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereThisWeek()
        {
            var query = new Query<Account>()
                .WhereThisWeek(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisWeek);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereThisWeek(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisWeek);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereThisWeek(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisWeek);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereThisWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisWeek);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereThisYear()
        {
            var query = new Query<Account>()
                .WhereThisYear(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisYear);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereThisYear(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisYear);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereThisYear(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisYear);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereThisYear<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.ThisYear);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereToday()
        {
            var query = new Query<Account>()
                .WhereToday(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Today);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereToday(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Today);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereToday(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Today);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereToday<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Today);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereTomorrow()
        {
            var query = new Query<Account>()
                .WhereTomorrow(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Tomorrow);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereTomorrow(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Tomorrow);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereTomorrow(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Tomorrow);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereTomorrow<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Tomorrow);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereUnder()
        {
            Guid guid = new Guid();
            var query = new Query<Account>()
                .WhereUnder(Account.Fields.AccountId, guid);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Under);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query2 = new Query<Account>()
                .WhereUnder(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Under);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query3 = new Query<Account>().WhereUnder(a => a.AccountId, guid);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Under);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query4 = new Query<Account>().WhereUnder<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Under);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereUnderOrEqual()
        {
            Guid guid = new Guid();
            var query = new Query<Account>()
                .WhereUnderOrEqual(Account.Fields.AccountId, guid);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.UnderOrEqual);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query2 = new Query<Account>()
                .WhereUnderOrEqual(Account.Fields.AccountId, guid, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.UnderOrEqual);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query3 = new Query<Account>().WhereUnderOrEqual(a => a.AccountId, guid);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.UnderOrEqual);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.First(), guid);

            var query4 = new Query<Account>().WhereUnderOrEqual<Account>(Account.EntityLogicalName, a => a.AccountId, guid);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.UnderOrEqual);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.First(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereYesterday()
        {
            var query = new Query<Account>()
                .WhereYesterday(Account.Fields.CreatedOn);

            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Yesterday);
            Assert.AreEqual(query.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .WhereYesterday(Account.Fields.CreatedOn, Account.EntityLogicalName);

            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Yesterday);
            Assert.AreEqual(query2.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query3 = new Query<Account>().WhereYesterday(a => a.CreatedOn);

            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Yesterday);
            Assert.AreEqual(query3.QueryExpression.Criteria.Conditions.First().Values.Count, 0);

            var query4 = new Query<Account>().WhereYesterday<Account>(Account.EntityLogicalName, a => a.CreatedOn);

            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Operator, ConditionOperator.Yesterday);
            Assert.AreEqual(query4.QueryExpression.Criteria.Conditions.First().Values.Count, 0);
        }

        #endregion Conditions

        #region IOrganizationService calls

        private Guid item1Id = Guid.NewGuid();
        private Guid item2Id = Guid.NewGuid();

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

                Assert.AreEqual(records.Count, 1);
                Assert.AreEqual(records.First().Id, item1Id);
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

                Assert.AreEqual(records.Count, 1);
                Assert.AreEqual(records.First().Id, item1Id);
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

                Assert.AreEqual(records.Count, 1);
                Assert.AreEqual(records.First().Id, item1Id);
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

                Assert.AreEqual(records.Count, 1);
                Assert.AreEqual(records.First().Id, item1Id);
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
                Assert.AreEqual(record.Id, item1Id);
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
                Assert.AreEqual(record.Id, item1Id);
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
                    .GetLast(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(record.Id, item2Id);
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
                    .GetLastOrDefault(service);

                Assert.IsNotNull(record);
                Assert.AreEqual(record.Id, item2Id);
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

                Assert.AreEqual(records.Entities.Count, 2);
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
                Assert.AreEqual(record.Id, item1Id);
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
                Assert.AreEqual(record.Id, item1Id);
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

        #endregion IOrganizationService calls
    }
}