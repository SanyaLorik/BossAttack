using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class BotWalkManager : MonoBehaviour {
    private const float DESTINATION_CHANGE_THRESHOLD = 0.5f;
    
    [SerializeField] private BotManager _manager;

    [Header("Партиклы")]
    [SerializeField] private Transform[] _spawnPlaces;
    [SerializeField] private float _yToFind;
    
    
    public Action<bool> StartWandering;

    private CancellationTokenSource _botTokenSource;
    private Vector3 _lastDestination;

    
    private bool CanUseAgent => _navMeshHelper.CanUseAgent(_manager.Agent);
    
    
    [Inject] private GameData _gameData;
    [Inject] private NavMeshHelper _navMeshHelper;
    [Inject] private MapsToBattleChanger _mapsChanger;
    
    
    private void Awake() {
        _manager.Agent.updateRotation = false;
    }

    
    private void Update() {
        if (_manager.AnimatedLinkTraversal.IsJumpingTraversal)
            return;

        RotateByVelocity();
        MonitorMovement();
    }

    public void SetSpeed(float speed) {
        _manager.Agent.speed = speed;
    }
    
    public void SetStoppingDistance(float distance) {
        _manager.Agent.stoppingDistance = distance;
    }
    
    public void DisposeAllLogic() {
        UniTaskHelper.DisposeTask(ref _botTokenSource);
        _manager.BotJumpController.DisposeToken();
    }
        
    private async UniTask StartWanderingCycleAsync() {
        if (!gameObject.activeSelf) return;
        
        _botTokenSource = new CancellationTokenSource();

        float durationToStay = 0f;
        if (Random.value > 0.5f) {
            durationToStay = Random.Range(_gameData.TimeToStayAfterSpawn.From, _gameData.TimeToStayAfterSpawn.To);
        }
        
        await UniTask.WaitForSeconds(durationToStay, cancellationToken: _botTokenSource.Token);
        await LifeCycleAsync(_botTokenSource.Token);
    }
    
    
    private async UniTask LifeCycleAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            await UniTask.WaitUntil(() => CanUseAgent, cancellationToken: token);
            Vector3 target = GetTargetPoint(_spawnPlaces, _yToFind);
            _manager.Agent.SetDestination(target);
            
            await UniTask.WaitUntil(() => !_manager.Agent.pathPending && _manager.Agent.hasPath, cancellationToken: token);
            
            _manager.BotJumpController.Jump(token).Forget();

            await UniTask.WaitUntil(() => 
                    !_manager.Agent.pathPending && _manager.Agent.remainingDistance <= _manager.Agent.stoppingDistance,
                cancellationToken: token);

            float waitTime = Random.Range(
                _gameData.TimeToStayOnPoint.From, 
                _gameData.TimeToStayOnPoint.To);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
        }
    }

    public void StopWanderSpawn() {
        ResetLogic();
    }
    
    
    public void StartWanderSpawn() {
        ResetLogic();

        _manager.Agent.isStopped = false;
        StartWanderingCycleAsync().Forget();
    }

    
    public void ResetLogic()
    {
        DisposeAllLogic();
        if (_manager.Agent != null && _manager.Agent.enabled && _manager.Agent.isOnNavMesh)
        {
            _manager.Agent.velocity = Vector3.zero;
            _manager.Agent.ResetPath();

            _manager.Agent.Warp(_manager.Transform.position);
            _manager.Agent.nextPosition = _manager.Transform.position;

            _manager.Agent.isStopped = false;
        }

        _manager.WalkingParticles.Stop();
        StartWandering?.Invoke(false);
    }


    public void SetMovingStatus(bool enable) {
        if(!gameObject.activeSelf) return;
        _manager.Agent.isStopped = !enable;
        _manager.Agent.ResetPath();
    }

    
    public Vector3 GetTargetPoint(Transform point, float yToFind) {
        return _navMeshHelper.CalculateBotTargetPoint(point, yToFind);
    }
    
    
    public Vector3 GetTargetPoint(Transform[] points, float yToFind) {
        Transform point =  points.GetRandomElement();
        return _navMeshHelper.CalculateBotTargetPoint(point, yToFind);
    }


    public void SetAgentGoToPoint(Vector3 point) {
        if (!CanUseAgent) return;
        // Проверяем, реально ли изменилась цель
        if (Vector3.Distance(_lastDestination, point) > DESTINATION_CHANGE_THRESHOLD) {
            _manager.Agent.SetDestination(point);
            _lastDestination = point;
        }
        if (CanUseAgent)
            _manager.Agent.isStopped = false;
    }
    
    
    public async UniTask SetAgentGoToPointAsync(Vector3 point, CancellationToken token) {
        if (!CanUseAgent) return;

        _manager.Agent.SetDestination(point);

        await UniTask.WaitUntil(
            () => !token.IsCancellationRequested && CanUseAgent && !_manager.Agent.pathPending,
            cancellationToken: token
        );

        if (!CanUseAgent) return;

        await _manager.BotJumpController.Jump(token);

        if (!CanUseAgent) return;

        if (_manager.Agent.pathStatus != NavMeshPathStatus.PathComplete)
            return;

        await UniTask.WaitUntil(
            () => !token.IsCancellationRequested &&
                  CanUseAgent &&
                  !_manager.Agent.pathPending &&
                  _manager.Agent.remainingDistance <= _gameData.RunStoppingDistance,
            cancellationToken: token
        );
    }


    private void RotateByVelocity()
    {
        if (!_manager.Agent.enabled || !_manager.Agent.isOnNavMesh)
            return;

        Vector3 velocity = _manager.Agent.velocity;
        velocity.y = 0;

        // ВАЖНО: если агент почти стоит — НЕ крутим
        if (velocity.sqrMagnitude < 0.05f)
            return;

        _manager.Transform.rotation = Quaternion.Slerp(
            _manager.Transform.rotation,
            Quaternion.LookRotation(velocity),
            _gameData.RotationSpeed * Time.deltaTime
        );
    }
    
    
    private void MonitorMovement() {
        if (_manager.Agent.enabled && _manager.Agent.velocity.sqrMagnitude > 0.05f) {
            if (!_manager.WalkingParticles.IsPlaying) {
                _manager.WalkingParticles.Play();
                StartWandering?.Invoke(true);
            }
        }
        else {
            if (_manager.WalkingParticles.IsPlaying && !_manager.AnimatedLinkTraversal.IsJumpingTraversal) {
                _manager.WalkingParticles.Stop();
                StartWandering?.Invoke(false);
            }
        }
    }

    private void OnDestroy() {
        DisposeAllLogic();
    }
}