using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ===== CONSTANTS =====
    private const float PITCH_MAX = 88.0f;
    private const float PITCH_MIN = -88.0f;

    private const float FOOT_STEP_INTERVAL = 2.2f;
    // ===== CONSTANTS END ===
    
    // Camera vars
    public Transform playerCamera;
    public float sensitivity = 2.0f;
    private float pitch = 0.0f;
    
    // Movement vars
    public Rigidbody m_playerBody;
    private Collider m_collider;
    public float speed = 5;
    public float jumpForce;
    public KeyCode jumpKey = KeyCode.Space;
    public float gravityAccel;

    private Inventory m_inventory;
    private ButtonPromptDisplay bp;
    
    public float pickupDistance = 2.0f;
    public float dropDistance = 1.25f;
    
    // Player sounds
    private AudioSource m_stepSource; // For footsteps and interaction sound effects.
    private float stepCounter = FOOT_STEP_INTERVAL;
    private bool stepRightFoot = true;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_playerBody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();

        m_inventory = Inventory.GetInstance();
        bp = GameObject.Find("UI_Canvas").GetComponent<ButtonPromptDisplay>();

        m_stepSource = GameObject.Find("FootStepSource").GetComponent<AudioSource>();
        PlaySound( "main_door" );

        RegisterEventListeners();
    }

    void RegisterEventListeners()
    {
        // keydown events
        EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.PICKUP_KEY ), HandlePickup );
        EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.DROP_KEY ), HandleDrop );
        EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INVENTORY_KEY ), HandleOpenInventory );

        // keyup events
        EventManager.Sub( InputManager.GetKeyUpEventName( Keybinds.INVENTORY_KEY ), m_inventory.CloseInventory );
    }
    
    public static void PlaySound(string soundEffectPath)
    {
        AudioClip soundEffect = Utilities.AssetLoader.GetSFX( soundEffectPath );
        AudioSource mainSource = GameObject.FindGameObjectWithTag( Globals.Tags.MAIN_SOURCE ).GetComponent<AudioSource>();
        if ( mainSource.isPlaying )
        {
            mainSource.Stop();
        }
        mainSource.clip = soundEffect;
        mainSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
        HandleFootSteps();
    }

    void HandleMouseInput()
    {
        Vector2 input = new Vector2( Input.GetAxis( Globals.Misc.MOUSE_X ), -Input.GetAxis( Globals.Misc.MOUSE_Y ) );

        pitch += input.y * sensitivity;
        pitch = Mathf.Clamp( pitch, PITCH_MIN, PITCH_MAX );
        playerCamera.localEulerAngles = Vector3.right * pitch;

        transform.Rotate( Vector3.up * input.x * sensitivity );
        DisplayInteractionPrompts();
    }

    public bool IsGrounded()
    {
        float distToGround = 1;//m_collider.bounds.extents.y;
        
        // Determines whether a vector from the player's center pointing down is barely touching the floor.
        return Physics.Raycast( transform.position + new Vector3(0,distToGround,0), 
            Vector3.down, distToGround + 0.1f );
    }

    void HandleKeyboardInput()
    {
        Vector3 input = new Vector3( Input.GetAxis( Globals.Misc.H_AXIS ), 0.0f, Input.GetAxis( Globals.Misc.V_AXIS ) );
        Vector3 velocity = transform.TransformDirection( input ) * speed;
        float yVelocity = m_playerBody.velocity.y;

        bool gr = IsGrounded();

        if ( Input.GetKeyDown( jumpKey ) && gr)
        {
            //Debug.Log( "Adding jump force" );
            m_playerBody.AddForce( Vector3.up * jumpForce, ForceMode.Impulse );
        } else if (!gr)
        {
            yVelocity -= gravityAccel * Time.deltaTime;
        }
        m_playerBody.velocity = new Vector3(velocity.x, yVelocity, velocity.z);
    }

    void HandleFootSteps()
    {
        Vector3 input = new Vector3( Input.GetAxis( Globals.Misc.H_AXIS ), 0.0f, Input.GetAxis( Globals.Misc.V_AXIS ) );
        Vector3 velocity = transform.TransformDirection( input ) * speed;
        
        bool gr = IsGrounded();
        
        // Only do foot steps if the player is grounded.
        if (gr)
        {
            if (stepCounter <= 0)
            {
                // Change pitch on right or left footstep
                if (stepRightFoot)
                {
                    m_stepSource.pitch = 1.0f;
                }
                else
                {
                    m_stepSource.pitch = 0.85f;
                }
                m_stepSource.PlayOneShot(m_stepSource.clip);
                stepCounter = FOOT_STEP_INTERVAL;
            }
            else
            {
                if (velocity.magnitude < 0.05)
                {
                    // The player stopped moving. Reset their foot forward to be right.
                    stepRightFoot = true;
                }
                stepCounter -= Time.deltaTime * velocity.magnitude;
            }
        }
        else
        {
            // Reset the counter so they step as soon as they're grounded again.
            stepCounter = 0;
        }
    }

    void HandleOpenInventory()
    {
        m_inventory.openInventory();
        int spin = (int) Input.mouseScrollDelta.y;
        if (spin != 0) {
            m_inventory.SpinInventory(spin);
        }
    }

    void DisplayInteractionPrompts()
    {
        //TODO check pickup/interactable object name to get proper button prompt
        RaycastHit hitRes;
        bool hit = Physics.Raycast(playerCamera.position, playerCamera.forward, out hitRes, pickupDistance);
        bp.hidePrompt();
        if (hit && hitRes.collider.gameObject.tag == Globals.Tags.PICKUP_ITEM)
        {
            bp.SetButton('e');
            bp.showPrompt(Globals.UIStrings.PICKUP_HANDKERCHIEF);
        } else if (hit && hitRes.collider.gameObject.tag == Globals.Tags.INTERACTABLE)
        {
            bp.SetButton('f');
            if (hitRes.collider.gameObject.name == "Drawer")
            {
                bp.showPrompt(Globals.UIStrings.INTERACT_DRAWER);
            } else if (hitRes.collider.gameObject.name == "Switch")
            {
                bp.showPrompt(Globals.UIStrings.INTERACT_SWITCH);
            }else if (hitRes.collider.gameObject.name == "IntroNote")
            {
                bp.showPrompt(Globals.UIStrings.INTERACT_NOTE);
            }
            else if (hitRes.collider.gameObject.name == "Mirror" && m_inventory.GetSelectedItem().Equals("Handkerchief"))
            {
                bp.showPrompt(Globals.UIStrings.USE_HANDKERCHIEF);
            }
        }
    }

    void HandlePickup()
    {
        RaycastHit hitRes;
        if ( Physics.Raycast( playerCamera.position, playerCamera.forward, out hitRes, pickupDistance ) &&
            hitRes.collider.gameObject.tag == Globals.Tags.PICKUP_ITEM )
        {
            PickupItem item = hitRes.collider.gameObject.GetComponent<PickupItem>();
            ItemPickupResult res = Inventory.GetInstance().PickupItem( ref item );
            if ( res != ItemPickupResult.SUCCESS )
            {
                Debug.Log( "Inventory failed to store item" );
            }
        }
    }

    void HandleDrop()
    {
        if ( !Physics.Raycast( playerCamera.position, playerCamera.forward, dropDistance - 0.1f )
                && Inventory.GetInstance().DropItem( playerCamera.position + playerCamera.forward * dropDistance ) )
        {
            Debug.Log( "Dropped Item" );
        }
        else
        {
            Debug.Log( "Failed to drop item" );
        }
    }
}
