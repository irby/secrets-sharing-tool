using MediatR;
using Microsoft.Extensions.Logging;
using SecretsSharingTool.Data;

namespace SecretsSharingTool.Core.Handlers;

public abstract class BaseRequestHandler<T, TK> : IRequestHandler<T, TK> where T : IRequest<TK>
{
    protected readonly AppUnitOfWork Database;
    protected readonly ILogger Logger;
    public BaseRequestHandler(AppUnitOfWork unitOfWork, ILogger logger)
    {
        Database = unitOfWork;
        Logger = logger;
    }

    public abstract Task<TK> Handle(T request, CancellationToken cancellationToken);
}

public abstract class BaseRequestHandler<T> : IRequestHandler<T> where T : IRequest
{
    protected readonly AppUnitOfWork AppUnitOfWork;
    protected readonly ILogger Logger;
        
    protected BaseRequestHandler(AppUnitOfWork appUnitOfWork, ILogger logger)
    {
        AppUnitOfWork = appUnitOfWork;
        Logger = logger;
    }

    public abstract Task<Unit> Handle(T request, CancellationToken cancellationToken);
}
