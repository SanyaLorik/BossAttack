using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class Bullet : MonoBehaviour {
    [SerializeField] private GameObject _bulletModel;
    [SerializeField] private ParticleSystem _psToEnd;
    [SerializeField] private ParticleSystem _psToShoot;
    [SerializeField] private ParticleSystem _psWhileFlight;
    [Header("Colors")]
    [SerializeField] private Color[] _particleColors;
    [SerializeField] private Sound3dEmitter _soundEmitter;

    
    public void SetPosition(Vector3 target) {
        transform.position = target;
    }

    public void InitShoot() {
        // _bulletModel.ActiveSelf();
        ParticleSystem.MainModule module = _psToEnd.main;
        SetColorToAllParticles();
        _psWhileFlight.ActiveSelf();
        _psWhileFlight.Play();
    }
    
    public void PlayToEnd() {
        // _bulletModel.DisactiveSelf();
        _psWhileFlight.Stop();
        // _psWhileFlight.DisactiveSelf();
        _psToEnd.Play();
        _soundEmitter.Play(SoundType.Bullet);
    }
    
    private void SetColorToAllParticles() {
        Color newColor = _particleColors.GetRandomElement();
        SetColor(_psToEnd, newColor);
        SetColor(_psToShoot, newColor);
        SetColor(_psWhileFlight, newColor);
    }
    
    private static void SetColor(ParticleSystem ps, Color color) {
        var main = ps.main;
        main.startColor = color;
    }

}