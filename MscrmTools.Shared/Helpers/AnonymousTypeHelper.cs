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
            var newInitializer = anonymousTypeInitializer.Body as NewExpression;
            var memberInitializer = anonymousTypeInitializer.Body as MemberExpression;
            var unaryInitialiazer = anonymousTypeInitializer.Body as UnaryExpression;

            if (newInitializer?.Members == null && memberInitializer?.Member == null && unaryInitialiazer?.Operand == null)
            {
                throw new ArgumentException("lambda must return an object initializer");
            }

            if (newInitializer?.Members != null)
            {
                return newInitializer.Members.Select(GetLogicalAttributeName<T>).ToArray();
            }
            else if (memberInitializer?.Member != null)
            {
                return new string[] { GetLogicalAttributeName<T>(memberInitializer?.Member) };
            }
            else
            {
                return new string[] { GetLogicalAttributeName<T>(((MemberExpression)unaryInitialiazer?.Operand).Member) };
            }
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

            var attr = memberExp.Member.GetCustomAttribute<AttributeLogicalNameAttribute>();

            if (attr == null)
            {
                throw new ArgumentException(memberExp.Member.Name + "does not contain an AttributeLogicalNameAttribute. Unable to determine logical name");
            }

            return attr.LogicalName;
        }

        /// <summary>
        /// Normally just returns the name of the property, in lowercase.  But Id must be looked up via reflection.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetLogicalAttributeName<T>(MemberInfo property) where T : Entity

        {
            var attribute = typeof(T).GetMember(property.Name).First().GetCustomAttribute<AttributeLogicalNameAttribute>();

            if (attribute == null)
            {
                throw new ArgumentException(property.Name + " does not contain an AttributeLogicalNameAttribute.  Unable to determine logical name");
            }

            return attribute.LogicalName;
        }
    }
}