using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Connector object between two mirrors
public class MirrorConnector : MonoBehaviour
{
    public MirrorPlane presentMirror; // Mirror that is in the regular world
    public MirrorPlane pastMirror; // Mirror in the past world

    private Transform m_player; // Player transform, programmatically selected and updated
    private Transform m_playerCamera;

    // Textures to render to.
    private RenderTexture m_presentMirrorTexture;
    private RenderTexture m_pastMirrorTexture;

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
    }

    // Update is called once per frame
    void Update()
    {
        SetMirrorCameraPositions();
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
}
