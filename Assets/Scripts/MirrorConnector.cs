using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Connector object between two mirrors
public class MirrorConnector : MonoBehaviour
{
    public MirrorPlane presentMirror; // Mirror that is in the regular world
    public MirrorPlane pastMirror; // Mirror in the past world

    public bool teleportable = false;
    public bool active = true; // Only toggleable in Unity

    private Transform m_player; // Player transform, programmatically selected and updated
    private Transform m_playerCamera;

    // Textures to render to.
    private RenderTexture m_presentMirrorTexture;
    private RenderTexture m_pastMirrorTexture;
    private bool m_canTeleport = false;
    private bool m_active;

    // Start is called before the first frame update
    void Start()
    {
        // There must be a presentMirror on start
        if (presentMirror == null)
        {
            Debug.LogError("No presentMirror in Mirror " + this.name);
            return;
        }

        // Find the player
        try
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            m_player = player.transform;
            m_playerCamera = player.GetComponentInChildren<Camera>().transform;
        }
        catch (UnityException e)
        {
            Debug.LogError(e.Message);
        }

        // Create render textures
        RenderTextureDescriptor textureDescriptor = new RenderTextureDescriptor(512, 512, RenderTextureFormat.Default);
        m_presentMirrorTexture = new RenderTexture(textureDescriptor);

        if (pastMirror != null)
        {
            m_pastMirrorTexture = new RenderTexture(textureDescriptor);

            presentMirror.SetCameraRenderTexture(m_presentMirrorTexture);
            pastMirror.SetCameraRenderTexture(m_pastMirrorTexture);

            // After setting mirror camera textures, need to set them to render on the mirrors
            presentMirror.SetMirrorDisplayTexture(m_pastMirrorTexture);
            pastMirror.SetMirrorDisplayTexture(m_presentMirrorTexture);
        }
        else
        {
            presentMirror.SetCameraRenderTexture(m_presentMirrorTexture);
            presentMirror.SetMirrorDisplayTexture(m_presentMirrorTexture);
        }

        m_active = active;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_active)
        {
            return;
        }

        SetMirrorCameraPositions();

        // Check if player is in teleport range only if teleportable
        if (teleportable && InTeleporterRange() && LookingAtTeleporter())
        {
            Debug.Log("Can teleport");

            m_canTeleport = true;
            HandleUserTeleport();
        }
        else
        {
            m_canTeleport = false;
        }
    }

    private void SetMirrorCameraPositions()
    {

        if (pastMirror == null)
        {
            presentMirror.ReflectOverMirror(m_player, m_playerCamera);
            return;
        }

        if (GlobalState.GetVar<bool>("isPresent"))
        {
            pastMirror.SetOppositeCameraPosition(m_player, m_playerCamera, presentMirror.transform);
            presentMirror.ReflectOverMirror(m_player, m_playerCamera);
        }
        else
        {
            presentMirror.SetOppositeCameraPosition(m_player, m_playerCamera, pastMirror.transform);
            pastMirror.ReflectOverMirror(m_player, m_playerCamera);
        }
    }

    private void HandleUserTeleport()
    {
        bool pressedInteract = Input.GetKeyDown(KeyCode.M); // TODO: don't hardcode this, use input system.
        bool teleporting = GlobalState.GetVar<bool>(Globals.Vars.TELEPORTING);
        if (pressedInteract && !teleporting)
        {
            GlobalState.SetVar<bool>(Globals.Vars.TELEPORTING, true);
            StartCoroutine(Teleport());
        }
    }

    private bool InTeleporterRange()
    {
        if (pastMirror == null || presentMirror == null) // No teleporing if a mirror is missing
        {
            return false;
        }

        if (presentMirror.isDirty || pastMirror.isDirty) // Do not teleport for dirty mirrors
        {
            return false;
        }

        if (GlobalState.GetVar<bool>(Globals.Vars.IS_PRESENT_WORLD))
        {
            return Vector3.Distance(m_player.position, presentMirror.transform.position) < Globals.Misc.MAX_INTERACT_DISTANCE;
        }
        else
        {
            if (pastMirror != null) // Should not fail
            {
                return Vector3.Distance(m_player.position, pastMirror.transform.position) < Globals.Misc.MAX_INTERACT_DISTANCE;
            }
        }
        return false;
    }

    private bool LookingAtTeleporter()
    {
        if (pastMirror == null || presentMirror == null) // No teleporing if a mirror is missing
        {
            return false;
        }

        if (GlobalState.GetVar<bool>(Globals.Vars.IS_PRESENT_WORLD))
        {
            if (Vector3.Dot(m_playerCamera.forward, presentMirror.transform.up) < 0.0f)
            {
                return CheckRayIntersectMirror(presentMirror.gameObject);
            }
        }
        else
        {
            if (Vector3.Dot(m_playerCamera.forward, pastMirror.transform.up) < 0.0f)
            {
                return CheckRayIntersectMirror(pastMirror.gameObject);
            }
        }

        return false;
    }

    private bool CheckRayIntersectMirror(GameObject mirrorObject)
    {
        Transform camera = m_playerCamera;
        RaycastHit hit;
        if (Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
             && hit.collider.gameObject.Equals( mirrorObject ))
        {
            return true;
        }
        return false;
    }


    public bool CanTeleport()
    {
        return m_canTeleport;
    }

    public IEnumerator Teleport()
    {
        bool present = GlobalState.GetVar<bool>(Globals.Vars.IS_PRESENT_WORLD);

        Vector3 mirrorPosition = present ? pastMirror.transform.position : presentMirror.transform.position;
        m_player.position = new Vector3(mirrorPosition.x, m_player.position.y + 0.05f, mirrorPosition.z + 2.0f);

        yield return new WaitForSecondsRealtime( Globals.Teleporting.INPUT_LOCK_COOLDOWN );

        GlobalState.SetVar<bool>( Globals.Vars.TELEPORTING, false );
        EventManager.Fire( Globals.Events.TELEPORT );

        yield return new WaitForSecondsRealtime( Globals.Teleporting.TELEPORTER_COOLDOWN );
    } 

}
