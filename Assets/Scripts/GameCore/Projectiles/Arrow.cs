using Interfaces;
using Managers;
using UnityEngine;
using VContainer;

namespace GameCore.Projectiles
{
    public class Arrow : MonoBehaviour, IPooledObject
    {
        private IObjectResolver _objectResolver;
        private IGameManagerService _gameManagerService;
        private IObjectPoolService _objectPool;
        private bool _dependenciesInjected = false;

        public void SetObjectResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;

            if (_objectResolver != null)
            {
                _objectPool = _objectResolver.Resolve<IObjectPoolService>();
                _gameManagerService = _objectResolver.Resolve<IGameManagerService>();
                
                if (_gameManagerService != null)
                {
                    _dependenciesInjected = true;
                }
            }
        }
        public void OnObjectSpawn()
        {
            
        }
    }
}
