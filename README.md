# MscrmTools.FluentQueryExpressions
A library to create QueryExpression the fluent way

[![Build status](https://ci.appveyor.com/api/projects/status/lyre23w2ccnnyayr?svg=true)](https://ci.appveyor.com/project/MscrmTools/mscrmtools-fluentqueryexpressions) [![NuGet Badge](https://buildstats.info/nuget/MscrmTools.FluentQueryExpressions)](https://www.nuget.org/packages/MscrmTools.FluentQueryExpressions)

You can rely on :
- [EarlyBoundGenerator](https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools) from Daryl LaBar to create the Early Bound Entities to be used with this project (optional)
- [LateBoundConstantsGenerator](https://github.com/rappen/LateboundConstantsGenerator) from Jonas Rapp to create Late Bound constants to be used with this projet (optional)

## What's new on version 2

Lambda expressions everywhere for Early Bound queries

No more mix between Late Bound and Early Bound queries

Code is fully documented

Performance optimizations for `GetFirst`, `GetFirstOrDefault`, `GetSingle`, `GetSingleOrDefault`, `GetLast` and `GetLastOrDefault` methods

## Query properties

```
var earlyBoundQuery = new Query<Account>()
                .Top(10)
                .Distinct()
                .NoLock()
                .SetPaging(1, 100, true);      
                
var lateBoundQuery = new Query("account")
                .Top(10)
                .Distinct()
                .NoLock()
                .SetPaging(1, 100, true);     
                
```

## Selecting fields

```
var earlyBoundQuery = new Query<Account>()
                .Select(a => a.AccountNumber);
                
var earlyBoundQuery2 = new Query<Account>()
                .Select(a => new { a.Name, a.AccountNumber});
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber");
                
```

## Adding conditions

```
var earlyBoundQuery = new Query<Account>()
                .Select(a => new { a.Name, a.AccountNumber})
                .WhereEqual(a => a.Address1_City, "Paris");
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber")
                .WhereEqual("address1_city", "Paris");
                
```

## Adding filters

```
var earlyBoundQuery = new Query<Account>()
                .Select(a => new { a.Name, a.AccountNumber})
                .AddFilter(f => f
                    .SetLogicalOperator(LogicalOperator.Or)
                    .WhereEqual(a => a.Address1_City, "Paris")
                    .WhereEqual(a => a.Address1_City, "Nantes")
                );
                
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber")
                .AddFilter(new Filter(LogicalOperator.Or)
                    .WhereEqual("address1_city", "Paris")
                    .WhereEqual("address1_city", "Nantes")
                );
                
```

## Comparing Columns (online only, from 2020 July 1st)
```
var earlyBoundQuery = new Query<Account>()
    .Select(a => new { a.Name, a.AccountNumber})
    .Compare(a => a.CreatedOn).LessThan(a => a.ModifiedOn);

var lateBoundQuery = new Query("account")
    .Select("name", "accountnumber")
    .Compare("createdon").LessThan("modifiedon");
```

## Adding link entity

```
var earlyBoundQuery = new Query<Account>()
                .Select(a => new { a.Name, a.AccountNumber})
                .AddLink<Contact>(a => a.AccountId, c => c.ParentCustomerId, l => l
                    .SetAlias("cont")
                    .Select(c => c.Fullname)
                    .SetLogicalOperator(LogicalOperator.Or)
                    .WhereEqual(c => c.Address1_City, "Paris")
                    .WhereEqual(c => c.Address1_City, "Nantes")
                    , JoinOperator.LeftOuter)
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
                .Select(a => new { a.Name, a.AccountNumber})
                .OrderBy(a => a.Name);
                
var earlyBoundQuery2 = new Query<Account>()
                .Select(a => new { a.Name, a.AccountNumber})
                .OrderByDescending(a => a.Name);
                
                
var lateBoundQuery = new Query("account")
                .Select("name", "accountnumber")
                .OrderBy("name");
                
var lateBoundQuery2 = new Query("account")
                .Select("name", "accountnumber")
                .OrderByDescending("name");
                
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
