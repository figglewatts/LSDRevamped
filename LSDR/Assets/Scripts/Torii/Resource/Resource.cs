namespace Torii.Resource
{
    /// <summary>
    ///     Store a resource with a given type.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    public class Resource<T> : GenericResource
    {
        /// <summary>
        ///     The data stored on this resource.
        /// </summary>
        public T Data;

        /// <summary>
        ///     Create a new resource with some data, a lifespan, and a type.
        /// </summary>
        /// <param name="data">The data to store on this resource.</param>
        /// <param name="lifespan">This resource's lifespan ID.</param>
        /// <param name="type">Whether this resource is streamed or unity.</param>
        public Resource(T data, int lifespan = 0, ResourceType type = ResourceType.Streamed) : base(lifespan, type)
        {
            Data = data;
        }

        /// <summary>
        ///     Create an empty resource.
        /// </summary>
        /// <param name="lifespan">The lifespan.</param>
        /// <param name="type">What kind of resource it is.</param>
        public Resource(int lifespan = 0, ResourceType type = ResourceType.Streamed) : base(lifespan, type)
        {
            // intentionally empty
        }

        /// <summary>
        ///     Get this resource's data.
        /// </summary>
        /// <returns>This resource's data.</returns>
        public override object GetData()
        {
            return Data;
        }
    }
}
