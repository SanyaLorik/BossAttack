using UnityEngine;

[CreateAssetMenu(fileName = "BossConfig", menuName = "Configs/BossConfig")]
public class BossConfig : ScriptableObject {
    [field: SerializeField] public int BaseDamage;
    [field: SerializeField] public int LevelAddDamage;
    [field: SerializeField] public int BaseHp;
    [field: SerializeField] public int LevelAddHp;
    // Not depends on the players level
    [field: SerializeField] public float MoveSpeed;
    [field: SerializeField] public float RateOfFire;
    [field: SerializeField] public float StopingDistance;
    [field: SerializeField] public float DistanceToAtack;
}