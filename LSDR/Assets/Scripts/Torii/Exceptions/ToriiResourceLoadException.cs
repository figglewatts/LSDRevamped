using System;

namespace Torii.Exceptions
{
    /// <summary>
    ///     An exception that is thrown when there is an error loading a resource.
    /// </summary>
    public class ToriiResourceLoadException : Exception
    {
        public ToriiResourceLoadException(Type resourceType)
        {
            ResourceType = resourceType;
        }

        public ToriiResourceLoadException(string message, Type resourceType) : base(message)
        {
            ResourceType = resourceType;
        }

        public ToriiResourceLoadException(string message, Exception innerException, Type resourceType) : base(message,
            innerException)
        {
            ResourceType = resourceType;
        }

        public Type ResourceType { get; private set; }
    }
}
