using UnityEngine;

public abstract class BulletBase : MonoBehaviour {
    public abstract void SetPosition(Vector3 target);
    public abstract void InitShoot();
    public abstract void PlayToEnd();
}