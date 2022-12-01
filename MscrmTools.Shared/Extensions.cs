using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace MscrmTools.FluentQueryExpressions
{
    /// <summary>
    /// Extensions for the Dataverse <see cref="IOrganizationService"/>
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Retrieves multiple records from a fluent query.
        /// </summary>
        /// <typeparam name="T">Strongly typed Dataverse table</typeparam>
        /// <param name="service">The Dataverse service.</param>
        /// <param name="query">The fluent query.</param>
        /// <returns>A list of <typeparamref name="T"/> records</returns>
        public static List<T> RetrieveMultiple<T>(this IOrganizationService service, Query<T> query) where T : Entity
        {
            return query.GetAll(service);
        }
    }
}