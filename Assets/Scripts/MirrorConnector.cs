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

        EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INTERACT_KEY ), HandleUserTeleport );
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
        if (!m_canTeleport)
        {
            return;
        }

        // bool pressedInteract = Input.GetKeyDown(KeyCode.M); // TODO: don't hardcode this, use input system.
        bool teleporting = GlobalState.GetVar<bool>(Globals.Vars.TELEPORTING);
        if (!teleporting)
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
            return PlayerInFrontOfMirror(presentMirror.transform);
        }
        else
        {
            return PlayerInFrontOfMirror(pastMirror.transform);
        }
    }

    private bool PlayerInFrontOfMirror(Transform mirrorTransform)
    {
        // Since mirrors can be rotated in arbitrary direction, this uses the dot product definition
        // and pythogrean theorem to figure out if the player is in front of a mirror in a square
        // See: https://mathinsight.org/dot_product

        Vector3 mirrorDirection = new Vector3(mirrorTransform.up.x, 0.0f, mirrorTransform.up.z);

        Vector3 playerPositionRelative = m_player.position - mirrorTransform.position;
        playerPositionRelative = new Vector3(playerPositionRelative.x, 0.0f, playerPositionRelative.z);

        // Projection of player displacement onto mirror direction
        float adjacent = Vector3.Dot(playerPositionRelative, mirrorDirection.normalized);

        float playerDist = Vector3.Distance(playerPositionRelative, new Vector3(0.0f, 0.0f, 0.0f));
        float opposite = playerDist * playerDist - adjacent * adjacent;
        opposite = Mathf.Sqrt(opposite);

        return 0.0f <= adjacent && adjacent < Globals.Misc.MAX_INTERACT_DISTANCE
            && opposite < 1.5f; // TODO: IDK how to get the mirror's width.
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
