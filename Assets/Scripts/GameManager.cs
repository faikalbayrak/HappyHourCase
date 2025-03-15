using System;
using System.Collections;
using System.Collections.Generic;
using GameCore.Player;
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

    private List<GameObject> _createdEnemies = new List<GameObject>(); // Düşman listesini başlat
    private IObjectResolver _objectResolver;
    public List<GameObject> CreatedEnemies => _createdEnemies;

    [Inject]
    public void Init(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;

        SpawnPlayer();
        SpawnEnemy();
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(_playerPrefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation);
        player.GetComponent<PlayerController>().SetObjectResolver(_objectResolver);
    }

    private void SpawnEnemy()
    {
        if (_enemySpawnPoints == null || _enemySpawnPoints.Count == 0)
        {
            Debug.LogError("Enemy spawn points are not set!");
            return;
        }
        
        List<Transform> selectedSpawnPoints = GetRandomSpawnPoints(3);

        foreach (var spawnPoint in selectedSpawnPoints)
        {
            var enemy = Instantiate(_enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            _createdEnemies.Add(enemy);
        }
    }

    private List<Transform> GetRandomSpawnPoints(int count)
    {
        List<Transform> selectedPoints = new List<Transform>();

        if (_enemySpawnPoints.Count <= count)
        {
            selectedPoints.AddRange(_enemySpawnPoints);
        }
        else
        {
            List<Transform> availablePoints = new List<Transform>(_enemySpawnPoints);

            for (int i = 0; i < count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availablePoints.Count);
                selectedPoints.Add(availablePoints[randomIndex]);
                availablePoints.RemoveAt(randomIndex);
            }
        }

        return selectedPoints;
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