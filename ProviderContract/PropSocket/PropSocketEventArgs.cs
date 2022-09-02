using System;

namespace ProviderContract.PropSocket
{
    public class PropSocketEventArgs
    {
        public byte[] CompressedData { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}
