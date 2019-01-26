using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Torii.Resource
{
    public class Resource<T> : GenericResource
    {
        public T Data;
        public Resource(T data, int lifespan = 0, ResourceType type = ResourceType.Streamed) : base(lifespan, type)
        {
            Data = data;
        }

        public Resource(int lifespan = 0, ResourceType type = ResourceType.Streamed) : base(lifespan, type)
        {
            // intentionally empty
        }

        public override object GetData()
        {
            return Data;
        }
    }
}