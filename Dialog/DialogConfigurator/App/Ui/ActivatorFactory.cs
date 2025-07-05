using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DialogConfigurator.App.Ui;

public static class ActivatorFactory {
    private static readonly Dictionary<Type, Delegate> _cache = [];

    public static T CreateWithId<T>(string id) {
        Type type = typeof(T);

        if (!_cache.TryGetValue(type, out Delegate del)) {

            // Get constructor with a single string parameter
            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(string) })
                ?? throw new InvalidOperationException($"{type} must have a constructor with a single string parameter.");

            // Build: (string id) => new T(id)
            ParameterExpression param = Expression.Parameter(typeof(string), "id");
            NewExpression newExpr = Expression.New(ctor, param);
            Func<string, T> lambda = Expression.Lambda<Func<string, T>>(newExpr, param).Compile();

            _cache[type] = lambda;
            del = lambda;
        }

        return ((Func<string, T>)del)(id);
    }
}
