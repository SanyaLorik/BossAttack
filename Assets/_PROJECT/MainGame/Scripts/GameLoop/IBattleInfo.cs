using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleInfo {
    public List<IPlayer> Bosses { get; }
    public List<IPlayer> Players { get; }
    public List<IPlayer> Buildings { get; }
    public IPlayer MainPlayer { get; }
    public bool MainPlayerPlay { get; }
    
    
}