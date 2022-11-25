using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MscrmTools.FluentQueryExpressions.Helpers
{
    [ExcludeFromCodeCoverage]
    internal class AnonymousTypeHelper
    {
        /// <summary>
        /// Creates an array of attribute names array from an Anonymous Type Initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">The anonymous type initializer.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">lambda must return an object initializer</exception>
        public static string[] GetAttributeNamesArray<T>(Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity

        {
            var initializer = anonymousTypeInitializer.Body as NewExpression;

            if (initializer?.Members == null)

            {
                throw new ArgumentException("lambda must return an object initializer");
            }

            // Search for and replace any occurence of Id with the actual Entity's Id

            return initializer.Members.Select(GetLogicalAttributeName<T>).ToArray();
        }

        public static string GetAttributeName<T>(Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity

        {
            MemberExpression memberExp = null;
            if (anonymousTypeInitializer.Body is MemberExpression me)
            {
                memberExp = me;
            }
            else if (anonymousTypeInitializer.Body is UnaryExpression ue)
            {
                memberExp = (MemberExpression)ue.Operand;
            }

            if (memberExp?.Member == null)

            {
                throw new ArgumentException("lambda must return an object initializer");
            }

            // Search for and replace any occurence of Id with the actual Entity's Id

            return memberExp.Member.Name.ToLower();
        }

        /// <summary>
        /// Normally just returns the name of the property, in lowercase.  But Id must be looked up via reflection.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetLogicalAttributeName<T>(MemberInfo property) where T : Entity

        {
            var name = property.Name.ToLower();
            if (name == "id")
            {
                var attribute = typeof(T).GetProperty("Id")?.GetCustomAttributes<AttributeLogicalNameAttribute>().FirstOrDefault();

                if (attribute == null)
                {
                    throw new ArgumentException(property.Name + " does not contain an AttributeLogicalNameAttribute.  Unable to determine id");
                }

                name = attribute.LogicalName;
            }

            return name;
        }
    }
}