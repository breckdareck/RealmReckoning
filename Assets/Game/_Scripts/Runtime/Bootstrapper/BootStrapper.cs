using UnityEngine;

namespace Game._Scripts.Runtime.Bootstrapper
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
        }
    }
}