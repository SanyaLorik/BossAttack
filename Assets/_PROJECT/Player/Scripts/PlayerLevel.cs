using System;
using Architecture_M;
using UnityEngine;

public class PlayerLevel {
    public event Action<int> LevelUp;
    public int CurrentLevel => _gameSave.GetSave<GameSave>().PlayerLevel;
    
    
    private IGameSave _gameSave;
    
    public PlayerLevel(IGameSave gameSave) {
        _gameSave = gameSave;
    }
}