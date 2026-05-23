using System;

public static class GameEvents {
    public static event Action<IBonus> BonusUsed;
    public static event Action TriggerUsed;
    public static event Action ShakeCamera;
    public static event Action NewItemReceived;
    public static event Action PlayerPushed;


    public static void BonusUseInvoke(IBonus bonus) {
        BonusUsed?.Invoke(bonus);
    }
    
    public static void TriggerUseInvoke() {
        TriggerUsed?.Invoke();
    }
    
    public static void ShakeCameraInvoke() {
        ShakeCamera?.Invoke();
    }
    
    public static void NewItemReceiveInvoke() {
        NewItemReceived?.Invoke();
    }
    
    public static void PlayerPushInvoke() {
        PlayerPushed?.Invoke();
    }
    
    
}
