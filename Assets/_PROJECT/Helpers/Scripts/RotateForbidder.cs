using UnityEngine;

public class RotateForbidder : MonoBehaviour {
    private Quaternion _fixedRotation;

    private void Awake() {
        _fixedRotation = transform.rotation;
    }

    private void LateUpdate() {
        transform.rotation = _fixedRotation;
    }
}