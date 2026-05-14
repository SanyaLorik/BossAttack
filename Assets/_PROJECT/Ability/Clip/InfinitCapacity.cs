using System;

[Serializable]
public class InfinitCapacity : IAtackCapacity {
    public bool AllowToUse { get; private set; } = true;
    public int MaxCount { get; }
    public int CurrentCount { get; }
    public void SpendOne() { }

    public void StartCheckCapacity(bool start) { }
    public void ReloadFull() { }
}