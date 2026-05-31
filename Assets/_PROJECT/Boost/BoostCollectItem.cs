using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class BoostCollectItem : MonoBehaviour, IPlayer {
    [SerializeField] private DamageVisualizer _damageVisualizer;
    [SerializeField] private ParticleSystem _psToDestroy;
    [SerializeField] private GameObject[] _visualToHide;
    [SerializeField] private Collider _colliderToHide;
    [SerializeField] private Transform _pointToAtack;
    
    private PlayerBoostBoxesSystem _playerBoostBoxesSystem;
    private GameData _gameData;

    public TargetType TargetType => TargetType.Boost;

    public IDamagable Damagable => _damagable;
    private Damagable _damagable;


    public Transform PointToAtack => _pointToAtack;
    public Transform Transform  => transform;

    public void TeleportToPoint(Vector3 point) {
        Transform.position = point;
    }
    
    
    [Inject]
    private void Initialize(PlayerBoostBoxesSystem playerBoostBoxesSystem, GameData gameData) {
        _playerBoostBoxesSystem = playerBoostBoxesSystem;
        _gameData = gameData;
        
        _damagable = new Damagable(Transform, this);
        _damagable.SetMaxHpGetter(() => _gameData.BoostBoxHp);
        _damagable.Respawn(true);
        _damagable.DamagableDied += OnDamagableDied;
        
        _damageVisualizer.SetDamagable(_damagable);
    }



    private void OnDamagableDied(IDamagable damagable) {
        _damageVisualizer.Unsubscribe();
        Damagable.DamagableDied -= OnDamagableDied;
        
        _playerBoostBoxesSystem.PlusOne();
        _psToDestroy.Play();
        SetVisualModelState(false);
    }

    public void SetVisualModelState(bool enable) {
        _colliderToHide.enabled = enable;
        if (enable) {
            _visualToHide.ActiveSelf();
        }
        else {
            _visualToHide.DisactiveSelf();
        }
    }
    

    
    
    #region IPlayer
    public void SetPlayStatus(bool goPlay) { }

    public void RotateToTarget(Vector3 point) { }

    public void SetMovingStatus(bool enable) { }

    
    public IPusher Pusher { get; }
    public IBonusUser BonusUser { get; }
    public bool IsPlaying { get; }
    #endregion
    
}