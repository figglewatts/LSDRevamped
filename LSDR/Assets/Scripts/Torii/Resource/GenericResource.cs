using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Torii.Resource
{
    public abstract class GenericResource
    {
        public int Lifespan;
        public ResourceType ResourceType;

        public abstract object GetData();

        protected GenericResource(int lifespan, ResourceType type)
        {
            Lifespan = lifespan;
            ResourceType = type;
        }
    }

    public enum ResourceType
    {
        Streamed,
        Unity
    }
}