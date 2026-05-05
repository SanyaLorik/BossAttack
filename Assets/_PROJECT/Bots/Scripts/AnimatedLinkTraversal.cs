using UnityEngine;
using Zenject;

public class AnimatedLinkTraversal : MonoBehaviour {
    
    public float jumpDuration = 0.8f;
    public float jumpHeight = 2.5f;
    public AnimationCurve horizontalCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Inject] BotManager _manager;

    public bool IsJumpingTraversal { get; private set; }
    private float timer;

    private Vector3 start;
    private Vector3 end;

    void Update() {
        if (_manager.Agent.isOnOffMeshLink && !IsJumpingTraversal) {
            StartJump();
        }

        if (IsJumpingTraversal) {
            UpdateJump();
        }
    }

    void StartJump() {
        var link = _manager.Agent.currentOffMeshLinkData;

        // Берём РЕАЛЬНУЮ позицию
        start = _manager.Transform.position;
        end = link.endPos + Vector3.up * _manager.Agent.baseOffset;

        timer = 0f;
        IsJumpingTraversal = true;

        _manager.Agent.updatePosition = false;
    }

    void UpdateJump() {
        timer += Time.deltaTime;
        float t = timer / jumpDuration;

        // горизонталь отдельно
        float horizT = horizontalCurve.Evaluate(t);
        Vector3 flatPos = Vector3.Lerp(start, end, horizT);

        // чистая парабола
        float height = 4 * jumpHeight * t * (1 - t);

        Vector3 finalPos = flatPos + Vector3.up * height;
        _manager.Transform.position = finalPos;

        Vector3 direction = (end - _manager.Transform.position);
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _manager.Transform.rotation = Quaternion.Slerp(
                _manager.Transform.rotation,
                targetRotation,
                _manager.Agent.angularSpeed * Time.deltaTime
            );
        }

        if (t >= 1f) {
            FinishJump();
        }
    }

    void FinishJump() {
        if (_manager.Agent.enabled && _manager.Agent.isOnNavMesh && _manager.Agent.isOnOffMeshLink)
        {
            _manager.Agent.CompleteOffMeshLink();
        }

        _manager.Agent.updatePosition = true;
        IsJumpingTraversal = false;
    }
}