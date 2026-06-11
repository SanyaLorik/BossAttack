using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ActiveBonusesIconManager : MonoBehaviour {
    [SerializeReference] private ActiveBonuseView[] _activeBonuseViews;
    
    [Inject] PlayerBonusService _playerBonusService;

    private Dictionary<BonusType, ActiveBonuseView> _typeToView = new();

    
    private void OnEnable() {
        _playerBonusService.BonusActivated += OnBonusActivated;
        _playerBonusService.BonusDisactivated += OnBonusDisactivated;
    }

    
    private void OnDisable() {
        _playerBonusService.BonusActivated -= OnBonusActivated;
        _playerBonusService.BonusDisactivated -= OnBonusDisactivated;
    }


    private void Start() {
        InitDictionary();
    }

    private void InitDictionary() {
        foreach (var bonuseIconView in _activeBonuseViews) {
            if (!_typeToView.TryAdd(bonuseIconView.BonusType, bonuseIconView)) {
                Debug.LogError("Добавлены повторяющиеся view");
                throw new Exception();
            }
            bonuseIconView.DisactiveVisual();
        }
    }


    private void OnBonusDisactivated(ActiveBonus bonus) {
        if (_typeToView.ContainsKey(bonus.Bonus.Type) == false) {
            Debug.LogError("Деактивировался ранее не добавленный бонус");
            return;
        }
        _typeToView[bonus.Bonus.Type].DisactiveBonus();
    }


    private void OnBonusActivated(ActiveBonus bonus) {
        if (_typeToView.ContainsKey(bonus.Bonus.Type) == false) {
            Debug.LogError("Активировался ранее не добавленный бонус");
            return;
        }
        _typeToView[bonus.Bonus.Type].ActiveBonus(bonus);
    }
}