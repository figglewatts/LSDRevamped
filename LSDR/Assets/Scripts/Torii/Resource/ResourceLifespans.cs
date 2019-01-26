using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Torii.Resource
{
    public class ResourceLifespans
    {
        private readonly Dictionary<string, int> _lifespans;

        public ResourceLifespans()
        {
            _lifespans = new Dictionary<string, int>();
            CreateLifespan("global");
            CreateLifespan("scene");
        }

        public void CreateLifespan(string name)
        {
            _lifespans[name] = _lifespans.Count;
        }

        public bool LifespanExists(string name)
        {
            return _lifespans.ContainsKey(name);
        }

        public int this[string name]
        {
            get
            {
                if (!LifespanExists(name))
                {
                    throw new ArgumentException("Lifespan " + name + " not created. Check resourcelifespans.json", "name");
                }
                return _lifespans[name];
            }
        }
    }
}