using System;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private bool _permanent; 
    [SerializeField] private BuildingType _buildingType; 
    [SerializeField] private float _timeToBuild;
    [SerializeField] private Image _progressImage;
    [SerializeField] private GameObject _buildVisual;
    [SerializeField] private GameObject _afterBuildVisual;
    [SerializeField] private AbilitySystem _abilityBuilding;
    [SerializeField] private DamageVisualizer _damagableVisual; 


    private bool _isBuilded;
    private bool _playerInZone;
    private float _currentTime;
    private BuildItemToAtack _buildItem;
    private Damagable _damagable;
    
    
    [Inject] PlayerMovement _mainPlayer;
    [Inject] GameData _gameData;
    [Inject] PlayerRegister _playerRegister;
    [Inject] private BuildingStaticStatsCalculator _buildingStaticStatsCalculator;
    [Inject] private BattleManager _battleManager;


    private void Start() {
        InitBuilding();
        
        if (_permanent) {
            EndBuildAsync().Forget();
        }
        else {
            SetDefault();
        }
    }

    private void OnDisable() {
        _battleManager.MainPlayerWin -= OnMainPlayerWin;
        _battleManager.GameReadyToPlay -= OnGameReadyToPlay;
    }
    
    private void OnEnable() {
        _battleManager.MainPlayerWin += OnMainPlayerWin;
        _battleManager.GameReadyToPlay += OnGameReadyToPlay;
        
    }
    
    public void Destroy() {
        DestroyUnit(_damagable);
    }

    
    private void OnGameReadyToPlay() {
        if (_permanent) {
            _abilityBuilding.StartSystem();
        }
    }


    private void OnMainPlayerWin(bool win) {
        _abilityBuilding.Stop();
        if (!_permanent) {
            Destroy();
        }
    }


    private async UniTask EndBuildAsync() {
        await UniTask.WaitForSeconds(1f);
        EndBuild();
    }
    
    
    private void DestroyUnit(IDamagable unit) {
        // Не буду делать тк модификация списка 
        // _playerRegister.UnregisterUnit(_buildItem, TargetType.Player);
        Debug.Log($"Destroy unit  {_buildingType}");
        SetDefault();
    }

    

    public void SetDefault() {
        _isBuilded = false;
        _playerInZone = false;
        _currentTime = 0f;
        _buildVisual.ActiveSelf();
        _abilityBuilding.Stop();
        _afterBuildVisual.DisactiveSelf();
        _progressImage.fillAmount = 0;
    }
    
    
    private void InitBuilding() {
       // Финт ушами
        _buildItem = new BuildItemToAtack(transform);
        
        _damagable = new Damagable(transform, _buildItem);
        
        _buildItem.InitDamagable(_damagable);
        
        
        if (_buildingType != BuildingType.Mine) {
            _damagableVisual.SetDamagable(_damagable);
            _damagable.DamagableDied += DestroyUnit;
        }
        
        InitAtackValue();
        InitHp();
    }

    
    private void InitHp() {
        switch (_buildingType) {
            case BuildingType.Mine:
                _damagable.SetMaxHpGetter(() => _buildingStaticStatsCalculator.TurretHp);
                break;
            case BuildingType.Turret:
                _damagable.SetMaxHpGetter(() => _buildingStaticStatsCalculator.TurretHp);
                break;
            case BuildingType.Heal:
                _damagable.SetMaxHpGetter(() => _buildingStaticStatsCalculator.HealBuildingHp);
                break;
        }
    }

    private void InitAtackValue() {
        _abilityBuilding.SetSame(_buildItem);
        var effect = _abilityBuilding.Effect as IValueGetter;
        switch (_buildingType) {
            case BuildingType.Mine:
                effect.SetValueGetter(() => _buildingStaticStatsCalculator.MineValue);
                _abilityBuilding.SetValueGetter(() =>  _buildingStaticStatsCalculator.MineIntervalAtack);
                break;
            case BuildingType.Turret:
                effect.SetValueGetter(() => _buildingStaticStatsCalculator.TurretValue);
                _abilityBuilding.SetValueGetter(() =>  _buildingStaticStatsCalculator.TurretIntervalAtack);
                break;
            case BuildingType.Heal:
                effect.SetValueGetter(() => _buildingStaticStatsCalculator.HealValue);
                _abilityBuilding.SetValueGetter(() => _buildingStaticStatsCalculator.HealIntervalAtack);
                break;
        }
    }



    private void Update() {
        if (_isBuilded || _permanent) return;
        
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
        EndBuild();
    }
    

    private void EndBuild() {
        if (_buildingType != BuildingType.Mine && !_permanent) {
            _playerRegister.RegisterUnit(_buildItem);
            Debug.Log("регистрация build " + _buildItem.TargetType);
        }
        _isBuilded = true;
        _currentTime = _timeToBuild;
        // Debug.Log("Start ability build");

        _abilityBuilding.StartSystem();
        _afterBuildVisual.ActiveSelf();
        _buildVisual.DisactiveSelf();
        _damagable.SetSpawned();
        
        if (_permanent) {
            _damagableVisual.HideVisual();
        }
    }
    
}