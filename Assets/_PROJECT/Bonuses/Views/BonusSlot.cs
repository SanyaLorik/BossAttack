using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Architecture_M;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

[Serializable]
public enum KeyBoardKey {
    One,
    Two,
    Three
}


public class BonusSlot : MonoBehaviour {
    [field: SerializeField] public BonusItemConfig BonusItem { get; private set; }
    [field: SerializeField] public KeyBoardKey KeyBoardKey { get; private set; }
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private TextMeshProUGUI _bonusNameText;
    [SerializeField] private Image _reloadProgress;
    [SerializeField] private Image _reloadImage;
    [SerializeField] private Gradient _gradient;
    [Header("Время использования")]
    [SerializeField] private GameObject _useContainer;
    [SerializeField] private RectTransform _useTimeProgress;
    [SerializeField] private Button _button;
    [Header("Выключать на мобил.")]
    [SerializeField] private GameObject _desktopObject;


    private bool IsAvailable { get; set; }
    private IBonus Bonus => BonusItem.Bonus;
    private int BonusCount => Saves.GetBonusCount(BonusItem.Id);
    private float _yEnd;
    
    private GameSave Saves => _saver.GetSave<GameSave>();
    private CancellationTokenSource _tokenSource;
    
    [Inject] private PlayerBonusManager _playerBonusManager;
    [Inject] private DiContainer _diContainer;
    [Inject] private IGameSave _saver; 
    [Inject] LocalizationData _localization;
    [Inject] PlayerMovement _mainPlayer;
    [Inject] TutorialManager _tutorialManager;
    [Inject] GameData _gameData;
    [Inject] IDeviceTypeProvider _deviceTypeProvider;
    
    [Inject]
    private void Init() {
        _diContainer.QueueForInject(Bonus);
    }
    

    private void OnEnable() {
        CheckAvailable();
        SetProgressBarVisible(false);
        _button.onClick.AddListener(TryUse);
        Bonus.StopWork(_mainPlayer.BonusUser);
    }

    
    private void OnDisable() {
        Bonus.StopWork(_mainPlayer.BonusUser);
        _button.onClick.RemoveListener(TryUse);
        IsAvailable = false;
        _reloadProgress.fillAmount = 0f;
    }

    private void Start() {
        _yEnd = _useTimeProgress.rect.height;
        
        CheckAvailable();
        SetProgressBarVisible(false);
        _bonusNameText.text =
            _localization.GetTranslatedText(BonusItem, _localization.BonusesTranslates);
        
        if (_deviceTypeProvider.DeviceType == DeviceTypeEnum.Mobile)
            _desktopObject.DisactiveSelf();
    }

    private void Update() {
        if (CheckKey()) {
            TryUse();
        }
    }

    public void SetStateAvailable(bool available) {
        if (available) {
            CheckAvailable();
        }
        else {
            UniTaskHelper.DisposeTask(ref _tokenSource);
            IsAvailable = false;
            _reloadProgress.fillAmount = 1f;
        }
    }
    
    private bool CheckKey() {
        var keyboard = Keyboard.current;
        return KeyBoardKey switch
        {
            KeyBoardKey.One => keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame,
            KeyBoardKey.Two => keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame,
            KeyBoardKey.Three => keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame,
            _ => false
        };
    }
    

    private void TryUse() {
        if (!IsAvailable) {
            Debug.Log("Бонус на перезарядке или уже юзается ");
            return;
        }

        if (BonusCount == 0) {
            Debug.Log("Бонусов нема");
            return;
        }

        UseBonus();
    }
    
    
    private void SetProgressBarVisible(bool visible) {
        _useContainer.SetActive(visible);
        // if (visible) {
        //     Canvas.ForceUpdateCanvases();
        // }
    }
    

    private void UseBonus() {
        GameEvents.BonusUseInvoke(Bonus);
        Bonus.Use(_mainPlayer.BonusUser);
        GetOneBonus();
        IsAvailable = false;
        
        
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new  CancellationTokenSource();
        StartUseTimerAsync(_tokenSource.Token).Forget();
    }


    private async UniTask StartUseTimerAsync(CancellationToken token) {
        SetProgressBarVisible(true);
        float duration = _gameData.BonusDuration;
        float elapsedTime = _gameData.BonusDuration;
        
        
        while (elapsedTime > 0 && !token.IsCancellationRequested) {
            float progress = elapsedTime/duration;

            float y = GetYPoseByPercent(progress, _yEnd, _useTimeProgress);
            _useTimeProgress.offsetMax = new Vector2(_useTimeProgress.offsetMax.x, y);

            _reloadImage.color = _gradient.Evaluate(progress);

            elapsedTime -= Time.deltaTime;
            await UniTask.Yield(cancellationToken: token);
        }

        Bonus.StopWork(_mainPlayer.BonusUser);
        SetProgressBarVisible(false);
        if (BonusCount != 0) {
            SetReloadBonusTimerAsync(token).Forget();
        }
    }
    

    private async UniTask SetReloadBonusTimerAsync(CancellationToken token) {
        float duration = _gameData.BonusReload;
        float elapsedTime = _gameData.BonusReload;
        while (elapsedTime > 0 && !token.IsCancellationRequested) {
            _reloadProgress.fillAmount = elapsedTime/duration;
            
            elapsedTime -= Time.deltaTime;
            await UniTask.Yield(cancellationToken: token);
        }
        
        CheckAvailable();
        if (BonusCount != 0) {
            GameEvents.BonusReloadedInvoke();
        }
    }


    private void CheckAvailable() {
        _countText.text = BonusCount.ToString();
        if (BonusCount == 0) {
            IsAvailable = false;
            _reloadProgress.fillAmount = 1f;
        }
        else {
            IsAvailable = true;
            _reloadProgress.fillAmount = 0f;
        }
    }


    private void GetOneBonus() {
        // Сохранять в сейвах если туториал пройден
        bool useSaves = _tutorialManager.TutorialPassed;
        if (useSaves) {
            _saver.GetSave<GameSave>().SetMinusOneBonus(BonusItem.Id);
            _saver.Save();
        }
        CheckAvailable();
    }

    private static float GetYPoseByPercent(float percent, float yEnd, RectTransform parent) {
        if (yEnd < 0) {
            Canvas.ForceUpdateCanvases();
            yEnd = parent.rect.height;
        }
        return -yEnd * (1f - percent);
    }


}
