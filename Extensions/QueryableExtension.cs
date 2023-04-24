using Rio.Common.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace Rio.Common.Extensions;

public static class QueryableExtension
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T,bool>> predicate,bool condition)
    {
        return condition ? Guard.NotNull(source.Where(predicate)) : source;
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source,Expression<Func<T,bool>> predicate,Func<bool> conditionFunc)
    {
        return conditionFunc() ? Guard.NotNull(source.Where(predicate)) : source;
    }

    public static ListResultWithTotal<T> ToListResultWithTotal<T>(this IQueryable<T> source,int pageNumber,int pageSize)
    {
        if(pageNumber<=0)
        {
            pageNumber = 1;
        }
        if(pageSize<=0)
        {
            pageSize = 10;
        }
        var count=source.Count();
        if(count==0)
        {
            return ListResultWithTotal<T>.Empty;
        }

        if(pageNumber>1)
        {
            source = source.Skip((pageNumber - 1) * pageSize);
        }
        var items = source.Take(pageSize).ToArray();

        return new ListResultWithTotal<T>
        {
            TotalCount = count,
            Data = items
        };
    }
    
    public static PageListResult<T> ToPageList<T>(this IQueryable<T> source,int pageNumber,int pageSize)
    {
        if(pageNumber<=0)
        {
            pageNumber = 1;
        }
        if(pageSize<0)
        {
            pageSize = 10;
        }
        var count=source.Count();
        if(count==0)
        {
            return PageListResult<T>.Empty;
        }

        if(pageNumber>1)
        {
            source = source.Skip((pageNumber - 1) * pageSize);
        }
        var items = source.Take(pageSize).ToArray();

        return new PageListResult<T>
        {
            Data = items,
            TotalCount = count,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source,string propertyName,bool isAsc=false)
    {
        Guard.NotNull(source);
        if(string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        Type type = typeof(T);
        ParameterExpression arg = Expression.Parameter(typeof(T));
        PropertyInfo? propertyInfo= typeof(T).GetProperty(propertyName);
        if(null==propertyInfo)
        {
            throw new InvalidOperationException($"{propertyName} does not exists.");
        }

        Expression expression = Expression.Property(arg, propertyInfo);
        type = propertyInfo.PropertyType;

        var delegateType = typeof(Func<,>).MakeGenericType(type, type);
        var lambda = Expression.Lambda(delegateType, expression, arg);

        var methodName = isAsc ? "OrderBy" : "OrderByDescending";
        var result = typeof(Queryable).GetMethods().Single(
            method => method.Name == methodName
                && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2
                && method.GetParameters().Length == 2)
            .MakeGenericMethod(type, type)
            .Invoke(null, new object[] { source, lambda });

        return (IQueryable<T>)Guard.NotNull(result);
    }
}
