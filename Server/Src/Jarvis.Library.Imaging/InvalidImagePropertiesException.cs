using System;
using System.Runtime.Serialization;

namespace Jarvis.Library.Imaging
{
    [Serializable]
    internal class InvalidImagePropertiesException : Exception
    {
        public InvalidImagePropertiesException()
        {
        }

        public InvalidImagePropertiesException(string message) : base(message)
        {
        }

        public InvalidImagePropertiesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidImagePropertiesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}