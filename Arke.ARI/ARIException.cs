using System;

namespace Arke.ARI;

/// <inheritdoc />
public class AriException : Exception
{
    public int StatusCode { get; }

    public AriException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
