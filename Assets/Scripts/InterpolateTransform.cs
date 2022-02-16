using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MoveDirection
{
    START = 0,
    END
}

enum RotDirection
{
    START = 0,
    END
}

public class InterpolateTransform : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPostion;

    public Vector3 startRotation;
    public Vector3 endRotation;
    public bool interpolateRotation = false;
    public bool interpolatePosition = true;
    public bool loop = false;
    [Tooltip( "Duration to interpolate the over (in seconds)" )]
    public float interpDuration;
    private float m_elapsedTime = 0;
    private bool m_isMoving = false;
    private MoveDirection m_moveDir = MoveDirection.END;
    private RotDirection m_rotDir = RotDirection.END;
    private const float INTERP_DELTA = 0.05f;

    void Start()
    {
        EventManager.Sub( Globals.Events.INTERACT_KEY_PRESSED, OnUserInteract );
    }

    void OnUserInteract( GameObject player )
    {
        if ( this.m_isMoving )
        {
            return;
        }
        
        Transform camera = player.GetComponent<PlayerController>().playerCamera;
        RaycastHit hit;
        if ( Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
            && hit.collider.gameObject.Equals( this.gameObject ) )
        {
            this.m_isMoving = true;
        }
    }

    private void FinalizePosition()
    {
        this.gameObject.transform.position = m_moveDir == MoveDirection.END ? endPostion : startPosition;
        m_moveDir = m_moveDir == MoveDirection.END ? MoveDirection.START : MoveDirection.END;
    }

    private void FinalizeRotation()
    {
        this.gameObject.transform.rotation = Quaternion.Euler( m_rotDir == RotDirection.END ? endRotation : startRotation );
        m_rotDir = m_rotDir == RotDirection.END ? RotDirection.START : RotDirection.END;
    }

    private void CheckMotionFinished( float percentComplete, float targetPercent )
    {
        // can do some approximation here
        if ( percentComplete >= targetPercent - INTERP_DELTA )
        {
            if ( interpolatePosition )
            {
                FinalizePosition();
            }

            if ( interpolateRotation )
            {
                FinalizeRotation();
            }
            if ( !loop )
            {
                m_isMoving = false;
            }
            m_elapsedTime = 0;
        }
    }

    float InterpPosition()
    {
        float percentComplete = m_elapsedTime / interpDuration;

        if ( m_moveDir == MoveDirection.END )
        {
            this.gameObject.transform.position = Vector3.Lerp( startPosition, endPostion, percentComplete );
        }
        else
        {
            this.gameObject.transform.position = Vector3.Lerp( endPostion, startPosition, percentComplete );
        }
        
        return percentComplete;
    }

    float InterpRotation()
    {
        float percentComplete = m_elapsedTime / interpDuration;

        if ( m_rotDir == RotDirection.END )
        {
            this.gameObject.transform.rotation =  Quaternion.Euler( Vector3.Lerp( startRotation, endRotation, percentComplete ) );
        }
        else
        {
            this.gameObject.transform.rotation = Quaternion.Euler( Vector3.Lerp( endRotation, startRotation, percentComplete ) );
        }

        return percentComplete;
    }

    // Update is called once per frame
    void Update()
    {
        if ( !m_isMoving )
        {
            m_elapsedTime = 0;
            return;
        }

        m_elapsedTime += Time.deltaTime;
        
        float percentComplete = 0.0f;
        if ( interpolatePosition )
        {
            percentComplete += InterpPosition();
        }
        if ( interpolateRotation )
        {
            percentComplete += InterpRotation();
        }

        CheckMotionFinished( percentComplete, System.Convert.ToInt16( interpolatePosition ) +  System.Convert.ToInt16( interpolateRotation ) );
    }
}
