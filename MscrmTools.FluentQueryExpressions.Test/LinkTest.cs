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
        #region Link

        [TestMethod]
        public void ShouldAddLinkWithPublicMethods()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(l => l
                    .From(a => a.AccountId)
                    .To(c => c.ParentCustomerId)
                    .AddLink<Task>(lt => lt
                        .From(c => c.ContactId)
                        .To(t => t.RegardingObjectId)
                        .SetJoin(JoinOperator.LeftOuter)
                        .SetAlias("fakeAlias")
                        .SetLogicalOperator(LogicalOperator.Or)
                    )
                );

            var le = query.QueryExpression.LinkEntities.First().LinkEntities.First();
            Assert.AreEqual(Contact.EntityLogicalName, le.LinkFromEntityName);
            Assert.AreEqual(Contact.Fields.ContactId, le.LinkFromAttributeName);
            Assert.AreEqual(Task.EntityLogicalName, le.LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, le.LinkToAttributeName);
            Assert.AreEqual(JoinOperator.LeftOuter, le.JoinOperator);
            Assert.AreEqual("fakeAlias", le.EntityAlias);
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.FilterOperator);
        }

        #endregion Link

        #region Attributes

        [TestMethod]
        public void ShouldSelectAllAttributes()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(l => l
                    .Select(true)
                );

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(true, le.Columns.AllColumns);
        }

        [TestMethod]
        public void ShouldSelectOneAttributeWithLambda()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(l => l
                    .Select(c => c.FirstName)
                );

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(false, le.Columns.AllColumns);
            Assert.AreEqual(1, le.Columns.Columns.Count);
            Assert.IsTrue(le.Columns.Columns.Contains(Contact.Fields.FirstName));

            var query2 = new Query<Account>()
                .AddLink<Contact>(l => l
                    .Select(c => c.ContactId)
                );

            var le2 = query2.QueryExpression.LinkEntities.First();
            Assert.AreEqual(false, le2.Columns.AllColumns);
            Assert.AreEqual(1, le2.Columns.Columns.Count);
            Assert.IsTrue(le2.Columns.Columns.Contains(Contact.Fields.ContactId));
        }

        [TestMethod]
        public void ShouldSelectThreeAttributes()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(new Link(Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                    .Select(Contact.Fields.FirstName, Contact.Fields.LastName, Contact.Fields.ContactId)
                );

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(false, le.Columns.AllColumns);
            Assert.AreEqual(3, le.Columns.Columns.Count);
            Assert.IsTrue(le.Columns.Columns.Contains(Contact.Fields.FirstName));
            Assert.IsTrue(le.Columns.Columns.Contains(Contact.Fields.LastName));
            Assert.IsTrue(le.Columns.Columns.Contains(Contact.Fields.ContactId));
        }

        [TestMethod]
        public void ShouldSelectThreeAttributesWithLambda()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(l => l
                    .Select(c => new { c.FirstName, c.LastName, c.ContactId })
                );

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(false, le.Columns.AllColumns);
            Assert.AreEqual(3, le.Columns.Columns.Count);
            Assert.IsTrue(le.Columns.Columns.Contains(Contact.Fields.FirstName));
            Assert.IsTrue(le.Columns.Columns.Contains(Contact.Fields.LastName));
            Assert.IsTrue(le.Columns.Columns.Contains(Contact.Fields.ContactId));
        }

        #endregion Attributes

        #region Filters

        [TestMethod]
        public void ShouldAddEarlyBoundFilter()
        {
            var query = new Query<Account>().AddLink<Contact>(l => l
                    .AddFilter(new Filter<Contact>(LogicalOperator.Or))
                );

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.FilterOperator);
            Assert.AreEqual(1, le.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.Filters.First().FilterOperator);
        }

        [TestMethod]
        public void ShouldAddEarlyBoundFilters()
        {
            var query = new Query<Account>().AddLink<Contact>(l => l.AddFilters(new Filter<Contact>(LogicalOperator.Or), new Filter<Contact>(LogicalOperator.And)));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.FilterOperator);
            Assert.AreEqual(2, le.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.Filters[0].FilterOperator);
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.Filters[1].FilterOperator);

            var query2 = new Query<Account>().AddLink<Contact>(l => l.AddFilters(new Filter<Contact>(LogicalOperator.Or), new Filter<Contact>(LogicalOperator.And)).SetLogicalOperator(LogicalOperator.Or));

            var le2 = query2.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.Or, le2.LinkCriteria.FilterOperator);
            Assert.AreEqual(2, le2.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, le2.LinkCriteria.Filters[0].FilterOperator);
            Assert.AreEqual(LogicalOperator.And, le2.LinkCriteria.Filters[1].FilterOperator);
        }

        [TestMethod]
        public void ShouldAddEarlyBoundFiltersWithAnonymous()
        {
            var query = new Query<Account>().AddLink<Contact>(l => l.AddFilters(f1 => f1, f2 => f2));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.FilterOperator);
            Assert.AreEqual(2, le.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.Filters[0].FilterOperator);
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.Filters[1].FilterOperator);
        }

        [TestMethod]
        public void ShouldAddEarlyBoundFilterWithAnonymous()
        {
            var query = new Query<Account>().AddLink<Contact>(l => l.AddFilter(f => f));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.FilterOperator);
            Assert.AreEqual(1, le.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.Filters.First().FilterOperator);
        }

        [TestMethod]
        public void ShouldAddLateBoundFilter()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "accountid", "parentcustomerid").AddFilter(new Filter(LogicalOperator.Or)));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.FilterOperator);
            Assert.AreEqual(1, le.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.Filters.First().FilterOperator);
        }

        [TestMethod]
        public void ShouldAddLateBoundFilters()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "accountid", "parentcustomerid").AddFilters(new Filter(LogicalOperator.Or), new Filter(LogicalOperator.And)));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.FilterOperator);
            Assert.AreEqual(2, le.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.Filters[0].FilterOperator);
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.Filters[1].FilterOperator);

            var query2 = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "accountid", "parentcustomerid").AddFilters(new Filter(LogicalOperator.Or), new Filter(LogicalOperator.And)).SetLogicalOperator(LogicalOperator.Or));

            var le2 = query2.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.Or, le2.LinkCriteria.FilterOperator);
            Assert.AreEqual(2, le2.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, le2.LinkCriteria.Filters[0].FilterOperator);
            Assert.AreEqual(LogicalOperator.And, le2.LinkCriteria.Filters[1].FilterOperator);
        }

        [TestMethod]
        public void ShouldAddMixEarlyLateBoundFilters()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "accountid", "parentcustomerid").AddFilters(new Filter(LogicalOperator.Or), new Filter(LogicalOperator.And)));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.FilterOperator);
            Assert.AreEqual(2, le.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.Filters[0].FilterOperator);
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.Filters[1].FilterOperator);
        }

        [TestMethod]
        public void ShouldAddMixEarlyLateBoundFiltersWithObsoleteOperator()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "accountid", "parentcustomerid").AddFilters(new Filter(LogicalOperator.Or), new Filter(LogicalOperator.And)).SetLogicalOperator(LogicalOperator.Or));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.FilterOperator);
            Assert.AreEqual(2, le.LinkCriteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.Filters[0].FilterOperator);
            Assert.AreEqual(LogicalOperator.And, le.LinkCriteria.Filters[1].FilterOperator);
        }

        [TestMethod]
        public void ShouldSetLogicalOperatorOr()
        {
            var query = new Query<Account>().AddLink<Contact>(l => l.SetLogicalOperator(LogicalOperator.Or));
            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.Or, le.LinkCriteria.FilterOperator);

            var query2 = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "accountid", "parentcustomerid").SetLogicalOperator(LogicalOperator.Or));
            var le2 = query2.QueryExpression.LinkEntities.First();
            Assert.AreEqual(LogicalOperator.Or, le2.LinkCriteria.FilterOperator);
        }

        #endregion Filters

        #region Columns Comparer

        [TestMethod]
        public void ShouldCompareWhereEqualEarlyBound()
        {
            var query = new Query<Account>().AddLink<Contact>(l => l.Compare(c => c.CreatedOn).Equal(c => c.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereEqualLateBound()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "accountid", "parentcustomerid").Compare(Contact.Fields.CreatedOn).Equal(Contact.Fields.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterOrEqualThanEarlyBound()
        {
            var query = new Query<Account>().AddLink<Contact>(l => l.Compare(c => c.CreatedOn).GreaterOrEqualThan(c => c.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterOrEqualThanLateBound()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "accountid", "parentcustomerid").Compare(Contact.Fields.CreatedOn).GreaterOrEqualThan(Contact.Fields.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterThanEarlyBound()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(l => l.Compare(c => c.CreatedOn).GreaterThan(c => c.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterThanLateBound()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(new Link("contact", "accountid", "parentcustomerid").Compare(Contact.Fields.CreatedOn).GreaterThan(Contact.Fields.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessOrEqualThanEarlyBound()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(l => l.Compare(c => c.CreatedOn).LessOrEqualThan(c => c.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessOrEqualThanLateBound()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(new Link("contact", "accountid", "parentcustomerid").Compare(Contact.Fields.CreatedOn).LessOrEqualThan(Contact.Fields.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessThanEarlyBound()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(l => l.Compare(c => c.CreatedOn).LessThan(c => c.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessThanLateBound()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(new Link("contact", "accountid", "parentcustomerid").Compare(Contact.Fields.CreatedOn).LessThan(Contact.Fields.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereNotEqualEarlyBound()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(l => l.Compare(c => c.CreatedOn).NotEqual(c => c.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereNotEqualLateBound()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(new Link("contact", "accountid", "parentcustomerid").Compare(Contact.Fields.CreatedOn).NotEqual(Contact.Fields.ModifiedOn));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.CreatedOn, le.LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, le.LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(Contact.Fields.ModifiedOn, le.LinkCriteria.Conditions.First().Values.First());
            Assert.AreEqual(true, le.LinkCriteria.Conditions.First().CompareColumns);
        }

        #endregion Columns Comparer

        #region Link Entities

        [TestMethod]
        public void ShouldAddLinkEarlyBound()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(
                    a => a.AccountId,
                    c => c.ParentCustomerId,
                    l => l
                        .Select(c => new { c.FirstName, c.LastName })
                        .AddLink<Task>(
                            c => c.ContactId,
                            t => t.RegardingObjectId,
                            l2 => l2.Select(c => new { c.Subject }),
                        JoinOperator.LeftOuter
                    )
                );

            var innerLe = query.QueryExpression.LinkEntities.First().LinkEntities.First();
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkEntities.Count);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, innerLe.LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLe.LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLe.LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, innerLe.JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkEarlyBoundWithLink()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(
                    a => a.AccountId,
                    c => c.ParentCustomerId,
                    l => l
                        .Select(c => new { c.FirstName, c.LastName })
                        .AddLink(new Link<Contact, Task>(c => c.ContactId, t => t.RegardingObjectId)
                    )
                );

            var innerLe = query.QueryExpression.LinkEntities.First().LinkEntities.First();
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkEntities.Count);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, innerLe.LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLe.LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLe.LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.EntityAlias);
            Assert.AreEqual(JoinOperator.Inner, innerLe.JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkEarlyBoundWithOuterLink()
        {
            var query = new Query<Account>()
                 .AddLink<Contact>(
                     a => a.AccountId,
                     c => c.ParentCustomerId,
                     l => l
                         .Select(c => new { c.FirstName, c.LastName })
                         .AddLink(new Link<Contact, Task>(c => c.ContactId, t => t.RegardingObjectId, JoinOperator.LeftOuter)
                     )
                 );

            var innerLe = query.QueryExpression.LinkEntities.First().LinkEntities.First();
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkEntities.Count);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, innerLe.LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLe.LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLe.LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, innerLe.JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkEarlyBoundWithoutReturnedLink()
        {
            var query = new Query<Account>()
                .AddLink<Contact>(
                    a => a.AccountId,
                    c => c.ParentCustomerId,
                    l => l.AddLink<Task>(
                            c => c.ContactId,
                            t => t.RegardingObjectId,
                            JoinOperator.LeftOuter
                    )
                );

            var innerLe = query.QueryExpression.LinkEntities.First().LinkEntities.First();
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkEntities.Count);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, innerLe.LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLe.LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLe.LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, innerLe.JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkLateBoundWithLink()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(
                    new Link(Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                        .AddLink(Contact.Fields.ContactId, Task.EntityLogicalName, Task.Fields.RegardingObjectId)
                );

            var innerLe = query.QueryExpression.LinkEntities.First().LinkEntities.First();
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkEntities.Count);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, innerLe.LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLe.LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLe.LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.EntityAlias);
            Assert.AreEqual(JoinOperator.Inner, innerLe.JoinOperator);

            var query2 = new Query(Account.EntityLogicalName)
                .AddLink(
                    new Link(Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                        .AddLink(new Link(Task.EntityLogicalName, Task.Fields.RegardingObjectId, Contact.Fields.ContactId))
                );

            var innerLe2 = query2.QueryExpression.LinkEntities.First().LinkEntities.First();
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkEntities.Count);
            Assert.AreEqual(Task.EntityLogicalName, innerLe2.LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, innerLe2.LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLe2.LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLe2.LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, innerLe2.EntityAlias);
            Assert.AreEqual(JoinOperator.Inner, innerLe2.JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinkLateBoundWithOuterLink()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddLink(
                    new Link(Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, Account.Fields.AccountId)
                        .AddLink(Contact.Fields.ContactId, Task.EntityLogicalName, Task.Fields.RegardingObjectId, JoinOperator.LeftOuter)
                );

            var innerLe = query.QueryExpression.LinkEntities.First().LinkEntities.First();
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkEntities.Count);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, innerLe.LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLe.LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLe.LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, innerLe.EntityAlias);
            Assert.AreEqual(JoinOperator.LeftOuter, innerLe.JoinOperator);
        }

        [TestMethod]
        public void ShouldAddLinks()
        {
            var query = new Query<Account>()
               .AddLink<Contact>(
                   a => a.AccountId,
                   c => c.ParentCustomerId,
                   l => l
                    .AddLink<Task>(
                           c => c.ContactId,
                           t => t.RegardingObjectId,
                           JoinOperator.LeftOuter
                    )
                    .AddLink<Contact>(
                           c => c.ContactId,
                           parent => parent.ParentCustomerId,
                           JoinOperator.LeftOuter
                    )
               );

            var innerLes = query.QueryExpression.LinkEntities.First().LinkEntities;
            Assert.AreEqual(2, innerLes.Count);
            Assert.AreEqual(Task.EntityLogicalName, innerLes[0].LinkToEntityName);
            Assert.AreEqual(Task.Fields.RegardingObjectId, innerLes[0].LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLes[0].LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLes[0].LinkFromEntityName);
            Assert.AreEqual(Task.EntityLogicalName, innerLes[0].EntityAlias);
            Assert.AreEqual(Contact.EntityLogicalName, innerLes[1].LinkToEntityName);
            Assert.AreEqual(Contact.Fields.ParentCustomerId, innerLes[1].LinkToAttributeName);
            Assert.AreEqual(Contact.Fields.ContactId, innerLes[1].LinkFromAttributeName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLes[1].LinkFromEntityName);
            Assert.AreEqual(Contact.EntityLogicalName, innerLes[1].EntityAlias);
        }

        #endregion Link Entities

        #region Order

        [TestMethod]
        public void ShouldAddOrderEarlyBound()
        {
            var query = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.OrderBy(a => a.LastName));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.LastName, le.Orders.First().AttributeName);
            Assert.AreEqual(OrderType.Ascending, le.Orders.First().OrderType);

            var query2 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.OrderByDescending(a => a.LastName));

            var le2 = query2.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.LastName, le2.Orders.First().AttributeName);
            Assert.AreEqual(OrderType.Descending, le2.Orders.First().OrderType);
        }

        [TestMethod]
        public void ShouldAddOrderLateBound()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").OrderBy(Contact.Fields.LastName));

            var le = query.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.LastName, le.Orders.First().AttributeName);
            Assert.AreEqual(OrderType.Ascending, le.Orders.First().OrderType);

            var query2 = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").OrderByDescending(Contact.Fields.LastName));

            var le2 = query2.QueryExpression.LinkEntities.First();
            Assert.AreEqual(Contact.Fields.LastName, le2.Orders.First().AttributeName);
            Assert.AreEqual(OrderType.Descending, le2.Orders.First().OrderType);
        }

        #endregion Order

        #region Conditions

        [TestMethod]
        public void ShouldSetWhere()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").Where(Contact.Fields.ContactId, ConditionOperator.Above, guid));

            Assert.AreEqual(Contact.Fields.ContactId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.Where(c => c.ContactId, ConditionOperator.Above, guid));

            Assert.AreEqual(Contact.Fields.ContactId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereAbove()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereAbove(Contact.Fields.ContactId, guid));

            Assert.AreEqual(Contact.Fields.ContactId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereAbove(c => c.ContactId, guid));

            Assert.AreEqual(Contact.Fields.ContactId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereAboveOrEqual()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereAboveOrEqual(Contact.Fields.ContactId, guid));

            Assert.AreEqual(Contact.Fields.ContactId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereAboveOrEqual(c => c.ContactId, guid));

            Assert.AreEqual(Contact.Fields.ContactId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereBeginsWith()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereBeginsWith(Contact.Fields.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereBeginsWith(a => a.LastName, "test"));

            Assert.AreEqual(Contact.Fields.LastName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereBetween()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereBetween(Contact.Fields.AnnualIncome, 10, 50));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereBetween(a => a.AnnualIncome, 10, 50));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereContains()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereContains(Contact.Fields.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereContains(a => a.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereContainValues()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereContainValues(a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotBeginWith()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereDoesNotBeginWith(Contact.Fields.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereDoesNotBeginWith(a => a.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContain()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereDoesNotContain(Contact.Fields.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereDoesNotContain(c => c.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContainValues()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereDoesNotContainValues(Contact.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereDoesNotContainValues(a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotEndWith()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereDoesNotEndWith(Contact.Fields.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereDoesNotEndWith(c => c.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEndsWith()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEndsWith(Contact.Fields.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEndsWith(c => c.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEqual()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEqual(Contact.Fields.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEqual(c => c.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEqualBusinessId()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEqualBusinessId(Account.Fields.OwningBusinessUnit));

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEqualBusinessId(a => a.OwningBusinessUnit));

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserId()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEqualUserId(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEqualUserId(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserLanguage()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEqualUserLanguage("no_language_attribute"));

            Assert.AreEqual("no_language_attribute", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEqualUserLanguage(c => c.AnnualIncome));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchy()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEqualUserOrUserHierarchy(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchyAndTeams()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEqualUserOrUserHierarchyAndTeams(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserTeams()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEqualUserOrUserTeams(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEqualUserOrUserTeams(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserTeams()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereEqualUserTeams(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereEqualUserTeams(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereGreaterEqual()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereGreaterEqual(Contact.Fields.AnnualIncome, 10));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereGreaterEqual(c => c.AnnualIncome, 10));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereGreaterThan()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereGreaterThan(Contact.Fields.AnnualIncome, 10));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereGreaterThan(c => c.AnnualIncome, 10));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereIn()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereIn(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereIn(a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereInFiscalPeriod(Contact.Fields.CreatedOn, 1));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereInFiscalPeriod(a => a.CreatedOn, 1));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereInFiscalPeriodAndYear(Contact.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereInFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereInFiscalYear(Contact.Fields.CreatedOn, 2018));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(2018, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereInFiscalYear(a => a.CreatedOn, 2018));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(2018, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereInList()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values is IList);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereIn(a => a.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereInOrAfterFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereInOrAfterFiscalPeriodAndYear(Contact.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereInOrAfterFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereInOrBeforeFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereInOrBeforeFiscalPeriodAndYear(Contact.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereInOrBeforeFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereLast7Days()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLast7Days(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLast7Days(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastFiscalPeriod(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastFiscalPeriod(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastFiscalYear(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastFiscalYear(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastMonth()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastMonth(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastMonth(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastWeek()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastWeek(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastWeek(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastXDays()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastXDays(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastXDays(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalPeriods()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastXFiscalPeriods(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastXFiscalPeriods(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalYears()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastXFiscalYears(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastXFiscalYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXHours()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastXHours(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastXHours(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXMonths()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastXMonths(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastXMonths(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXWeeks()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastXWeeks(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastXWeeks(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXYears()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastXYears(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastXYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLastYear(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLastYear(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLessEqual()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLessEqual(Contact.Fields.AnnualIncome, 10));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLessEqual(c => c.AnnualIncome, 10));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLessThan()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereLessThan(Contact.Fields.AnnualIncome, 10));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLessThan(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLike()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereLike(Contact.Fields.FirstName, "%test%"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereLike(c => c.FirstName, "%test%"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereMask()
        {
            var obj = new object();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereMask(Contact.Fields.FirstName, obj));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereMask(c => c.FirstName, obj));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNext7Days()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereNext7Days(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNext7Days(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereNextFiscalPeriod(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextFiscalPeriod(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereNextFiscalYear(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextFiscalYear(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextMonth()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNextMonth(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextMonth(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextWeek()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereNextWeek(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextWeek(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextXDays()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereNextXDays(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextXDays(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalPeriods()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid").WhereNextXFiscalPeriods(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextXFiscalPeriods(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalYears()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNextXFiscalYears(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextXFiscalYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXHours()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNextXHours(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextXHours(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXMonths()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNextXMonths(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextXMonths(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXWeeks()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNextXWeeks(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextXWeeks(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXYears()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNextXYears(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextXYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNextYear(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNextYear(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotBetween()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotBetween(Contact.Fields.AnnualIncome, 10, 50));

            Assert.AreEqual(Contact.Fields.AnnualIncome, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotBetween(a => a.CreatedOn, 10, 50));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[0]);
            Assert.AreEqual(50, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqual()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotEqual(Contact.Fields.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotEqual(c => c.FirstName, "test"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualBusinessId()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit));

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotEqualBusinessId(a => a.OwningBusinessUnit));

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualUserId()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotEqualUserId(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotEqualUserId(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotIn()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotIn(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotIn(a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereNotInList()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values is IList);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotIn(a => a.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereNotLike()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotLike(Contact.Fields.FirstName, "%test%"));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotLike(c => c.FirstName, "%test%"));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual("%test%", query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotMask()
        {
            var obj = new object();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotMask(Contact.Fields.FirstName, obj));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotMask(c => c.FirstName, obj));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(obj, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotNull()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotNull(Contact.Fields.FirstName));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotNull(c => c.FirstName));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotOn()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotOn(Contact.Fields.CreatedOn, date));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotOn(a => a.CreatedOn, date));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotUnder()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNotUnder(Account.Fields.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNotUnder(a => a.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNull()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereNull(Contact.Fields.FirstName));

            Assert.AreEqual(Contact.Fields.FirstName, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereNull(c => c.FirstName));

            Assert.AreEqual(Contact.Fields.FirstName, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXDays()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOlderThanXDays(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOlderThanXDays(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXHours()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOlderThanXHours(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOlderThanXHours(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMinutes()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOlderThanXMinutes(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOlderThanXMinutes(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMonths()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOlderThanXMonths(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOlderThanXMonths(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXWeeks()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOlderThanXWeeks(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOlderThanXWeeks(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXYears()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOlderThanXYears(Contact.Fields.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOlderThanXYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOn()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOn(Contact.Fields.CreatedOn, date));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOn(a => a.CreatedOn, date));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOnOrAfter()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOnOrAfter(Contact.Fields.CreatedOn, date));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOnOrAfter(a => a.CreatedOn, date));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOnOrBefore()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereOnOrBefore(Contact.Fields.CreatedOn, date));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereOnOrBefore(a => a.CreatedOn, date));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereThisFiscalPeriod(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereThisFiscalPeriod(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereThisFiscalYear(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereThisFiscalYear(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisMonth()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereThisMonth(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereThisMonth(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisWeek()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereThisWeek(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereThisWeek(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisYear()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereThisYear(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereThisYear(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereToday()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereToday(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereToday(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereTomorrow()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereTomorrow(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereTomorrow(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereUnder()
        {
            Guid guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereUnder(Account.Fields.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereUnder(a => a.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereUnderOrEqual()
        {
            Guid guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereUnderOrEqual(Account.Fields.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereUnderOrEqual(a => a.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereYesterday()
        {
            var query = new Query(Account.EntityLogicalName).AddLink(new Link("contact", "parentcustomerid", "accountid")
                .WhereYesterday(Contact.Fields.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l.WhereYesterday(a => a.CreatedOn));

            Assert.AreEqual(Contact.Fields.CreatedOn, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.LinkEntities.First().LinkCriteria.Conditions.First().Values.Count);
        }

        #endregion Conditions
    }
}