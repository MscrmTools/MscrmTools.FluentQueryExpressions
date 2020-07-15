using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class LinkTest
    {
        [TestMethod]
        public void ShouldAddFilter()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .AddFilters(LogicalOperator.Or, new Filter()));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Filters.Count, 1);
        }

        [TestMethod]
        public void ShouldAddFilters()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .AddFilters(
                        new Filter(),
                        new Filter()
                    )
                );

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.FilterOperator, LogicalOperator.And);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Filters.Count, 2);
        }

        [TestMethod]
        public void ShouldAddLink()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .AddLink(new Link<Task>(Task.Fields.RegardingObjectId, Contact.Fields.ContactId)));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkEntities.Count, 1);
        }

        [TestMethod]
        public void ShouldAddLink2()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .AddLink(new Link(Task.EntityLogicalName, Task.Fields.RegardingObjectId, Contact.Fields.ContactId)));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkEntities.Count, 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkEntities.First().LinkFromEntityName, Contact.EntityLogicalName);
        }

        [TestMethod]
        public void ShouldAddOrder()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId,
                        JoinOperator.LeftOuter)
                    .Order(Contact.Fields.FullName, OrderType.Ascending));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().Orders.First().AttributeName, Contact.Fields.FullName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().Orders.First().OrderType, OrderType.Ascending);
        }

        [TestMethod]
        public void ShouldCreateLink()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter));

            Assert.AreEqual(query.QueryExpression.LinkEntities.Count, 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromEntityName, Account.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromAttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToAttributeName, Contact.Fields.ParentCustomerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToEntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().EntityAlias, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().JoinOperator, JoinOperator.LeftOuter);
        }

        [TestMethod]
        public void ShouldCreateLink2()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .SetAlias("toto")
                    .SetDefaultFilterOperator(LogicalOperator.Or)
                    .Select(true));

            Assert.AreEqual(query.QueryExpression.LinkEntities.Count, 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromEntityName, Account.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromAttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToAttributeName, Contact.Fields.ParentCustomerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToEntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().EntityAlias, "toto");
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().JoinOperator, JoinOperator.LeftOuter);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().Columns.AllColumns, true);
        }

        [TestMethod]
        public void ShouldCreateLink3()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .SetAlias("toto")
                    .SetDefaultFilterOperator(LogicalOperator.Or)
                    .Select());

            Assert.AreEqual(query.QueryExpression.LinkEntities.Count, 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromEntityName, Account.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromAttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToAttributeName, Contact.Fields.ParentCustomerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToEntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().EntityAlias, "toto");
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().JoinOperator, JoinOperator.LeftOuter);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().Columns.Columns.Count, 0);
        }

        [TestMethod]
        public void ShouldCreateLink4()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .SetAlias("toto")
                    .SetDefaultFilterOperator(LogicalOperator.Or)
                    .Select(Contact.Fields.FullName));

            Assert.AreEqual(query.QueryExpression.LinkEntities.Count, 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromEntityName, Account.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromAttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToAttributeName, Contact.Fields.ParentCustomerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToEntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().EntityAlias, "toto");
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().JoinOperator, JoinOperator.LeftOuter);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.FilterOperator, LogicalOperator.Or);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().Columns.Columns.First(), Contact.Fields.FullName);
        }

        [TestMethod]
        public void ShouldCreateLinkNotGeneric()
        {
            var query = new Query<Account>()
                .AddLink(new Link(Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter));

            Assert.AreEqual(query.QueryExpression.LinkEntities.Count, 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromEntityName, Account.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkFromAttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToAttributeName, Contact.Fields.ParentCustomerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkToEntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().EntityAlias, Contact.EntityLogicalName);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().JoinOperator, JoinOperator.LeftOuter);
        }

        #region Conditions

        [TestMethod]
        public void ShouldCompareWhereEqual()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.Fields.NumberOfChildren).Equal(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.EntityLogicalName, Contact.Fields.NumberOfChildren).Equal(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterOrEqualThan()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.Fields.NumberOfChildren).GreaterOrEqualThan(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.EntityLogicalName, Contact.Fields.NumberOfChildren).GreaterOrEqualThan(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterThan()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.Fields.NumberOfChildren).GreaterThan(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.EntityLogicalName, Contact.Fields.NumberOfChildren).GreaterThan(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereLessOrEqualThan()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.Fields.NumberOfChildren).LessOrEqualThan(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.EntityLogicalName, Contact.Fields.NumberOfChildren).LessOrEqualThan(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereLessThan()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.Fields.NumberOfChildren).LessThan(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.EntityLogicalName, Contact.Fields.NumberOfChildren).LessThan(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldCompareWhereNotEqual()
        {
            var query = new Query<Account>()
                    .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                        .Compare(Contact.Fields.NumberOfChildren).NotEqual(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Compare(Contact.EntityLogicalName, Contact.Fields.NumberOfChildren).NotEqual(Contact.Fields.AnnualIncome));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Contact.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Contact.Fields.NumberOfChildren);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), Contact.Fields.AnnualIncome);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().CompareColumns, true);
        }

        [TestMethod]
        public void ShouldSetWhere()
        {
            var guid = Guid.NewGuid();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).Where(Account.Fields.AccountId, ConditionOperator.Above, guid));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), guid);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).Where(Account.EntityLogicalName, Account.Fields.AccountId, ConditionOperator.Above, guid));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereAbove()
        {
            var guid = Guid.NewGuid();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereAbove(Account.Fields.AccountId, guid));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), guid);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereAbove(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Above);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereAboveOrEqual()
        {
            var guid = Guid.NewGuid();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereAboveOrEqual(Account.Fields.AccountId, guid));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.AboveOrEqual);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), guid);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereAboveOrEqual(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.AboveOrEqual);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereBeginsWith()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereBeginsWith(Account.Fields.Name, "test"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.BeginsWith);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereBeginsWith(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.BeginsWith);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereBetween()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereBetween(Account.Fields.NumberOfEmployees, 10, 50));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Between);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 50);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereBetween(Account.Fields.NumberOfEmployees, 10, 50, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Between);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 50);
        }

        [TestMethod]
        public void ShouldSetWhereChildOf()
        {
            var guid = Guid.NewGuid();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereChildOf(Account.Fields.AccountId, guid));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ChildOf);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), guid);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereChildOf(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ChildOf);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereContains()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereContains(Account.Fields.Name, "test"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Contains);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereContains(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Contains);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereContainValues()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ContainValues);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereContainValues(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ContainValues);
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotBeginWith()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereDoesNotBeginWith(Account.Fields.Name, "test"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.DoesNotBeginWith);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereDoesNotBeginWith(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.DoesNotBeginWith);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContain()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereDoesNotContain(Account.Fields.Name, "test"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.DoesNotContain);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereDoesNotContain(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.DoesNotContain);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContainValues()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereDoesNotContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.DoesNotContainValues);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereDoesNotContainValues(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.DoesNotContainValues);
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotEndWith()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereDoesNotEndWith(Account.Fields.Name, "test"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.DoesNotEndWith);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereDoesNotEndWith(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.DoesNotEndWith);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereEndsWith()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEndsWith(Account.Fields.Name, "test"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EndsWith);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEndsWith(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EndsWith);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereEqual()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqual(Account.Fields.Name, "test"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqual(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Equal);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereEqualBusinessId()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualBusinessId(Account.Fields.OwningBusinessUnit));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualBusinessId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualBusinessId(Account.Fields.OwningBusinessUnit, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualBusinessId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserId()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserId(Account.Fields.OwnerId));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserId(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserLanguage()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserLanguage("no_language_attribute"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, "no_language_attribute");
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserLanguage);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserLanguage("no_language_attribute", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, "no_language_attribute");
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserLanguage);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchy()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchy);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchy);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchyAndTeams()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserTeams()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserOrUserTeams(Account.Fields.OwnerId));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserTeams);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserOrUserTeams(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserOrUserTeams);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserTeams()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserTeams(Account.Fields.OwnerId));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserTeams);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereEqualUserTeams(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.EqualUserTeams);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereGreaterEqual()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereGreaterEqual(Account.Fields.NumberOfEmployees, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereGreaterEqual(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.GreaterEqual);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereGreaterThan()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereGreaterThan(Account.Fields.NumberOfEmployees, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereGreaterThan(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.GreaterThan);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereIn()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereIn(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriod()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInFiscalPeriod(Account.Fields.CreatedOn, 1));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriod);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 1);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInFiscalPeriod(Account.Fields.CreatedOn, 1, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriod);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 1);
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriodAndYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriodAndYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 2018);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InFiscalPeriodAndYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 2018);
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInFiscalYear(Account.Fields.CreatedOn, 2018));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InFiscalYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 2018);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInFiscalYear(Account.Fields.CreatedOn, 2018, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InFiscalYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 2018);
        }

        [TestMethod]
        public void ShouldSetWhereInList()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .WhereIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values is IList);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .WhereIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.In);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereInOrAfterFiscalPeriodAndYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInOrAfterFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InOrAfterFiscalPeriodAndYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 2018);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInOrAfterFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InOrAfterFiscalPeriodAndYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 2018);
        }

        [TestMethod]
        public void ShouldSetWhereInOrBeforeFiscalPeriodAndYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInOrBeforeFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 2018);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereInOrBeforeFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 1);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 2018);
        }

        [TestMethod]
        public void ShouldSetWhereLast7Days()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLast7Days(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Last7Days);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLast7Days(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Last7Days);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalPeriod()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastFiscalPeriod(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastFiscalPeriod);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastFiscalPeriod);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastFiscalYear(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastFiscalYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastFiscalYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastMonth()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastMonth(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastMonth);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastMonth(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastMonth);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastWeek()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastWeek(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastWeek);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastWeek(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastWeek);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLastXDays()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXDays(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXDays);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXDays);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalPeriods()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXFiscalPeriods(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXFiscalPeriods);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXFiscalPeriods(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXFiscalPeriods);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalYears()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXFiscalYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXFiscalYears);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXFiscalYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXFiscalYears);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXHours()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXHours(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXHours);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXHours);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXMonths()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXMonths(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXMonths);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXMonths);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXWeeks()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXWeeks(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXWeeks);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXWeeks);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastXYears()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXYears);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastXYears);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLastYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastYear(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLastYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LastYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereLessEqual()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLessEqual(Account.Fields.NumberOfEmployees, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLessEqual(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LessEqual);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLessThan()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLessThan(Account.Fields.NumberOfEmployees, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLessThan(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.LessThan);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereLike()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLike(Account.Fields.Name, "%test%"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Like);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), "%test%");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereLike(Account.Fields.Name, "%test%", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Like);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), "%test%");
        }

        [TestMethod]
        public void ShouldSetWhereMask()
        {
            var obj = new object();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereMask(Account.Fields.Name, obj));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Mask);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), obj);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereMask(Account.Fields.Name, obj, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Mask);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), obj);
        }

        [TestMethod]
        public void ShouldSetWhereMasksSelect()
        {
            var obj = new object();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereMasksSelect(Account.Fields.Name, obj));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.MasksSelect);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), obj);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereMasksSelect(Account.Fields.Name, obj, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.MasksSelect);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), obj);
        }

        [TestMethod]
        public void ShouldSetWhereNext7Days()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNext7Days(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Next7Days);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNext7Days(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Next7Days);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalPeriod()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextFiscalPeriod(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextFiscalPeriod);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextFiscalPeriod);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextFiscalYear(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextFiscalYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextFiscalYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextMonth()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextMonth(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextMonth);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextMonth(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextMonth);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextWeek()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextWeek(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextWeek);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextWeek(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextWeek);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNextXDays()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXDays(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXDays);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXDays);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalPeriods()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXFiscalPeriods(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXFiscalPeriods);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXFiscalPeriods(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXFiscalPeriods);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalYears()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXFiscalYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXFiscalYears);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXFiscalYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXFiscalYears);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXHours()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXHours(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXHours);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXHours);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXMonths()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXMonths(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXMonths);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXMonths);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXWeeks()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXWeeks(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXWeeks);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXWeeks);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextXYears()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXYears);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextXYears);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereNextYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextYear(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNextYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NextYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNotBetween()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotBetween(Account.Fields.NumberOfEmployees, 10, 50));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotBetween);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 50);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotBetween(Account.Fields.NumberOfEmployees, 10, 50, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.NumberOfEmployees);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotBetween);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0], 10);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1], 50);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqual()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotEqual(Account.Fields.Name, "test"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), "test");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotEqual(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotEqual);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), "test");
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualBusinessId()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotEqualBusinessId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwningBusinessUnit);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotEqualBusinessId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualUserId()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotEqualUserId(Account.Fields.OwnerId));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotEqualUserId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotEqualUserId(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.OwnerId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotEqualUserId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNotIn()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotIn(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereNotInList()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .WhereNotIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values is IList);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .WhereNotIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CustomerTypeCode);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotIn);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereNotLike()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotLike(Account.Fields.Name, "%test%"));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotLike);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), "%test%");

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotLike(Account.Fields.Name, "%test%", Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotLike);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), "%test%");
        }

        [TestMethod]
        public void ShouldSetWhereNotMask()
        {
            var obj = new object();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotMask(Account.Fields.Name, obj));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotMask);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), obj);

            var query2 = new Query<Account>()
                 .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotMask(Account.Fields.Name, obj, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotMask);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), obj);
        }

        [TestMethod]
        public void ShouldSetWhereNotNull()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotNull(Account.Fields.Name));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotNull);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotNull(Account.Fields.Name, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotNull);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereNotOn()
        {
            var date = new DateTime();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotOn(Account.Fields.CreatedOn, date));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), date);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotOn(Account.Fields.CreatedOn, date, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), date);
        }

        [TestMethod]
        public void ShouldSetWhereNotUnder()
        {
            var guid = new Guid();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotUnder(Account.Fields.AccountId, guid));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotUnder);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), guid);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNotUnder(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.NotUnder);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereNull()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNull(Account.Fields.Name));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Null);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereNull(Account.Fields.Name, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.Name);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Null);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXDays()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXDays(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXDays);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXDays);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXHours()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXHours(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXHours);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXHours);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMinutes()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXMinutes(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXMinutes);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXMinutes(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXMinutes);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMonths()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXMonths(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXMonths);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXMonths);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXWeeks()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXWeeks(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXWeeks);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXWeeks);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXYears()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXYears);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOlderThanXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OlderThanXYears);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), 10);
        }

        [TestMethod]
        public void ShouldSetWhereOn()
        {
            var date = new DateTime();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOn(Account.Fields.CreatedOn, date));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.On);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), date);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOn(Account.Fields.CreatedOn, date, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.On);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), date);
        }

        [TestMethod]
        public void ShouldSetWhereOnOrAfter()
        {
            var date = new DateTime();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOnOrAfter(Account.Fields.CreatedOn, date));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OnOrAfter);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), date);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOnOrAfter(Account.Fields.CreatedOn, date, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OnOrAfter);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), date);
        }

        [TestMethod]
        public void ShouldSetWhereOnOrBefore()
        {
            var date = new DateTime();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOnOrBefore(Account.Fields.CreatedOn, date));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OnOrBefore);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), date);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereOnOrBefore(Account.Fields.CreatedOn, date, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.OnOrBefore);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), date);
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalPeriod()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisFiscalPeriod(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisFiscalPeriod);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisFiscalPeriod);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisFiscalYear(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisFiscalYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisFiscalYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereThisMonth()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisMonth(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisMonth);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisMonth(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisMonth);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereThisWeek()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisWeek(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisWeek);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisWeek(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisWeek);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereThisYear()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisYear(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisYear);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereThisYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.ThisYear);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereToday()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereToday(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Today);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereToday(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Today);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereTomorrow()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereTomorrow(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Tomorrow);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereTomorrow(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Tomorrow);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        [TestMethod]
        public void ShouldSetWhereUnder()
        {
            Guid guid = new Guid();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereUnder(Account.Fields.AccountId, guid));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Under);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), guid);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereUnder(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Under);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereUnderOrEqual()
        {
            Guid guid = new Guid();
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereUnderOrEqual(Account.Fields.AccountId, guid));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.UnderOrEqual);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), guid);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereUnderOrEqual(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.AccountId);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.UnderOrEqual);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First(), guid);
        }

        [TestMethod]
        public void ShouldSetWhereYesterday()
        {
            var query = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereYesterday(Account.Fields.CreatedOn));

            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Yesterday);
            Assert.AreEqual(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);

            var query2 = new Query<Account>()
                .AddLink(new Link<Contact>(Contact.Fields.ParentCustomerId, Account.Fields.AccountId).WhereYesterday(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().EntityName, Account.EntityLogicalName);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName, Account.Fields.CreatedOn);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator, ConditionOperator.Yesterday);
            Assert.AreEqual(query2.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count, 0);
        }

        #endregion Conditions
    }
}