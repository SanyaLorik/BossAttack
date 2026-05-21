using Zenject;

public class MainGameInstaller : MonoInstaller {
    public override void InstallBindings() {
        BindLogic();
        BindViews();
        BindBuilds();
        BindModifiers();
        BindBonus();
    }
    
    
    private void BindLogic() {
        BildStatsCalculators();
        Container.Bind<MainGameStarter>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BattleManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerRegister>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayersDiesObserver>().AsSingle().NonLazy();
    }

    private void BildStatsCalculators() {
        Container.BindInterfacesAndSelfTo<BossStatsCalculator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerStatsCalculator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<BuildingStatsCalculator>().AsSingle().NonLazy();
    }


    private void BindViews() {
        Container.Bind<GameOver>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BattleStartVisualizer>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BattleDiesInformator>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<MapsToBattleChanger>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindBuilds() {
        Container.Bind<SpawnerInNavMesh>().AsSingle().NonLazy();
    }
    
    private void BindModifiers() {
        Container.BindInterfacesAndSelfTo<ModifierShopManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    private void BindBonus() {
        Container.BindInterfacesAndSelfTo<PlayerBonusService>().AsSingle().NonLazy();
        Container.Bind<BonusSpawner>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ActiveBonusCreator>().AsSingle().NonLazy();
    }
}