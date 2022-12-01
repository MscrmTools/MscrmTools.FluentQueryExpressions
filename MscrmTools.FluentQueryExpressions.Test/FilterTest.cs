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
    public class FilterTest
    {
        [TestMethod]
        [Obsolete("See ShouldAddFilterWithoutOperator instead")]
        public void ShouldAddFilter()
        {
            var query = new Query("account")
                .AddFilters(LogicalOperator.Or, new Filter(LogicalOperator.Or).AddFilters(new Filter(LogicalOperator.Or)));

            Assert.AreEqual(LogicalOperator.Or, query.QueryExpression.Criteria.FilterOperator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, query.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);

            var query2 = new Query<Account>()
             .AddFilters(new Filter<Account>(LogicalOperator.Or).AddFilters(new Filter<Account>(LogicalOperator.Or))).SetLogicalOperator(LogicalOperator.Or);

            Assert.AreEqual(LogicalOperator.Or, query2.QueryExpression.Criteria.FilterOperator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, query2.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);
        }

        [TestMethod]
        public void ShouldAddFilterWithoutOperator()
        {
            var query = new Query("account")
               .AddFilters(new Filter().AddFilters(new Filter())).SetLogicalOperator(LogicalOperator.Or);

            Assert.AreEqual(LogicalOperator.Or, query.QueryExpression.Criteria.FilterOperator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.And, query.QueryExpression.Criteria.Filters.First().FilterOperator);
            Assert.AreEqual(LogicalOperator.And, query.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);

            var query2 = new Query<Account>()
                .AddFilters(new Filter<Account>()
                    .AddFilters(new Filter<Account>()));

            Assert.AreEqual(LogicalOperator.And, query2.QueryExpression.Criteria.FilterOperator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Filters.Count);
            Assert.AreEqual(LogicalOperator.And, query2.QueryExpression.Criteria.Filters.First().FilterOperator);
            Assert.AreEqual(LogicalOperator.And, query2.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);
        }

        [TestMethod]
        public void ShouldCreateFilter()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter()
                    .AddFilters(new Filter()));

            Assert.AreEqual(LogicalOperator.And, query.QueryExpression.Criteria.Filters.First().FilterOperator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.First().Filters.Count);
            Assert.AreEqual(LogicalOperator.And, query.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);

            var query2 = new Query<Account>()
               .AddFilters(new Filter<Account>()
                   .AddFilters(new Filter<Account>()));

            Assert.AreEqual(LogicalOperator.And, query2.QueryExpression.Criteria.Filters.First().FilterOperator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Filters.First().Filters.Count);
            Assert.AreEqual(LogicalOperator.And, query2.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);

            var query3 = new Query<Account>()
               .AddFilters(new Filter<Account>()
                   .AddFilter(f => f));

            Assert.AreEqual(LogicalOperator.And, query3.QueryExpression.Criteria.Filters.First().FilterOperator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Filters.First().Filters.Count);
            Assert.AreEqual(LogicalOperator.And, query3.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);
        }

        [TestMethod]
        [Obsolete("See ShouldSetLogicalOperatorOr instead")]
        public void ShouldCreateFilterWithOr()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter()
                    .AddFilters(LogicalOperator.Or, new Filter()));

            Assert.AreEqual(LogicalOperator.Or, query.QueryExpression.Criteria.Filters.First().FilterOperator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.First().Filters.Count);
            Assert.AreEqual(LogicalOperator.And, query.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);

            var query2 = new Query<Account>()
               .AddFilters(new Filter<Account>()
                   .AddFilters(LogicalOperator.Or, new Filter<Account>()));

            Assert.AreEqual(LogicalOperator.Or, query2.QueryExpression.Criteria.Filters.First().FilterOperator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Filters.First().Filters.Count);
            Assert.AreEqual(LogicalOperator.And, query2.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);

            var query3 = new Query<Account>()
          .AddFilters(new Filter<Account>()
              .AddFilter(LogicalOperator.Or, f => f));

            Assert.AreEqual(LogicalOperator.And, query3.QueryExpression.Criteria.Filters.First().FilterOperator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Filters.First().Filters.Count);
            Assert.AreEqual(LogicalOperator.Or, query3.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);
        }

        [TestMethod]
        public void ShouldSetLogicalOperatorOr()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter(LogicalOperator.Or)
                    .AddFilters(new Filter()
                        .SetLogicalOperator(LogicalOperator.Or)));

            Assert.AreEqual(LogicalOperator.Or, query.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);

            var query2 = new Query<Account>()
               .AddFilters(new Filter<Account>(LogicalOperator.Or)
                   .AddFilters(new Filter<Account>()
                       .SetLogicalOperator(LogicalOperator.Or)));

            Assert.AreEqual(LogicalOperator.Or, query2.QueryExpression.Criteria.Filters.First().Filters.First().FilterOperator);
        }

        #region Compare

        [TestMethod]
        public void ShouldCompareWhereEqual()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.Fields.NumberOfEmployees).Equal(Account.Fields.Revenue));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).Equal(Account.Fields.Revenue));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);

            var query3 = new Query<Account>()
               .AddFilters(new Filter<Account>().Compare(a => a.NumberOfEmployees).Equal(a => a.Revenue));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query3.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);

            var query4 = new Query<Account>()
                .AddFilters(new Filter<Account>().Compare(Account.EntityLogicalName, a => a.NumberOfEmployees).Equal(a => a.Revenue));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query4.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterOrEqualThan()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.Fields.NumberOfEmployees).GreaterOrEqualThan(Account.Fields.Revenue));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).GreaterOrEqualThan(Account.Fields.Revenue));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereGreaterThan()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.Fields.NumberOfEmployees).GreaterThan(Account.Fields.Revenue));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).GreaterThan(Account.Fields.Revenue));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessOrEqualThan()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.Fields.NumberOfEmployees).LessOrEqualThan(Account.Fields.Revenue));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).LessOrEqualThan(Account.Fields.Revenue));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereLessThan()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.Fields.NumberOfEmployees).LessThan(Account.Fields.Revenue));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).LessThan(Account.Fields.Revenue));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);
        }

        [TestMethod]
        public void ShouldCompareWhereNotEqual()
        {
            var query = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.Fields.NumberOfEmployees).NotEqual(Account.Fields.Revenue));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);

            var query2 = new Query(Account.EntityLogicalName)
                .AddFilters(new Filter().Compare(Account.EntityLogicalName, Account.Fields.NumberOfEmployees).NotEqual(Account.Fields.Revenue));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(Account.Fields.Revenue, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
            Assert.AreEqual(true, query2.QueryExpression.Criteria.Filters.First().Conditions.First().CompareColumns);
        }

        #endregion Compare

        #region Conditions

        [TestMethod]
        public void ShouldSetWhere()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().Where(Account.Fields.AccountId, ConditionOperator.Above, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().Where(Account.EntityLogicalName, Account.Fields.AccountId, ConditionOperator.Above, guid));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().Where(a => a.AccountId, ConditionOperator.Above, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().Where<Contact>(Contact.EntityLogicalName, a => a.FirstName, ConditionOperator.Equal, "Tanguy"));

            Assert.AreEqual(Contact.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Contact.Fields.FirstName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("Tanguy", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereAbove()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereAbove(Account.Fields.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereAbove(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereAbove(a => a.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereAbove<Account>(Account.EntityLogicalName, a => a.AccountId, guid));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Above, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereAboveOrEqual()
        {
            var guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereAboveOrEqual(Account.Fields.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereAboveOrEqual(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereAboveOrEqual(a => a.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereAboveOrEqual<Account>(Account.EntityLogicalName, a => a.AccountId, guid));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.AboveOrEqual, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereBeginsWith()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereBeginsWith(Account.Fields.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereBeginsWith(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereBeginsWith(a => a.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereBeginsWith<Account>(Account.EntityLogicalName, a => a.Name, "test"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.BeginsWith, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereBetween()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereBetween(Account.Fields.NumberOfEmployees, 10, 50));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(50, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereBetween(Account.Fields.NumberOfEmployees, 10, 50, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(50, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereBetween(a => a.NumberOfEmployees, 10, 50));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(50, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereBetween<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10, 50));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Between, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(50, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereContains()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereContains(Account.Fields.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereContains(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereContains(a => a.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereContains<Account>(Account.EntityLogicalName, a => a.Name, "test"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Contains, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereContainValues()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereContainValues(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereContainValues(a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereContainValues<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ContainValues, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotBeginWith()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereDoesNotBeginWith(Account.Fields.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereDoesNotBeginWith(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereDoesNotBeginWith(a => a.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereDoesNotBeginWith<Account>(Account.EntityLogicalName, a => a.Name, "test"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotBeginWith, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContain()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereDoesNotContain(Account.Fields.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereDoesNotContain(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereDoesNotContain(a => a.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereDoesNotContain<Account>(Account.EntityLogicalName, a => a.Name, "test"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContain, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotContainValues()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereDoesNotContainValues(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereDoesNotContainValues(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereDoesNotContainValues(a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereDoesNotContainValues<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotContainValues, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereDoesNotEndWith()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereDoesNotEndWith(Account.Fields.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereDoesNotEndWith(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereDoesNotEndWith(a => a.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereDoesNotEndWith<Account>(Account.EntityLogicalName, a => a.Name, "test"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.DoesNotEndWith, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEndsWith()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEndsWith(Account.Fields.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEndsWith(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEndsWith(a => a.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEndsWith<Account>(Account.EntityLogicalName, a => a.Name, "test"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EndsWith, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEqual()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqual(Account.Fields.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqual(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqual(a => a.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqual<Account>(Account.EntityLogicalName, a => a.Name, "test"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Equal, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereEqualBusinessId()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualBusinessId(Account.Fields.OwningBusinessUnit));

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualBusinessId(Account.Fields.OwningBusinessUnit, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualBusinessId(a => a.OwningBusinessUnit));

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualBusinessId<Account>(Account.EntityLogicalName, a => a.OwningBusinessUnit));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserId()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserId(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserId(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserId(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserId<Account>(Account.EntityLogicalName, a => a.OwnerId));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserLanguage()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserLanguage("no_language_attribute"));

            Assert.AreEqual("no_language_attribute", query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserLanguage("no_language_attribute", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual("no_language_attribute", query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserLanguage(a => a.NumberOfEmployees));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserLanguage<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserLanguage, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchy()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserOrUserHierarchy(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserOrUserHierarchy(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserOrUserHierarchy<Account>(Account.EntityLogicalName, a => a.OwnerId));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchy, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserHierarchyAndTeams()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserOrUserHierarchyAndTeams(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserOrUserHierarchyAndTeams(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserOrUserHierarchyAndTeams<Account>(Account.EntityLogicalName, a => a.OwnerId));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserHierarchyAndTeams, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserOrUserTeams()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserOrUserTeams(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserOrUserTeams(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserOrUserTeams(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserOrUserTeams<Account>(Account.EntityLogicalName, a => a.OwnerId));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserOrUserTeams, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereEqualUserTeams()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserTeams(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereEqualUserTeams(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserTeams(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereEqualUserTeams<Account>(Account.EntityLogicalName, a => a.OwnerId));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.EqualUserTeams, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereGreaterEqual()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereGreaterEqual(Account.Fields.NumberOfEmployees, 10));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereGreaterEqual(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereGreaterEqual(a => a.NumberOfEmployees, 10));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereGreaterEqual<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterEqual, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereGreaterThan()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereGreaterThan(Account.Fields.NumberOfEmployees, 10));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereGreaterThan(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereGreaterThan(a => a.NumberOfEmployees, 10));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereGreaterThan<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Single());
        }

        [TestMethod]
        public void ShouldSetWhereIn()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereIn(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereIn(a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereIn(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInFiscalPeriod(Account.Fields.CreatedOn, 1));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInFiscalPeriod(Account.Fields.CreatedOn, 1, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereInFiscalPeriod(a => a.CreatedOn, 1));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereInFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriod, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereInFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereInFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalPeriodAndYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereInFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInFiscalYear(Account.Fields.CreatedOn, 2018));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(2018, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInFiscalYear(Account.Fields.CreatedOn, 2018, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(2018, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereInFiscalYear(a => a.CreatedOn, 2018));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(2018, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereInFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 2018));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InFiscalYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(2018, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereInList()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values is IList);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values is IList);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereIn(a => a.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values is IList);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereIn<Account>(Account.EntityLogicalName, a => a.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.In, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereInOrAfterFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInOrAfterFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInOrAfterFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereInOrAfterFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereInOrAfterFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrAfterFiscalPeriodAndYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereInOrBeforeFiscalPeriodAndYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInOrBeforeFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereInOrBeforeFiscalPeriodAndYear(Account.Fields.CreatedOn, 1, 2018, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereInOrBeforeFiscalPeriodAndYear(a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereInOrBeforeFiscalPeriodAndYear<Account>(Account.EntityLogicalName, a => a.CreatedOn, 1, 2018));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.InOrBeforeFiscalPeriodAndYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(1, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(2018, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereLast7Days()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLast7Days(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLast7Days(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLast7Days(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLast7Days<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Last7Days, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastFiscalPeriod(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastFiscalPeriod(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalPeriod, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastFiscalYear(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastFiscalYear(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastFiscalYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastMonth()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastMonth(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastMonth(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastMonth(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastMonth, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastWeek()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastWeek(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastWeek(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastWeek(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastWeek, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLastXDays()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXDays(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXDays(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXDays, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalPeriods()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXFiscalPeriods(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXFiscalPeriods(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXFiscalPeriods(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXFiscalPeriods<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalPeriods, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXFiscalYears()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXFiscalYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXFiscalYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXFiscalYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXFiscalYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXFiscalYears, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXHours()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXHours(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXHours(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXHours, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXMonths()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXMonths(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXMonths(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXMonths, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXWeeks()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXWeeks(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXWeeks(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXWeeks, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastXYears()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastXYears, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLastYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastYear(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLastYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastYear(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLastYear<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LastYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereLessEqual()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLessEqual(Account.Fields.NumberOfEmployees, 10));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLessEqual(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLessEqual(a => a.NumberOfEmployees, 10));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLessEqual<Account>(Account.EntityLogicalName, a => a.NumberOfEmployees, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessEqual, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLessThan()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLessThan(Account.Fields.NumberOfEmployees, 10));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLessThan(Account.Fields.NumberOfEmployees, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLessThan(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLessThan<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereLike()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLike(Account.Fields.Name, "%test%"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("%test%", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereLike(Account.Fields.Name, "%test%", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("%test%", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereLike(a => a.Name, "%test%"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("%test%", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereLike<Account>(Account.EntityLogicalName, a => a.Name, "%test%"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Like, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("%test%", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereMask()
        {
            var obj = new object();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereMask(Account.Fields.Name, obj));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(obj, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereMask(Account.Fields.Name, obj, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(obj, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereMask(a => a.Name, obj));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(obj, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereMask<Account>(Account.EntityLogicalName, a => a.Name, obj));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Mask, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(obj, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNext7Days()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNext7Days(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNext7Days(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNext7Days(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNext7Days<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Next7Days, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextFiscalPeriod(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextFiscalPeriod(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalPeriod, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextFiscalYear(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextFiscalYear(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextFiscalYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextMonth()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextMonth(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextMonth(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextMonth(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextMonth, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextWeek()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextWeek(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextWeek(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextWeek(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextWeek, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNextXDays()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXDays(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXDays(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXDays, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalPeriods()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXFiscalPeriods(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXFiscalPeriods(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXFiscalPeriods(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXFiscalPeriods<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalPeriods, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXFiscalYears()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXFiscalYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXFiscalYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXFiscalYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXFiscalYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXFiscalYears, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXHours()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXHours(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXHours(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXHours, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXMonths()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXMonths(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXMonths(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXMonths, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXWeeks()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXWeeks(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXWeeks(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXWeeks, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextXYears()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextXYears, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNextYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextYear(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNextYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextYear(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNextYear<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NextYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotBetween()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotBetween(Account.Fields.NumberOfEmployees, 10, 50));

            Assert.AreEqual(Account.Fields.NumberOfEmployees, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(50, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotBetween(Account.Fields.NumberOfEmployees, 10, 50, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.NumberOfEmployees, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(50, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotBetween(a => a.CreatedOn, 10, 50));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(50, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotBetween<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10, 50));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotBetween, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[0]);
            Assert.AreEqual(50, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values[1]);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqual()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotEqual(Account.Fields.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotEqual(Account.Fields.Name, "test", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotEqual(a => a.Name, "test"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotEqual<Account>(Account.EntityLogicalName, a => a.Name, "test"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqual, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("test", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualBusinessId()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit));

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotEqualBusinessId(Account.Fields.OwningBusinessUnit, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotEqualBusinessId(a => a.OwningBusinessUnit));

            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotEqualBusinessId<Account>(Account.EntityLogicalName, a => a.OwningBusinessUnit));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwningBusinessUnit, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualBusinessId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotEqualUserId()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotEqualUserId(Account.Fields.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotEqualUserId(Account.Fields.OwnerId, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotEqualUserId(a => a.OwnerId));

            Assert.AreEqual(Account.Fields.OwnerId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotEqualUserId<Account>(Account.EntityLogicalName, a => a.OwnerId));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.OwnerId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotEqualUserId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotIn()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotIn(Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotIn(a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotIn(Account.EntityLogicalName, a => a.CustomerTypeCode, 1, 2, 3));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(1));
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(2));
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Contains(3));
        }

        [TestMethod]
        public void ShouldSetWhereNotInList()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotIn(Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values is IList);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotIn(Account.EntityLogicalName, Account.Fields.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query.QueryExpression.Criteria.Filters.First().Conditions.First().Values is IList);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotIn(a => a.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.Fields.CustomerTypeCode, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values is IList);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotIn(Account.EntityLogicalName, a => a.CustomerTypeCode, new List<int> { 1, 2, 3 }));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CustomerTypeCode, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotIn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.IsTrue(query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values is IList);
        }

        [TestMethod]
        public void ShouldSetWhereNotLike()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotLike(Account.Fields.Name, "%test%"));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("%test%", query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotLike(Account.Fields.Name, "%test%", Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("%test%", query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotLike(a => a.Name, "%test%"));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("%test%", query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotLike<Account>(Account.EntityLogicalName, a => a.Name, "%test%"));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotLike, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual("%test%", query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotMask()
        {
            var obj = new object();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotMask(Account.Fields.Name, obj));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(obj, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotMask(Account.Fields.Name, obj, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(obj, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotMask(a => a.Name, obj));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(obj, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotMask<Account>(Account.EntityLogicalName, a => a.Name, obj));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotMask, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(obj, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotNull()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotNull(Account.Fields.Name));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotNull(Account.Fields.Name, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotNull(a => a.Name));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotNull<Account>(Account.EntityLogicalName, a => a.Name));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotNull, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereNotOn()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotOn(Account.Fields.CreatedOn, date));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotOn(Account.Fields.CreatedOn, date, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotOn(a => a.CreatedOn, date));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotOn<Account>(Account.EntityLogicalName, a => a.CreatedOn, date));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNotUnder()
        {
            var guid = Guid.Empty;
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotUnder(Account.Fields.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNotUnder(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotUnder(a => a.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNotUnder<Account>(Account.EntityLogicalName, a => a.AccountId, guid));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.NotUnder, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereNull()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNull(Account.Fields.Name));

            Assert.AreEqual(Account.Fields.Name, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereNull(Account.Fields.Name, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereNull(a => a.Name));

            Assert.AreEqual(Account.Fields.Name, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereNull<Account>(Account.EntityLogicalName, a => a.Name));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.Name, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Null, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXDays()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXDays(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXDays(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXDays(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXDays<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXDays, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXHours()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXHours(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXHours(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXHours(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXHours<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXHours, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMinutes()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXMinutes(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXMinutes(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXMinutes(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXMinutes<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMinutes, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXMonths()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXMonths(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXMonths(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXMonths(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXMonths<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXMonths, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXWeeks()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXWeeks(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXWeeks(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXWeeks(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXWeeks<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXWeeks, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOlderThanXYears()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXYears(Account.Fields.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOlderThanXYears(Account.Fields.CreatedOn, 10, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXYears(a => a.CreatedOn, 10));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOlderThanXYears<Account>(Account.EntityLogicalName, a => a.CreatedOn, 10));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OlderThanXYears, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(10, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOn()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOn(Account.Fields.CreatedOn, date));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOn(Account.Fields.CreatedOn, date, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOn(a => a.CreatedOn, date));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOn<Account>(Account.EntityLogicalName, a => a.CreatedOn, date));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.On, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOnOrAfter()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOnOrAfter(Account.Fields.CreatedOn, date));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOnOrAfter(Account.Fields.CreatedOn, date, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOnOrAfter(a => a.CreatedOn, date));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOnOrAfter<Account>(Account.EntityLogicalName, a => a.CreatedOn, date));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrAfter, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereOnOrBefore()
        {
            var date = new DateTime();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOnOrBefore(Account.Fields.CreatedOn, date));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereOnOrBefore(Account.Fields.CreatedOn, date, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereOnOrBefore(a => a.CreatedOn, date));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereOnOrBefore<Account>(Account.EntityLogicalName, a => a.CreatedOn, date));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.OnOrBefore, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(date, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalPeriod()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisFiscalPeriod(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisFiscalPeriod(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisFiscalPeriod(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisFiscalPeriod<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalPeriod, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisFiscalYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisFiscalYear(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisFiscalYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisFiscalYear(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisFiscalYear<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisFiscalYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisMonth()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisMonth(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisMonth(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisMonth(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisMonth<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisMonth, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisWeek()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisWeek(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisWeek(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisWeek(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisWeek<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisWeek, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereThisYear()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisYear(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereThisYear(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisYear(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereThisYear<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.ThisYear, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereToday()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereToday(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereToday(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereToday(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereToday<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Today, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereTomorrow()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereTomorrow(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereTomorrow(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereTomorrow(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereTomorrow<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Tomorrow, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        [TestMethod]
        public void ShouldSetWhereUnder()
        {
            Guid guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereUnder(Account.Fields.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereUnder(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereUnder(a => a.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereUnder<Account>(Account.EntityLogicalName, a => a.AccountId, guid));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Under, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereUnderOrEqual()
        {
            Guid guid = Guid.NewGuid();
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereUnderOrEqual(Account.Fields.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereUnderOrEqual(Account.Fields.AccountId, guid, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereUnderOrEqual(a => a.AccountId, guid));

            Assert.AreEqual(Account.Fields.AccountId, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereUnderOrEqual<Account>(Account.EntityLogicalName, a => a.AccountId, guid));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.AccountId, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.UnderOrEqual, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(guid, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.First());
        }

        [TestMethod]
        public void ShouldSetWhereYesterday()
        {
            var query = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereYesterday(Account.Fields.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query2 = new Query(Account.EntityLogicalName).AddFilters(new Filter().WhereYesterday(Account.Fields.CreatedOn, Account.EntityLogicalName));

            Assert.AreEqual(Account.EntityLogicalName, query2.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query2.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query2.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query3 = new Query<Account>().AddFilters(new Filter<Account>().WhereYesterday(a => a.CreatedOn));

            Assert.AreEqual(Account.Fields.CreatedOn, query3.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query3.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);

            var query4 = new Query<Account>().AddFilters(new Filter<Account>().WhereYesterday<Account>(Account.EntityLogicalName, a => a.CreatedOn));

            Assert.AreEqual(Account.EntityLogicalName, query4.QueryExpression.Criteria.Filters.First().Conditions.First().EntityName);
            Assert.AreEqual(Account.Fields.CreatedOn, query4.QueryExpression.Criteria.Filters.First().Conditions.First().AttributeName);
            Assert.AreEqual(ConditionOperator.Yesterday, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Operator);
            Assert.AreEqual(0, query4.QueryExpression.Criteria.Filters.First().Conditions.First().Values.Count);
        }

        #endregion Conditions
    }
}