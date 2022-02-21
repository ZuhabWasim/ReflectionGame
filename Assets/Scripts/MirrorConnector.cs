using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Connector object between two mirrors
public class MirrorConnector : MonoBehaviour
{
    public MirrorCameraPosition regularMirror; // Mirror that is in the regular world
    public MirrorCameraPosition pastMirror; // Mirror in the past world

    private Transform m_player; // Player transform, programmatically selected and updated
    private Transform m_playerCamera;

    // Start is called before the first frame update
    void Start()
    {
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
            regularMirror.ReflectOverMirror(m_player, m_playerCamera);
            return;
        }

        if (GlobalState.GetVar<bool>("isPresent"))
        {
            pastMirror.SetOppositeCameraPosition(m_player, m_playerCamera, regularMirror.mirrorPlane);
            regularMirror.ReflectOverMirror(m_player, m_playerCamera);
        }
        else
        {
            regularMirror.SetOppositeCameraPosition(m_player, m_playerCamera, pastMirror.mirrorPlane);
            pastMirror.ReflectOverMirror(m_player, m_playerCamera);
        }
    }
}
