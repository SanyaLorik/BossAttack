using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;




public class PlayerRegister : MonoBehaviour, IBattleInfo {
    [Header("Для теста прокидываю босов через инспектор")]
    [SerializeField] private List<BotManager> _boses;

    public List<IPlayer> Enemys { get; private set; } = new (8);
    public List<IPlayer> Players { get; private set; } = new (8);
    
    public List<IPlayer> Buildings { get; private set; } = new (8);
    
    public IPlayer MainPlayer => _playerMovement;
    public bool MainPlayerPlay { get; private set; }
    public int PlayersCount => Players.Count;

    
    [Inject] private PlayerMovement _playerMovement;

    
    private void Awake() {
        // Всегда играть будет покашт
        RegisterUnit(_playerMovement, TargetType.Player);
    }

    private void Start() {
        InitBoses();
    }

    
    private void InitBoses() {
        _boses.ForEach(b => RegisterUnit(b, TargetType.Enemy));
    }

    
    public void SetMainPlayerPlay(bool play) {
        
    }
    
    
    
    public void RegisterUnit(IPlayer player, TargetType type) {
        switch (type) {
            case TargetType.Player:
                if(Players.Contains(player)) return; 
                Players.Add(player);
                break;
            
            case TargetType.Enemy:
                if(Enemys.Contains(player)) return; 
                Enemys.Add(player);
                break;
        }
    }

    
    public void UnregisterUnit(IPlayer player, TargetType type) {
        switch (type) {
            case TargetType.Player:
                Players.Remove(player);
                break;
            
            case TargetType.Enemy:
                Enemys.Remove(player);
                break;
        }
    }

}