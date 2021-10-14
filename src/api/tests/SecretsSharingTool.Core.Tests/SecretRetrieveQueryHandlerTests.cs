using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Moq;
using NuGet.Frameworks;
using NUnit.Framework;
using SecretSharingTool.Data.Models;
using SecretsSharingTool.Core.Retrieve;

namespace SecretsSharingTool.Core.Tests
{
    public sealed class SecretRetrieveQueryHandlerTests : BaseHandlerTests<SecretRetrieveQueryHandler>
    {
        private SecretRetrieveQueryHandler _handler;
        private Secret _secret;
        private string _privateKey = "MIICWwIBAAKBgQCxpmsT5kcrA8JsqA0+xYOmRqovxil064zs2J63hc3ULGRExu+3EAEZg00/YhrPs3+HPEVQ8e+fLbOMhroIDHaWt+sZJqbdD+Gs2Unb0S+X7+2TjfHjd41ynVEkdQoK3sxJH14WSqH989caFz2myG4mTdBbVbQLZ0zbJAFngPgWhQIDAQABAoGADRkYC/+Of71nOFurnlUxv7C2G4+gvt4fJddS3HUhF+OuAOQqCHVFn3vu1h2FnIq/fFkVQ0KTSEk2U8YbMTy6AYNbUiwVVee2d8Vf3uGmDhiY+fPlCE0uH6pm4K42jBR6LnsJwwCTxxBfIlpxbbWPKO/JRoDe1V73AnOGFwW/CAECQQDWtJ9fUnvA3kepKy9e6jEwDJ9CPlOOKdut0eZzdcmLE8zjq2SWbJTbUtyFb7E6TB4vKrFZI611uhtPUh4GYxUVAkEA09FPAJMA1Eqgvvkmbp5r6DFZVe1n9BGIEsDGNpmkxPpyodS0jAIlD7YcAKaIroK/Rl75IZRGUz+Z+Ye8IvM3sQJALe5N+dJnbwceRW5bn+5xv1mz6DP1JACIYlL4/dJA32PI/Rt5VUS9Q34idtlCDLqj6mszrWIYhYretMVYbv1bUQJAUAMcyKa/BXI7TmhnMBn/wPIDQQYw4m1M7WiMd0uKhi3k52Sl3s1gmpK7+TLvJpyVDEwL57dO6Vt5Gl0/oCQnwQJAAtAzW2stCcM0veDWU16gzZzTOhEJgGIQHItDZq060ro/ZhYN4c5b17yoo1FUFm3tPY+Az+fGm4B6eewDz/CbiQ==";

        [SetUp]
        public void SetUp()
        {
            var sampleJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "sample_json", "sample_secret.json");
            var sampleSecretData = File.ReadAllText(sampleJsonPath);

            _secret = Newtonsoft.Json.JsonConvert.DeserializeObject<Secret>(sampleSecretData);
            _secret!.ExpireDateTime = DateTimeOffset.UtcNow.AddHours(1);
            
            _handler = new SecretRetrieveQueryHandler(UnitOfWork, Logger);
        }
        
        #region Handler Tests

        [Test]
        public async Task WhenSecretIsValid_CorrectPrivateKeySupplied_ShouldReturnValidResult()
        {
            await UnitOfWork.Secrets.AddAsync(_secret);
            await UnitOfWork.SaveChangesAsync();

            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Convert.FromBase64String(_privateKey)
            };
            
            // Validate the secret is active before running
            Assert.IsTrue(_secret.IsActive);
            Assert.IsNull(_secret.ModifiedOn);

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.AreEqual("helloworld", result.Message);
            
            // Validate the secret is deactivated after running
            Assert.IsFalse(_secret.IsActive);
            Assert.IsNotNull(_secret.ModifiedOn);
        }

        [Test]
        public async Task WhenSecretIsExpired_CorrectPrivateKeySupplied_ShouldReturnNull()
        {
            _secret.ExpireDateTime = DateTimeOffset.UtcNow.AddSeconds(-10);
            
            await UnitOfWork.Secrets.AddAsync(_secret);
            await UnitOfWork.SaveChangesAsync();
            
            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Convert.FromBase64String(_privateKey)
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretIsInactive_CorrectPrivateKeySupplied_ShouldReturnNull()
        {
            _secret.IsActive = false;

            await UnitOfWork.Secrets.AddAsync(_secret);
            await UnitOfWork.SaveChangesAsync();

            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Convert.FromBase64String(_privateKey)
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretIsValid_InvalidPrivateKeySupplied_ShouldReturnNull()
        {
            await UnitOfWork.Secrets.AddAsync(_secret);
            await UnitOfWork.SaveChangesAsync();

            // Private key is modified
            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Convert.FromBase64String(_privateKey.Replace("7","9"))
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretIsAltered_CorrectPrivateKeySupplied_ShouldReturnNull()
        {
            _secret.Message = Convert.FromBase64String(Convert.ToBase64String(_secret.Message).Replace("T","7"));
            
            await UnitOfWork.Secrets.AddAsync(_secret);
            await UnitOfWork.SaveChangesAsync();

            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Convert.FromBase64String(_privateKey)
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretCannotBeFound_CorrectPrivateKeySupplied_ShouldReturnNull()
        {
            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Convert.FromBase64String(_privateKey)
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretIsValid_PrivateKeyIsBruteForced_ShouldExpireSecret()
        {
            await UnitOfWork.Secrets.AddAsync(_secret);
            await UnitOfWork.SaveChangesAsync();

            // Query is modified
            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Convert.FromBase64String(_privateKey.Replace("7","9"))
            };

            for (var i = 0; i < _handler.NumberOfAllowedAttempts; i++)
            {
                await _handler.Handle(query, CancellationToken.None);
                
                // Validate the secret is still active
                Assert.IsTrue(_secret.IsActive);
                Assert.IsNull(_secret.ModifiedOn);
            }

            await _handler.Handle(query, CancellationToken.None);

            // Validate the secret is deactivated after running
            Assert.IsFalse(_secret.IsActive);
            Assert.IsNotNull(_secret.ModifiedOn);
            Assert.IsNull(_secret.Message);
            Assert.IsNull(_secret.SignedHash);
            Assert.AreEqual(_secret.NumberOfAttempts, _handler.NumberOfAllowedAttempts);
        }
        
        #endregion
        
        
        
        
        
        
        
        #region Validation Tests

        [Test]
        public async Task WhenQueryIsValid_ShouldNotHaveError()
        {
            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Convert.FromBase64String(_privateKey)
            };

            var validator = new SecretRetrieveQueryValidator();
            var result = await validator.ValidateAsync(query);
            Assert.IsTrue(result.IsValid);
        }
        
        [Test]
        public async Task WhenQueryIsMissingId_ShouldHaveError()
        {
            var query = new SecretRetrieveQuery()
            {
                Id = Guid.Empty,
                PrivateKey = Convert.FromBase64String(_privateKey)
            };

            var validator = new SecretRetrieveQueryValidator();
            var result = await validator.ValidateAsync(query);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.First().PropertyName, "Id");
            Assert.AreEqual(result.Errors.First().ErrorMessage, "'Id' must not be empty.");
        }
        
        [Test]
        public async Task WhenQueryIsMissingPrivateKey_ShouldHaveError()
        {
            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = null
            };

            var validator = new SecretRetrieveQueryValidator();
            var result = await validator.ValidateAsync(query);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.First().PropertyName, "PrivateKey");
            Assert.AreEqual(result.Errors.First().ErrorMessage, "'Private Key' must not be empty.");
        }
        
        [Test]
        public async Task WhenQueryHasEmptyPrivateKey_ShouldHaveError()
        {
            var query = new SecretRetrieveQuery()
            {
                Id = _secret.Id,
                PrivateKey = Array.Empty<byte>()
            };

            var validator = new SecretRetrieveQueryValidator();
            var result = await validator.ValidateAsync(query);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.First().PropertyName, "PrivateKey");
            Assert.AreEqual(result.Errors.First().ErrorMessage, "'Private Key' must not be empty.");
        }
        
        #endregion
    }
}