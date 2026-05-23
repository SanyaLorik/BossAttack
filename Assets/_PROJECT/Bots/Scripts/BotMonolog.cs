using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BotMonolog : MonoBehaviour {
    [SerializeField] private GameObject _monologCloud;
    [SerializeField] private TMP_Text _monologText;
    [SerializeField] private TMP_Text _botNicknameText;


    public string NickName { get; private set; }


    private LocalizationData _localization; 
    private NicknameRandomizer _nicknameRandomizer; 
    
    [Inject]
    private void Initialize(LocalizationData localization, NicknameRandomizer nicknameRandomizer) {
        _localization = localization;
        _nicknameRandomizer = nicknameRandomizer; 
    }

    
    private void Start() {
        ChangeNickname();
        Stfu();
    }


    public void ChangeNickname() {
        NickName = _nicknameRandomizer.GetRandomName();
        _botNicknameText.text = NickName;
    }

    public void HideNickname() {
        _botNicknameText.DisactiveSelf();
    }
    
    public void ShowNickname() {
        _botNicknameText.ActiveSelf();
    }

    public void SaySomething() {
        _monologCloud.ActiveSelf();
        _monologText.text = _localization.BotsPhrases[Random.Range(0, _localization.BotsPhrases.Length)];
    }

    public void Stfu() {
        _monologCloud.DisactiveSelf();
        _monologText.text = string.Empty;
    }
    
}
