using Interfaces;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SceneScopes
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameManager gameManager;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameManager).As<IGameManagerService>();
        }
    }
}
