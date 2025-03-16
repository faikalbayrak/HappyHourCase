using UnityEngine;

namespace Interfaces
{
    public interface IObjectPoolService
    {
        public void SetGameManager(IGameManagerService gameManagerService);
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation);
        public void ReturnToPool(string tag, GameObject objectToReturn);
    }
}
