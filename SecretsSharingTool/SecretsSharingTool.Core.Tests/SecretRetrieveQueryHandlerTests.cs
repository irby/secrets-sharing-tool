using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SecretSharingTool.Data.Models;
using SecretsSharingTool.Core.Retrieve;

namespace SecretsSharingTool.Core.Tests
{
    public sealed class SecretRetrieveQueryHandlerTests : BaseHandlerTests
    {
        private SecretRetrieveQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _handler = new SecretRetrieveQueryHandler(UnitOfWork);
        }
        
        #region Handler Tests

        [Test]
        public async Task WhenSecretIsValid_CorrectPrivateKeySupplied_ShouldReturnValidResult()
        {
            var guid = Guid.NewGuid();

            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "i1ypPo06jXSAb1tOtY6iqKwcGzLpCIGXBr1E3iAJfzvjCzNbFIV4MEL2eRSqF1AGk3QzDQHk2QjtxHIXbJjcZDskLidwTpkG5Rr6HcGBlJyD7mtfuME7uEPVPaFiJghtjGecE6rBIkPBzIn2LhHvWbCJ5Tn7vgNY8G0dnOeBqes="),
                SignedHash = Convert.FromBase64String("HM+320eIh64DLHOGUMkKq7B7mquKzssEWSinPyWFepNrHqCiseJVtJqmkvt9ugY+SaKe+dKltRCRaDVa4IEyaDoQ3/xLkrVAiQ58fu34QdVTw7Cn33Ih61NBowTvRymIyWbxVSvjzLpU+PJsf3v6uBrMp9eP4OEXZ1Itzb1OD3Q="),
                IsActive = true,
                ExpireDate = DateTime.UtcNow.AddHours(1)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKBgQDYuH3JynQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
            };
            
            // Validate the secret is active before running
            Assert.IsTrue(secret.IsActive);
            Assert.IsNull(secret.ModifiedOn);

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.AreEqual("helloworld", result.Message);
            
            // Validate the secret is deactivated after running
            Assert.IsFalse(secret.IsActive);
            Assert.IsNotNull(secret.ModifiedOn);
        }

        [Test]
        public async Task WhenSecretIsExpired_CorrectPrivateKeySupplied_ShouldReturnNull()
        {
            var guid = Guid.NewGuid();

            // Value is expired
            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "i1ypPo06jXSAb1tOtY6iqKwcGzLpCIGXBr1E3iAJfzvjCzNbFIV4MEL2eRSqF1AGk3QzDQHk2QjtxHIXbJjcZDskLidwTpkG5Rr6HcGBlJyD7mtfuME7uEPVPaFiJghtjGecE6rBIkPBzIn2LhHvWbCJ5Tn7vgNY8G0dnOeBqes="),
                SignedHash = Convert.FromBase64String("HM+320eIh64DLHOGUMkKq7B7mquKzssEWSinPyWFepNrHqCiseJVtJqmkvt9ugY+SaKe+dKltRCRaDVa4IEyaDoQ3/xLkrVAiQ58fu34QdVTw7Cn33Ih61NBowTvRymIyWbxVSvjzLpU+PJsf3v6uBrMp9eP4OEXZ1Itzb1OD3Q="),
                IsActive = true,
                ExpireDate = DateTime.UtcNow.AddSeconds(-10)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();
            
            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKBgQDYuH3JynQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretIsInactive_CorrectPrivateKeySupplied_ShouldReturnNull()
        {
            var guid = Guid.NewGuid();

            // Secret is no longer active
            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "i1ypPo06jXSAb1tOtY6iqKwcGzLpCIGXBr1E3iAJfzvjCzNbFIV4MEL2eRSqF1AGk3QzDQHk2QjtxHIXbJjcZDskLidwTpkG5Rr6HcGBlJyD7mtfuME7uEPVPaFiJghtjGecE6rBIkPBzIn2LhHvWbCJ5Tn7vgNY8G0dnOeBqes="),
                SignedHash = Convert.FromBase64String("HM+320eIh64DLHOGUMkKq7B7mquKzssEWSinPyWFepNrHqCiseJVtJqmkvt9ugY+SaKe+dKltRCRaDVa4IEyaDoQ3/xLkrVAiQ58fu34QdVTw7Cn33Ih61NBowTvRymIyWbxVSvjzLpU+PJsf3v6uBrMp9eP4OEXZ1Itzb1OD3Q="),
                IsActive = false,
                ExpireDate = DateTime.UtcNow.AddHours(1)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKBgQDYuH3JynQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretIsValid_InvalidPrivateKeySupplied_ShouldReturnNull()
        {
            var guid = Guid.NewGuid();

            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "i1ypPo06jXSAb1tOtY6iqKwcGzLpCIGXBr1E3iAJfzvjCzNbFIV4MEL2eRSqF1AGk3QzDQHk2QjtxHIXbJjcZDskLidwTpkG5Rr6HcGBlJyD7mtfuME7uEPVPaFiJghtjGecE6rBIkPBzIn2LhHvWbCJ5Tn7vgNY8G0dnOeBqes="),
                SignedHash = Convert.FromBase64String("HM+320eIh64DLHOGUMkKq7B7mquKzssEWSinPyWFepNrHqCiseJVtJqmkvt9ugY+SaKe+dKltRCRaDVa4IEyaDoQ3/xLkrVAiQ58fu34QdVTw7Cn33Ih61NBowTvRymIyWbxVSvjzLpU+PJsf3v6uBrMp9eP4OEXZ1Itzb1OD3Q="),
                IsActive = true,
                ExpireDate = DateTime.UtcNow.AddHours(1)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            // Private key is modified
            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKBgQDYuH3JDAQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretIsAltered_CorrectPrivateKeySupplied_ShouldReturnNull()
        {
            var guid = Guid.NewGuid();

            // Message is altered
            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "i1ypPo06jXSAb1tOtY6iqKwcGzLpCIGDNr1E3iAJfzvjCzNbFIV4MEL2eRSqF1AGk3QzDQHk2QjtxHIXbJjcZDskLidwTpkG5Rr6HcGBlJyD7mtfuME7uEPVPaFiJghtjGecE6rBIkPBzIn2LhHvWbCJ5Tn7vgNY8G0dnOeBqes="),
                SignedHash = Convert.FromBase64String("HM+320eIh64DLHOGUMkKq7B7mquKzssEWSinPyWFepNrHqCiseJVtJqmkvt9ugY+SaKe+dKltRCRaDVa4IEyaDoQ3/xLkrVAiQ58fu34QdVTw7Cn33Ih61NBowTvRymIyWbxVSvjzLpU+PJsf3v6uBrMp9eP4OEXZ1Itzb1OD3Q="),
                IsActive = true,
                ExpireDate = DateTime.UtcNow.AddHours(1)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKBgQDYuH3JynQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretCannotBeFound_CorrectPrivateKeySupplied_ShouldReturnNull()
        {
            var guid = Guid.NewGuid();

            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKBgQDYuH3JynQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
            };

            var result = await _handler.Handle(query, CancellationToken.None);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task WhenSecretIsValid_PrivateKeyIsBruteForced_ShouldExpireSecret()
        {
            var guid = Guid.NewGuid();

            var secret = new Secret()
            {
                Id = guid,
                Message = Convert.FromBase64String(
                    "i1ypPo06jXSAb1tOtY6iqKwcGzLpCIGXBr1E3iAJfzvjCzNbFIV4MEL2eRSqF1AGk3QzDQHk2QjtxHIXbJjcZDskLidwTpkG5Rr6HcGBlJyD7mtfuME7uEPVPaFiJghtjGecE6rBIkPBzIn2LhHvWbCJ5Tn7vgNY8G0dnOeBqes="),
                SignedHash = Convert.FromBase64String("HM+320eIh64DLHOGUMkKq7B7mquKzssEWSinPyWFepNrHqCiseJVtJqmkvt9ugY+SaKe+dKltRCRaDVa4IEyaDoQ3/xLkrVAiQ58fu34QdVTw7Cn33Ih61NBowTvRymIyWbxVSvjzLpU+PJsf3v6uBrMp9eP4OEXZ1Itzb1OD3Q="),
                IsActive = true,
                ExpireDate = DateTime.UtcNow.AddHours(1)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            // Query is modified
            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKEgQDYuH3JynQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
            };

            for (var i = 0; i < _handler.NumberOfAllowedAttempts; i++)
            {
                await _handler.Handle(query, CancellationToken.None);
                
                // Validate the secret is still active
                Assert.IsTrue(secret.IsActive);
                Assert.IsNull(secret.ModifiedOn);
            }

            await _handler.Handle(query, CancellationToken.None);

            // Validate the secret is deactivated after running
            Assert.IsFalse(secret.IsActive);
            Assert.IsNotNull(secret.ModifiedOn);
            Assert.AreEqual(secret.NumberOfAttempts, _handler.NumberOfAllowedAttempts);
        }
        
        #endregion
        
        
        
        
        
        
        
        #region Validation Tests

        [Test]
        public async Task WhenQueryIsValid_ShouldNotHaveError()
        {
            var guid = Guid.NewGuid();

            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKBgQDYuH3JynQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
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
                PrivateKey = Convert.FromBase64String(
                    "MIICXAIBAAKBgQDYuH3JynQszkQer8Bon0bIubyBBKUWHgxNCdfCfkzyTn1WftoPm3ayp8jihiUli+m9Z5A5LVxQ2VVskb2xJ4gdp7T1UU+9BuFq+CbWXW8z8SqOFDUUkiHLDNeM7g4DJ4TJNLv439NG80nun9p1sGOIDOuvEj8xF5mxs3YomXREKwIDAQABAoGAJXM0IXO/waJfOSJL0Ih9leAqx+zCjmDg5UsnIq/qohCYLiM7kWjfCR5fY22k66kS2i0USh0hj8MOtPU2X2+iYJPXIS8Pv+0uDO6gWZEzfvy9Zo9mxjHmZ2kP4pfNmbby2z3YbrYhWOCMIjxjYO2srhvhWlqEbaBiOFQmX4adAiECQQD/Z2vm4NygAygFwIb9fRG1WV910jrNoD6QK1ZSWMcRr1pXmbyYq0xAsqGrvM5XthpASuUzSEgXRdWvtzHXEqb7AkEA2Tn13pnZQqIVuMvNLlOllcxcSPRvX7hV/68L9b0csoSGpp5Bdt+qiw4Mfs4q+Sh+oJVYX/mv82x1w7aP7hwQkQJAWPaBU06ICwSOcFJ7sthZzr1uzu+HpBBpOnad/tkFnldiNJlMZDFmwjJ2tgdyKIM56aMs6wwGcHTW1foN1kic+QJARpOzU7Cuyxx4bEMjFfMtcH8mWLm6WeI9ZsZL33qjEUlHqmjuoh3HfkiBPM3lXnCOu8PqI7yvMyrBFGaDWvyZUQJBALXKKW5e/Rj1PlpomUCHvbOQ8XmjxED/vs81qmlcgk9UkP0mzbnVwL2/MxT2gp5cHBxZPlMcJ2i6VZ691XPrH+M=")
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
            var guid = Guid.NewGuid();

            var query = new SecretRetrieveQuery()
            {
                Id = guid,
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
            var guid = Guid.NewGuid();

            var query = new SecretRetrieveQuery()
            {
                Id = guid,
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