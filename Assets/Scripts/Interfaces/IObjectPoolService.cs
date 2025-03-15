using UnityEngine;

namespace Interfaces
{
    public interface IObjectPoolService
    {
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation);
        public void ReturnToPool(string tag, GameObject objectToReturn);
    }
}
