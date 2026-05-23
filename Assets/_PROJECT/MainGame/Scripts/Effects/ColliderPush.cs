using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ColliderPush : MonoBehaviour {
    [SerializeField] private Collider _collider;
    public IPusher LastPlayerContact { get; private set; }
    
    private float _lastRepulseTime = -999f;
  
    [Inject] GameData _gameData;
    
    
    private IPlayer _player;

    private IPlayer Player {
        get {
            _player ??= GetComponentInParent<IPlayer>();
            return _player;
        }
    }
    
    private void Awake() {
        _collider.enabled = false;
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out IPlayer collidedPlayer)) return;
        if (collidedPlayer.BonusUser.IsInvincibleAfterBonus) return;
        TryPush(collidedPlayer.Pusher, Player.Transform, collidedPlayer.Transform);
    }


    private void TryPush(IPusher lastPlayerContact, Transform thisPlayer, Transform enemyPlayer) {
        if (Time.time - _lastRepulseTime < _gameData.PushColldown)  return;
        
        Vector3 direction = (enemyPlayer.position - thisPlayer.position).normalized;
        if (direction.sqrMagnitude < 0.001f)
            direction = new Vector3(Random.value, Random.value, Random.value);

        LastPlayerContact = lastPlayerContact;
        lastPlayerContact.PushAway(direction);
        _lastRepulseTime = Time.time;
    }
}