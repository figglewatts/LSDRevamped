using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Torii.Resource
{
    public interface IResourceHandler
    {
        Type HandlerType { get; }

        void Load(string path, int span);
    }
}