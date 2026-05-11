using System;

public interface IEffectValue {
    public void SetValueGetter(Func<float> valueGetter);
}