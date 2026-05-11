using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ModifierShopCardView : MonoBehaviour {
    [field: SerializeField] public ModifierItemConfig Modifier { get; private set; }
    [field: SerializeField] public Button BuyButton { get; private set; }
    [field: SerializeField] public Transform Card;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _countText;

    
    [Inject] private NumberFormatter _priceFormatter;
    [Inject] private LocalizationData _localization;
    [Inject] private PlayerBank _bank;

    private void Start() {
        Initialize();
    }

    public void SetCount(int count) {
        _countText.text = _priceFormatter.ValuteFormatter(count);
    }

    public void CheckPlayerBankToBuy() {
        BuyButton.interactable = _bank.CanBuy(Modifier.Price);
    }

    
    private void Initialize() {
        _priceText.text = _priceFormatter.ValuteFormatter(Modifier.Price);
        _nameText.text = _localization.GetTranslatedText(Modifier, _localization.ModifiersTranslates);
    }

    
}