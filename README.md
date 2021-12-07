# DevTrends.ConfigurationExtensions

This package provides a couple of extension methods that allow you to bind configuration to records and classes with parameterized (non parameterless) constructors.

It also works nicely with nullable types and nullable reference types (NRTs) and should help you avoid some of those annoying 'possible null reference' warnings.

The package works on the 'fail fast' principal and any missing non-nullable configuration values will result in an exception at startup.

Note that the package extends the built-in configuration so it will pick up settings from all the usual sources such as appsettings, environment variables etc.

## Installation

    dotnet add package DevTrends.ConfigurationExtensions

## Usage

Given a record:

    public record Foo(string Bar, int Blah)
    
If you have the following in your appsettings:

    "Foo": {
      "Bar": "hello",
      "Blah": 42
    }

You can bind the record to the settings by using:

    builder.Configuration.Bind<Foo>()
    
You will also need to add the following namespace to program.cs:

    using DevTrends.ConfigurationExtensions;
    
## Dependency Injection

You will probably want to register the strongly typed configuration classes with the DI container so instead of using the above, you can use:

    builder.Services.AddSingleton(builder.Configuration.Bind<Foo>());
    
## What is supported?

You can bind to properties of the following type (plus nullable version including nullable reference types such as string?):

* string
* int
* bool
* decimal
* datetime
* nested classes

Your classes or records must have a public constructor which sets all properties that you wish to automatically populate.

If you have any option configuration values, be sure to use nullable types.
