using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torii.UI
{
    /// <summary>
    /// Interface used for populating a list of UI elements.
    /// </summary>
    /// <typeparam name="TContained">The type of the contained object.</typeparam>
    public interface IPopulator<TContained>
    {
        /// <summary>
        /// Populate the container with these objects.
        /// </summary>
        /// <param name="with">The list of objects to populate with.</param>
        void Populate(List<TContained> with);
        
        /// <summary>
        /// Clear the contents of the container.
        /// </summary>
        void Clear();
    }
}