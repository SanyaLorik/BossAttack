using System;
using SanyaBeerExtension;
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
    [SerializeField] private GameObject _buildVisual;
    [SerializeField] private AbilitySystem _building;


    private bool _isBuilded;
    private bool _playerInZone;
    private float _currentTime;
    
    
    [Inject] PlayerMovement _mainPlayer;
    [Inject] GameData _gameData;

    private void Start() {
        SetDefault();
    }

    
    public void SetDefault() {
        _buildVisual.ActiveSelf();
        _progressImage.fillAmount = 0;
        _building.DisactiveSelf();
    }


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
        EndBuild();
    }

    private void EndBuild() {
        _building.ActiveSelf();
        _buildVisual.DisactiveSelf();
    }
    
}