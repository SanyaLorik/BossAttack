using UnityEngine;
using Zenject;

public class BotPrefabInstaller : MonoInstaller {
    [SerializeField] private GameObject _target;
    public override void InstallBindings()
    {
        var botManager = _target.GetComponent<BotManager>(); 
        
        Container.Bind<BotManager>()
            .FromInstance(botManager)
            .AsSingle();
    }
}