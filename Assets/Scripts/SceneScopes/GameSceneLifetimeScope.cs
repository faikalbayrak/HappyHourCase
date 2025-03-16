using Interfaces;
using Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SceneScopes
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private SkillManager skillManager;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameManager).As<IGameManagerService>();
            builder.RegisterInstance(skillManager).As<ISkillManagerService>();
        }
    }
}
