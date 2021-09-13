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
                    "BXlxme+vWl88kjag3/x+jw=="),
                EncryptedSymmetricKey = Convert.FromBase64String("SNBf1fAL0XeNuu8tzPQz8irlUez5CEeC7kCaAK29xKct3yRtOv1qovUtnnVPsodTMrkkfTYe0FJdS6gc4/LyF225XgvGi/iENywUC8FZa9g2K3JcDYWPP2o2OcNtngMRRXdHo+v5xJ9lc1i+/Zv7bXJdcl0sb8SucTAE/ukN3z0hklmnTox8yo8mc6jqtmFQlSaddEJ/kToKVpBoTVJUUt5FHTLzabnCDOtBScO0s3Q2LC/GRLQgNKf1kuE+ED1on1/N330ZnhPXlBR1GjN1LG8P//20UPenOxfXCVLPqy0ae/6x7BFV6/ezwLz60fA1PqJe7nMSvnr3hiMQu1n8+Q=="),
                Iv = Convert.FromBase64String("ujc2xMLElb2znnB+x/yaog=="),
                SignedHash = Convert.FromBase64String("YMiIJS1UDJC630GrFT7ee6myPZsShxN/CHtITwmq4qO81NtdOvKunLptAbXwmVBMYSPcbGW392cnIeBfrjPNV231IIPzS1Vr3YZksaxDMJEJvpSecFC1M2yHCegJkyKuprsiCVsSXbZkGaXtqDLNygoW/XcJ2d0jafk1YPcpPtqdu0cxfPVnLAl320Wxas4+TqFhkduM9S9g4qpVM8Ltq0rSJ7qn3F2wdCZ8VESNGsdXvsNcBvDo2UBatvfjR9pRsHnIobGIbMhofuVuxXxYND58xqiHGdIqlGHfbutk/QglHSSd22lyr4vo04iqV8RsWD+GwFl+UDcQu4FCRlX1qw=="),
                IsActive = true,
                ExpireDate = DateTime.UtcNow.AddHours(1)
            };
            
            await UnitOfWork.Secrets.AddAsync(secret);
            await UnitOfWork.SaveChangesAsync();

            var query = new SecretRetrieveQuery()
            {
                Id = guid,
                PrivateKey = Convert.FromBase64String(
                    "MIIEpQIBAAKCAQEAwaURWfiNkzFCSxaS8H5uGDwxXWUijSi/b+UNpyi58wsbFcjb8/2mKsu3fOg3MpGLnsjZd4cj6nGXQ60Uz0hs1hOSbXP1cWvsk+rgs7J5PDmcILoQ7l4NgY3vpyi12lPZt5eA32Xart9WgUkT7CV7iolccdhm9fkWX5NFQSfsVSe/7zQx5STyh9z/FMx5sfU7cxVAGeW/PuXG4eGhkMqRZUOR4vor6hCWmejbQQ/OxkLj8QyVw8FRKepKg+KK7Bn/V2utmXHucXi8oesW587KSaz9dwbctlBDWWMt2//0P+y565a5WWq/hAsoh2hHTQP0W+73bxnZSkw4W+iekD1SiQIDAQABAoIBAQCXWEDAdc+IzAYJE8KSLe40CM94NDQV6VP9yyKpk94JQ5POqeamwobdicyF1iRV5IUf+9BjZ/69HDxiC2NpQiy2ACQAYZHiLLyBOyCVsZapaElLFsFm26FosRsuJVZEJibSI5n4f+2zoSBF+WyVsxHkS8kxvTVzZ1OCLWMCMoa2YfckBI0YtD4u5Sw7983V4RbBUu4SesjX1oUbX+c8VI0/ZqmR5D4lU9cRExNGocb1Bxi8Ar0C47x/l1qcGBuugmrcmvXW7GxPHeO19TIyHf4I4g/5H7nFP04lyIgsmTi/LtTYOgT2vdxDWONnrVbIeJFaa+sKXIIskzoQqU2TEd0RAoGBAOy1hKEINxGvGwRxots1K0cHYKQTshKYZzeNUzRANBao+NWrm87UCbAUzNY2w7DWMXb4XeF/Mg8KeccOJlWPgsKw7km9nWX6T6hi6kjOU+UMSBtILV5/AGoyRPGXHt16aFNJxqKJk38d2yKJMRT0Cs6QYVCiUpPpe6WU9rbCzP/lAoGBANFtGKRnWcoj7NSxNOU8Km/gxikIyqA6M7SzU9Kv4oFfftWAze7DcN66hE9mtEQrBgvpIu5vkXcniCw9CjPb9UGpxjukCrHXPrxu0ankj5I8B3ZqkQxbnyOFmJDCPajFB41OUVmiuu7i9e2wrwZ03XMrH23xHFp6pvJ9CdWgyjXVAoGAZaT0lOmgLBnTMi9RzWuhb9i9KPfEKbQD7pjjN7ayJ2DbqXUNbN0kY9Yqt3nAwnjXuMyiE6i46DiJXm63C1qavduYF1Yy9o1sE1SWYjknUV3awMxXnxJOLUv0ywEnoddkYQ93GmiVS6qYZNYgm2zIDzd0clmhuB40mD/FSWmQ+fkCgYEApFSnznFuimgwMSEv8WSJpCpLM+27GYkyA2vnrV3oLT4ixyN/9AeV7J+MrMVZVFgyMWxpxZD0Ivx5JMVs2Q6S30h5zVjGL5BHoRKQIHDBm6zQ6kFqubi3Ied1tLlGKsUYTPAUc3J2nnanJTDL1hQjpHhE/D8C0ZiGoaYt7lxx71UCgYEA6/Uoedvma5mRIReF5zJtswb9VwMlrX5RAz3weCIjbBJbPG1lP0salyAsmfjqoQBM514HhO9BC3rTGdks1a5WPhhPfBkEBy7z8MYDuCVyRrQIAzoSopBICf+B6WA6iSUINWiaxdYKfFa+p4kZbRtS6ohYV92ITQA/tV3FE9P6mE8=")
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