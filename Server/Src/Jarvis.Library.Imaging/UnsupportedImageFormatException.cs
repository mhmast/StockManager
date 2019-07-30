using System;
using System.Runtime.Serialization;

namespace Jarvis.Library.Imaging
{
    [Serializable]
    internal class UnsupportedImageFormatException : Exception
    {
        public UnsupportedImageFormatException()
        {
        }

        public UnsupportedImageFormatException(string message) : base(message)
        {
        }

        public UnsupportedImageFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedImageFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}