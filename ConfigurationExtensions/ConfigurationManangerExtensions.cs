using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ConfigurationExtensions;

public static class ConfigurationManangerExtensions
{
    public static T Bind<T>(this ConfigurationManager configurationManager, string? sectionKey = null)
    {
        return (T)Bind(configurationManager, typeof(T), sectionKey)!;
    }

    private static object? Bind(this ConfigurationManager configurationManager, Type type,
        string? sectionKey, bool isNullable = false)
    {
        var keyToUse = sectionKey ?? type.Name;

        var constructors = type.GetConstructors().ToList();

        if (constructors.Count < 1)
        {
            throw new ArgumentException($"Type '{type.Name}' does not have a public constructor", nameof(type));
        }

        var query = constructors.Select(x => new
        {
            Constructor = x,
            Parameters = x.GetParameters().ToList()
        }).OrderByDescending(x => x.Parameters.Count).First();

        if (isNullable && query.Parameters.Count > 0 && !configurationManager.GetSection(keyToUse).Exists())
        {
            return null;
        }

        var parameters = new List<object?>();

        foreach (var parameter in query.Parameters)
        {
            var key = $"{keyToUse}:{parameter.Name}";

            parameters.Add(GetValue(configurationManager, key, parameter));
        }

        return query.Constructor.Invoke(parameters.ToArray());
    }

    private static object? GetValue(ConfigurationManager configurationManager, string key, ParameterInfo parameter)
    {
        var nullableType = GetNullableType(parameter);

        var type = nullableType ?? parameter.ParameterType;

        if (type.IsClass && type != typeof(string))
        {
            return Bind(configurationManager, type, key, nullableType != null);
        }

        var value = configurationManager[key];

        if (nullableType != null)
        {
            if (value == null) return null;
        }
        else
        {
            if (value == null)
            {
                throw new ConfigurationBindException($"Missing configuration key '{key}'. Unable to set {parameter.Name}.");
            }
        }

        if (type == typeof(string)) return value;

        if (type == typeof(int))
        {
            if (int.TryParse(value, out var intValue))
            {
                return intValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to an int. Source: '{key}'");
        }

        if (type == typeof(bool))
        {
            if (bool.TryParse(value, out var boolValue))
            {
                return boolValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a bool. Source: '{key}'");
        }

        if (type == typeof(decimal))
        {
            if (decimal.TryParse(value, out var decimalValue))
            {
                return decimalValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a decimal. Source: '{key}'");
        }

        if (type == typeof(DateTime))
        {
            if (DateTime.TryParse(value, out var dateTimeValue))
            {
                return dateTimeValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a datetime. Source: '{key}'");
        }

        throw new ConfigurationBindException($"Unhandled type '{type.FullName}'");
    }

    private static Type? GetNullableType(ParameterInfo parameter)
    {
        if (parameter.ParameterType.IsClass
            && new NullabilityInfoContext().Create(parameter).WriteState != NullabilityState.NotNull)
        {
            return parameter.ParameterType;
        }

        return Nullable.GetUnderlyingType(parameter.ParameterType);
    }
}
