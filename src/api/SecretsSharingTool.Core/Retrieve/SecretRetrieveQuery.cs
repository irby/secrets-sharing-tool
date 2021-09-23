using System;
using MediatR;
using SecretsSharingTool.Core.Create;

namespace SecretsSharingTool.Core.Retrieve
{
    public class SecretRetrieveQuery : IRequest<SecretRetrieveQueryResponse>
    {
        public Guid Id { get; set; }
        public byte[] PrivateKey { get; set; }
    }
}