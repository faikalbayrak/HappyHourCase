using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using GameCore.Enemy;
using GameCore.Player;
using Interfaces;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameManager : MonoBehaviour, IGameManagerService
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private List<Transform> _enemySpawnPoints;

    [Header("Skill System Dependecies")] 
    [SerializeField] private SkillManager _skillManager;
    [SerializeField] private Animator _skillPanelAnimator;
    [SerializeField] private List<Image> _skillsEnabledFrames;
    
    [Header("Player Dependencies")]
    [SerializeField] private FloatingJoystick _joyStick;
    [SerializeField] private Transform _cameraFollow;
    [SerializeField] private Transform _virtualCameraPos;

    private List<GameObject> _createdEnemies = new List<GameObject>();
    private IObjectResolver _objectResolver;
    private IObjectPoolService _objectPoolService;
    private CinemachineImpulseSource _cinemachineImpulseSource;
    
    private bool _isSkillPanelOpened = false;
    private static readonly int Open = Animator.StringToHash("Open");
    private static readonly int Close = Animator.StringToHash("Close");
    
    public List<GameObject> CreatedEnemies => _createdEnemies;

    private void Awake()
    {
        _cinemachineImpulseSource = _virtualCameraPos.GetComponent<CinemachineImpulseSource>();
    }

    [Inject]
    public void Init(IObjectResolver objectResolver, IObjectPoolService objectPoolService)
    {
        _objectResolver = objectResolver;
        _objectPoolService = objectPoolService;
        
        _objectPoolService.SetGameManager(this);
        
        SpawnPlayer();
        SpawnEnemy(5);
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(_playerPrefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation);
        player.GetComponent<PlayerController>().SetObjectResolver(_objectResolver);
    }

    public void SpawnEnemy(int count)
    {
        if (_enemySpawnPoints == null || _enemySpawnPoints.Count == 0)
        {
            Debug.LogError("Enemy spawn points are not set!");
            return;
        }
        
        List<Transform> selectedSpawnPoints = GetRandomSpawnPoints(count);

        foreach (var spawnPoint in selectedSpawnPoints)
        {
            GameObject enemy = _objectPoolService.SpawnFromPool("Enemy", spawnPoint.position, spawnPoint.rotation);
            if (enemy != null)
            {
                if (enemy.TryGetComponent<EnemyController>(out var enemyController))
                {
                    enemyController.SetIGameManagerService(this);
                }
                
                if(!_createdEnemies.Contains(enemy))
                    _createdEnemies.Add(enemy);
            }
        }
    }

    private List<Transform> GetRandomSpawnPoints(int count)
    {
        List<Transform> selectedPoints = new List<Transform>();
        
        HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
        foreach (var enemy in _createdEnemies)
        {
            if (enemy.activeInHierarchy)
            {
                occupiedPositions.Add(enemy.transform.position);
            }
        }
        
        List<Transform> availablePoints = new List<Transform>();
        foreach (var spawnPoint in _enemySpawnPoints)
        {
            if (!occupiedPositions.Contains(spawnPoint.position))
            {
                availablePoints.Add(spawnPoint);
            }
        }

        if (availablePoints.Count == 0)
        {
            Debug.LogWarning("No available spawn points!");
            return selectedPoints;
        }
        
        int maxSelection = Mathf.Min(count, availablePoints.Count);
        for (int i = 0; i < maxSelection; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availablePoints.Count);
            selectedPoints.Add(availablePoints[randomIndex]);
            availablePoints.RemoveAt(randomIndex);
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

    public void ExecuteCinemachineImpulse()
    {
        _cinemachineImpulseSource.GenerateImpulse(.25f);
    }

    public void ToggleSkillPanelActive()
    {
        if (_isSkillPanelOpened)
        {
            _skillPanelAnimator.SetTrigger(Close);
        }
        else
        {
            _skillPanelAnimator.SetTrigger(Open);
        }

        _isSkillPanelOpened = !_isSkillPanelOpened;
    }

    public void OnClickSkillButton(int skill)
    {
        if (!_skillsEnabledFrames[skill].enabled)
        {
            _skillsEnabledFrames[skill].enabled = true;
        }
        else
        {
            _skillsEnabledFrames[skill].enabled = false;
        }

        _skillManager.ToggleSkill(skill);
    }

    public Transform GetNearestEnemy(Transform self)
    {
        Transform nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in _createdEnemies)
        {
            if (!enemy.activeInHierarchy || enemy.transform == self) 
                continue;

            float distance = Vector3.Distance(self.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
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