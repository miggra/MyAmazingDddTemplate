using FluentAssertions;
using Mono.Cecil;
using NetArchTest.Rules;

namespace MyAmazingDddTemplate.ArchitectureTests;

public class EntityRulesTests : BaseTest
{
    private readonly PredicateList _entities;
    public EntityRulesTests()
    {
        _entities = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Domain.Abstractions.Entity<>));
    }

    [Fact]
    public void Entities_ShouldNotHavePublicSetters()
    {
        // Arrange & Act
        var result = _entities.Should()
            .MeetCustomRule(new NoPublicSettersRule())
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            because: "Entity properties must use 'private set' to maintain encapsulation.\n" +
            $"Violations found in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Entities_ShouldNotHavePublicFields()
    {
        // Arrange & Act
        var result = _entities.Should()
            .MeetCustomRule(new NoPublicFieldsRule())
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            because: "Entities should not expose public fields. Use properties instead.\n" +
            $"Violations found in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Entities_ShouldHavePrivateCollectionFields()
    {
        // Arrange & Act
        var result = _entities.Should()
            .MeetCustomRule(new PrivateCollectionFieldsRule())
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            because: "Entity collection fields must be private to prevent external modification.\n" +
            $"Violations found in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Entities_ShouldExposeCollectionsAsReadOnly()
    {
        // Arrange & Act
        var result = _entities.Should()
            .MeetCustomRule(new ReadOnlyCollectionPropertiesRule())
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            because: "Entity collections should be exposed as IReadOnlyCollection<T> or ReadOnlyCollection<T>.\n" +
            $"Violations found in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Entities_ShouldBeInDomainNamespace()
    {
        // Arrange & Act
        var result = _entities.Should()
            .ResideInNamespace("MyAmazingDddTemplate.Domain")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            because: "All entities should be in the Domain namespace.\n" +
            $"Violations found in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
}

/// <summary>
/// Кастомное правило для проверки отсутствия публичных сеттеров
/// </summary>
public class NoPublicSettersRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        // Проверяем все свойства типа
        var publicSetters = type.Properties
            .Where(p => p.SetMethod != null) // У свойства есть setter
            .Where(p => p.SetMethod.IsPublic) // Setter публичный
            .ToList();

        // Правило соблюдено, если нет публичных сеттеров
        return !publicSetters.Any();
    }
}

internal class NoPublicFieldsRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        var publicFirlds = type.Fields
            .Where(f => f.IsPublic)
            .ToList();

        return !publicFirlds.Any();
    }
}

/// <summary>
/// Правило для проверки, что поля коллекций приватные
/// </summary>
public class PrivateCollectionFieldsRule : ICustomRule
{
    private static readonly string[] CollectionTypes = 
    { 
        "List`1", "IList`1", "Collection`1", 
        "ICollection`1", "HashSet`1", "Dictionary`2",
        "IDictionary`2", "ISet`1"
    };

    public bool MeetsRule(TypeDefinition type)
    {
        // Находим все поля, которые являются коллекциями
        var collectionFields = type.Fields
            .Where(f => IsCollectionType(f.FieldType))
            .ToList();

        // Проверяем, что все коллекции приватные
        var nonPrivateCollections = collectionFields
            .Where(f => !f.IsPrivate)
            .ToList();

        return !nonPrivateCollections.Any();
    }

    private bool IsCollectionType(TypeReference fieldType)
    {
        var typeName = fieldType.Name;
        return CollectionTypes.Any(ct => typeName.Contains(ct));
    }
}

/// <summary>
/// Правило для проверки, что коллекции экспортируются как readonly
/// </summary>
public class ReadOnlyCollectionPropertiesRule : ICustomRule
{
    private static readonly string[] WritableCollectionTypes = 
    { 
        "List`1", "IList`1", "Collection`1", 
        "ICollection`1", "HashSet`1", "Dictionary`2",
        "IDictionary`2", "ISet`1"
    };

    private static readonly string[] ReadOnlyCollectionTypes = 
    { 
        "IReadOnlyCollection`1", "IReadOnlyList`1",
        "ReadOnlyCollection`1", "IReadOnlyDictionary`2",
        "IEnumerable`1"
    };

    public bool MeetsRule(TypeDefinition type)
    {
        // Находим все публичные свойства, возвращающие коллекции
        var collectionProperties = type.Properties
            .Where(p => p.GetMethod != null && p.GetMethod.IsPublic)
            .Where(p => IsCollectionType(p.PropertyType))
            .ToList();

        // Проверяем, что все публичные коллекции - readonly
        var writableCollections = collectionProperties
            .Where(p => !IsReadOnlyCollectionType(p.PropertyType))
            .ToList();

        return !writableCollections.Any();
    }

    private bool IsCollectionType(TypeReference propertyType)
    {
        var typeName = propertyType.Name;
        return WritableCollectionTypes.Any(ct => typeName.Contains(ct)) ||
               ReadOnlyCollectionTypes.Any(ct => typeName.Contains(ct));
    }

    private bool IsReadOnlyCollectionType(TypeReference propertyType)
    {
        var typeName = propertyType.Name;
        return ReadOnlyCollectionTypes.Any(ct => typeName.Contains(ct));
    }
}
