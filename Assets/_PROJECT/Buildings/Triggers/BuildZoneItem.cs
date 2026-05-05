using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[Serializable]
public enum BuildType {
    Heal,
    Turret,
    Mine
}

public class BuildZoneItem : MonoBehaviour {
    [SerializeField] private BuildType _buildType;
    [SerializeField] private float _timeToBuild;
    [SerializeField] private Image _progressImage;


    private bool _isBuilded;
    private bool _playerInZone;
    private float _currentTime;
    
    
    [Inject] PlayerMovement _mainPlayer;
    [Inject] BattleItemsBuilder _builder;
    [Inject] GameData _gameData;

    
    private void Update() {
        if (_isBuilded) return;
        
        if (_playerInZone) {
            _currentTime += Time.deltaTime;
        }
        else if (_currentTime > 0) {
            _currentTime -= Time.deltaTime / _gameData.TimeDividerToUnbild;
            _currentTime = Mathf.Clamp(_currentTime, 0, _currentTime);
        }

        UpdateVisual();
        CheckEndBuild();
    }

    
    private void UpdateVisual() {
        float progress = Mathf.Clamp01(_currentTime / _timeToBuild);
        _progressImage.fillAmount = progress;
    }
    

    private void OnTriggerEnter(Collider collider) {
        if(!ReadyToBuild(collider)) return;
        _playerInZone = true;
    }
    
    
    private void OnTriggerExit(Collider collider) {
        if(!ReadyToBuild(collider)) return;
        _playerInZone = false;
    }
    

    private bool ReadyToBuild(Collider collider) {
        if(_isBuilded) return false;
        if(!collider.TryGetComponent(out IPlayer player)) return false;
        if(player != _mainPlayer) return false;
        return true;
    }


    private void CheckEndBuild() {
        if(_isBuilded || _currentTime < _timeToBuild) return;
        _isBuilded = true;
        _builder.BuildItemByType(_buildType, transform.position);
    }
    
}