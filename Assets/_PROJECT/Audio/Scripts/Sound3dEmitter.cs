using UnityEngine;
using Zenject;

public class Sound3dEmitter : MonoBehaviour {
    [SerializeField] private int _sourcesCount = 10;

    private AudioSource[] _sources;
    private int _index;
    

    [Inject] private SoundManager _soundManager;

    private void Awake() {
        _sources = new AudioSource[_sourcesCount];
        
        for (int i = 0; i < _sourcesCount; i++) {
            _sources[i] = new GameObject($"AudioSource{i+1}").AddComponent<AudioSource>();
        }
    }

    
    public void Play(SoundType soundType) {
        var source = GetFreeSource();
        _soundManager.Play3dSound(source, soundType);
    }
    
    
    private AudioSource GetFreeSource() {
        foreach (var src in _sources) {
            if (!src.isPlaying) {
                return src;
            }
        }

        AudioSource source = _sources[_index];

        _index++;

        if (_index >= _sources.Length) {
            _index = 0;
        }

        return source;
    }

}