using SanyaBeerExtension;
using UnityEngine;

public class PaintBullet : BulletBase {
    [SerializeField] private TemporaryAbility _abilityToEnd;
    [SerializeField] private ParticleSystem _psToEnd;
    [SerializeField] private ParticleSystem _psWhileFlight;
    [SerializeField] private VisualEffectPlayer[] _effectsToEnd;
    [Header("Colors")]
    [SerializeField] private Color[] _particleColors;
    [SerializeField] private Sound3dEmitter _soundEmitter;

    
    public override void SetPosition(Vector3 target) {
        transform.position = target;
    }

    public override void InitShoot() {
        // _bulletModel.ActiveSelf();
        ParticleSystem.MainModule module = _psToEnd.main;
        SetColorToAllParticles();
        _psWhileFlight.ActiveSelf();
        _psWhileFlight.Play();
    }
    
    public override void PlayToEnd() {
        _psWhileFlight.Stop();
        _psWhileFlight.DisactiveSelf();
        _psToEnd.Play();
        _soundEmitter.Play(SoundType.Bullet);
        foreach (var effect in _effectsToEnd) {
            effect.Play();
        }
        
        // После долетания взрыв 
        if (_abilityToEnd != null) _abilityToEnd.Use();
        
    }
    
    private void SetColorToAllParticles() {
        Color newColor = _particleColors.GetRandomElement();
        SetColor(_psToEnd, newColor);
        SetColor(_psWhileFlight, newColor);
    }
    
    private static void SetColor(ParticleSystem ps, Color color) {
        var main = ps.main;
        main.startColor = color;
    }

}