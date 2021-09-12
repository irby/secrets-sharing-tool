using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SecretSharingTool.Data.Database;

namespace SecretsSharingTool.Core.Tests
{
    public abstract class BaseHandlerTests
    {
        protected AppUnitOfWork UnitOfWork;

        [SetUp]
        public virtual void Initialize()
        {
            var mockUnitOfWorkOptions = new DbContextOptionsBuilder<AppUnitOfWork>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            UnitOfWork = new AppUnitOfWork(mockUnitOfWorkOptions);
        }
    }
}