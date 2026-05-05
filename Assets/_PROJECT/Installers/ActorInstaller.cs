using Zenject;

public class ActorInstaller : MonoInstaller {
    public override void InstallBindings()
    {
        var player = GetComponent<IPlayer>(); 
        
        Container.Bind<IPlayer>()
            .FromInstance(player)
            .AsSingle();
    }
}