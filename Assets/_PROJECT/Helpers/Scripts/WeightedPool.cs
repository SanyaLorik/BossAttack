using System.Collections.Generic;
using System.Linq;

public class WeightedPool<T> {
    private readonly List<WeightedPrefabItem<T>> _items;
    private float _totalWeight;
    
    public WeightedPool(List<WeightedPrefabItem<T>> items) {
        _items = items;
        RecalculateWeight();
    }
    
    public void RecalculateWeight() {
        _totalWeight = _items.Sum(x => x.Weight);
    }

    public T GetRandom() {
        return WeightedHelper.GetRandomPrefabItemByWeight(_items, _totalWeight);
    }
}