using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace Managers
{
    public class SceneLoadManager : MonoBehaviour, ISceneLoadService
    {
        [Header("Addressable Scenes")]
        [SerializeField] private AssetReference splashScene;
        [SerializeField] private AssetReference mainScene;
        [SerializeField] private AssetReference gameScene;

        [SerializeField] private GameObject loadingScreenObject;
        [SerializeField] private GameObject splashScreenObject;
        
        public SceneInstance LastScfeneInstance => _sceneStack.Peek();
        public event Action OnGameSceneLoaded;
        private Stack<SceneInstance> _sceneStack = new Stack<SceneInstance>();
        
        private DateTime _splashScreenOpenedTime;
        
        private async void Awake()
        {
            LoadSplashScene();
            await Task.Delay(4000);
            DirectlyPlayGame();
            //LoadMainScene();
        }
        
        private async UniTask DirectlyPlayGame()
        {
            if (_sceneStack.Count > 1)
            {
                try
                {
                    // if last loaded scene is splash scene, we need to toggle splash screen temp object in base scene to block black screen
                    if (_sceneStack.Pop().Scene.name == splashScene.Asset.name)
                        ToggleSplashScreen(true);
                    else
                        ToggleLoadingScreen(true);
                }
                catch (NullReferenceException e)
                {
                    Debug.LogError("_sceneStack.Pop() is not available!");
                }
            }
            else
            {
                ToggleSplashScreen(true);
            }
            
            
            await UnloadLast();
            var gameLoader = Addressables.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
            
            _sceneStack.Push(await gameLoader);

            await Task.Delay(2000);
            
            OnGameSceneLoaded?.Invoke();
        }
        
        private async UniTask LoadSplashScene()
        {
            var splashLoader = Addressables.LoadSceneAsync(splashScene, LoadSceneMode.Additive);
            _sceneStack.Push(await splashLoader); 
            _splashScreenOpenedTime = DateTime.Now;
        }
        
        private async UniTask LoadMainScene()
        {
            if (_sceneStack.Count > 1)
            {
                try
                {
                    // if last loaded scene is splash scene, we need to toggle splash screen temp object in base scene to block black screen
                    if (_sceneStack.Pop().Scene.name == splashScene.Asset.name)
                        ToggleSplashScreen(true);
                    else
                        ToggleLoadingScreen(true);
                }
                catch (NullReferenceException e)
                {
                    Debug.LogError("_sceneStack.Pop() is not available!");
                }
            }
            else
            {
                ToggleSplashScreen(true);
            }
            
            
            await UnloadLast();
            var mainLoader = Addressables.LoadSceneAsync(mainScene, LoadSceneMode.Additive);
            _sceneStack.Push(await mainLoader);
            
            ToggleLoadingScreen(false);
            ToggleSplashScreen(false);
        }
        
        private async UniTask LoadGameScene()
        {
            await UnloadLast();
            var gameLoader = Addressables.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
            _sceneStack.Push(await gameLoader);
        }
        
        public async UniTask Load(string sceneName, bool unloadLast = false)
        {
            if(unloadLast)
                await UnloadLast();
            
            var sceneInstance = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _sceneStack.Push(sceneInstance);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        public async UniTask UnloadLast()
        {
            await Addressables.UnloadSceneAsync(_sceneStack.Pop(), UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }
        
        public async UniTask UnloadLastWithCount(int i)
        {
            for (var j = 0; j < i; j++)
            {
                await UnloadLast();
            }
        }
        
        public void ToggleLoadingScreen(bool state)
        {
            if(loadingScreenObject != null)
                loadingScreenObject.SetActive(state);
        }
        
        public void ToggleSplashScreen(bool state)
        {
            if(splashScreenObject != null)
                splashScreenObject.SetActive(state);
        }
        
        public async UniTask Load(ISceneLoadService.SceneName sceneName)
        {
            switch (sceneName)
            {
                case ISceneLoadService.SceneName.SplashScene:
                    await LoadSplashScene();
                    break;
                case ISceneLoadService.SceneName.MainScene:
                    await LoadMainScene();
                    break;
                case ISceneLoadService.SceneName.GameScene:
                    await LoadGameScene();
                    break;
            }
        }
    }
}
