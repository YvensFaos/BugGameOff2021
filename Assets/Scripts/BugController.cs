using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class BugController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField] private Animator animator;
    
    [Header("Properties")]
    [SerializeField]
    private float velocity;
    [SerializeField] private float rotationSpeed = 10.0f;
    
    private Vector2 _move;
    
    private readonly float MinimalMovementToRotate = 0.1f;
    private static readonly int Velocity = Animator.StringToHash("Velocity");

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

    #region Player Input Methods

    /// <summary>
    /// Set the Vector2 values for the Move binding in the Input System.
    /// Necessary for the input system.
    /// </summary>
    /// <param name="value"></param>
    public void OnMove(InputValue value)
    {
        if (GameLogic.Logic.IsPaused())
        {
            return;
        }
        _move = value.Get<Vector2>();
    }
    
    #endregion

    private void Update()
    {
        MovementControl();
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

        var selfForward = selfTransform.forward;
        var velocityDirection = selfForward * (movementIntensity * velocity);
        characterController.Move((velocityDirection) * Time.deltaTime);
        animator.SetFloat(Velocity, movementIntensity);
    }
}
