using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class BugController : MonoBehaviour
{
    [Header("Components")] [SerializeField]
    private CharacterController characterController;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;

    [Header("Properties")] [SerializeField]
    private float velocity;

    [SerializeField] private float dashDistance;
    [SerializeField] private float dashTime;
    [SerializeField] private float rotationSpeed = 10.0f;
    [SerializeField] private Transform pushPositionChecker;
    [SerializeField] private float pushCheckDistance;
    [SerializeField] private float pushForce;

    [Header("Checkers")] [SerializeField] private LayerMask dashForbiddenLayers;
    [SerializeField] private LayerMask pushCheckLayers;
    [SerializeField] private LayerMask fatalLayers;

    [Header("Animation")] [SerializeField] private float pushAnimationTimer = 0.9f;
    [SerializeField] private float deadAnimationTimer = 0.9f;

    private Vector2 _move;
    private Vector2 _dash;
    private bool _canMove;
    private bool _isDead;
    private bool _isRespawning;

    private readonly float MinimalMovementToRotate = 0.1f;
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int Push = Animator.StringToHash("Push");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int Dead = Animator.StringToHash("Dead");

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }

    private void Start()
    {
        SetupInputCommands();
        _canMove = true;
    }

    private void OnDestroy()
    {
        UnsubscribeCommands();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((fatalLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            _isDead = true;
            StopAllCoroutines();
            StartCoroutine(DyingCoroutine());
        }
    }

    #region Player Input Methods

    /// <summary>
    /// Set the Vector2 values for the Move binding in the Input System.
    /// Necessary for the input system.
    /// </summary>
    /// <param name="value"></param>
    public void OnMove(InputValue value)
    {
        if (_canMove && !GameLogic.Logic.IsPaused())
        {
            _move = value.Get<Vector2>();
        }
    }

    private void SetupInputCommands()
    {
        playerInput.actions["DashLeft"].performed += DashLeftCommand;
        playerInput.actions["DashRight"].performed += DashRightCommand;
        playerInput.actions["DashUp"].performed += DashUpCommand;
        playerInput.actions["DashDown"].performed += DashDownCommand;
        playerInput.actions["Push"].performed += PushCommand;
        playerInput.actions["Respawn"].performed += Respawn;
    }

    private void UnsubscribeCommands()
    {
        playerInput.actions["DashLeft"].performed -= DashLeftCommand;
        playerInput.actions["DashRight"].performed -= DashRightCommand;
        playerInput.actions["DashUp"].performed -= DashUpCommand;
        playerInput.actions["DashDown"].performed -= DashDownCommand;
        playerInput.actions["Respawn"].performed -= Respawn;
    }

    #endregion

    private void Update()
    {
        if (_canMove)
        {
            MovementControl();
        }
    }

    private void LateUpdate()
    {
        ForceStayingInGroundLevel();
    }

    private void ForceStayingInGroundLevel()
    {
        var transform1 = transform;
        var position = transform1.position;
        position.y = 0.0f;
        transform1.position = position;
    }

    private void MovementControl()
    {
        var selfTransform = transform;
        var movementIntensity = _move.magnitude;

        if (movementIntensity > MinimalMovementToRotate)
        {
            //Invert _move.x to ensure the player is rotating in the same direction as the gamepad / mouse
            var vec3 = new Vector3(-_move.x, 0.0f, _move.y);
            var angle = Vector3.SignedAngle(vec3, Vector3.forward, Vector3.up);
            var rotationQuaternion = Quaternion.Euler(0.0f, angle, 0.0f);
            selfTransform.rotation = Quaternion.Lerp(selfTransform.rotation, rotationQuaternion,
                rotationSpeed * Time.deltaTime);
        }

        var gravity = Vector3.down * 10.0f;

        var selfForward = selfTransform.forward;
        var velocityDirection = selfForward * (movementIntensity * velocity);
        characterController.Move((velocityDirection + gravity) * Time.deltaTime);
        animator.SetFloat(Velocity, movementIntensity);
    }

    private void PushCommand(InputAction.CallbackContext callback)
    {
        if (_canMove && !GameLogic.Logic.IsPaused())
        {
            animator.SetTrigger(Push);
            StartCoroutine(PushWaiting());
        }
    }

    private void Respawn(InputAction.CallbackContext callback)
    {
        if (_isDead)
        {
            StopAllCoroutines();
            StartCoroutine(RespawnCoroutine());
        } else if (!_isRespawning)
        {
            _isRespawning = true;
            var moveTo = GameLogic.Logic.CurrentRespawnPosition;
            characterController.enabled = false;
            animator.SetBool(Dead, false);
            
            transform.DOMove(moveTo, dashTime).OnComplete(() =>
            {
                characterController.enabled = true;
                _canMove = true;
                _isDead = false;
                _isRespawning = false;
            });
        }
    }

    #region Dash Functions
    private void DashLeftCommand(InputAction.CallbackContext callback)
    {
        DashCommand(new Vector2(1, 0));
    }
    
    private void DashRightCommand(InputAction.CallbackContext callback)
    {
        DashCommand(new Vector2(-1, 0));
    }

    private void DashUpCommand(InputAction.CallbackContext callback)
    {
        DashCommand(new Vector2(0, 1));
    }
    
    private void DashDownCommand(InputAction.CallbackContext callback)
    {
        DashCommand(new Vector2(0, -1));
    }
    
    private void DashCommand(Vector2 direction2D)
    {
        if (!_canMove) return;
        
        var direction3D = new Vector3(-direction2D.x, 0.0f, direction2D.y);
        var selfTransform = transform;
        var moveTo = selfTransform.position + direction3D * dashDistance;
        if(Physics.Raycast(selfTransform.position, direction3D, out RaycastHit info, dashDistance, dashForbiddenLayers))
        {
            var collisionPosition = info.point;
            var radius = characterController.radius;
            moveTo = collisionPosition - direction3D * radius;
        }
        
        var direction3DRotation = new Vector3(direction2D.x, 0.0f, direction2D.y);
        var angle = Vector3.SignedAngle(direction3DRotation, Vector3.forward, Vector3.up);
        var rotationQuaternion = Quaternion.Euler(0.0f, angle, 0.0f);
        transform.rotation = rotationQuaternion;

        Debug.DrawLine(selfTransform.position, moveTo, Color.red, 1.0f);
        characterController.enabled = false;
        _canMove = false;
        animator.SetBool(Dash, true);
        transform.DOMove(moveTo, dashTime).OnComplete(() =>
        {
            characterController.enabled = true;
            _canMove = true;
            animator.SetBool(Dash, false);
        });
    }
    #endregion

    private IEnumerator PushWaiting()
    {
        _canMove = false;
        //PushForce(); called by the animation
        yield return new WaitForSeconds(pushAnimationTimer);
        _canMove = true;
    }

    private IEnumerator DyingCoroutine()
    {
        _isDead = true;
        _canMove = false;
        animator.SetBool(Dead, true);
        yield return new WaitForSeconds(deadAnimationTimer);
        GameLogic.Logic.PostProcessManager.TransitionToDeadState();
    }
    
    private IEnumerator RespawnCoroutine()
    {
        _isRespawning = true;
        characterController.enabled = false;
        var moveTo = GameLogic.Logic.CurrentRespawnPosition;
        transform.DOMove(moveTo, dashTime);       
        animator.SetBool(Dead, false);
        yield return new WaitForSeconds(deadAnimationTimer);
        GameLogic.Logic.PostProcessManager.TransitionToRegularState();
        yield return new WaitForSeconds(0.5f);
        _move = Vector2.zero;
        characterController.enabled = true;
        _canMove = true;
        _isDead = false;
        _isRespawning = false;
    }

    /// <summary>
    /// Being called bu the animation
    /// </summary>
    public void PushForce()
    {
        var selfTransform = transform;
        if (Physics.Raycast(pushPositionChecker.position, selfTransform.forward, out RaycastHit info, pushCheckDistance,
            pushCheckLayers))
        {
            var push = info.collider.gameObject;
            if (push.TryGetComponent<PushableObject>(out var pushableObject))
            {
                var forw = selfTransform.forward;
                pushableObject.Push(forw, pushForce);
            }
        }
    }

    // Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, dashDistance);
        
        Gizmos.DrawRay(pushPositionChecker.position, Vector3.forward * pushCheckDistance);
    }
}
