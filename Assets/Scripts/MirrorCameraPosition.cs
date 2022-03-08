using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCameraPosition : MonoBehaviour
{

	private Camera mirrorCam;
	//private Renderer mirrorRenderer;
	private float mirrorWidth;
	private float mirrorHeight;

	private Transform m_mirrorPlaneTransform;
	private Transform m_mirrorCamTransform;
	private Vector3 m_defaultMirrorCamFwd;

	// Start is called before the first frame update
	void Start()
	{
		mirrorCam = GetComponentInChildren<Camera>();
		m_defaultMirrorCamFwd = mirrorCam.transform.forward;
		m_mirrorCamTransform = mirrorCam.transform;
		m_mirrorPlaneTransform = transform.parent;

		//mirrorRenderer = mirrorPlane.GetComponent<Renderer>();
		mirrorWidth = Mathf.Abs( m_mirrorPlaneTransform.localScale.x ) * 10f;
		mirrorHeight = Mathf.Abs( m_mirrorPlaneTransform.localScale.z ) * 10f;
	}

	public Vector3 GetMirrorNormal()
	{
		return m_defaultMirrorCamFwd;
	}

	// Update is called once per frame
	void Update()
	{
		m_mirrorCamTransform.LookAt( m_mirrorPlaneTransform );

		MirrorImp4();
	}

	// Finds reflection of (X1, Z1) in the line with slope m = dZ/dX that goes through point (X2, Z2)
	private static Vector2 RelfectOverLine( float X1, float Z1, float dX, float dZ, float X2, float Z2 )
	{
		// Relative positions
		float relativeX = X2 - X1;
		float relativeZ = Z2 - Z1;

		// Reflected variables in global coordinates
		float reflectedX = 0;
		float reflectedZ = 0;
		if ( Mathf.Abs( dX ) < 0.0001 ) // The mirror is parallel to the z axis
		{
			reflectedX = X2 + relativeX;
			reflectedZ = Z2 - relativeZ;
		}
		else if ( Mathf.Abs( dZ ) < 0.0001 ) // The mirror is parallel to the x axis
		{
			reflectedX = X2 - relativeX;
			reflectedZ = Z2 + relativeZ;
		}
		else // Reflect the point
		{
			// y = mx+b of original slope
			float originalSlope = -dZ / dX;
			float originalIntercept = -originalSlope * X2 + Z2;

			// y = mx+b of inverse slope going through player (will be perpendicular to the above)
			float invSlope = dX / dZ;
			float invIntercept = -invSlope * X1 + Z1;

			// determine intersection point between the two slopes above
			float intersectionX = ( invIntercept - originalIntercept ) / ( originalSlope - invSlope );
			float intersectionZ = invSlope * intersectionX + invIntercept;

			// Using midpoint = (x, y) = ((x1 + x2)/2, (y1 + y2)/2) to find reflected point
			reflectedX = 2 * intersectionX - X1;
			reflectedZ = 2 * intersectionZ - X2;
		}

		return new Vector2( reflectedX, reflectedZ );
	}

	// Reflects the camera over its parent mirror plane line
	public void ReflectOverMirror( Transform player, Transform playerCamera )
	{
		float inputX = player.position.x;
		float inputZ = player.position.z;

		float YRotation = m_mirrorPlaneTransform.rotation.eulerAngles.y;
		float a = Mathf.Cos( YRotation * Mathf.Deg2Rad );  // change in x direction
		float b = Mathf.Sin( YRotation * Mathf.Deg2Rad );  // change in z direction

		Vector2 xz = RelfectOverLine( inputX, inputZ, a, b, m_mirrorPlaneTransform.position.x, m_mirrorPlaneTransform.position.z );
		transform.position = new Vector3( xz.x, playerCamera.position.y, xz.y );
	}

	// Sets the camera position based on the opposite camera. Used for the past/present reflections
	public void SetOppositeCameraPosition( Transform player, Transform playerCamera, Transform otherMirrorPlane )
	{
		Vector3 playerToMirror = otherMirrorPlane.position - player.position; // Relative to the mirror

		// Note: the up/right vectors are in local coordinates
		Vector3 displacement = Vector3.Dot( playerToMirror, otherMirrorPlane.up ) * transform.up;
		displacement = displacement + Vector3.Dot( playerToMirror, otherMirrorPlane.right ) * transform.right;
		displacement += Vector3.Dot( playerToMirror, otherMirrorPlane.forward ) * transform.forward;

		transform.position = displacement + m_mirrorPlaneTransform.position;

		// Fix the camera y position to the player camere
		transform.position = new Vector3( transform.position.x, playerCamera.position.y, transform.position.z );
	}

	public void SetMirrorRenderTexture( RenderTexture texture )
	{
		mirrorCam = GetComponentInChildren<Camera>();
		mirrorCam.targetTexture = texture;
	}

	void MirrorImp1()
	{
		//For some reason this looks okay even though it makes no modifications to the mirror's aspect ratio

		//Using Vertical FOV axis
		//Edit FOV
		float h = Mathf.Abs( transform.position.z - m_mirrorPlaneTransform.position.z );
		float dc = Mathf.Abs( transform.position.x - m_mirrorPlaneTransform.position.x );
		float d = Mathf.Sqrt( h * h + dc * dc );
		mirrorCam.fieldOfView = Mathf.Rad2Deg * 2 * Mathf.Atan( mirrorHeight / ( 2 * d ) );
	}

	void MirrorImp2()
	{
		//UNFINISHED
		//This implementation was supposed to use more realistic physics

		//Using Vertical FOV axis
		//edit FOV
		//Using Vertical FOV axis
		//Edit FOV
		float h = Mathf.Abs( transform.position.z - m_mirrorPlaneTransform.position.z );
		float dc = Mathf.Abs( transform.position.x - m_mirrorPlaneTransform.position.x );
		float d = Mathf.Sqrt( h * h + dc * dc );
		mirrorCam.fieldOfView = Mathf.Rad2Deg * 2 * Mathf.Atan( mirrorHeight / ( 2 * d ) );

		float hyp = Mathf.Sqrt( ( h * h ) + ( ( mirrorWidth / 2 + dc ) * ( mirrorWidth / 2 + dc ) ) );
		float h2 = Mathf.Sqrt( ( h * h ) + ( ( dc - ( mirrorWidth / 2 ) ) * ( dc - ( mirrorWidth / 2 ) ) ) );

		//edit Aspect Ratio of Mirror Texture
		//(unfinished, need to update mirror texture scale and offset to make it the most accurate resolution)
		float deFactoWidth = mirrorWidth;
		float newRes = Mathf.Min( 1f, deFactoWidth / mirrorHeight );
		//mirrorRenderer.material.mainTextureScale = new Vector2(newRes, 1f);
		//mirrorRenderer.material.mainTextureOffset = new Vector2((1-newRes)/2, 1f);
	}

	void MirrorImp3()
	{
		//BAD, OLD implementation
		//Bad bc looking at the mirror close-up from the side results in a tiny FOV and zooms in like crazy

		//Using Horizontal FOV axis
		//edit FOV
		float h = Mathf.Abs( transform.position.z - m_mirrorPlaneTransform.position.z );
		float dc = Mathf.Abs( transform.position.x - m_mirrorPlaneTransform.position.x );
		float hyp = Mathf.Sqrt( ( h * h ) + ( ( mirrorWidth / 2 + dc ) * ( mirrorWidth / 2 + dc ) ) );
		float h2 = Mathf.Sqrt( ( h * h ) + ( ( dc - ( mirrorWidth / 2 ) ) * ( dc - ( mirrorWidth / 2 ) ) ) );
		mirrorCam.fieldOfView = Mathf.Rad2Deg * Mathf.Acos( ( ( h2 * h2 ) + ( hyp * hyp ) - ( mirrorWidth * mirrorWidth ) ) / ( 2 * h2 * hyp ) );
	}

	void MirrorImp4()
	{
		//Using Vertical FOV axis

		float h = Mathf.Abs( transform.position.z - m_mirrorPlaneTransform.position.z );
		float dc = Mathf.Abs( transform.position.x - m_mirrorPlaneTransform.position.x );
		float d = Mathf.Sqrt( h * h + dc * dc );

		float dy = Mathf.Abs( transform.position.y - m_mirrorPlaneTransform.position.y );
		float hyp = Mathf.Sqrt( ( d * d ) + ( ( mirrorHeight / 2 + dy ) * ( mirrorHeight / 2 + dy ) ) );
		float h2 = Mathf.Sqrt( ( d * d ) + ( ( dy - ( mirrorHeight / 2 ) ) * ( dy - ( mirrorHeight / 2 ) ) ) );
		mirrorCam.fieldOfView = Mathf.Rad2Deg * Mathf.Acos( ( ( h2 * h2 ) + ( hyp * hyp ) - ( mirrorHeight * mirrorHeight ) ) / ( 2 * h2 * hyp ) );
	}


}
