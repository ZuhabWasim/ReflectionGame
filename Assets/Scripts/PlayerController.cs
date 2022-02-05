using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ===== CONSTANTS =====
    private const float PITCH_MAX = 88.0f;
    private const float PITCH_MIN = -88.0f;
    private const string H_AXIS = "Horizontal";
    private const string V_AXIS = "Vertical";
    private const string PICKUP_OBJ = "PickupItem";
    // ===== CONSTANTS END ===
    
    // Camera vars
    public Transform playerCamera;
    public float sensitivity = 2.0f;
    private float pitch = 0.0f;
    
    // Movement vars
    private Rigidbody m_playerBody;
    public float speed = 5;
    public float jumpForce;
    public KeyCode jumpKey = KeyCode.Space;

    // Interaction Keys
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;
    public float pickupDistance = 2.0f;
    public float dropDistance = 1.25f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_playerBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }

    void HandleMouseInput()
    {
        Vector2 input = new Vector2( Input.GetAxis( "Mouse X" ), -Input.GetAxis( "Mouse Y" ) );

        pitch += input.y * sensitivity;
        pitch = Mathf.Clamp( pitch, PITCH_MIN, PITCH_MAX );
        playerCamera.localEulerAngles = Vector3.right * pitch;

        transform.Rotate( Vector3.up * input.x * sensitivity );
    }

    bool IsGrounded()
    {
        Collider collider = GetComponent<Collider>();
        float distToGround = collider.bounds.extents.y;
        return Physics.Raycast( transform.position, Vector3.down, distToGround + 0.01f );
    }

    void HandleKeyboardInput()
    {
        Vector3 input = new Vector3( Input.GetAxis( H_AXIS ), 0.0f, Input.GetAxis( V_AXIS ) );
        Vector3 velocity = transform.TransformDirection( input ) * speed;
        m_playerBody.velocity = new Vector3( velocity.x, m_playerBody.velocity.y, velocity.z );

        if ( Input.GetKeyDown( jumpKey ) && IsGrounded() )
        {
            m_playerBody.AddForce( Vector3.up * jumpForce, ForceMode.Impulse );
        }

        HandlePickupAndDrop();
    }

    void HandlePickupAndDrop()
    {
        if ( Input.GetKeyDown( pickupKey ) )
        {
            RaycastHit hitRes;
            if ( Physics.Raycast( playerCamera.position, playerCamera.forward, out hitRes, pickupDistance ) &&
                hitRes.collider.gameObject.tag == PICKUP_OBJ )
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
