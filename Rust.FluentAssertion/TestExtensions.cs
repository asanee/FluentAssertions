namespace Rust.FluentAssertion
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class TestExtensions
    {
        public static AssertObject<TProperty, TNew> And<T, TProperty, TNew>(this ValidationScope<T, TProperty> validationScope, Expression<Func<TProperty, TNew>> expression)
        {
            var scope = validationScope.AssertObject.Scope;

            return new AssertObject<TProperty, TNew>(On(
                scope.GetActual(),
                expression,
                scope.ExpressionName));
        }

        public static ValidationScope<T, TProperty> Be<T, TProperty>(this AssertObject<T, TProperty> assertObject, TProperty expect)
        {
            assertObject.AreEqual(expect);

            return new ValidationScope<T, TProperty>(assertObject);
        }

        public static AssertScope<T, TProperty> On<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression, string variableName = null)
        {
            return new AssertScope<T, TProperty>(obj, expression, variableName, expression.GetSignature());
        }

        public static void IsNotNull<T, TTarget>(this AssertScope<T, TTarget> helper) where TTarget : class
        {
            helper.IsTrue(x => x != null, "Is not null");
        }

        public static void IsNotNullOrWhiteSpace<T>(this AssertScope<T, string> helper)
        {
            helper.IsTrue(
                x => !string.IsNullOrWhiteSpace(x),
                "Is not null or white space");
        }

        public static void IsTrue<T>(this AssertScope<T, bool> a)
        {
            a.AreEqual(true);
        }

        public static void HasPredicatedItem<T, TItem, TCollection>(
            this AssertScope<T, TCollection> a,
            Expression<Func<TItem, bool>> predicate)
            where TCollection : class, IEnumerable<TItem>
        {
            a.IsTrue(x => x != null && x.Any(predicate.Compile()), "HasPredicatedItem " + predicate);
        }

        public static void HasAnyItem<T, TCollection>(this AssertScope<T, TCollection> a)
            where TCollection : class, IEnumerable
        {
            a.IsTrue(x => x != null && x.OfType<object>().Any(), "HasAnyItem");
        }

        public static void Contains<T, TItem, TCollection>(this AssertScope<T, TCollection> a, TItem item)
            where TCollection : class, IEnumerable<TItem>
        {
            a.IsTrue(x => x != null && x.Contains(item), "Contains " + item);
        }

        public static void IsFalse<T>(this AssertScope<T, bool> a)
        {
            a.AreEqual(false);
        }

        public static void MoreThanZero<T>(this AssertScope<T, decimal> a)
        {
            a.IsTrue(x => x > 0, "MoreThanZero");
        }

        public static void MoreThanZero<T>(this AssertScope<T, int> a)
        {
            a.IsTrue(x => x > 0, "MoreThanZero");
        }

        public static void IsZero<T>(this AssertScope<T, int> a)
        {
            a.IsTrue(x => x == 0, "IsZero");
        }

        public static void HasNoItem<T>(this AssertScope<T, int> a)
        {
            a.IsTrue(x => x == 0, "HasNoItem");
        }

        public static void IsBetween<T>(this AssertScope<T, decimal> a, decimal minValue, decimal maxValue)
        {
            a.IsTrue(x => x > 0, string.Format("Between({0},{1})", minValue, maxValue));
        }

        /// <summary>
        /// Get property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string PropertyName<T, TPropertyType>(this Expression<Func<T, TPropertyType>> expr)
        {
            return FindPropertyName(expr);
        }

        /// <summary>
        /// Get property name
        /// </summary>
        /// <param name="lambdaExpression"></param>
        /// <returns></returns>
        public static string FindPropertyName(LambdaExpression lambdaExpression)
        {
            var property = FindProperty(lambdaExpression);
            return property == null ? string.Empty : property.Name;
        }

        /// <summary>
        /// Get property from expression
        /// </summary>
        /// <param name="lambdaExpression"></param>
        /// <returns></returns>
        public static PropertyInfo FindProperty(this LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression;

            bool done = false;

            while (!done)
            {
                switch (expressionToCheck.NodeType)
                {
                    case ExpressionType.Convert:
                        expressionToCheck = ((UnaryExpression)expressionToCheck).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expressionToCheck = lambdaExpression.Body;
                        break;
                    case ExpressionType.MemberAccess:
                        var propertyInfo = ((MemberExpression)expressionToCheck).Member as PropertyInfo;
                        return propertyInfo;
                    default:
                        done = true;
                        break;
                }
            }

            return null;
        }

        public static Signature GetSignature(this LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression;

            bool done = false;

            while (!done)
            {
                switch (expressionToCheck.NodeType)
                {
                    case ExpressionType.Convert:
                        expressionToCheck = ((UnaryExpression)expressionToCheck).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expressionToCheck = lambdaExpression.Body;
                        break;
                    case ExpressionType.Call:
                        return new Signature(SignatureType.Method, ((MethodCallExpression)expressionToCheck).Method.Name);

                    case ExpressionType.MemberAccess:
                        var propertyInfo = ((MemberExpression)expressionToCheck).Member as PropertyInfo;

                        return new Signature(SignatureType.Property, propertyInfo == null ? string.Empty : propertyInfo.Name);

                    default:
                        done = true;
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Get method info from an expression
        /// </summary>
        /// <param name="lambdaExpression"></param>
        /// <returns></returns>
        public static MethodInfo FindMethod(this LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression;

            bool done = false;

            while (!done)
            {
                switch (expressionToCheck.NodeType)
                {
                    case ExpressionType.Convert:
                        expressionToCheck = ((UnaryExpression)expressionToCheck).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expressionToCheck = lambdaExpression.Body;
                        break;
                    case ExpressionType.Call:
                        return ((MethodCallExpression)expressionToCheck).Method;
                    default:
                        done = true;
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Get member type
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            var methodInfo = memberInfo as MethodInfo;

            if (methodInfo != null)
            {
                return methodInfo.ReturnType;
            }

            var propertyInfo = memberInfo as PropertyInfo;

            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            var fieldInfo = memberInfo as FieldInfo;

            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            return null;
        }
    }
}
