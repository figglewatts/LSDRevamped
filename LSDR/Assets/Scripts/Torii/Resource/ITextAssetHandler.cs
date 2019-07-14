using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Torii.Resource
{
    /// <summary>
    /// Text assets need special handling. Implement this to define your own kind of text asset.
    /// </summary>
    public interface ITextAssetHandler
    {
        Type HandlerType { get; }

        object Process(TextAsset textAsset);
    }
}