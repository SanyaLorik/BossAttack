using Zenject;

public class PlayerSingleInstaller : MonoInstaller {
    
    public override void InstallBindings() {
        BindPlayerSingletones();
    }

    private void BindPlayerSingletones() {
        Container.Bind<PlayerMovement>()
            .FromComponentInHierarchy()
            .AsSingle();
        
        Container.Bind<PlayerStateManager>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
        
        
        Container.Bind<PlayerBank>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
        
        
        Container.Bind<PlayerFaceChooser>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
    }
}