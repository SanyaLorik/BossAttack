using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct WeightedItem<T> {
    [SerializeReference, SubclassSelector] public T Item;
    [Range(0,1), SerializeField] public float Weight;
}

[Serializable]
public struct WeightedPrefabItem<T> {
    [field: SerializeField] public T Item { get; private set; }
    [field: SerializeField,  Range(0,1)] public float Weight  { get; private set; }
}

public static class WeightedHelper {
    public static T GetRandomRefItemByWeight<T>(List<WeightedItem<T>> itemWeight, float totalWeight) {
        float accumulated = 0;
        float choosedWeight = Random.Range(0, totalWeight);
        foreach (var modifierValue in itemWeight) {
            accumulated += modifierValue.Weight;
            if (accumulated > choosedWeight) {
                // Debug.Log($"Выбивание веса {choosedWeight}, айтем: {modifierValue.Item.GetType()}");
                return modifierValue.Item;
            }
        }
        return itemWeight[^1].Item;
    }
    
    public static T GetRandomPrefabItemByWeight<T>(List<WeightedPrefabItem<T>> itemWeight, float totalWeight) {
        float accumulated = 0;
        float choosedWeight = Random.Range(0, totalWeight);
        foreach (var modifierValue in itemWeight) {
            accumulated += modifierValue.Weight;
            if (accumulated > choosedWeight) {
                // Debug.Log($"Выбивание веса {choosedWeight}, айтем: {modifierValue.Item.GetType()}");
                return modifierValue.Item;
            }
        }
        return itemWeight[^1].Item;
    }
}