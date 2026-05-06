using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    [SerializeField] private int _hp;
    [SerializeField] private HuntingBehaviour _huntingBehaviour;
    public Damagable _damagable;

    private void Awake() {
        _damagable = new Damagable(_hp, transform);
    }

    private void OnEnable() {
        _huntingBehaviour.StartHunting();
    }

    private void OnDisable() {
        _huntingBehaviour.StopHunting();
    }
}