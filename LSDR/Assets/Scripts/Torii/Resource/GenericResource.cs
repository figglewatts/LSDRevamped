using System;

namespace Torii.Resource
{
    /// <summary>
    ///     Generic base class for all resources. Concrete resources will be generic subclasses.
    /// </summary>
    public abstract class GenericResource
    {
        /// <summary>
        ///     The lifespan ID of this resource.
        /// </summary>
        public int Lifespan;

        /// <summary>
        ///     Any cleanup actions to take when this resource is expired.
        /// </summary>
        public Action OnExpire;

        /// <summary>
        ///     What type this resource is.
        /// </summary>
        public ResourceType ResourceType;

        /// <summary>
        ///     Create a new generic resource with a lifespan and type.
        /// </summary>
        /// <param name="lifespan">The lifespan ID of this resource.</param>
        /// <param name="type">What type this resource is.</param>
        protected GenericResource(int lifespan, ResourceType type)
        {
            Lifespan = lifespan;
            ResourceType = type;
        }

        /// <summary>
        ///     Get the data stored on this resource.
        /// </summary>
        /// <returns>Resource data.</returns>
        public abstract object GetData();
    }

    /// <summary>
    ///     Resources can either be loaded from disk or from Unity's resources. This enum delineates them.
    /// </summary>
    public enum ResourceType
    {
        Streamed,
        Unity
    }
}
