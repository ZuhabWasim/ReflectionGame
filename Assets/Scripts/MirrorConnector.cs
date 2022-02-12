using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Connector object between two mirrors
public class MirrorConnector : MonoBehaviour
{
    public MirrorCameraPosition regularMirror; // Mirror that is in the regular world
    public MirrorCameraPosition pastMirror; // Mirror in the past world

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetMirrorCameraPositions();
    }

    private void SetMirrorCameraPositions()
    {
        if (GlobalState.GetVar<bool>("isPresent"))
        {
            pastMirror.SetOppositeCameraPosition(regularMirror.mirrorPlane);
            regularMirror.ReflectOverMirror();
        }
        else
        {
            regularMirror.SetOppositeCameraPosition(pastMirror.mirrorPlane);
            pastMirror.ReflectOverMirror();
        }
    }
}
