using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SecretSharingTool.Data.Database;

namespace SecretsSharingTool.Core.Shared
{
    public abstract class BaseRequestHandler<T, TK> : IRequestHandler<T, TK> where T : IRequest<TK>
    {
        protected readonly AppUnitOfWork AppUnitOfWork;
        protected readonly ILogger Logger;
        
        protected BaseRequestHandler(AppUnitOfWork appUnitOfWork, ILogger logger)
        {
            AppUnitOfWork = appUnitOfWork;
            Logger = logger;
        }
        
        public virtual Task<TK> Handle(T request, CancellationToken cancellationToken)
        {
            // Intentionally left blank
            throw new System.NotImplementedException();
        }
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

        public virtual Task<Unit> Handle(T request, CancellationToken cancellationToken)
        {
            // Intentionally left blank
            throw new System.NotImplementedException();
        }
    }
}