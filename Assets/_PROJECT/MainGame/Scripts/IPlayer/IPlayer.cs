using System;
using UnityEngine;

[Flags]
public enum TargetType {
    None      = 0,
    Player    = 1 << 0,
    Boss      = 1 << 1,
    Boost     = 1 << 2,
    Building  = 1 << 3,
}

public interface IPlayer : ICombatTarget {
    public void SetPlayStatus(bool goPlay);
    public void RotateToTarget(Vector3 point);
    public void SetMovingStatus(bool enable);
    public void SetVisualModelState(bool enable);
    
    public IPusher Pusher { get; }
    public IBonusUser BonusUser { get; }
    
    public bool IsPlaying { get; }
    Transform PointToAtack { get; }
    Transform Transform { get; }
}