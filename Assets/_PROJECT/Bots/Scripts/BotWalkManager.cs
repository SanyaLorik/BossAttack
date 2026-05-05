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
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private NavMeshAgent _agent;
    
    
    [Header("Партиклы")]
    [SerializeField] private DualLegParticles _walkingParticles;
    [SerializeField] private Transform[] _spawnPlaces;
    [SerializeField] private float _yToFind;
    [SerializeField] private AnimatedLinkTraversal _animatedLinkTraversal;
    [SerializeField] private BotJumpController jumpController;
    
    
    public Action<bool> StartWandering;

    private CancellationTokenSource _botTokenSource;
    private Vector3 _lastDestination;
    private bool CanUseAgent => _navMeshHelper.CanUseAgent(_agent);
    
    
    [Inject] private GameData _gameData;
    [Inject] private NavMeshHelper _navMeshHelper;
    [Inject] private BotsMainManager _mainManager;
    [Inject] private MapsToBattleChanger _mapsChanger;
    
    
    private void Awake() {
        _agent.updateRotation = false;
    }

    
    private void Update() {
        RotateByVelocity();
        MonitorMovement();
    }
    
    public void DisposeAllLogic() {
        UniTaskHelper.DisposeTask(ref _botTokenSource);
        jumpController.DisposeToken();
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
            _agent.SetDestination(target);
            
            await UniTask.WaitUntil(() => !_agent.pathPending && _agent.hasPath, cancellationToken: token);
            
            jumpController.Jump(token).Forget();

            await UniTask.WaitUntil(() => 
                    !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance,
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

        _agent.isStopped = false;
        StartWanderingCycleAsync().Forget();
    }

    
    public void ResetLogic() {
        Debug.Log("StartWanderSpawn");
        
        DisposeAllLogic();

        if (_agent != null && _agent.enabled && _agent.isOnNavMesh) {
            _agent.velocity = Vector3.zero;
            _agent.ResetPath();
            _agent.nextPosition = transform.position;
            _agent.isStopped = false;
        }

        _walkingParticles.Stop();
        StartWandering?.Invoke(false);
    }


    public void SetMovingStatus(bool enable) {
        if(!gameObject.activeSelf) return;
        _agent.isStopped = !enable;
        _agent.ResetPath();
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
            _agent.SetDestination(point);
            _lastDestination = point;
        }
        if (CanUseAgent)
            _agent.isStopped = false;
    }
    
    
    public async UniTask SetAgentGoToPointAsync(Vector3 point, CancellationToken token) {
        if (!CanUseAgent) return;

        _agent.SetDestination(point);

        await UniTask.WaitUntil(
            () => !token.IsCancellationRequested && CanUseAgent && !_agent.pathPending,
            cancellationToken: token
        );

        if (!CanUseAgent) return;

        await jumpController.Jump(token);

        if (!CanUseAgent) return;

        if (_agent.pathStatus != NavMeshPathStatus.PathComplete)
            return;

        await UniTask.WaitUntil(
            () => !token.IsCancellationRequested &&
                  CanUseAgent &&
                  !_agent.pathPending &&
                  _agent.remainingDistance <= _gameData.RunStoppingDistance,
            cancellationToken: token
        );
    }


    private void RotateByVelocity() {
        Vector3 velocity = _agent.velocity;
        velocity.y = 0;
        
        
        if (velocity.sqrMagnitude < 0.001f)
            return;
    
        float sqrMag = velocity.sqrMagnitude;
    
        // Ранний выход если стоим (уже есть)
        if (sqrMag < 0.01f && !_animatedLinkTraversal.IsJumpingTraversal)
            return;
    
        // ДОПОЛНИТЕЛЬНО: не вращать если почти смотрим куда надо
        Quaternion targetRotation = Quaternion.LookRotation(velocity);
    
        // Если уже почти повернуты - пропускаем Slerp
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            return;
    
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _gameData.RotationSpeed * Time.deltaTime
        );
    }
    
    
    private void MonitorMovement() {
        if (_agent.enabled && _agent.velocity.sqrMagnitude > 0.05f) {
            if (!_walkingParticles.IsPlaying) {
                _walkingParticles.Play();
                StartWandering?.Invoke(true);
            }
        }
        else {
            if (_walkingParticles.IsPlaying && !_animatedLinkTraversal.IsJumpingTraversal) {
                _walkingParticles.Stop();
                StartWandering?.Invoke(false);
            }
        }
    }

    private void OnDestroy() {
        DisposeAllLogic();
    }
}