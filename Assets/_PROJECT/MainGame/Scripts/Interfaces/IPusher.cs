using UnityEngine;

public interface IPusher {
    public void PushAway(Vector3 direction);
    public Transform Transform { get; }
    public IPusher LastPlayerContact { get; }
}