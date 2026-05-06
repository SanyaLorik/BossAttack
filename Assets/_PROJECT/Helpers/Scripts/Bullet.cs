using System;
using UnityEngine;

public class Bullet : MonoBehaviour {


    public void SetPosition(Vector3 target) {
        transform.position = target;
    }
}