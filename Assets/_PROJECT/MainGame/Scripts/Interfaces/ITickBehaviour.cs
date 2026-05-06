using UnityEngine;

public interface ITickBehaviour {
    void OnTick(Vector3 origin, IDamagable damagable);
}