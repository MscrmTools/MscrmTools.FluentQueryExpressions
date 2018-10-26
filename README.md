# MscrmTools.FluentQueryExpressions
A library to create QueryExpression the fluent way

[![Build status](https://ci.appveyor.com/api/projects/status/lyre23w2ccnnyayr?svg=true)](https://ci.appveyor.com/project/MscrmTools/mscrmtools-fluentqueryexpressions)

You can rely on :
- [EarlyBoundGenerator](https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools) from Daryl LaBar to create the Early Bound Entities to be used with this project (optional)
- [LateBoundConstantsGenerator](https://github.com/rappen/LateboundConstantsGenerator) from Jonas Rapp to create Late Bound constants to be used with this projet (optional)

## Query properties

```
var query = new Query<Account>()
                .Top(10)
                .Distinct()
                .NoLock()
                .SetPaging(1, 100, true);                
                
```

## Selecting fields

```
var earlyBoundQuery = new Query<Account>()
                .Select(Account.Fields.Name, Account.Fields.AccountNumber);
                
var earlyBoundQuery2 = new Query<Account>()
                .Select(a => new { a.Name, a.AccountNumber});
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber");
                
```

## Adding conditions

```
var earlyBoundQuery = new Query<Account>()
                .Select(Account.Fields.Name, Account.Fields.AccountNumber)
                .WhereEqual(Account.Address1_City, "Paris");
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber")
                .WhereEqual("address1_city", "Paris");
                
```

## Adding filters

```
var earlyBoundQuery = new Query<Account>()
                .Select(Account.Fields.Name, Account.Fields.AccountNumber)
                .AddFilters(new Filter(LogicalOperator.Or)
                    .WhereEqual(Account.Address1_City, "Paris")
                    .WhereEqual(Account.Address1_City, "Nantes")
                );
                
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber")
                .AddFilters(new Filter(LogicalOperator.Or)
                    .WhereEqual("address1_city", "Paris")
                    .WhereEqual("address1_city", "Nantes")
                );
                
```

## Adding link entity

```
var earlyBoundQuery = new Query<Account>()
                .Select(Account.Fields.Name, Account.Fields.AccountNumber)
                .AddLink<Contact>(new Link(Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .SetAlias("cont")
                    .Select(Contact.Fields.Fullname)
                    .WhereEqual(Contact.Address1_City, "Paris")
                    .WhereEqual(Contact.Address1_City, "Nantes")
                );
                
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber")
                 .AddLink(new Link(Contact.EntityLogicalName, Contact.Fields.ParentCustomerId, Account.Fields.AccountId, JoinOperator.LeftOuter)
                    .SetAlias("cont")
                    .Select("fullname")
                    .WhereEqual("address1_city", "Paris")
                    .WhereEqual("address1_city", "Nantes")
                );
                
```

## Adding order

```
var earlyBoundQuery = new Query<Account>()
                .Select(Account.Fields.Name, Account.Fields.AccountNumber)
                .Order(Account.Fields.Name, OrderType.Ascending);
                
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber")
                .Order("name", OrderType.Ascending);
                
```

## Executing query

Either retrieve ```QueryExpression``` property or use one of the method to get records

```
var qe = new Query("account").QueryExpression;

List<Account> records = new Query<Account>().GetAll(iOrganizationService);
Account record = new Query<Account>().GetFirst(iOrganizationService);
Account record = new Query<Account>().GetFirstOrDefault(iOrganizationService);
Account record = new Query<Account>().GetLast(iOrganizationService);
Account record = new Query<Account>().GetLastOrDefault(iOrganizationService);
Account record = new Query<Account>().GetSingle(iOrganizationService);
Account record = new Query<Account>().GetSingleOrDefault(iOrganizationService);
EntityCollection records = new Query<Account>().GetRecords(iOrganizationService);

```
