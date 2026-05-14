public interface IAbilityCotroller {
    public AbilitySystem AbilitySystem { get; }
    public void ReloadAbility();
    public void StopAbility();
    public void StartAbility();
}