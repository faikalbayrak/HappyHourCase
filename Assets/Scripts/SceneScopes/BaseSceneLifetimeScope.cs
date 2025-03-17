using Interfaces;
using Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SceneScopes
{
    public class BaseSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private SceneLoadManager sceneLoadManager;
        [SerializeField] private ObjectPool objectPoolManager;
        [SerializeField] private AudioManager audioManager;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(sceneLoadManager).As<ISceneLoadService>();
            builder.RegisterInstance(objectPoolManager).As<IObjectPoolService>();
            builder.RegisterInstance(audioManager).As<IAudioService>();
        }
    }
}
