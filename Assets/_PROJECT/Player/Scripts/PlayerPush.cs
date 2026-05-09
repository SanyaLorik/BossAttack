using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerPush : IPusher {
    private GameData _gameData;
    private CharacterController _controller;

    public PlayerPush(GameData gameData, CharacterController controller) {
        _gameData = gameData;
        _controller = controller;
    }
    
    public void PushAway(Vector3 direction) {
        GameEvents.PlayerPushInvoke();
        PushWithController(_controller, direction.normalized).Forget();
    }
    
    
    private async UniTask PushWithController(CharacterController controller, Vector3 direction) {
        float elapsed = 0f;

        Vector3 horizontal = direction.normalized * _gameData.PlayerPushForce;
        float verticalVelocity = _gameData.PlayerUpPushRatio;

        Vector3 velocity = horizontal;

        while (elapsed < _gameData.PushTime)
        {
            elapsed += Time.deltaTime;

            // гравитация
            verticalVelocity += Physics.gravity.y * Time.deltaTime;

            Vector3 move = new Vector3(
                velocity.x,
                verticalVelocity,
                velocity.z
            );

            controller.Move(move * Time.deltaTime);

            // затухание только по XZ
            velocity *= 0.97f;

            await UniTask.Yield();
        }
    }

}