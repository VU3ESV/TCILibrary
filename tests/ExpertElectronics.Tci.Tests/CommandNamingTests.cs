using System;
using System.Linq;
using System.Reflection;
using ExpertElectronics.Tci.Interfaces;
using Xunit;

namespace ExpertElectronics.Tci.Tests;

/// <summary>
/// Verifies that every TCI command class advertises a unique, non-empty wire name.
/// This guards the reflection-driven dispatcher in <see cref="TransceiverController"/>.
/// </summary>
public class CommandNamingTests
{
    [Fact]
    public void Every_ITciCommand_implementation_has_a_unique_lowercase_name()
    {
        var commandTypes = typeof(ITciCommand).Assembly
            .GetTypes()
            .Where(t => typeof(ITciCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        Assert.NotEmpty(commandTypes);

        var seen = new System.Collections.Generic.Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (var t in commandTypes)
        {
            var nameProperty = t.GetProperty("Name", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(nameProperty);
            var name = (string)nameProperty.GetValue(null);
            Assert.False(string.IsNullOrWhiteSpace(name), $"{t.FullName} has empty Name");
            Assert.Equal(name, name.ToLowerInvariant());
            Assert.False(seen.ContainsKey(name), $"Duplicate command name '{name}' on {t.FullName} and {(seen.ContainsKey(name) ? seen[name].FullName : "")}");
            seen[name] = t;
        }
    }

    [Fact]
    public void Every_ITciCommand_implementation_has_a_static_Create_factory()
    {
        var commandTypes = typeof(ITciCommand).Assembly
            .GetTypes()
            .Where(t => typeof(ITciCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        foreach (var t in commandTypes)
        {
            var create = t.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(create);
            var parameters = create.GetParameters();
            Assert.Single(parameters);
            Assert.Equal(typeof(ITransceiverController), parameters[0].ParameterType);
        }
    }
}
