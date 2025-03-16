using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface IGameManagerService
    {
        public void SpawnEnemy(int count);
        public PlayerMovementDependencies GetPlayerMovementDependencies();
        public Transform GetVirtualCamPos();
        public List<GameObject> CreatedEnemies { get; }
    }
}
