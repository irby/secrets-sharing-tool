using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SecretSharingTool.Data.Database;

namespace SecretsSharingTool.Core.Tests
{
    public abstract class BaseHandlerTests<T>
    {
        protected AppUnitOfWork UnitOfWork;
        protected ILogger<T> Logger;

        [SetUp]
        public virtual void Initialize()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole();
            });
            Logger = loggerFactory.CreateLogger<T>();
            var mockUnitOfWorkOptions = new DbContextOptionsBuilder<AppUnitOfWork>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            UnitOfWork = new AppUnitOfWork(mockUnitOfWorkOptions);
        }
    }
}