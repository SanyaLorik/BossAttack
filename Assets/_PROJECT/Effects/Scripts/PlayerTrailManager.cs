using SanyaBeerExtension;
using UnityEngine;
using Zenject;


public class PlayerTrailManager : MonoBehaviour {
    [SerializeField] private GameObject[] _defaultTrails;
    [SerializeField] private GameObject[] _speedTrails;
    [SerializeField] private GameObject[] _jumpTrails;
    [SerializeField] private GameObject[] _invincibleTrails;
    
    
    [Inject] IPlayer _player;
    
    private BonusStatus _currentBonusStatus = BonusStatus.Default;

    
    private void Start() {
        OffAll();
        _defaultTrails.ActiveSelf();
    }

    
    private void OnEnable() {
        _player.BonusUser.BonusStatusChanged += OnBonusStatusChanged;
    }

    
    private void OnDisable() {
        _player.BonusUser.BonusStatusChanged -= OnBonusStatusChanged;
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
