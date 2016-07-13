using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MutipleOAuth.Externsions
{
    public static class DBExternsions
    {

        public static void AddIfNoExist<TEntity>(this IDbSet<TEntity> set, Expression<Func<TEntity, object>> predicate, params TEntity[] entities) where TEntity : class
        {
            var pi = GetPropertyName(predicate);
            var parameter = Expression.Parameter(typeof(TEntity));
            foreach (var entity in entities)
            {
                var id = pi.GetValue(entity, null);
                var body = Expression.Equal(Expression.Property(parameter, pi), Expression.Constant(id));
                var found = set.SingleOrDefault(Expression.Lambda<Func<TEntity, bool>>(body, new[] { parameter }));
                if (found == null)
                {
                    set.Add(entity);
                }
            }

        }
        private static PropertyInfo GetPropertyName<T>(Expression<Func<T, object>> property)
        {
            LambdaExpression lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)(lambda.Body);
                memberExpression = (MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                throw new Exception("the expression should be UnaryExpression");
            }


            return (PropertyInfo)memberExpression.Member;
        }
    }
}
