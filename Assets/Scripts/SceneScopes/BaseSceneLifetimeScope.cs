using Interfaces;
using Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BaseSceneLifetimeScope : LifetimeScope
{
    [SerializeField] private SceneLoadManager sceneLoadManager;
    [SerializeField] private ObjectPool objectPoolManager;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(sceneLoadManager).As<ISceneLoadService>();
        builder.RegisterInstance(objectPoolManager).As<IObjectPoolService>();
    }
}
