using System;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[Serializable]
public enum BuildingType {
    Mine,
    Turret,
    Heal
}

public class BuildZoneItem : MonoBehaviour {
    [SerializeField] private BuildingType _buildingType; 
    [SerializeField] private float _timeToBuild;
    [SerializeField] private Image _progressImage;
    [SerializeField] private GameObject _buildVisual;
    [SerializeField] private GameObject _afterBuildVisual;
    [SerializeField] private AbilitySystem _building;
    [SerializeField] private DamageVisualizer _damagableVisual; 


    private bool _isBuilded;
    private bool _playerInZone;
    private float _currentTime;
    private IPlayer _buildItem;
    private Damagable _damagable;
    
    
    [Inject] PlayerMovement _mainPlayer;
    [Inject] GameData _gameData;
    [Inject] PlayerRegister _playerRegister;
    [Inject] private BuildingStatsCalculator _buildingStatsCalculator;


    private void Start() {
        SetDefault();

        InitBuilding();
    }

    
    private void OnDisable() {
        if (_damagable != null) {
            _damagable.DamagableDied -= DestroyUnit;
        }
    }
    
    
    private void InitBuilding() {
        _damagable = new Damagable(transform);
        _damagable.DamagableDied += DestroyUnit;

        _damagableVisual.SetDamagable(_damagable);
        _buildItem = new BuildItemToAtack(transform, _damagable);
        
        InitValue();
        InitHp();
    }

    
    private void InitHp() {
        switch (_buildingType) {
            case BuildingType.Mine:
                _damagable.SetMaxHpGetter(() => _buildingStatsCalculator.TurretHp);
                break;
            case BuildingType.Turret:
                _damagable.SetMaxHpGetter(() => _buildingStatsCalculator.TurretHp);
                break;
            case BuildingType.Heal:
                _damagable.SetMaxHpGetter(() => _buildingStatsCalculator.HealBuildingHp);
                break;
        }
    }

    private void InitValue() {
        _building.SetSame(_buildItem);
        var effect = _building.Effect as IValueGetter;
        switch (_buildingType) {
            case BuildingType.Mine:
                effect.SetValueGetter(() => _buildingStatsCalculator.MineValue);
                break;
            case BuildingType.Turret:
                effect.SetValueGetter(() => _buildingStatsCalculator.TurretValue);
                break;
            case BuildingType.Heal:
                effect.SetValueGetter(() => _buildingStatsCalculator.HealValue);
                break;
        }
    }

    


    private void DestroyUnit() {
        // Не буду делать тк модификация списка 
        // _playerRegister.UnregisterUnit(_buildItem, TargetType.Player);
        SetDefault();
    }
    

    public void SetDefault() {
        _isBuilded = false;
        _playerInZone = false;
        _currentTime = 0f;
        _buildVisual.ActiveSelf();
        _afterBuildVisual.DisactiveSelf();
        _progressImage.fillAmount = 0;
        _building.Stop();
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
        _playerRegister.RegisterUnit(_buildItem, TargetType.Player);
        _building.ActiveSelf();
        _building.Start();
        _afterBuildVisual.ActiveSelf();
        _buildVisual.DisactiveSelf();
        _damagable.SetSpawned();
    }
    
}