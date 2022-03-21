using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Connector object between two mirrors
public class MirrorConnector : MonoBehaviour
{
	public MirrorPlane presentMirror; // Mirror that is in the regular world
	public MirrorPlane pastMirror; // Mirror in the past world

	public int mirrorHeight = 1;

	[Tooltip( "If this mirror can be used to teleport." )]
	public bool teleportable = false;
	[Tooltip( "Check this if the player rotation is incorrect on teleport" )]
	public bool teleportIncorrectRotationFix = false; // this is a hack to get teleport working correctly

	[Tooltip( "If the associated mirrors should be active at start. Do not toggle mid-game, it will not work." )]
	public bool active = true; // Only toggleable in Unity

	// Mirror interactable interfaces
	private MirrorInteractable presentInteractable;
	private MirrorInteractable pastInteractable;

	private Transform m_player; // Player transform, programmatically selected and updated
	private Transform m_playerCamera;

	// Textures to render to.
	private RenderTexture m_presentMirrorTexture;
	private RenderTexture m_pastMirrorTexture;

	// Boolean flags for interactability
	private bool m_active;
	private bool m_canTeleport = false;

	[Tooltip( "Event which should set this object to become active" )]
	public string makeInteractableEvent = "";
	[Tooltip( "Event which should set this object to become inactive" )]
	public string makeNonInteractableEvent = "";
	private InteractionIcon interactionIcon;

	private GameObject[] deactivateVolumes;

	// Start is called before the first frame update
	void Start()
	{
		if ( makeInteractableEvent != string.Empty )
		{
			EventManager.Sub( makeInteractableEvent, () => { Activate(); } );
		}

		if ( makeNonInteractableEvent != string.Empty )
		{
			EventManager.Sub( makeNonInteractableEvent, () => { Deactivate(); } );
		}

		deactivateVolumes = GameObject.FindGameObjectsWithTag( "MirrorInactiveZone" );

		// There must be a presentMirror on start
		if ( presentMirror == null )
		{
			Debug.LogError( "No presentMirror in Mirror " + this.name + ", this is a fatal error" );
			return;
		}

		if ( pastMirror == null )
		{
			Debug.LogError( "No pastMirror in Mirror " + this.name + ", this is a fatal error" );
			return;
		}

		// Find interactables
		SetupMirrorInteractable( presentMirror, presentInteractable );
		SetupMirrorInteractable( pastMirror, pastInteractable );

		// Find the player
		try
		{
			GameObject player = GameObject.FindGameObjectWithTag( "Player" );
			m_player = player.transform;
			m_playerCamera = player.GetComponentInChildren<Camera>().transform;
		}
		catch ( UnityException e )
		{
			Debug.LogError( e.Message );
		}

		// Create render textures
		RenderTextureDescriptor textureDescriptor = new RenderTextureDescriptor( 512, 512 * mirrorHeight, RenderTextureFormat.Default );
		m_presentMirrorTexture = new RenderTexture( textureDescriptor );
		m_pastMirrorTexture = new RenderTexture( textureDescriptor );

		presentMirror.SetCameraRenderTexture( m_presentMirrorTexture );
		pastMirror.SetCameraRenderTexture( m_pastMirrorTexture );

		// After setting mirror camera textures, need to set them to render on the mirrors
		presentMirror.SetMirrorDisplayTexture( m_pastMirrorTexture );
		pastMirror.SetMirrorDisplayTexture( m_presentMirrorTexture );

		m_active = active;
		if ( m_active )
		{
			Activate();
		}
		else
		{
			Deactivate();
		}

		interactionIcon = GameObject.Find( Globals.Misc.UI_Canvas ).GetComponent<InteractionIcon>();
	}

	// Update is called once per frame
	void Update()
	{
		// This needs to be done here because there is no order for Start()
		if ( !m_active )
		{
			return;
		}

		SetMirrorCameraPositions();

		// Check if player is in teleport range only if teleportable
		if ( teleportable && InTeleporterRange() )
		{
			m_canTeleport = true;
			if ( LookingAtTeleporter() )
			{
				interactionIcon.ShowReflectionIcon();
			}
		}
		else
		{
			m_canTeleport = false;
		}
	}

	public void CheckForDeactivateZone()
	{
		for ( int i = 0; i < deactivateVolumes.Length; i++ )
		{
			Bounds b = deactivateVolumes[ i ].GetComponent<Collider>().bounds;
			if ( presentMirror.GetComponent<Renderer>().bounds.Intersects( b )
			|| pastMirror.GetComponent<Renderer>().bounds.Intersects( b ) )
			{
				Deactivate();
				return;
			}
		}
		Activate();
	}

	private void SetMirrorCameraPositions()
	{

		// if (pastMirror == null)
		// {
		//     presentMirror.ReflectOverMirror(m_player, m_playerCamera);
		//     return;
		// }

		if ( GlobalState.GetVar<bool>( "isPresent" ) )
		{
			pastMirror.SetOppositeCameraPosition( m_player, m_playerCamera, presentMirror.transform );
			presentMirror.ReflectOverMirror( m_player, m_playerCamera );
		}
		else
		{
			presentMirror.SetOppositeCameraPosition( m_player, m_playerCamera, pastMirror.transform );
			pastMirror.ReflectOverMirror( m_player, m_playerCamera );
		}
	}

	// Teleports only if m_canTeleport is true
	public void HandleUserTeleport()
	{
		if ( !m_canTeleport )
		{
			return;
		}

		bool teleporting = GlobalState.GetVar<bool>( Globals.Vars.TELEPORTING );
		if ( !teleporting )
		{
			GlobalState.SetVar<bool>( Globals.Vars.TELEPORTING, true );
			StartCoroutine( Teleport() );
		}
	}

	private bool InTeleporterRange()
	{
		if ( pastMirror == null || presentMirror == null ) // No teleporing if a mirror is missing
		{
			return false;
		}

		if ( presentMirror.isDirty || pastMirror.isDirty ) // Do not teleport for dirty mirrors
		{
			return false;
		}

		if ( GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD ) )
		{
			return PlayerInFrontOfMirror( presentMirror.transform );
		}
		else
		{
			return PlayerInFrontOfMirror( pastMirror.transform );
		}
	}

	private bool PlayerInFrontOfMirror( Transform mirrorTransform )
	{
		// Since mirrors can be rotated in arbitrary direction, this uses the dot product definition
		// and pythogrean theorem to figure out if the player is in front of a mirror in a square
		// See: https://mathinsight.org/dot_product

		Vector3 mirrorDirection = new Vector3( mirrorTransform.up.x, 0.0f, mirrorTransform.up.z );

		Vector3 playerPositionRelative = m_player.position - mirrorTransform.position;
		playerPositionRelative = new Vector3( playerPositionRelative.x, 0.0f, playerPositionRelative.z );

		// Projection of player displacement onto mirror direction
		float adjacent = Vector3.Dot( playerPositionRelative, mirrorDirection.normalized );

		float playerDist = Vector3.Distance( playerPositionRelative, new Vector3( 0.0f, 0.0f, 0.0f ) );
		float opposite = playerDist * playerDist - adjacent * adjacent;
		opposite = Mathf.Sqrt( opposite );

		return 0.0f <= adjacent && adjacent < Globals.Misc.MAX_INTERACT_DISTANCE
			&& opposite < 1.5f; // TODO: IDK how to get the mirror's width.
	}

	private bool LookingAtTeleporter()
	{
		if ( pastMirror == null || presentMirror == null ) // No teleporing if a mirror is missing
		{
			return false;
		}

		if ( GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD ) )
		{
			if ( Vector3.Dot( m_playerCamera.forward, presentMirror.transform.up ) < 0.0f )
			{
				return CheckRayIntersectMirror( presentMirror.gameObject );
			}
		}
		else
		{
			if ( Vector3.Dot( m_playerCamera.forward, pastMirror.transform.up ) < 0.0f )
			{
				return CheckRayIntersectMirror( pastMirror.gameObject );
			}
		}

		return false;
	}

	private bool CheckRayIntersectMirror( GameObject mirrorObject )
	{
		Transform camera = m_playerCamera;
		RaycastHit hit;
		if ( Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
			 && hit.collider.gameObject.Equals( mirrorObject ) )
		{
			return true;
		}
		return false;
	}

	public bool CanTeleport()
	{
		return m_canTeleport;
	}

	private Vector3 GetPlayerFwdOnTeleport( Vector3 normal, Vector3 fwd, float relativeRotation, bool isPresent )
	{
		// fwd => incidence vector
		Vector3 reflected = -( fwd - ( 2 * Vector3.Dot( fwd, normal ) * normal ) );
		if ( isPresent ) // traveling from present to past
		{
			relativeRotation = teleportIncorrectRotationFix ? -relativeRotation : relativeRotation;
		}
		return Quaternion.AngleAxis( relativeRotation, Vector3.up ) * reflected;
	}

	public IEnumerator Teleport()
	{
		bool present = GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD );

		Vector3 mirrorPosition = present ? pastMirror.transform.position : presentMirror.transform.position;
		m_player.position = mirrorPosition + ( ( present ? pastMirror : presentMirror ).GetMirrorCameraPosition().GetMirrorNormal() * 1.0f );

		Vector3 mirrorNormal = ( present ? pastMirror : presentMirror ).GetMirrorCameraPosition().GetMirrorNormal();
		m_player.forward = GetPlayerFwdOnTeleport( mirrorNormal, m_player.forward,
			Vector3.Angle( presentMirror.GetMirrorCameraPosition().GetMirrorNormal(), pastMirror.GetMirrorCameraPosition().GetMirrorNormal() ), present ).normalized;

		yield return new WaitForSecondsRealtime( Globals.Teleporting.INPUT_LOCK_COOLDOWN );

		GlobalState.SetVar<bool>( Globals.Vars.TELEPORTING, false );
		EventManager.Fire( Globals.Events.TELEPORT );

		Activate();

		yield return new WaitForSecondsRealtime( Globals.Teleporting.TELEPORTER_COOLDOWN );
	}

	// Sets the mirrors/cameras to be active based on IS_PRESENT_WORLD
	public void Activate()
	{
		m_active = true;
		bool present = GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD );

		// Propagates whether a mirror is teleportable to the Mirror Interactable.
		presentMirror.GetComponent<MirrorInteractable>().setTeleportable( true );
		pastMirror.GetComponent<MirrorInteractable>().setTeleportable( true );

		if ( present )
		{
			// Need to set the present mirror camera to off, and past mirror texture to off
			presentMirror.SetCamera( false );
			presentMirror.SetNormalTexture();

			pastMirror.SetOpaqueTexture();
			pastMirror.SetCamera( true );
		}
		else
		{
			presentMirror.SetCamera( true );
			presentMirror.SetOpaqueTexture();

			pastMirror.SetNormalTexture();
			pastMirror.SetCamera( false );
		}
	}

	// Disable both of the mirrors (i.e. stops both cameras from working and sets them to inactive texture)
	public void Deactivate()
	{
		// Propagates whether a mirror is teleportable to the Mirror Interactable.
		presentMirror.GetComponent<MirrorInteractable>().setTeleportable( false );
		pastMirror.GetComponent<MirrorInteractable>().setTeleportable( false );

		m_active = false;
		presentMirror.Deactivate();
		pastMirror.Deactivate();
	}

	// Note: currently in progress
	public void TeleportSwitch()
	{
		bool isPresent = GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD );
		if ( isPresent )
		{
			presentMirror.Deactivate();

			//pastInteractable.teleportable = false;
			pastMirror.Activate();
		}
		else
		{
			presentMirror.Activate();

			//pastInteractable.teleportable = false;
			pastMirror.Deactivate();
		}
	}

	private void SetupMirrorInteractable( MirrorPlane mirror, MirrorInteractable interactable )
	{
		if ( mirror != null )
		{
			interactable = mirror.GetComponent<MirrorInteractable>();
			if ( interactable != null )
			{
				interactable.SetMirrorConnector( this );
			}
			else
			{
				Debug.LogWarning( "Mirror " + mirror.name + " has no MirrorInteractable. This is likely unintentional" );
			}
		}
	}
}
