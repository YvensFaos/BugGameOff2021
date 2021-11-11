using System;
using System.Collections;
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
    
    [Header("Animation")]
    [SerializeField]
    private float pushAnimationTimer = 0.9f;
    
    private Vector2 _move;
    private bool _canMove;
    
    private readonly float MinimalMovementToRotate = 0.1f;
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int Push = Animator.StringToHash("Push");

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
    
    private void SetupInputCommands()
    {
        playerInput.actions["Push"].performed += PushCommand;
    }

    private void UnsubscribeCommands()
    {
        playerInput.actions["Push"].performed -= PushCommand;
    }
    
    #endregion

    private void Update()
    {
        if (_canMove)
        {
            MovementControl();    
        }
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

    private void PushCommand(InputAction.CallbackContext callback)
    {
        animator.SetTrigger(Push);
        StartCoroutine(PushWaiting());
    }

    private IEnumerator PushWaiting()
    {
        _canMove = false;
        yield return new WaitForSeconds(pushAnimationTimer);
        _canMove = true;
    }
}
