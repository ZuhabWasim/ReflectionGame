using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    // ===== CONSTANTS =====
    private const float JUMP_ANIMATION_THRESHOLD = 0.2f;
    // ===== CONSTANTS END ===

    // Animator Parameters
    private int isWalkingHash;
    private int isJumpingHash;
    private int isBackingUpHash;
    private int isInteractingHash;

    // Components
    Animator animator;
    private PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // Animation Parameter Hashes
        isWalkingHash = Animator.StringToHash(Globals.Misc.IS_WALKING);
        isJumpingHash = Animator.StringToHash(Globals.Misc.IS_JUMPING);
        isBackingUpHash = Animator.StringToHash(Globals.Misc.IS_BACKING_UP);
        isInteractingHash = Animator.StringToHash(Globals.Misc.IS_INTERACTING);

        EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.PICKUP_KEY ),
            () => {
                AnimatePickup( true );
                }
            );
        
        EventManager.Sub( InputManager.GetKeyUpEventName( Keybinds.PICKUP_KEY ),
            () => {
                AnimatePickup( false );
                }
            );
    }

    void AnimatePickup( bool pickUpPress )
    {
        bool isInteracting = animator.GetBool( isInteractingHash );
        if ( !isInteracting && pickUpPress )
        {
            animator.SetBool( isInteractingHash, true );
        }

        if ( isInteracting && !pickUpPress )
        {
            animator.SetBool( isInteractingHash, false );
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Retrieve all animation parameters
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isBackingUp = animator.GetBool(isBackingUpHash);

        // Player movement
        float verticalMovement = Input.GetAxis("Vertical");
        float horizontalMovement = Math.Abs(Input.GetAxis("Horizontal"));

        // Whether the player is moving forward, backward, or sideways
        bool forwardPress = verticalMovement > 0;
        bool backwardPress = verticalMovement < 0;
        bool sidewaysPress = horizontalMovement > 0;

        // Player events
        // bool pickUpPress = Input.GetKeyDown( _playerController.pickupKey );
        bool jumpAcceleration = _playerController.m_playerBody.velocity.y > JUMP_ANIMATION_THRESHOLD;

        // Jumping
        if (!isJumping && jumpAcceleration)
        {
            animator.SetBool(isJumpingHash, true);
        }

        if (isJumping && !jumpAcceleration)
        {
            animator.SetBool(isJumpingHash, false);
        }
        

        // Only do other movement animations if you're grounded.
        if (_playerController.IsGrounded())
        {
            // Moving forward or sideways
            if (!isWalking && (forwardPress || sidewaysPress))
            {
                animator.SetBool(isWalkingHash, true);
            }

            if (isWalking && (!forwardPress && !sidewaysPress))
            {
                animator.SetBool(isWalkingHash, false);
            }

            // Moving backward
            if (!isBackingUp && backwardPress)
            {
                animator.SetBool(isBackingUpHash, true);
            }

            if (isBackingUp && !backwardPress)
            {
                animator.SetBool(isBackingUpHash, false);
            }
        }
    }
}