using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface IGameManagerService
    {
        public PlayerMovementDependencies GetPlayerMovementDependencies();
        public Transform GetVirtualCamPos();
        public List<GameObject> CreatedEnemies { get; }
    }
}
