using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace DMAP.Net.Helpers;

public static class ExpressionHelper
{
	/// <summary>
	/// Retrieves a propertyInfo from an expression accessing a property of a type.
	/// </summary>
	/// <param name="expression">Input unary or member expression.</param>
	/// <typeparam name="T">Class type</typeparam>
	/// <typeparam name="TProperty">Property type</typeparam>
	/// <returns>A PropertyInfo of the targeted Property</returns>
	/// <exception cref="InvalidExpressionException">Thrown if the exception type does not match.</exception>
	public static PropertyInfo GetPropertyFromExpression<T, TProperty>(this Expression<Func<T, TProperty>> expression)
	{
		switch (expression.Body)
		{
			case MemberExpression memberExpression:
				return (PropertyInfo)memberExpression.Member;

			case UnaryExpression unaryExpression:
			{
				var op = (MemberExpression)unaryExpression.Operand;
				return (PropertyInfo)op.Member;
			}

			default:
				throw new InvalidExpressionException(
					$"Expression is not of expected type. " +
					$" {typeof(MemberExpression)} or {typeof(UnaryExpression)} expected" +
					$", was {expression.Body.GetType()}"
				);
		}
	}
}