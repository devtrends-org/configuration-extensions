using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DevTrends.ConfigurationExtensions.Tests;

public class ConfigurationManagerExtensionsTests
{
    [Fact]
    public void NoPublicConstructor_Throws()
    {
        var configurationManager = new ConfigurationManager();

        Assert.Throws<ArgumentException>(() => configurationManager.Bind<NoPublicConstructor>());
    }

    [Fact]
    public void RequiredString_Throws_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<RequiredString>());
    }

    [Fact]
    public void RequiredString_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Foo:Foo", "text string")
        });

        var result = configurationManager.Bind<RequiredString>("Foo");

        Assert.Equal("text string", result.Foo);
    }

    [Fact]
    public void RequiredString_Binds_Using_Inferred_Section_Name()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("RequiredString:Foo", "text string")
        });

        var result = configurationManager.Bind<RequiredString>();

        Assert.Equal("text string", result.Foo);
    }

    [Fact]
    public void OptionalString_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("OptionalString:Foo", "optional string")
        });

        var result = configurationManager.Bind<OptionalString>();

        Assert.Equal("optional string", result.Foo);
    }

    [Fact]
    public void OptionalString_Binds_Null_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        var result = configurationManager.Bind<OptionalString>();

        Assert.Null(result.Foo);
    }

    [Fact]
    public void MultipleTypes_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31"),
        });

        var result = configurationManager.Bind<MultipleTypes>();

        Assert.Equal("s1", result.Foo);
        Assert.Equal(42, result.Bar);
        Assert.True(result.Other);
        Assert.Equal(3.14m, result.Misc);
        Assert.Equal(DateTime.Parse("1999-12-31"), result.Blah);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_Int()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31"),
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Bar", ex.Message);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_Bool()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31"),
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Other", ex.Message);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_Decimal()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31"),
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Misc", ex.Message);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_DateTime()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14")
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Blah", ex.Message);
    }

    [Fact]
    public void MultipleOptionalTypes_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleOptionalTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Blah", "1999-12-31"),
        });

        var result = configurationManager.Bind<MultipleOptionalTypes>();

        Assert.Equal("s1", result.Foo);
        Assert.Equal(42, result.Bar);
        Assert.True(result.Other);
        Assert.Equal(3.14m, result.Misc);
        Assert.Equal(DateTime.Parse("1999-12-31"), result.Blah);
    }

    [Fact]
    public void MultipleOptionalTypes_Binds_Null_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        var result = configurationManager.Bind<MultipleOptionalTypes>();

        Assert.Null(result.Foo);
        Assert.Null(result.Bar);
        Assert.Null(result.Other);
        Assert.Null(result.Misc);
        Assert.Null(result.Blah);
    }

    [Fact]
    public void Nested_NoPublicConstructor_Throws()
    {
        var configurationManager = new ConfigurationManager();

        Assert.Throws<ArgumentException>(() => configurationManager.Bind<NestedNoPublicConstructor>());
    }

    [Fact]
    public void NestedRequired_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedRequired:Something", "hi"),
            new KeyValuePair<string, string>("NestedRequired:Other:Foo", "nested")
        });

        var result = configurationManager.Bind<NestedRequired>();

        Assert.Equal("hi", result.Something);
        Assert.NotNull(result.Other);
        Assert.Equal("nested", result.Other.Foo);
    }

    [Fact]
    public void NestedRequired_Throws_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedRequired:Something", "hi")
        });

        Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<NestedRequired>());
    }

    [Fact]
    public void NestedOptional_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedOptional:Something", "hi"),
            new KeyValuePair<string, string>("NestedOptional:Other:Foo", "nested")
        });

        var result = configurationManager.Bind<NestedOptional>();

        Assert.Equal("hi", result.Something);
        Assert.NotNull(result.Other);
        Assert.Equal("nested", result.Other.Foo);
    }

    [Fact]
    public void NestedOptional_Binds_Null_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedOptional:Something", "hi")
        });

        var result = configurationManager.Bind<NestedOptional>();

        Assert.Equal("hi", result.Something);
        Assert.NotNull(result.Other);
        Assert.Null(result.Other.Foo);
    }

    [Fact]
    public void NestedOptionalType_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedOptionalType:Something", "hi"),
            new KeyValuePair<string, string>("NestedOptionalType:Other:Foo", "nested")
        });

        var result = configurationManager.Bind<NestedOptionalType>();

        Assert.Equal("hi", result.Something);
        Assert.NotNull(result.Other);
        Assert.Equal("nested", result.Other!.Foo);
    }

    [Fact]
    public void NestedOptionalType_Binds_Null_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedOptionalType:Something", "hi")
        });

        var result = configurationManager.Bind<NestedOptionalType>();

        Assert.Equal("hi", result.Something);
        Assert.Null(result.Other);
    }
}

public class NoPublicConstructor
{
    private NoPublicConstructor()
    {
    }
}

public record RequiredString(string Foo);

public record OptionalString(string? Foo);

public record MultipleTypes(string Foo, int Bar, bool Other, decimal Misc, DateTime Blah);

public record MultipleOptionalTypes(string? Foo, int? Bar, bool? Other, decimal? Misc, DateTime? Blah);

public record NestedRequired(string Something, RequiredString Other);

public record NestedOptional(string Something, OptionalString Other);

public record NestedNoPublicConstructor(NoPublicConstructor Other);

public record NestedOptionalType(string Something, RequiredString? Other);