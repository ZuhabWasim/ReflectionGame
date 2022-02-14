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
    public GameObject triggerA;
    public GameObject triggerB;
    public UserLocation userLocation;
    public KeyCode interactKey = KeyCode.F;

    private ButtonPromptDisplay bp;
    private bool m_canTeleport = false;
    private bool m_teleporting = false;
    // consts
    const float TELEPORTER_COOLDOWN = 1.75f;
    const float INPUT_LOCK_COOLDOWN = 0.5f;

    void Start()
    {
        GlobalState.AddVar<bool>( Globals.Vars.TELEPORTING, false );
        triggerA.GetComponent<TeleporterTrigger>().SetTeleporter( this );
        triggerB.GetComponent<TeleporterTrigger>().SetTeleporter( this );

        bp = GameObject.Find("UI_Canvas").GetComponent<ButtonPromptDisplay>();
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
            GlobalState.SetVar<bool>( Globals.Vars.TELEPORTING, true );
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
        GlobalState.SetVar<bool>( Globals.Vars.TELEPORTING, false );

        EventManager.Fire( Globals.Events.TELEPORT );

        // add a small delay so the user doesn't accidentally teleport back
        yield return new WaitForSecondsRealtime( TELEPORTER_COOLDOWN );
        
        m_teleporting = false;
    }

    public void SetCanTeleport( bool canTeleport )
    {
        if ( canTeleport )
        {
            m_canTeleport = true;
            bp.showPrompt( Globals.UIStrings.INTERACT_MIRROR );
        }
        else
        {
            m_canTeleport = false;
            bp.hidePrompt();
        }   
    }
}
