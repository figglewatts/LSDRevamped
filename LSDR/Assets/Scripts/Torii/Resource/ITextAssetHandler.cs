using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Torii.Resource
{
    public interface ITextAssetHandler
    {
        Type HandlerType { get; }

        object Process(TextAsset textAsset);
    }
}