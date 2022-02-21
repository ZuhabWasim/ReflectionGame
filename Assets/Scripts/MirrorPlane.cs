using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPlane : MonoBehaviour
{

    public bool isDirty = false;
    public Material dirtyMirrorMaterial; // Dirty texture to use for dirty mirrors

    // TODO: generate this automatically
    public Material mirrorMaterial; // the original mirror material (i.e. the reflective one)

    private Renderer m_mirrorRenderer;

    // Child components of the mirror
    private MirrorCameraPosition m_mirrorCameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_mirrorRenderer = GetComponent<Renderer>();
        m_mirrorCameraPosition = GetComponentInChildren<MirrorCameraPosition>();
        if (m_mirrorCameraPosition == null)
        {
            Debug.LogError("Cannot find MirrorCameraPosition in Mirror " + this.name);
        }

        mirrorMaterial = m_mirrorRenderer.material;
        if (isDirty)
        {
            // Change the renderer's material to the texture
            m_mirrorRenderer.material = dirtyMirrorMaterial;
        }
        else
        {
            m_mirrorRenderer.material = mirrorMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CleanMirror()
    {
        if (isDirty)
        {
            isDirty = false;
            m_mirrorRenderer.material = mirrorMaterial;
            // mirrorMaterial.Lerp( dirtyMirrorMaterial, mirrorMaterial, 0 ); TODO: Fix this, doesn't work atm
        }
    }

    public void ReflectOverMirror(Transform player, Transform playerCamera)
    {
        m_mirrorCameraPosition.ReflectOverMirror(player, playerCamera);
    }

    public void SetOppositeCameraPosition(Transform player, Transform playerCamera, Transform otherMirrorPlane)
    {
        m_mirrorCameraPosition.SetOppositeCameraPosition(player, playerCamera, otherMirrorPlane);
    }
}
