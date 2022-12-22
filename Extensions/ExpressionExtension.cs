﻿
using Rio.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace Rio.Extensions;

public static class ExpressionExtension
{
    public static Expression<Func<T,bool>> Or<T>(this Expression<Func<T,bool>> expr1,Expression<Func<T,bool>> expr2)
    {
        var parameter=Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);
        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(Guard.NotNull(left), Guard.NotNull(right)), parameter);
    }

    public static Expression<Func<T,bool>> And<T>(this Expression<Func<T,bool>> expr1,Expression<Func<T,bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);
        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(Guard.NotNull(left), Guard.NotNull(right)), parameter);
    }

    public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, bool condition)
    {
        if (!condition)
        {
            return expr1;
        }
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);
        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(Guard.NotNull(left), Guard.NotNull(right)), parameter);
    }

    private sealed class ReplaceExpressionVisitor:ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue,Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;

            return base.Visit(node);
        }
    }

    public static MethodInfo GetMethod<T>(this Expression<T> expression)
    {
        if(expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if(!( expression is MethodCallExpression methodCallExpression))
        {
            throw new InvalidCastException("Cannot not converted to MethodCallExpression");
        }

        return methodCallExpression.Method;
    }

    public static MethodCallExpression GetMethodExpression<T>(this Expression<Action<T>> method)
    {
        if (method.Body.NodeType != ExpressionType.Call)
        {
            throw new ArgumentException("Method call expected",method.Body.ToString());
        }
        return (MethodCallExpression)method.Body;
    }

    public static string GetMemberName<TEntity, TMember>(this Expression<Func<TEntity, TMember>> memberExpression)
        => GetMemberInfo(memberExpression).Name;

    public static MemberInfo GetMemberInfo<TEntity,TMember>(this Expression<Func<TEntity,TMember>> expression)
    {
        if(expression.NodeType!=ExpressionType.Lambda)
        {
            throw new ArgumentException(string.Format(Resource.propertyExpression_must_be_lambda_expression,nameof(expression)),nameof(expression));
        }

        var lambda = (LambdaExpression)expression;

        var memberExpression = ExtractMemberExpression(lambda.Body);
        if(memberExpression==null)
        {
            throw new ArgumentException(string.Format(Resource.propertyExpression_must_be_lambda_expression, nameof(expression)), nameof(expression));
        }

        return memberExpression.Member;
    }

    public static PropertyInfo GetPropertyInfo<TEntity,TProperty>(this Expression<Func<TEntity,TProperty>> expression)
    {
        var member = GetMemberInfo(expression);
        if (null == member)
            throw new InvalidOperationException("no property found");

        if (member is PropertyInfo propertyInfo)
            return propertyInfo;

        return CacheUtil.GetTypeProperties(typeof(TEntity))
            .First(p => p.Name.Equals(member.Name));
    }

    private static MemberExpression ExtractMemberExpression(Expression expression)
    {
        if(expression.NodeType==ExpressionType.MemberAccess)
        {
            return (MemberExpression)expression;
        }

        if(expression.NodeType== ExpressionType.Convert)
        {
            var operand = ((UnaryExpression)expression).Operand;
            return ExtractMemberExpression(operand);
        }

        throw new InvalidOperationException(nameof(ExtractMemberExpression));
    }
}

