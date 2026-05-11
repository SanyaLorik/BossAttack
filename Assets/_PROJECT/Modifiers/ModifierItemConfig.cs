using UnityEngine;

public enum ModifierType {
    Capacity,
    Damage,
    RateOfFire,
}

[CreateAssetMenu(fileName = "ModifierItemConfig", menuName = "Configs/ModifierItemConfig")]
public class ModifierItemConfig : ScriptableObject {
    public string Id => ModifierType.ToString();
    [SerializeField] public ModifierType ModifierType;
    [field: SerializeField] public int Price { get; private set; }
    
}


