using Architecture_M;
using LuringPlayer_M;
using MediaKit_M.SkinChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameSave : GameSaveBase,
    IDailyRewardSaveLoader, IWheelFortuneSaveLoader, IDailyQuestSaveLoader, ISkinSaveLoader, ICommunitySaveLoader
{
    public long Money;
    public bool IsBoughtPurchase = false;
    public bool TutorialPassed = false;
    public int PlayerLevel = 1;

    
    // Modifiers
    public List<ModifierItem> Modifiers = new ();
    
    // Task
    public List<TaskItem> Tasks = new ();

    // Pets
    public List<PetsData> Pets = new ();
    
    // Daily Rewards
    public DailyRewardSave DailyRewardSave;
    public WheelFortuneSave WheelFortuneSave;
    public DailyQuestSave DailyQuestSave;
    public SkinSave SkinSave;
    public CommunitySave CommunitySave;

    public void UpdateModifierLevel(string id, int level, bool clear = false) {
        ModifierItem modifier = Modifiers.FirstOrDefault(b => b.Id == id);
        if (modifier == null) {
            Modifiers.Add(new ModifierItem {
                Id = id,
                Level = level,
            });
        }
        else {
            if (clear) {
                modifier.Level = level;
            }
            else {
                modifier.Level += level;
            }
            Debug.Log($"Added {level} level: {modifier.GetType()}"); 
        }
    }

    public int GetModifierLevel(string id) {
        ModifierItem modifier = Modifiers.FirstOrDefault(b => b.Id == id);
        var count = modifier == null ? 0 : modifier.Level;
        return count;
    }
    
    
    // Tasks--------------------------------
    public void UpdateTaskInfo(string id, int count, bool isGetReward) {
        TaskItem task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task != null) {
            task.Count = count;
            task.IsGetReward = isGetReward;
        }
        else {
            Debug.LogError("Задача не была инициализирована");
        }
    }
    
    /// <summary>
    /// Используется для получения таски, если игрок играет впервые то инициализирует оную
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TaskItem GetTaskInfo(string id) {
        TaskItem task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) {
            task = new TaskItem() {
                Id = id,
                Count = 0,
                IsGetReward = false
            };
            Tasks.Add(task);
        }
        return task;
    }

    public int AddNewPet(string id, int count) {
        var pet = Pets.FirstOrDefault(pet => pet.Id == id);
        if (pet == null) {
            Pets.Add(new PetsData() {
                Id = id,
                Count = count,
            });
            return count;
        }
        pet.Count+=count;
        return pet.Count;
    }
    
    
    public DailyRewardSave Load()
    {
        return DailyRewardSave;
    }

    WheelFortuneSave IWheelFortuneSaveLoader.Load()
    {
        return WheelFortuneSave;
    }

    DailyQuestSave IDailyQuestSaveLoader.Load()
    {
        return DailyQuestSave;
    }

    SkinSave ISkinSaveLoader.Load()
    {
        return SkinSave;
    }

    CommunitySave ICommunitySaveLoader.Load()
    {
        return CommunitySave;
    }
}


[Serializable]
public class ModifierItem {
    public string Id = "";
    public int Level = 0;
}

[Serializable]
public class TaskItem {
    public string Id = "";
    public int Count = 0;
    public bool IsGetReward = false;
}


[Serializable]
public class PetsData {
    public string Id = "";
    public int Count = 0;
}