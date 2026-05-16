using UnityEngine;

public class AbilitySoundListener : MonoBehaviour {
    [SerializeField] private AbilitySystem _abilitySystem;
    [SerializeField] private Sound3dEmitter _emitter;


    private void OnEnable()
    { 
        _abilitySystem.SoundPlayed += OnSound;
    }

    private void OnDisable()
    {
        _abilitySystem.SoundPlayed -= OnSound;
    }

    private void OnSound(ISoundPlayer sound) {
        _emitter.Play(sound.SoundType);
    }
}