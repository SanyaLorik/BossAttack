using UnityEngine;



public interface IPlayer {
    public void SetPlayStatus(bool goPlay);
    public void TeleportToPoint(Vector3 point);
    public void RotateToTarget(Vector3 point);
    public void SetMovingStatus(bool enable);
    public IPusher Pusher { get; }
    public IBonusUser BonusUser { get; }
    
    public bool IsPlaying { get; }
    Transform Transform { get; }
}