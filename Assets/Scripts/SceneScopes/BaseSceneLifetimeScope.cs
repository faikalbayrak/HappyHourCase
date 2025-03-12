using Interfaces;
using Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BaseSceneLifetimeScope : LifetimeScope
{
    [SerializeField] private SceneLoadManager sceneLoadManager;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(sceneLoadManager).As<ISceneLoadService>();
    }
}
