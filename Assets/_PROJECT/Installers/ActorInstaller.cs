using UnityEngine;
using Zenject;

public class ActorInstaller : MonoInstaller {
    [SerializeField] private GameObject _target;
    
    public override void InstallBindings()
    {
        var player = _target.GetComponent<IPlayer>(); 
        
        Container.Bind<IPlayer>()
            .FromInstance(player)
            .AsSingle();
    }
}