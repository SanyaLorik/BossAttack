public interface IAtackCapacity {
    public bool AllowToUse { get; }
    public int MaxCount { get; }
    public int CurrentCount { get; }
    public void SpendOne();
    public void StartCheckCapacity(bool start);
    public void ReloadFull();
}