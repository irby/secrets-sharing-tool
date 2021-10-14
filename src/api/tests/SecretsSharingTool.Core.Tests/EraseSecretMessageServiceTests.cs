using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SecretSharingTool.Data.Database;
using SecretSharingTool.Data.Models;
using SecretsSharingTool.Core.Erase;

namespace SecretsSharingTool.Core.Tests
{
    public sealed class EraseSecretMessageServiceTests : BaseHandlerTests<EraseSecretMessageService>
    {
        private EraseSecretMessageService _service;
        
        [SetUp]
        public void SetUp()
        {
            var mockServiceFactory = new Mock<IServiceScopeFactory>();
            var mockServiceScope = new Mock<IServiceScope>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider.Setup(p => p.GetService(typeof(AppUnitOfWork)))
                .Returns(UnitOfWork);
            mockServiceScope.Setup(p => p.ServiceProvider)
                .Returns(mockServiceProvider.Object);
            mockServiceFactory.Setup(p => p.CreateScope())
                .Returns(mockServiceScope.Object);
            
            _service = new EraseSecretMessageService(mockServiceFactory.Object, Logger);
        }

        [Test]
        public async Task WhenSecretIsExpired_ShouldEraseMessage()
        {
            var guid = Guid.NewGuid();

            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "lL0fisrg4/u5FLSX6f7Akw=="),
                EncryptedSymmetricKey = Convert.FromBase64String("hck/SRVmJ2WxnSTzTdrb5bCTdnK1a5iDRVg6ttooCQdDA6P0Kz91L7dubmChYF5OswQawM8Uxp6XDe6Vz8cZv1xPDmXNRN8Z3tFnBpfgvO/k4sN9J21Ix2Ucw1mQXjMuYgBSJz9PAZCtKGhcdm+sr2sStheSbCO+OVPFWYn7t8I="),
                Iv = Convert.FromBase64String("7GocUIxCXthkiEH0Sim6Yg=="),
                SignedHash = Convert.FromBase64String("R6qo0u8Vj8QFEiHoaWLGzEJ/deznkTgY2apcX/iS/lbJZLv6Fd0auJPXmjPLQ03FALdnjQdFOzvJwnObePCjMj2C1vnM+tBmc+1mq+1+0OI4fp1mWJ2pqRD2cXqHaVx3umQ4r1nzoDfBiVo28ZzMNdDrAIRX3M90mXCcgSdbAm8="),
                IsActive = true,
                ExpireDateTime = DateTimeOffset.UtcNow.AddSeconds(-10)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            await _service.Handle(CancellationToken.None);
            
            Assert.IsNull(UnitOfWork.Secrets.First(p => p.Id == guid).Message);
        }
        
        [Test]
        public async Task WhenSecretIsInactive_ShouldEraseMessage()
        {
            var guid = Guid.NewGuid();

            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "lL0fisrg4/u5FLSX6f7Akw=="),
                EncryptedSymmetricKey = Convert.FromBase64String("hck/SRVmJ2WxnSTzTdrb5bCTdnK1a5iDRVg6ttooCQdDA6P0Kz91L7dubmChYF5OswQawM8Uxp6XDe6Vz8cZv1xPDmXNRN8Z3tFnBpfgvO/k4sN9J21Ix2Ucw1mQXjMuYgBSJz9PAZCtKGhcdm+sr2sStheSbCO+OVPFWYn7t8I="),
                Iv = Convert.FromBase64String("7GocUIxCXthkiEH0Sim6Yg=="),
                SignedHash = Convert.FromBase64String("R6qo0u8Vj8QFEiHoaWLGzEJ/deznkTgY2apcX/iS/lbJZLv6Fd0auJPXmjPLQ03FALdnjQdFOzvJwnObePCjMj2C1vnM+tBmc+1mq+1+0OI4fp1mWJ2pqRD2cXqHaVx3umQ4r1nzoDfBiVo28ZzMNdDrAIRX3M90mXCcgSdbAm8="),
                IsActive = false,
                ExpireDateTime = DateTimeOffset.UtcNow.AddHours(1)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            await _service.Handle(CancellationToken.None);
            
            Assert.IsNull(UnitOfWork.Secrets.First(p => p.Id == guid).Message);
        }
        
        [Test]
        public async Task WhenSecretIsActiveAndNotExpired_ShouldNotEraseMessage()
        {
            var guid = Guid.NewGuid();

            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "lL0fisrg4/u5FLSX6f7Akw=="),
                EncryptedSymmetricKey = Convert.FromBase64String("hck/SRVmJ2WxnSTzTdrb5bCTdnK1a5iDRVg6ttooCQdDA6P0Kz91L7dubmChYF5OswQawM8Uxp6XDe6Vz8cZv1xPDmXNRN8Z3tFnBpfgvO/k4sN9J21Ix2Ucw1mQXjMuYgBSJz9PAZCtKGhcdm+sr2sStheSbCO+OVPFWYn7t8I="),
                Iv = Convert.FromBase64String("7GocUIxCXthkiEH0Sim6Yg=="),
                SignedHash = Convert.FromBase64String("R6qo0u8Vj8QFEiHoaWLGzEJ/deznkTgY2apcX/iS/lbJZLv6Fd0auJPXmjPLQ03FALdnjQdFOzvJwnObePCjMj2C1vnM+tBmc+1mq+1+0OI4fp1mWJ2pqRD2cXqHaVx3umQ4r1nzoDfBiVo28ZzMNdDrAIRX3M90mXCcgSdbAm8="),
                IsActive = true,
                ExpireDateTime = DateTimeOffset.UtcNow.AddHours(1)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            await _service.Handle(CancellationToken.None);
            
            Assert.IsNotNull(UnitOfWork.Secrets.First(p => p.Id == guid).Message);
        }
    }
}