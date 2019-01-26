using System;
using System.Collections;
using System.Collections.Generic;

namespace Torii.Exceptions
{
    public class ToriiResourceLoadException : Exception
    {
        public Type ResourceType { get; private set; }

        public ToriiResourceLoadException(Type resourceType) : base()
        {
            ResourceType = resourceType;
        }

        public ToriiResourceLoadException(string message, Type resourceType) : base(message)
        {
            ResourceType = resourceType;
        }

        public ToriiResourceLoadException(string message, Exception innerException, Type resourceType) : base(message, innerException)
        {
            ResourceType = resourceType;
        }
    }
}