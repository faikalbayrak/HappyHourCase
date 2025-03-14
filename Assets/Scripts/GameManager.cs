using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Player;
using UnityEngine;
using VContainer;

public class GameManager : MonoBehaviour, IGameManagerService
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private List<Transform> _enemySpawnPoints;
    
    [Header("Player Dependencies")]
    [SerializeField] private FloatingJoystick _joyStick;
    [SerializeField] private Transform _cameraFollow;
    [SerializeField] private Transform _virtualCameraPos;
    
    private List<GameObject> _createdEnemies;
    private IObjectResolver _objectResolver;
    public List<GameObject> CreatedEnemies => _createdEnemies;

    [Inject]
    public void Init(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(_playerPrefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation);
        player.GetComponent<PlayerController>().SetObjectResolver(_objectResolver);
    }

    private void SpawnEnemy()
    {
        
    }

    public PlayerMovementDependencies GetPlayerMovementDependencies()
    {
        return new PlayerMovementDependencies(_joyStick, _cameraFollow);
    }

    public Transform GetVirtualCamPos()
    {
        return _virtualCameraPos;
    }
}
public class PlayerMovementDependencies
{
    public FloatingJoystick JoyStick { get; set; }
    public Transform CameraFollow { get; set; }

    public PlayerMovementDependencies(FloatingJoystick joyStick, Transform cameraFollow)
    {
        JoyStick = joyStick;
        CameraFollow = cameraFollow;
    }
}