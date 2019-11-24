using Torii.Util;

namespace Torii.Coroutine
{
    /// <summary>
    /// Used to allow for access to StartCoroutine anywhere, like within a ScriptableObject.
    /// Just do Coroutines.Instance.StartCoroutine(...)
    /// </summary>
    public class Coroutines : MonoSingleton<Coroutines>
    {
    }
}
