using System;
using Architecture_M;
using UnityEngine;
using Zenject;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IPlayer {
    [SerializeField] private CharacterController _controller; // 
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _playerVisual;
    [SerializeField] private PlayerBonusController _playerBonusController;
    [SerializeField] private DamageVisualizer _damagableVisual; 

    
    [field: SerializeField] public Transform Transform { get; private set; }

    public Vector2 MoveInput => _inputDirection2.Direction2;
    private float _currentRoll;
    private float _rollVelocity;
    private bool _wasGroundedLastFrame;
    private Vector3 _externalMotion;
    private float _walkSpeed;
    private float _firstJumpForce;
    private float _secondJumpForce;

    public event Action PlayerTeleportToTarget;
    public event Action JumpPressed;
    public event Action DoubleJumpPressed;
    public event Action<bool> RunningStateChanged;
    public event Action Floored;
    
    public bool IsGrounded { get; private set; }
    public bool IsRunning { get; private set; }
    public bool PlayerInSpawn { get; private set; } = true;

    public IPusher Pusher => _pusher;
    public PlayerPush _pusher;
    public IBonusUser BonusUser => _playerBonusController;
    public IDamagable Damagable => _damagable;
    private Damagable _damagable; 
    
    
    public CharacterController Controller => _controller;
    private bool MoveEnable { get; set; } = true;
    public event Action<bool> MoveEnabled;
    public event Action PlayerHited;
    public event Action<bool> InitedToPlay;
    
    
    [Inject] private IInputDirection2 _inputDirection2;
    [Inject] private IInputActivity _inputActivity;
    [Inject] private IInputJumping _inputJumping;
    [Inject] private GameData _gameData;
    [Inject] private PlayerStateManager _stateManager;
    
    // Для гравитации и прыжков
    private float _verticalVelocity;
    private int _jumpsUsed;

    
    private void Awake() {
        _damagable = new Damagable(_gameData.PlayerMaxHp, transform);
        _damagableVisual.SetDamagable(_damagable);
        _pusher = new PlayerPush(_gameData, _controller);
    }
    

    private void Update() {
        Walk();
    }
    
    
    private void RotateToCamera() {
        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(forward);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRot,
            _gameData.RotateSpeed * Time.deltaTime
        );
    }
    
    
    private void OnEnable() {
        _inputJumping.OnJumped += OnJump;
    }
    
    
    private void OnDisable() {
        _inputJumping.OnJumped -= OnJump;
    }

    
    /// <summary>
    /// Какой-то внешний стимулятор движения, например лифт
    /// </summary>
    /// <param name="motion"></param>
    public void AddExternalMotion(Vector3 motion) {
        _externalMotion += motion;
    }

    public void UpdateWalkSpeed(float speed) {
        _walkSpeed = speed;
    }


    public void UpdateFirstJumpForce(float force) {
        _firstJumpForce = force;
    }
    
    public void UpdateSecondJumpForce(float force) {
        _secondJumpForce = force;
    }
    

    public void TeleportToPoint(Vector3 target) {
        SetCharacterControllerState(false);
        transform.position = target;
        SetCharacterControllerState(true);
    
        _verticalVelocity = 0; // Сброс скорости
        _jumpsUsed = 0; // Сброс прыжков
        PlayerTeleportToTarget?.Invoke();
    }


    public void SetPlayStatus(bool goPlay) {
        Debug.Log("Player set play status " + goPlay);
        InitedToPlay?.Invoke(goPlay);
        BonusUser.SetDefault();
        
        IsPlaying = goPlay;
        PlayerInSpawn = !goPlay;
        _stateManager.SetupCanvases(goPlay);
        
        if (goPlay) {
            _damagable.SetSpawned();
        }
        else {
            _damagable.SetDied();
            TeleportInSpawn(); 
        }
    }

    public void SetPlayStatusSilent(bool play) {
        BonusUser.SetDefault();
        IsPlaying = play;
    }


    public bool IsPlaying { get;  private set; }
    public event Action<BonusStatus, bool> BonusStatusChanged;

    
    

    public void HideVisualModel(bool state) {
        _playerVisual.SetActive(!state);
    }

    


    public void SetMovingStatus(bool enable) {
        if (enable) {
            _inputActivity.Enable();
            MoveEnable = true;
        }
        else {
            _inputActivity.Disable();
            MoveEnable = false;
        }
        MoveEnabled?.Invoke(MoveEnable);
    }

    
    public void TeleportInSpawn() {
        TeleportToPoint(_spawnPoint.position);
        RotateToTarget(_spawnPoint.position);
    }


    public void OnJump() {
        if (_jumpsUsed == 0) {
            _verticalVelocity = _firstJumpForce;
            JumpPressed?.Invoke();
            _jumpsUsed = 1;
        }
        else if (_jumpsUsed == 1) {
            _verticalVelocity = _secondJumpForce;
            DoubleJumpPressed?.Invoke();
            _jumpsUsed = 2;
        }
    }


    public void RotateToTarget(Vector3 targetPosition) {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        SetCharacterControllerState(false);
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
        SetCharacterControllerState(true);
        PlayerTeleportToTarget?.Invoke();
    }


    private void Walk() {
        
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * MoveInput.x + camForward * MoveInput.y;
        bool hasInput = move.sqrMagnitude > 0.001f && MoveEnable;

        if (hasInput != IsRunning) {
            IsRunning = hasInput;
            RunningStateChanged?.Invoke(IsRunning);
        }

        // ГРАВИТАЦИЯ
        if (!_controller.isGrounded) {
            _verticalVelocity += Physics.gravity.y * _gameData.GravityScale * Time.deltaTime;
        }

        Vector3 horizontalMove = hasInput
            ? move.normalized * _walkSpeed * Time.deltaTime
            : Vector3.zero;

        Vector3 verticalMove = Vector3.up * _verticalVelocity * Time.deltaTime;

        
        _controller.Move(horizontalMove + verticalMove + _externalMotion);
        _externalMotion = Vector3.zero;
        // Проверяем grounded ПОСЛЕ Move
        IsGrounded = _controller.isGrounded;

        
        // Прилипание к земле (анти-дребезг)
        if (IsGrounded) {
            if (_verticalVelocity < 0f) {
                _verticalVelocity = -2f;
            }
        }

        // Настоящее приземление
        bool justLanded = IsGrounded && !_wasGroundedLastFrame && _verticalVelocity < -0.1f;

        if (justLanded) {
            _jumpsUsed = 0;
            Floored?.Invoke();
        }

        _wasGroundedLastFrame = IsGrounded;

        
        if (hasInput) {
            WalkRotate(move);
        }
        else {
            RotateToCamera();
        }
    }
    
    
    private void WalkRotate(Vector3 move) {
        if (move.sqrMagnitude > 0.0001f) {
            float TargetPosY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            
            float y = Mathf.LerpAngle(
                transform.eulerAngles.y,
                TargetPosY,
                _gameData.RotateSpeed * Time.deltaTime
            );
            
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                y,
                transform.eulerAngles.z
            );
        }
    }
    
    private void SetCharacterControllerState(bool state) {
        _controller.enabled = state;
    }
    
}