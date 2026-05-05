using Zenject;

public class MainGameInstaller : MonoInstaller {
    public override void InstallBindings() {
        BindLogic();
        BindViews();
        BindBuilds();
    }
    
    
    private void BindLogic() {
        Container.Bind<MainGameStarter>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BattleManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }


    private void BindViews() {
        Container.Bind<GameOver>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BattleStartVisualizer>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BattleDiesInformator>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<MapsToBattleChanger>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindBuilds() {
        Container.Bind<BattleItemsBuilder>().AsSingle().NonLazy();
        Container.Bind<SpawnerInNavMesh>().AsSingle().NonLazy();
    }
}