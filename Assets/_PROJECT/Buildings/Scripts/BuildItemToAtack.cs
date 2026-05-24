using UnityEngine;

public class BuildItemToAtack : IPlayer {

    public BuildItemToAtack(Transform origin) {
        Transform = origin;
        PointToAtack = origin;
    }

    public void InitDamagable(IDamagable damagable) {
        Damagable = damagable;
    }
    
    public IDamagable Damagable { get; private set; }
    public void SetPlayStatus(bool goPlay) { }

    public void TeleportToPoint(Vector3 point) { }

    public void RotateToTarget(Vector3 point) { }

    public void SetMovingStatus(bool enable) { }
    
    public void SetVisualModelState(bool enable) { }

    public IPusher Pusher { get; }
    public IBonusUser BonusUser { get; }
    public bool IsPlaying { get; }
    public Transform PointToAtack { get; }
    public Transform Transform { get; }
}