using System;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GameData : GameDataBase
{
    [field: Header("Player")]
    [field: SerializeField] public float WalkSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float SecondJumpForce { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }
    [field: SerializeField] public float GravityScale { get; private set; }
    [field: SerializeField] public int InitBonusCounts { get; private set; }
    
    
    [field: Header("Camera")]
    [field: Header("Дефолтные значения в процентах")]
    [field: SerializeField, Range(0,1)] public float MobileCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float DesktopCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float DefaultCameraSens { get; private set; }
    [field: SerializeField, Range(0,1)] public float PlayZoomInDesktop { get; private set; }
    [field: SerializeField, Range(0,1)] public float PlayZoomInMobile { get; private set; }
    [field: SerializeField, Range(0,1)] public float ZoomSpeed { get; private set; }
    [field: SerializeField] public float VerticalAxisValueToStartPlay { get; private set; }
    
    [field: Header("Множители сенсы")]
    [field: SerializeField] public float JoystickSensivityMultiplier  { get; private set; }
    [field: SerializeField] public float MouseSensivityMultiplier { get; private set; }
    
    [field: Header("Ограничители")]
    [field: SerializeField] public PairedValue<float> ZoomDiapasone  { get; private set; }
    [field: SerializeField] public float MinSensValue  { get; private set; }
    
    
    
    [field: Header("Бонусы")]
    [field: Header("Значения")]
    [field: SerializeField] public float VelocityBonusSpeed { get; private set; }
    [field: SerializeField] public float JumpBonusHeight { get; private set; }
    [field: SerializeField] public float DoubleJumpBonusHeight { get; private set; }
    [field: Header("Время")]
    [field: SerializeField] public float SpeedBonusDuration { get; private set; }
    [field: SerializeField] public float JumpBonusDuration { get; private set; }
    [field: SerializeField] public float ReloadBonusDuration { get; private set; }
    [field: SerializeField] public float InvincibleBonusDuration { get; private set; }
    
    [field: Header("Логика спавна")]
    [field: SerializeField] public int MaxCountBonusesInMap { get; private set; }
    [field: SerializeField] public float DurationToSpawnNewBonus { get; private set; }
    
    
    
    [field: Header("Толчки")]
    [field: SerializeField] public float BotPushForce { get; private set; }
    [field: SerializeField] public float BotUpPushRatio { get; private set; }   
    [field: SerializeField] public float PlayerPushForce { get; private set; }
    [field: SerializeField] public float PlayerUpPushRatio { get; private set; }
    [field: SerializeField] public float PushTime { get; private set; }
    [field: SerializeField] public float PushColldown { get; private set; }
    

    [field: Header("Птенцы")]
    [field: SerializeField] public int MaxPetsCount { get; private set; }
    [field: SerializeField] public PairedValue<int> BotPetCountDiapasone { get; private set; }
    
    
    [field: Header("БОТЫ")]
    [field: SerializeField] public int CountBotsToGame { get; private set; }
    [field: SerializeField] public float BotSpeed { get; private set; }
    [field: SerializeField] public PairedValue<int> CountSpeakingBotsPerTime  { get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanceToJump { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToStayOnPoint { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToStayAfterSpawn { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToSpeak { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanceToBotChangeNicknameAfterPlay { get; private set; }
    
    [field: Header("Боты в игре")]
    [field: SerializeField] public float DistanceToFloor { get; private set; }
    [field: SerializeField] public float DurationToHuntWithoutCheck { get; private set; }
    [field: SerializeField] public float DurationToGoInPoint { get; private set; }
    [field: SerializeField] public float RunStoppingDistance { get; private set; }
    [field: SerializeField] public float BotJumpDuration { get; private set; }
    [field: SerializeField] public float BotFallSpeed { get; private set; }
    [field: SerializeField] public float BotJumpBonusDuration { get; private set; }
    [field: SerializeField] public float BotDefaultJumpHeight { get; private set; }
    [field: SerializeField] public float BotJumpBonusHeight { get; private set; }
    [field: SerializeField] public PairedValue<float> BotUseNewBonusTime { get; private set; }
    [field: SerializeField, Range(0,1)] public float BotChanceToUseBonus { get; private set; }
    
    [field: Header("Постройки")]
    [field: SerializeField] public float TimeDividerToUnbild { get; private set; }
    [field: SerializeField] public float DistanceToFindNavMeshToBuild { get; private set; }
    
        

    [field: Header("Главная Игра")]
    [field: SerializeField] public float NewGameTimer { get; private set; }
    [field: SerializeField] public float DelayAfterGameOverToNewTimer { get; private set; }
    [field: SerializeField] public float DefaultSpeedInRound { get; private set; }
    [field: SerializeField] public float ColldownToStartRound { get; private set; }
    [field: SerializeField] public float TimeToShowDieInfo { get; private set; }
    [field: SerializeField] public float TimeAfterEndRound { get; private set; }
    [field: SerializeField] public float MinimumTimeToFindNewTarget { get; private set; }


    [field: Header("Визуал")] 
    [field: SerializeField] public float PaintTimeToWaitAfterDestroyBullet { get; private set; }
    [field: SerializeField, Range(0f,1f)] public float YBulletOffset { get; private set; }
    
    
    [field: Header("Настройка урона босса")]
    [field: SerializeField] public int BossMeleeDamageBase { get; private set; }
    [field: SerializeField] public int BossShootDamageBase { get; private set; }
    [field: SerializeField] public float BossMeleeLevelAddDamage { get; private set; }    
    [field: SerializeField] public float BossShootLevelAddDamage { get; private set; }
    [field: Header("Настройка босса")]
    [field: SerializeField] public float BossStoppingDistanceInMelee { get; private set; }    
    [field: SerializeField] public float BossStoppingDistanceInShooting { get; private set; }    
    [field: SerializeField] public float BossSpeedInMelee { get; private set; }     
    [field: SerializeField] public float BossSpeedInShooting { get; private set; }    
   
    [field: Header("Настройка скорости ударов босса")]
    [field: SerializeField] public float BossIntervalToAtackInMelee { get; private set; }     
    [field: SerializeField] public float BossIntervalToAtackInShooting { get; private set; } 
    
    
    [field: Header("Настройка хп у всех юнитов")]
    [field: SerializeField] public int BossHpBase { get; private set; }
    [field: SerializeField] public float BossLevelAddHp { get; private set; }
    
    [field: SerializeField] public int PlayerHpBase { get; private set; }
    [field: SerializeField] public float PlayerLevelAddHp { get; private set; }
    [field: SerializeField] public int HealBuildingHpBase { get; private set; }
    [field: SerializeField] public int TurretHpBase { get; private set; }
    [field: SerializeField] public float BuildingAddLevelHp { get; private set; }
    
    [field: Header("Настройка Урона/Хилла построек")]
    [field: Header("Турель")]
    [field: SerializeField] public int TurretValueBase { get; private set; }
    [field: SerializeField] public int TurretAddLevelValue { get; private set; }
    
    [field: Header("Хилка")]
    [field: SerializeField] public int HealBuildingValueBase { get; private set; }
    [field: SerializeField] public int HealBuildingAddLevelValue { get; private set; }
    
    [field: Header("Мина")]
    [field: SerializeField] public int MineBuildingValueBase { get; private set; }
    [field: SerializeField] public int MineBuildingAddLevelValue { get; private set; }
    
    [field: Header("Интервал атаки")]
    [field: SerializeField] public float MineIntervalAtack { get; private set; }
    [field: SerializeField] public float TurretIntervalAtack { get; private set; }
    [field: SerializeField] public float HealIntervalAtack { get; private set; }
    
    
    [field: Header("Настройка Модификаторов игрока")]
    [field: Header("Урон")]
    [field: SerializeField] public float PlayerDamageBase { get; private set; }
    [field: SerializeField] public float PlayerLevelAddDamage { get; private set; }
    
    
    [field: Header("Скорострельность")]
    [field: SerializeField] public float PlayerRateOfFireBase { get; private set; }
    [field: SerializeField] public float PlayerLevelAddRateOfFire { get; private set; }
    [field: SerializeField] public float PlayerRateOfFireMinimum { get; private set; }

    
    [field: Header("Обойма")]
    [field: SerializeField] public float PlayerCapacityBase { get; private set; }
    [field: SerializeField] public float PlayerLevelAddCapacity { get; private set; }
    [field: SerializeField] public float ClipReloadOneBulletDuration { get; private set; }

    


}