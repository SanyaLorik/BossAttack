using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class BotPushBehaviour  : MonoBehaviour {

    
    public bool IsPushed { get; private set; }
    public Action<bool> Grounded;
    public Action FallAfterPush;
    
    private CancellationTokenSource _pushTokenSource;
    
    private NavMeshAgent Agent => _manager.Agent;
    
    [Inject] MapsToBattleChanger _mapsChanger;
    [Inject] GameData _gameData;
    [Inject] BotsMainManager _mainManager;
    [Inject] BotManager _manager;
    
    
    public void PushAway(Vector3 direction) {
        if (IsPushed) return;

        UniTaskHelper.DisposeTask(ref _pushTokenSource);
        _pushTokenSource = new CancellationTokenSource();

        PushAwayAsync(direction, _pushTokenSource.Token).Forget();
    }
    
    
    public void DisposeToken() {
        UniTaskHelper.DisposeTask(ref _pushTokenSource);
    }
    
    
    private async UniTask EnterPushModeAsync() {
        IsPushed = true;

        Agent.isStopped = true;
        Agent.ResetPath();
        Agent.velocity = Vector3.zero;

        Agent.enabled = false;

        await UniTask.Yield(PlayerLoopTiming.Update);
    }


    
    private async UniTask PushAwayAsync(Vector3 direction, CancellationToken token) {
        await EnterPushModeAsync();
        await PushJump(direction, token);
    }
    
    
    private async UniTask PushJump(Vector3 direction, CancellationToken token) {
        float height = _gameData.BotUpPushRatio;
        float duration = _gameData.PushTime;
        float force = _gameData.BotPushForce;

        Vector3 startPos = transform.position;
        Vector3 velocity = direction.normalized * force;

        float startY = startPos.y;
        float t = 0f;

        Grounded?.Invoke(false);
        _manager.JumpParticles.Play();

        // ПАРАБОЛА
        _manager.Rb.isKinematic = true;
        _manager.Rb.useGravity = false;

        while (t < duration && !token.IsCancellationRequested) {
            t += Time.deltaTime;
            float n = t / duration;

            Vector3 targetPos = startPos + velocity * t;
            targetPos.y = startY + Mathf.Sin(n * Mathf.PI) * height;

            Vector3 delta = targetPos - transform.position;
            float dist = delta.magnitude;

            if (dist > 0f) {
                Vector3 dir = delta / dist;

                float extra = 0.15f;
                float castDist = dist + extra;

                float radius = 0.3f;
                float heightCapsule = 1.8f;

                Vector3 center = transform.position;
                Vector3 p1 = center + Vector3.up * (heightCapsule / 2 - radius);
                Vector3 p2 = center - Vector3.up * (heightCapsule / 2 - radius);

                if (Physics.CapsuleCast(p1, p2, radius, dir, out RaycastHit hit, castDist)) {
                    Vector3 safePos = hit.point - dir * 0.05f;
                    transform.position = safePos;

                    break;
                }
            }

            transform.position = targetPos;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        // ВКЛЮЧАЕМ ФИЗИКУ
        await FallWithPhysics(token);
    }
    
    private async UniTask FallWithPhysics(CancellationToken token)
    {
        float maxTime = 4f;
        float t = 0f;

        _manager.Rb.isKinematic = false;
        _manager.Rb.useGravity = true;

        _manager.Rb.angularVelocity = Vector3.zero;
        _manager.Rb.linearVelocity = Vector3.down * _gameData.BotFallSpeed;
        

        while (!token.IsCancellationRequested)
        {
            t += Time.fixedDeltaTime;

            Vector3 pos = _manager.Rb.position;

            // ГЛАВНОЕ: ищем NavMesh напрямую (НЕ через коллайдер)
            if (NavMesh.SamplePosition(pos, out NavMeshHit navHit, _mapsChanger.FallBotFindSamplePosition, NavMesh.AllAreas))
            {
                // приземляемся только если падаем вниз
                if (_manager.Rb.linearVelocity.y <= 0f)
                {
                    FinishLanding(navHit.position);
                    return;
                }
            }

            // Дополнительная страховка (если уже ниже NavMesh)
            if (!NavMesh.SamplePosition(pos, out _, _mapsChanger.FallBotFindSamplePosition, NavMesh.AllAreas))
            {
                if (NavMesh.SamplePosition(pos + Vector3.up * 5f, out NavMeshHit fallbackHit, 10f, NavMesh.AllAreas))
                {
                    FinishLanding(fallbackHit.position);
                    return;
                }
            }

            // выпали в бездну
            if (t > maxTime || pos.y < -200f)
            {
                Debug.Log("Bot fell into void");
                _mainManager.FellInVoidBot(this);

                IsPushed = false;
                _manager.StopPhys();
                Grounded?.Invoke(true);
                return;
            }

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
    }
    
    
    private void FinishLanding(Vector3 navMeshPos) {
        IsPushed = false;
        _manager.StopPhys();

        Agent.enabled = true;

        if (NavMesh.SamplePosition(navMeshPos, out var hit, 1f, NavMesh.AllAreas)) {
            Agent.Warp(hit.position);
        }

        // ВАЖНО: проверка перед любыми действиями
        if (Agent.isOnNavMesh) {
            Agent.nextPosition = Agent.transform.position;
            Agent.ResetPath();
            Agent.isStopped = false;
            FallAfterPush?.Invoke();
        }
        else {
            Debug.LogWarning("Agent not on NavMesh after landing");
            _mainManager.FellInVoidBot(this);
            return;
        }

        Grounded?.Invoke(true);
        _manager.LandParticles.Play();
    }
    
    
   
}