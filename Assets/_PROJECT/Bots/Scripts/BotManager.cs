using System;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class BotManager : MonoBehaviour, IPlayer {
    [field: SerializeField] public bool ShowInSpawn { get; private set; }
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public BotWalkManager BotWalkManager { get; private set; }
    [field: SerializeField] public BotPushBehaviour BotPushBehaviour { get; private set; }
    [field: SerializeField] public BotBonusController BotBonusController { get; private set; }
    [field: SerializeField] public BotJumpController BotJumpController { get; private set; }

    [SerializeField] private BotAnimator _botAnimator;
    [SerializeField] private BotMonolog _botMonolog;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private PlayerRoleBehaviour _roleBehaviour;
    [SerializeField] private Rigidbody _rb;

    public IBonusUser BonusUser => BotBonusController;
    public IPusher Pusher { get; private set; }

    public bool IsPlaying { get; private set; }
    public bool IsPushed => BotPushBehaviour.IsPushed;
    public event Action<bool> PlayerStatusChanged;


    public string Nickname => _botMonolog.NickName;
    public PlayerRoleBehaviour RoleBehaviour => _roleBehaviour;
    private bool CanUseAgent => _navMeshHelper.CanUseAgent(_agent);


    [Inject] private GameData _gameData;
    [Inject] private SpawnManager _spawn;
    [Inject] private MapsToBattleChanger _mapsManager;
    [Inject] private NavMeshHelper _navMeshHelper;



    private void Start() {
        if (!ShowInSpawn)
            gameObject.DisactiveSelf();
        else
            SetStartWanderIfActive(true);
    }

    private void DisposeAllTokens() {
        BotWalkManager.DisposeAllLogic();
    }


    public void PushAway(Vector3 direction) {
        // Перед пушем останавливаем всю логику прыжки бля бег и тп
        DisposeAllTokens();
        BotPushBehaviour.PushAway(direction);
    }
    
    
    private void SetStartWanderIfActive(bool startWander) {
        if (ShowInSpawn == false) return;
        
        if(startWander) BotWalkManager.StartWanderSpawn();
        else BotWalkManager.StopWanderSpawn();
    }


    public void SetPlayStatus(bool goPlay) {
        BonusUser.SetDefault();
        
        PlayerStatusChanged?.Invoke(goPlay);
        IsPlaying = goPlay;
        _agent.enabled = true;
        StopPhys();
        BotWalkManager.DisposeAllLogic();
        
        gameObject.SetActive(ShowInSpawn || goPlay);
        
        if (goPlay) {
            ActiveBotInGame();
            SetBotStfu();
        }
        // Возвращение на спавн
        else {
            Debug.Log($"Возвращение на спавн игрока {_botMonolog.NickName} in {_spawn.SpawnPoint.position}");
            Debug.Log($"Игрок play статус {IsPlaying} in {_spawn.SpawnPoint.position}");
            SetBotStateBeforeGame();
            TeleportToPoint(_spawn.SpawnPoint.position);
            ChangeNicknameByChance();
        }
        SetStartWanderIfActive(!goPlay);
    }

    private void ChangeNicknameByChance() {
        if(Random.value > _gameData.ChanceToBotChangeNicknameAfterPlay) return;
        _botMonolog.ChangeNickname();
    }

    public void SetPlayStatusSilent(bool goPlay) {
        IsPlaying = goPlay;
    }


    public void TeleportToPoint(Vector3 pos) {
            // 1. СНАЧАЛА отменяем всё у BotWalkManager
            BotWalkManager.ResetLogic(); 
        
            if (NavMesh.SamplePosition(pos, out var hit, _mapsManager.CurrentMapYToFind, NavMesh.AllAreas)) {
                _agent.enabled = false;
                transform.position = hit.position;
                _agent.enabled = true;
            
                // После включения агент может ещё не быть isOnNavMesh
                // Даём кадр на инициализацию через ForceUpdateCanvases не поможет,
                // лучше просто проверить
                if (_agent.isOnNavMesh) {
                    _agent.isStopped = true;
                }
                // Debug.Log($"Телепорт: {transform.position}");
            } 
            else
            {
                Debug.LogError($"SamplePosition НЕ нашел точку рядом с {pos}");
            }
    }

    
    public void SetMovingStatus(bool enable) {
        BotWalkManager.SetMovingStatus(enable);
    }

    


    public void RotateToTarget(Vector3 targetPosition) {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }
    
    
    private void SetBotStateBeforeGame() {
        if (ShowInSpawn) {
            _agent.ActiveSelf();
        }
        else {
            _agent.DisactiveSelf();
        }
    }

    
    private void ActiveBotInGame() {
        if (ShowInSpawn == false) {
            _agent.ActiveSelf();
        }
    }
    

    public void SetBotSpeak() {
        Debug.Log("Set bot speak");
        if (!IsPlaying) {
            _botMonolog.SaySomething();
        }
    }

    public void SetBotStfu() {
        _botMonolog.Stfu();
    }

    public void StopPhys() {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        
        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

}