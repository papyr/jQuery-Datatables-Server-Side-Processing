﻿namespace JQDT.DataProcessing.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using JQDT.Extensions;

    internal class NullCheckExpressionBuilder
    {
        /// <summary>
        /// Builds the null check expression.
        /// </summary>
        /// <param name="modelParamExpr">The model parameter expr.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns><see cref="Expression"/></returns>
        internal Expression BuildNullCheckExpression(ParameterExpression modelParamExpr, string propertyPath)
        {
            var nullCheckExprCollection = new List<Expression>();
            var propPathCollection = propertyPath.Split('.');
            for (int i = 1; i < propPathCollection.Length + 1; i++)
            {
                var propSelectExpr = modelParamExpr.NestedProperty(string.Join(".", propPathCollection.Take(i)));
                var propertyType = propSelectExpr.Type;
                if (!propertyType.IsClass || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    // Do not add a null check if the type is class or nullable struct;
                    continue;
                }

                var nullCheckExpr = Expression.NotEqual(propSelectExpr, Expression.Constant(null));

                nullCheckExprCollection.Add(nullCheckExpr);
            }

            var joinedAndExpr = this.GetAndExpression(nullCheckExprCollection);

            return joinedAndExpr;
        }

        /// <summary>
        /// Joins the expressions inside a collection of <see cref="Expression"/> with AND clause.
        /// </summary>
        /// <param name="expressions">The contain expression collection.</param>
        /// <returns>Joined <see cref="Expression"/></returns>
        internal Expression GetAndExpression(List<Expression> expressions)
        {
            if (expressions.Count == 0)
            {
                return null;
            }

            if (expressions.Count == 1)
            {
                return expressions.First();
            }

            var numberOfExpressions = expressions.Count;
            var counter = 0;
            Expression andExpr = null;
            do
            {
                andExpr = Expression.AndAlso(andExpr ?? expressions[counter], expressions[counter + 1]);

                counter++;
            }
            while (counter < numberOfExpressions - 1);

            return andExpr;
        }
    }
}