using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace eQuiz.Repositories.Extentions
{
    public static class QueriableExtentions
    {

        private static readonly Type ENUMERABLE_TYPE = typeof(Enumerable);
        private const string SINGLE_METHOD_NAME = "Single";

        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> sequence, string path) where TEntity: class
        {
            DbQuery<TEntity> objectQuery = sequence as DbQuery<TEntity>;
            if (objectQuery != null)
            {
                return objectQuery.Include(path);
            }
            return sequence;
        }

        public static DbQuery<TSource> Include<TSource, TResult>(this IQueryable<TSource> sequence, Expression<Func<TSource, TResult>> path) where TSource: class
        {
            DbQuery<TSource> query = sequence as DbQuery<TSource>;
            if (query == null)
            {
                throw new ArgumentNullException("Query can not be null");
            }

            var properties = new List<string>();
            Action<string> add = (str) => properties.Insert(0, str);
            var expression = path.Body;

            do
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var member = (MemberExpression)expression;
                        if (member.Member.MemberType != MemberTypes.Property)
                        {
                            throw new ArgumentException("The selected member must be a property.", "path");
                        }
                        add(member.Member.Name);
                        expression = member.Expression;
                        break;
                    case ExpressionType.Call:
                        var method = (MethodCallExpression)expression;
                        if (method.Method.Name != SINGLE_METHOD_NAME || method.Method.DeclaringType != ENUMERABLE_TYPE)
                        {
                            throw new ArgumentException(
                                    string.Format("Method '{0}' is not supported, only method '{1}' is supported to singularize navigation properties.",
                                        string.Join(Type.Delimiter.ToString(), method.Method.DeclaringType.FullName, method.Method.Name),
                                        string.Join(Type.Delimiter.ToString(), ENUMERABLE_TYPE.FullName, SINGLE_METHOD_NAME)),
                                    "path");
                        }
                        expression = (MemberExpression)method.Arguments.Single();
                        break;
                    default:
                        throw new ArgumentException("The property selector expression has an incorrect format.", "path");
                }

            } while (expression.NodeType != ExpressionType.Parameter);

            return query.Include(string.Join(Type.Delimiter.ToString(), properties));
        }

    }
}