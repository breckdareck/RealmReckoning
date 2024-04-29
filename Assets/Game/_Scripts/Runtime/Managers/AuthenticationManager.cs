using Game._Scripts.Runtime.Bootstrapper;
using Game._Scripts.Runtime.UnityServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game._Scripts.Runtime.Managers
{
    public class AuthenticationManager : MonoBehaviour
    {
        private AsyncOperation _scene;

        private void Start()
        {
            _scene = SceneManager.LoadSceneAsync("MainMenu");
            _scene.allowSceneActivation = false;
        }

        public async void AnonymousLoginClicked()
        {
            using (new LoaderSystem.Load())
            {
                await AuthService.LoginAnonymously();
                _scene.allowSceneActivation = true;
            }
        }
    }
}