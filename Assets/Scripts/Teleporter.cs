using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserLocation
{
    A,
    B
}

public class Teleporter : MonoBehaviour
{

    public GameObject player;
    public GameObject spawnerA;
    public GameObject spawnerB;
    public UserLocation userLocation;
    public KeyCode interactKey = KeyCode.F;

    private bool m_canTeleport = false;
    private bool m_teleporting = false;
    // consts
    const string PLAYER_TAG = "Player";
    const float TELEPORTER_COOLDOWN = 1.75f;
    const float INPUT_LOCK_COOLDOWN = 0.5f;

    void Start()
    {
        GlobalState.AddVar<bool>( "teleporting", false );
    }

    // Update is called once per frame
    void Update()
    {
        HandleUserInput();
    }

    void HandleUserInput()
    {
        bool pressedInteract = Input.GetKeyDown( interactKey );
        if ( m_canTeleport && pressedInteract && !m_teleporting )
        {
            m_teleporting = true;
            GlobalState.SetVar<bool>( "teleporting", true );
            StartCoroutine( Teleport() );
        }
    }

    IEnumerator Teleport()
    {
        Debug.Log( "Teleporting.." );
        player.transform.position = userLocation == UserLocation.A ? spawnerB.transform.position : spawnerA.transform.position; 
        
        userLocation = userLocation == UserLocation.A ? UserLocation.B : UserLocation.A;

        // don't hold the input for entire duration of teleporter cooldown
        yield return new WaitForSecondsRealtime( INPUT_LOCK_COOLDOWN );
        GlobalState.SetVar<bool>( "teleporting", false );

        // add a small delay so the user doesn't accidentally teleport back
        yield return new WaitForSecondsRealtime( TELEPORTER_COOLDOWN );
        
        m_teleporting = false;
    }

    void OnTriggerEnter( Collider other )
    {
        if ( other.tag == PLAYER_TAG )
        {
            Debug.Log( "Can Teleport now" );
            m_canTeleport = true;
        }
    }

    void OnTriggerExit( Collider other )
    {
        if ( other.tag == PLAYER_TAG )
        {
            Debug.Log( "Left Teleport zone" );
            m_canTeleport = false;
        }
    }
}
