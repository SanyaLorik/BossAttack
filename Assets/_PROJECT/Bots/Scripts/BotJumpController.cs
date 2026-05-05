using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class BotJumpController : MonoBehaviour {
    [SerializeField] private bool _allowToJump;

    
    private CancellationTokenSource _jumpTokenSource; 
    private bool _isJumping;
        
    private float _jumpForce;
    private float _jumpDuration;
    private NavMeshAgent _agent => _manager.Agent;
    
    public Action<bool> Grounded;
    public Action OnJump;
    public Action OnDoubleJump;
    
    [Inject] private GameData _gameData;
    [Inject] private BotManager _manager;

    
    private void Start() {
        SetBigJump(false);
    }
    
    public void DisposeToken() {
        UniTaskHelper.DisposeTask(ref _jumpTokenSource);
    }

    public void SetBigJump(bool bigJump) {
        _jumpForce = bigJump ? _gameData.BotJumpBonusHeight : _gameData.BotDefaultJumpHeight;
        _jumpDuration = bigJump ? _gameData.BotJumpBonusDuration : _gameData.BotJumpDuration;
    }
    
    
    public async UniTask Jump(CancellationToken token) {
        if (_allowToJump) return;
        if (Random.value > _gameData.ChanceToJump) return;
        
        float startPathLength = _agent.remainingDistance;
        float jumpLength = startPathLength / Random.Range(1.5f, 2f);


        UniTaskHelper.DisposeTask(ref _jumpTokenSource);
        _jumpTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

        _isJumping = true;
        await UniTask.WaitUntil(() => 
                !_agent.pathPending &&
                _agent.remainingDistance <= jumpLength &&
                _agent.remainingDistance > _agent.stoppingDistance, 
            cancellationToken: _jumpTokenSource.Token);

        FakeJump(_jumpTokenSource.Token).Forget();
    }


    
    
    private async UniTask FakeJump(CancellationToken token) {
        float height = _jumpForce;
        float t = 0f;

        PlayVisual();

        float startY = _manager.Transform.position.y;
        while (t < _jumpDuration && !token.IsCancellationRequested) {
            t += Time.deltaTime;
            float normalized = t / _jumpDuration;
            float yOffset = Mathf.Sin(normalized * Mathf.PI) * height;

            Vector3 pos = _manager.Transform.position;
            pos.y = startY + yOffset;

            _manager.Transform.position = pos;

            await UniTask.Yield(token);
        }
        Grounded?.Invoke(true);
        _manager.LandParticles.Play();
        _isJumping = false;
    }

    private void PlayVisual() {
        _manager.JumpParticles.Play();
        if (Random.value > 0.7f) {
            OnJump?.Invoke();
        }
        else {
            OnDoubleJump?.Invoke();
        }
        Grounded?.Invoke(false);
    }
}