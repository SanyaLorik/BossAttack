using System;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;


[RequireComponent(typeof(NavMeshAgent))]
public class BotManager : MonoBehaviour, IPlayer {
    [field: SerializeField] public TargetType TargetType { get; private set; }
    
    [SerializeField] private GameObject _playerSkin;
    [field: SerializeField] public bool IsBoss { get; private set; }
    [field: SerializeField] public AbilitySystem Ability { get; private set; }
    
    [field: SerializeField] public bool ShowInSpawn { get; private set; }
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public Transform PointToAtack { get; private set; }
    
    [field: SerializeField] public BotWalkManager BotWalkManager { get; private set; }
    [field: SerializeField] public BotPushBehaviour BotPushBehaviour { get; private set; }
    [field: SerializeField] public BotBonusController BotBonusController { get; private set; }
    [field: SerializeField] public BotJumpController BotJumpController { get; private set; }

    [field: SerializeField] public BotAnimator BotAnimator  { get; private set; }
    [field: SerializeField] public BotMonolog BotMonolog { get; private set; }
    [field: SerializeField] public AnimatedLinkTraversal AnimatedLinkTraversal { get; private set; }
    [field: SerializeField] public Rigidbody Rb  { get; private set; }
    
    [field: SerializeField] public NavMeshAgent Agent  { get; private set; }
    
    [field: Header("Particles")]
    [field: SerializeField] public JumpParticlesController JumpParticles  { get; private set; }
    [field: SerializeField] public JumpParticlesController LandParticles  { get; private set; }
    [field: SerializeField] public DualLegParticles WalkingParticles  { get; private set; }
    [SerializeField] private DamageVisualizer _damageVisualizer;
    [SerializeField] private BotWanderingInBattle _botWanderingInBattle;
    
    
    public IBonusUser BonusUser => BotBonusController;
    public IDamagable Damagable => _damagable;
    
    
    private Damagable _damagable;

    
    public void SetVisualModelState(bool enable) {
        _playerSkin.SetActive(enable);
    }

    
    public IPusher Pusher { get; private set; }

    public bool IsPlaying { get; private set; }
    public bool IsPushed => BotPushBehaviour.IsPushed;
    public event Action<bool> PlayerStatusChanged;


    public string Nickname => BotMonolog.NickName;
    private bool CanUseAgent => _navMeshHelper.CanUseAgent(Agent);

    [Inject] private GameData _gameData;
    [Inject] private RespawnManager _respawn;
    [Inject] private MapsToBattleChanger _mapsManager;
    [Inject] private NavMeshHelper _navMeshHelper;
    
    [Inject] private PlayerStaticStatsCalculator _playerStaticStatsCalculator;

    
    private void Awake() {
        _damagable = new Damagable(Transform, this);
        _damageVisualizer.SetDamagable(_damagable);
        if (!IsBoss) {
            _damagable.SetMaxHpGetter(() => _playerStaticStatsCalculator.PlayerHp);
            _damageVisualizer.FastHide();
        }
    }


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
        if (ShowInSpawn == false || IsBoss) return;
        
        if(startWander) BotWalkManager.StartWanderSpawn();
        else BotWalkManager.StopWanderSpawn();
    }


    public void SetPlayStatus(bool goPlay) {
        if(IsBoss) return;
        BonusUser.SetDefault();
        
        PlayerStatusChanged?.Invoke(goPlay);
        IsPlaying = goPlay;
        Agent.enabled = true;
        StopPhys();
        BotWalkManager.DisposeAllLogic();
        
        gameObject.SetActive(ShowInSpawn || goPlay);
        
        if (goPlay) {
            ActiveBotInGame();
            SetBotStfu();
            _damagable.SetSpawned();
            _botWanderingInBattle.StartWandering();
        }
        // Возвращение на спавн
        else {
            _botWanderingInBattle.StopWandering();
            Debug.Log($"Возвращение на спавн игрока {BotMonolog.NickName} in {_respawn.SpawnPoint.position}");
            Debug.Log($"Игрок play статус {IsPlaying} in {_respawn.SpawnPoint.position}");
            SetBotStateBeforeGame();
            TeleportToPoint(_respawn.SpawnPoint.position);
            ChangeNicknameByChance();
            _damageVisualizer.FastHide();
        }
        SetStartWanderIfActive(!goPlay);
    }

    private void ChangeNicknameByChance() {
        if(Random.value > _gameData.ChanceToBotChangeNicknameAfterPlay) return;
        BotMonolog.ChangeNickname();
    }

    public void SetPlayStatusSilent(bool goPlay) {
        IsPlaying = goPlay;
    }


    public void TeleportToPoint(Vector3 pos) {
            // 1. СНАЧАЛА отменяем всё у BotWalkManager
            BotWalkManager.ResetLogic(); 
        
            if (NavMesh.SamplePosition(pos, out var hit, _mapsManager.CurrentMapYToFind, NavMesh.AllAreas)) {
                Agent.enabled = false;
                transform.position = hit.position;
                Agent.enabled = true;
            
                // После включения агент может ещё не быть isOnNavMesh
                // Даём кадр на инициализацию через ForceUpdateCanvases не поможет,
                // лучше просто проверить
                if (Agent.isOnNavMesh) {
                    Agent.isStopped = true;
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
            Agent.ActiveSelf();
        }
        else {
            Agent.DisactiveSelf();
        }
    }

    
    private void ActiveBotInGame() {
        if (ShowInSpawn == false) {
            Agent.ActiveSelf();
        }
    }
    

    public void SetBotSpeak() {
        if (!IsPlaying) {
            BotMonolog.SaySomething();
        }
    }

    public void SetBotStfu() {
        BotMonolog.Stfu();
    }

    public void StopPhys() {
        Rb.linearVelocity = Vector3.zero;
        Rb.angularVelocity = Vector3.zero;
        
        Rb.isKinematic = true;
        Rb.useGravity = false;
    }

}