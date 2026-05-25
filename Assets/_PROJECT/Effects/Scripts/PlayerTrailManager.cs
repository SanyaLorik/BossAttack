using SanyaBeerExtension;
using UnityEngine;


public class PlayerTrailManager : MonoBehaviour {
    [SerializeField] private GameObject[] _defaultTrails;
    [SerializeField] private GameObject[] _speedTrails;
    [SerializeField] private GameObject[] _jumpTrails;
    [SerializeField] private GameObject[] _invincibleTrails;
    
    
    private IPlayer _player;

    private IPlayer Player {
        get {
            _player ??= GetComponentInParent<IPlayer>();
            return _player;
        }
    }
    
    private BonusStatus _currentBonusStatus = BonusStatus.Default;

    
    private void Start() {
        OffAll();
        _defaultTrails.ActiveSelf();
    }

    
    private void OnEnable() {
        Player.BonusUser.BonusStatusChanged += OnBonusStatusChanged;
    }

    
    private void OnDisable() {
        Player.BonusUser.BonusStatusChanged -= OnBonusStatusChanged;
    }


    private void OnBonusStatusChanged(BonusStatus status, bool enable) {
        // Eсли чето врубаем обязательно все предыдущие вырубаем
        if (enable) {
            OffAll();
            _currentBonusStatus = status;
            if (status == BonusStatus.SuperSpeed) {
                _speedTrails.ActiveSelf();
            }
            else if(status == BonusStatus.SuperJump) {
                _jumpTrails.ActiveSelf();
            }
            else if (status == BonusStatus.Invincible) {
                _invincibleTrails.ActiveSelf();
            }
        }
        // Если вырубился ласт бонус, включаем дефолт
        else {
            if (status == _currentBonusStatus) {
                _currentBonusStatus = BonusStatus.Default;
                OffAll();
                _defaultTrails.ActiveSelf();
            }
        }
        
    }

    
    private void OffAll() {
        _speedTrails.DisactiveSelf();
        _jumpTrails.DisactiveSelf();
        _invincibleTrails.DisactiveSelf();
        _defaultTrails.DisactiveSelf();
    }
    
}
