using System;

namespace Torii.Resource
{
    /// <summary>
    ///     Interface for a resource handler. Defines the Type that the handler should be triggered by, as well as
    ///     a Load() method for the implementation of actually loading the resource.
    /// </summary>
    public interface IResourceHandler
    {
        /// <summary>
        ///     The Type that this handler should be triggered by, i.e. typeof(Texture2D) would be triggered by
        ///     <c>ResourceManager.Load&lt;Texture2D&gt;(...)</c>.
        /// </summary>
        Type HandlerType { get; }

        /// <summary>
        ///     Load the resource given by 'path'.
        /// </summary>
        /// <param name="path">The path on disk of the resource to load.</param>
        /// <param name="span">The span ID to associate with the resource.</param>
        void Load(string path, int span);
    }
}
