using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace SecretsSharingTool.Core.Tests.Handlers;

public abstract class BaseValidatorTest<T> where T : class, IValidator
{
    protected T Validator { get; }
    
    protected BaseValidatorTest()
    {
        var collection = new ServiceCollection();
        collection.AddTransient<T>();
        
        var provider = collection.BuildServiceProvider();

        Validator = provider.GetService<T>()!;
    }
}