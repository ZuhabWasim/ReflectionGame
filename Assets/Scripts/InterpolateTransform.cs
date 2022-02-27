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

public enum InterpolateTransformTriggerMethod
{
    USER_INTERACT = 0,
    SCRIPT,
    BOTH
}

public class InterpolateTransform : InteractableAbstract
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
    public InterpolateTransformTriggerMethod triggerMethod = InterpolateTransformTriggerMethod.USER_INTERACT;
    private float m_elapsedTime = 0;
    private bool m_isMoving = false;
    private MoveDirection m_moveDir = MoveDirection.END;
    private RotDirection m_rotDir = RotDirection.END;
    private const float INTERP_DELTA = 0.05f;

    void Start()
    {
    }

    public override void OnUserInteract()
    {
        if ( triggerMethod == InterpolateTransformTriggerMethod.SCRIPT )
        {
            return;
        }
        if ( this.m_isMoving )
        {
            return;
        }

        if (myType == ItemType.OPEN)
        {
            SetType(ItemType.CLOSE);
        } else if (myType == ItemType.CLOSE)
        {
            SetType(ItemType.OPEN);
        }

        this.m_isMoving = true;
    }

    public void TriggerMotion()
    {
        if ( triggerMethod == InterpolateTransformTriggerMethod.USER_INTERACT )
        {
            return;
        }
        this.m_isMoving = true;
    }

    private void FinalizePosition()
    {
        this.gameObject.transform.localPosition = m_moveDir == MoveDirection.END ? endPostion : startPosition;
        m_moveDir = m_moveDir == MoveDirection.END ? MoveDirection.START : MoveDirection.END;
    }

    private void FinalizeRotation()
    {
        this.gameObject.transform.localRotation = Quaternion.Euler( m_rotDir == RotDirection.END ? endRotation : startRotation );
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
            this.gameObject.transform.localPosition = Vector3.Lerp( startPosition, endPostion, percentComplete );
        }
        else
        {
            this.gameObject.transform.localPosition = Vector3.Lerp( endPostion, startPosition, percentComplete );
        }
        
        return percentComplete;
    }

    float InterpRotation()
    {
        float percentComplete = m_elapsedTime / interpDuration;

        if ( m_rotDir == RotDirection.END )
        {
            this.gameObject.transform.localRotation =  Quaternion.Euler( Vector3.Lerp( startRotation, endRotation, percentComplete ) );
        }
        else
        {
            this.gameObject.transform.localRotation = Quaternion.Euler( Vector3.Lerp( endRotation, startRotation, percentComplete ) );
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
