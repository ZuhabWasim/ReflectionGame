using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPlane : MonoBehaviour
{

    public bool isDirty = false;
    public Material dirtyMirrorMaterial; // Dirty texture to use for dirty mirrors

    private Material mirrorMaterial; // the original mirror material (i.e. the reflective one)
    private Renderer mirrorRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mirrorRenderer = GetComponent<Renderer>();
        mirrorMaterial = mirrorRenderer.material;
        if (isDirty)
        {
            // Change the renderer's material to the texture
            mirrorRenderer.material = dirtyMirrorMaterial;
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
            isDirty = true;
            mirrorRenderer.material = mirrorMaterial;
        }
    }
}
