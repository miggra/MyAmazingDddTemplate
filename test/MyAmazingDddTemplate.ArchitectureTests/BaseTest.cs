using MyAmazingDddTemplate.Domain;
using MyAmazingDddTemplate.Application;
using MyAmazingDddTemplate.Infrastructure;
using System.Reflection;

namespace MyAmazingDddTemplate.ArchitectureTests;
public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(SomeDomainClass).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(SomeApplicationClass).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(SomeInfrastructureClass).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}