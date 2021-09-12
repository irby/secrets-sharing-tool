using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NUnit.Framework;
using SecretSharingTool.Data.Database;
using SecretsSharingTool.Core.Create;

namespace SecretsSharingTool.Core.Tests
{
    public class SecretCreationCommandHandlerTests : BaseHandlerTests
    {
        private SecretCreationCommandHandler _commandHandler;
        
        [SetUp]
        public void Setup()
        {
            _commandHandler = new SecretCreationCommandHandler(UnitOfWork);
        }

        [Test]
        public async Task WhenValidCommandIsSupplied_ShouldReturnValidResult()
        {
            var command = new SecretCreationCommand()
            {
                Message = "helloworld",
                SecondsToLive = 60
            };
            
            Assert.AreEqual(UnitOfWork.Secrets.Count(), 0);
            
            var result = await _commandHandler.Handle(command, CancellationToken.None);
            
            Assert.IsNotNull(result.Id);
            Assert.IsNotNull(result.Key);
            
            Assert.AreEqual(UnitOfWork.Secrets.Count(), 1);
            
            var secret = UnitOfWork.Secrets.First();
            Assert.IsTrue(secret.IsActive);
            Assert.IsTrue(secret.ExpireDate <= DateTime.UtcNow.AddSeconds(60));
            Assert.IsNotNull(secret.CreatedOn);
            Assert.IsNull(secret.ModifiedOn);
        }

        [Test]
        public async Task WhenCommandIsValid_ShouldNotHaveError()
        {
            var command = new SecretCreationCommand()
            {
                Message = "helloworld",
                SecondsToLive = 60
            };

            var validator = new SecretCreationCommandValidator();
            var result = await validator.ValidateAsync(command, CancellationToken.None);
            Assert.IsTrue(result.IsValid);
        }
        
        [Test]
        public async Task WhenCommandIsMissingMessage_ShouldHaveError()
        {
            var command = new SecretCreationCommand()
            {
                Message = null,
                SecondsToLive = 60
            };

            var validator = new SecretCreationCommandValidator();
            var result = await validator.ValidateAsync(command, CancellationToken.None);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.First().PropertyName, "Message");
            Assert.AreEqual(result.Errors.First().ErrorMessage, "'Message' must not be empty.");
        }
        
        [Test]
        public async Task WhenCommandMessageIsEmpty_ShouldHaveError()
        {
            var command = new SecretCreationCommand()
            {
                Message = string.Empty,
                SecondsToLive = 60
            };

            var validator = new SecretCreationCommandValidator();
            var result = await validator.ValidateAsync(command, CancellationToken.None);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.First().PropertyName, "Message");
            Assert.AreEqual(result.Errors.First().ErrorMessage, "'Message' must not be empty.");
        }
        
        [Test]
        public async Task WhenCommandSecondsToLiveIsEqualToZero_ShouldHaveError()
        {
            var command = new SecretCreationCommand()
            {
                Message = "helloworld",
                SecondsToLive = 0
            };

            var validator = new SecretCreationCommandValidator();
            var result = await validator.ValidateAsync(command, CancellationToken.None);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.First().PropertyName, "SecondsToLive");
            Assert.AreEqual(result.Errors.First().ErrorMessage, "'Seconds To Live' must be greater than '0'.");
        }
        
        [Test]
        public async Task WhenCommandSecondsToLiveIsLessThanZero_ShouldHaveError()
        {
            var command = new SecretCreationCommand()
            {
                Message = "helloworld",
                SecondsToLive = -1
            };

            var validator = new SecretCreationCommandValidator();
            var result = await validator.ValidateAsync(command, CancellationToken.None);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.First().PropertyName, "SecondsToLive");
            Assert.AreEqual(result.Errors.First().ErrorMessage, "'Seconds To Live' must be greater than '0'.");
        }
    }
}