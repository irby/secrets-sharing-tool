using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SecretsSharingTool.Core.Tests.Database;
using SecretsSharingTool.Data;

namespace SecretsSharingTool.Core.Tests.Handlers;

[Collection(DatabaseCollection.Name)]
public abstract class BaseHandlerTest
{
    private bool _hasMediatorBeenAccessed;
    private readonly IMediator _mediator;
    protected IMediator Mediator
    {
        get
        {
            _hasMediatorBeenAccessed = true;
            return _mediator;
        }
    }

    private readonly AppUnitOfWork _setupDb;
    private readonly AppUnitOfWork _cleanupDb;

    protected AppUnitOfWork Database => _hasMediatorBeenAccessed ? _cleanupDb : _setupDb;

    protected readonly IServiceCollection ServiceCollection;
    protected readonly ServiceProvider ServiceProvider;

    protected BaseHandlerTest(DatabaseFixture fixture)
    {
        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddLogging();
        ServiceCollection.AddDatabase(fixture.Configuration.BuildConnectionString(true));
        ServiceCollection.AddCoreServices();
        
        ConfigureAdditionalServices();

        ServiceProvider = ServiceCollection.BuildServiceProvider();

        _mediator = ServiceProvider.GetService<IMediator>()!;
        _setupDb = ServiceProvider.GetService<AppUnitOfWork>()!;
        _cleanupDb = ServiceProvider.GetService<AppUnitOfWork>()!;
    }

    protected virtual void ConfigureAdditionalServices() { }
}
