using UnityEngine;

public class BuildItemToAtack : IPlayer {

    public BuildItemToAtack(Transform origin, IDamagable damagable) {
        Transform = origin;
        PointToAtack = origin;
        Damagable = damagable;
    }
    
    public IDamagable Damagable { get; }
    public void SetPlayStatus(bool goPlay) { }

    public void TeleportToPoint(Vector3 point) { }

    public void RotateToTarget(Vector3 point) { }

    public void SetMovingStatus(bool enable) { }

    public IPusher Pusher { get; }
    public IBonusUser BonusUser { get; }
    public bool IsPlaying { get; }
    public Transform PointToAtack { get; }
    public Transform Transform { get; }
}