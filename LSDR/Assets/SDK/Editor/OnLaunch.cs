using LSDR.IO.ResourceHandlers;
using Torii.Resource;
using UnityEditor;

namespace LSDR.SDK
{
    [InitializeOnLoad]
    public class OnLaunch
    {
        static OnLaunch()
        {
            ResourceManager.RegisterHandler(new LBDHandler());
            ResourceManager.RegisterHandler(new TIXHandler());
            ResourceManager.RegisterHandler(new TIXTexture2DHandler());
        }
    }
}
