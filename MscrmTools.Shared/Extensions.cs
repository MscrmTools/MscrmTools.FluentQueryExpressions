using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace MscrmTools.FluentQueryExpressions
{
    public static class Extensions
    {
        public static List<T> RetrieveMultiple<T>(this IOrganizationService service, Query<T> query) where T : Entity
        {
            return query.GetAll(service);
        }
    }
}