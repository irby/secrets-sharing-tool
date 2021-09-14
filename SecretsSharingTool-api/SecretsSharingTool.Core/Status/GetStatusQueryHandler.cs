using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SecretSharingTool.Data.Database;
using SecretsSharingTool.Core.Shared;

namespace SecretsSharingTool.Core.Status
{
    public sealed class GetStatusQueryHandler : BaseRequestHandler<GetStatusQuery, bool>
    {
        public GetStatusQueryHandler(AppUnitOfWork appUnitOfWork, ILogger logger) : base(appUnitOfWork, logger)
        {
        }

        public override async Task<bool> Handle(GetStatusQuery request, CancellationToken cancellationToken)
        {
            return await AppUnitOfWork.Database.CanConnectAsync(cancellationToken);
        }
    }
}