using System;

public interface IEffectValue {
    public void SetValueGetter(Func<int> valueGetter);
}