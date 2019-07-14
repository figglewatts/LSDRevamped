using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Torii.Resource
{
    /// <summary>
    /// Used to store resource lifespans and map them to IDs.
    /// </summary>
    public class ResourceLifespans
    {
        private readonly Dictionary<string, int> _lifespans; // resource names to IDs

        /// <summary>
        /// Create a new lifespan storage object. Comes with two default lifespans 'global' and 'scene'.
        /// </summary>
        public ResourceLifespans()
        {
            _lifespans = new Dictionary<string, int>();
            CreateLifespan("global");
            CreateLifespan("scene");
        }

        /// <summary>
        /// Create a new lifespan. IDs auto increment starting at 2.
        /// </summary>
        /// <param name="name">The name of this lifespan.</param>
        public void CreateLifespan(string name)
        {
            _lifespans[name] = _lifespans.Count;
        }

        /// <summary>
        /// Check to see if a lifespan exists.
        /// </summary>
        /// <param name="name">The name of the lifespan.</param>
        /// <returns>True if it existed, false otherwise.</returns>
        public bool LifespanExists(string name)
        {
            return _lifespans.ContainsKey(name);
        }

        /// <summary>
        /// Array accessor for lifespans.
        /// </summary>
        /// <param name="name">The name of this lifespan.</param>
        /// <exception cref="ArgumentException">If the lifespan did not exist.</exception>
        public int this[string name]
        {
            get
            {
                if (!LifespanExists(name))
                {
                    throw new ArgumentException("Lifespan " + name + " not created. Check resourcelifespans.json", 
                        nameof(name));
                }
                return _lifespans[name];
            }
        }
    }
}