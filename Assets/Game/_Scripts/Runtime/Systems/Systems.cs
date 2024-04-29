using Game._Scripts.Runtime.Utilities;

namespace Game._Scripts.Runtime.Systems
{
    /// <summary>
    /// Don't specifically need anything here other than the fact it's persistent.
    /// I like to keep one main object which is never killed, with sub-systems as children.
    /// </summary>
    public class Systems : PersistentSingleton<Systems>
    {
    }
}