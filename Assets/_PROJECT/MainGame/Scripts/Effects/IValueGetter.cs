using System;

public interface IValueGetter {
    public void SetValueGetter(Func<float> valueGetter);
}