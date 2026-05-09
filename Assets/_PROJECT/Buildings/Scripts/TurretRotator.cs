using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class TurretRotator : ITickBehaviour {
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Transform _rotateTransform;
    
    private Transform _target;
    private CancellationTokenSource _tokenSource;
    
    
    public void OnTick(Vector3 origin, IPlayer player) {
        _target = player.Transform;
        if (_tokenSource == null) {
            _tokenSource = new CancellationTokenSource();
            RotateCycleAsync(_tokenSource.Token).Forget();
        }
    }

    private async UniTaskVoid RotateCycleAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            RotateToTarget();
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
        
    
    private void RotateToTarget() {
        if (_target == null) return;
        Vector3 dir = _target.position - _rotateTransform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        _rotateTransform.rotation = Quaternion.Slerp(
            _rotateTransform.rotation,
            targetRot,
            _rotationSpeed * Time.deltaTime
        );
    }

    
}