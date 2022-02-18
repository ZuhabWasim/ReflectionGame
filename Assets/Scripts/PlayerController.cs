using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ===== CONSTANTS =====
    private const float PITCH_MAX = 88.0f;
    private const float PITCH_MIN = -88.0f;
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

    // Gameplay vars
    private bool isPresent = true; // Start in present

    // Interaction Keys
    public KeyCode openInventoryKey = Globals.Keybinds.InventoryKey;
    public KeyCode pickupKey = Globals.Keybinds.PickupKey;
    public KeyCode dropKey = Globals.Keybinds.DropKey;
    public KeyCode interactKey = Globals.Keybinds.InteractKey;
    
    private bool interactKeyDown = false;
    
    public float pickupDistance = 2.0f;
    public float dropDistance = 1.25f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_playerBody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }



    void HandleMouseInput()
    {
        Vector2 input = new Vector2( Input.GetAxis( Globals.Misc.MOUSE_X ), -Input.GetAxis( Globals.Misc.MOUSE_Y ) );

        pitch += input.y * sensitivity;
        pitch = Mathf.Clamp( pitch, PITCH_MIN, PITCH_MAX );
        playerCamera.localEulerAngles = Vector3.right * pitch;

        transform.Rotate( Vector3.up * input.x * sensitivity );
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

        HandlePickupAndDrop();
        HandleInteractKeyPress();
        HandleOpenInventory();
    }

    void HandleInteractKeyPress()
    {
        if ( Input.GetKeyDown( interactKey ) && !interactKeyDown )
        {
            EventManager.Fire( Globals.Events.INTERACT_KEY_PRESSED, this.gameObject );
            interactKeyDown = true;
        }
        else if ( Input.GetKeyUp( interactKey ) )
        {
            interactKeyDown = false;
        }
    }

    void HandleOpenInventory()
    {
        if (Input.GetKey(openInventoryKey)) {
            Inventory.GetInstance().openInventory();
            int spin = (int) Input.mouseScrollDelta.y;
            if (spin != 0) {
                Inventory.GetInstance().spinInventory(spin);
            }
        } else {
            Inventory.GetInstance().closeInventory();
        }
    }

    void HandlePickupAndDrop()
    {
        if ( Input.GetKeyDown( pickupKey ) )
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

        if ( Input.GetKeyDown( dropKey ) )
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
}
