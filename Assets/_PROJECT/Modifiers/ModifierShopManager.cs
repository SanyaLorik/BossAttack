using System;
using System.Collections.Generic;
using Architecture_M;
using DG.Tweening;
using MirraSDK_M;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class ModifierShopManager : MonoBehaviour {
    [SerializeField] private DelayedTrigger _trigger;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _closeButton;
    [Header("Карточки")]
    [SerializeField] private Ease _easeToShowCards;
    [SerializeField] private float _showCardsDuration;
    [Header("Кнопки купить")]
    [SerializeField] private List<ModifierShopCardView> _itemCards;
    [SerializeField] private Button _randomByAdv;
    [SerializeField] private Transform _randomByAdvTransform;

    private GameSave Saves => _save.GetSave<GameSave>();

    public event Action ModifierUpdated; 
    
    [Inject] private IGameSave _save;
    [Inject] private AdvHelper _advHelper;
    [Inject] private AdvertisingMonetizationMirra _advertisingMonetization;
    [Inject] private PlayerBank _bank;
    

    
    private void OnEnable() {
        _closeButton.onClick.AddListener(CloseCanvas);
        _itemCards.ForEach(c => c.BuyButton.onClick.AddListener(() => BuyOneItem(c.Modifier.Id, c)));
        _advHelper.AddToButtonAdvRewardListener(_randomByAdv, GetRandom);
    }

    public int GetModifierLevelWithType(ModifierType modifierType) {
        ModifierShopCardView modifier = _itemCards.Find(m => m.Modifier.ModifierType == modifierType);
        
        if (modifier == null) {
            Debug.LogError("Не найден модификатор с типом " + modifierType);
        }
        
        string modifierId = modifier.Modifier.Id;
        return Saves.GetModifierLevel(modifierId);
    }


    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        _trigger.DelayedTriggerAction(OpenCardsCanvasAnimation);
        _advHelper.DisableTimer();
    }
    
    
    private void OnTriggerExit(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        _trigger.CancelTriggerActionFull();
        _advHelper.EnableTimer();
    }
    
    
    private void OnOpenCanvas() {
        _itemCards.ForEach(c => c.SetCount(Saves.GetModifierLevel(c.Modifier.Id)));
        _itemCards.ForEach(c => c.CheckPlayerBankToBuy());
    }
    

    private void BuyOneItem(string modifierId, ModifierShopCardView modifierShopCard) {
        _bank.SpendMoney(modifierShopCard.Modifier.Price);
        Saves.UpdateModifierLevel(modifierId,1);
        _save.Save();
        ModifierUpdated?.Invoke();
        modifierShopCard.SetCount(Saves.GetModifierLevel(modifierId));
        _itemCards.ForEach(c => c.CheckPlayerBankToBuy());
    }

    
    private void GetRandom() { 
        ModifierShopCardView modifierShopCardView = _itemCards.GetRandomElement();
        Saves.UpdateModifierLevel(modifierShopCardView.Modifier.Id, 1);
        _save.Save();
        modifierShopCardView.SetCount(Saves.GetModifierLevel(modifierShopCardView.Modifier.Id));
    }

    
    private void OpenCardsCanvasAnimation() {
        _advHelper.ShowAdv();

        OnOpenCanvas();
        _canvas.ActiveSelf();
        GameEvents.TriggerUseInvoke();
        _itemCards.ForEach(c => c.Card.localScale = Vector3.zero);
        _randomByAdvTransform.localScale = Vector3.zero;
        
        Sequence sequence = DOTween.Sequence();
        foreach (var card in _itemCards) {
            sequence.Append(
                card.Card
                    .DOScale(1f, _showCardsDuration)
                    .SetEase(_easeToShowCards)
            );
        }
        sequence.Append(
            _randomByAdvTransform
                .DOScale(1f, _showCardsDuration)
                .SetEase(_easeToShowCards)
        );
    }


    private void CloseCanvas() {
        _canvas.DisactiveSelf();
    }
    
}
