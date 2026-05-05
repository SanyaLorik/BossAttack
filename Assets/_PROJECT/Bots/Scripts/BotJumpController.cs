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
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private JumpParticlesController _jumpParticlesController;
    [SerializeField] private JumpParticlesController _landParticleController;

    
    private CancellationTokenSource _jumpTokenSource; 
    private bool _isJumping;
        
    private float _jumpForce;
    private float _jumpDuration;
    
    public Action<bool> Grounded;
    public Action OnJump;
    public Action OnDoubleJump;
    
    [Inject] private GameData _gameData;

    private void Start() {
        SetBigJump(false);
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
        CancellationToken jumpToken = _jumpTokenSource.Token;

        _isJumping = true;
        await UniTask.WaitUntil(() => 
                !_agent.pathPending &&
                _agent.remainingDistance <= jumpLength &&
                _agent.remainingDistance > _agent.stoppingDistance, 
            cancellationToken: jumpToken);

        FakeJump(jumpToken).Forget();
    }

    public void DisposeToken() {
        UniTaskHelper.DisposeTask(ref _jumpTokenSource);
    }
    
    
    private async UniTask FakeJump(CancellationToken token) {
        float height = _jumpForce;
        float t = 0f;

        _jumpParticlesController.Play();
        if (Random.value > 0.7f) {
            OnJump?.Invoke();
        }
        else {
            OnDoubleJump?.Invoke();
        }

        float startY = transform.position.y;
        Grounded?.Invoke(false);
        while (t < _jumpDuration && !token.IsCancellationRequested) {
            t += Time.deltaTime;
            float normalized = t / _jumpDuration;
            float yOffset = Mathf.Sin(normalized * Mathf.PI) * height;

            Vector3 pos = transform.position;
            pos.y = startY + yOffset;

            transform.position = pos;

            await UniTask.Yield(token);
        }
        Grounded?.Invoke(true);
        _landParticleController.Play();
        _isJumping = false;
    }
}