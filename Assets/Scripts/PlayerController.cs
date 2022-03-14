using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// ===== CONSTANTS =====
	private const float PITCH_MAX = 88.0f;
	private const float PITCH_MIN = -88.0f;

	private const float FOOT_STEP_INTERVAL = 2.2f;
	private const string FOOTSTEP_AUDIO_SOURCE_NAME = "FootStepSource";
	private const float PRESENT_HEIGHT = 1.5f;
	private const float PAST_HEIGHT = 1.0f;
	private const float RAYCAST_RATE = 10f; // how many raycasts to do per sec

	// ===== CONSTANTS END ===

	// Camera vars
	public Transform playerCamera;
	public float sensitivity = 2.0f;
	public float zoomThreshold = 1.0f;
	public int defaultFOV = 70;
	public int zoomedFOV = 40;
	private bool m_zoomAnimating = false;
	private float pitch = 0.0f;

	// Movement vars
	public Rigidbody m_playerBody;
	private Collider m_collider;
	public float speed = 5;
	public float jumpForce;
	public KeyCode jumpKey = KeyCode.Space;
	public float gravityAccel;

	private Inventory m_inventory;
	private bool inventoryOpened = false;

	public InteractableAbstract targetObject;

	private ButtonPromptDisplay bp;
	private ButtonPromptDisplay bp2;
	private InteractionIcon interactionIcon;
	private PauseMenu pauseMenu;
	private AudioSource m_footstepSource;

	//public float pickupDistance = 2.0f;
	public float dropDistance = 1.25f;

	// Player sounds
	private float stepCounter = FOOT_STEP_INTERVAL;
	private bool stepRightFoot = true;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		m_playerBody = GetComponent<Rigidbody>();
		m_collider = GetComponent<Collider>();

		m_inventory = Inventory.GetInstance();
		targetObject = null;
		bp = GameObject.Find( Globals.Misc.UI_Canvas ).GetComponents<ButtonPromptDisplay>()[ 0 ];
		bp2 = GameObject.Find( Globals.Misc.UI_Canvas ).GetComponents<ButtonPromptDisplay>()[ 1 ];
		interactionIcon = GameObject.Find( Globals.Misc.UI_Canvas ).GetComponent<InteractionIcon>();
		pauseMenu = GameObject.Find( Globals.Misc.UI_Canvas ).GetComponent<PauseMenu>();
		m_footstepSource = GameObject.Find( FOOTSTEP_AUDIO_SOURCE_NAME ).GetComponent<AudioSource>();

		RegisterEventListeners();
		RegisterAudioSources();

		AudioPlayer.Play( Globals.AudioFiles.Section1.MAIN_DOOR, Globals.Tags.MAIN_SOURCE );
		AudioPlayer.Play( Globals.VoiceLines.Section1.DARK_IN_HERE, Globals.Tags.DIALOGUE_SOURCE );
		AudioPlayer.Play( Globals.AudioFiles.Ambience.PRESENT_AMBIENCE, Globals.Tags.AMBIENCE_SOURCE );
		StartCoroutines();
	}

	void StartCoroutines()
	{
		StartCoroutine( LookAtObject() );
	}

	void RegisterEventListeners()
	{
		// keydown events
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INTERACT_KEY ), HandleInteractPress );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INTERACT_KEY ), SkipCurrentVoiceline );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.USE_ITEM_KEY ), HandleUseItemPress );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.DROP_KEY ), HandleDrop );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INVENTORY_KEY ), HandleOpenInventory );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.ESCAPE_KEY ), HandleEscape );

		// keyup events
		EventManager.Sub( InputManager.GetKeyUpEventName( Keybinds.INVENTORY_KEY ), HandleCloseInventory );

		// Other events
		EventManager.Sub( Globals.Events.TELEPORT, SwitchHeightOnTeleport );
		EventManager.Sub( Globals.Events.TELEPORT, ChangeAudioOnTeleport );
	}

	void RegisterAudioSources()
	{
		AudioPlayer.RegisterAudioPlayer( Globals.Tags.MAIN_SOURCE, GameObject.FindGameObjectWithTag( Globals.Tags.MAIN_SOURCE ).GetComponent<AudioSource>() );
		AudioPlayer.RegisterAudioPlayer( Globals.Tags.DIALOGUE_SOURCE, GameObject.FindGameObjectWithTag( Globals.Tags.DIALOGUE_SOURCE ).GetComponent<AudioSource>() );
		AudioPlayer.RegisterAudioPlayer( Globals.Tags.AMBIENCE_SOURCE, GameObject.FindGameObjectWithTag( Globals.Tags.AMBIENCE_SOURCE ).GetComponent<AudioSource>() );
		AudioPlayer.RegisterAudioPlayer( Globals.Tags.MUSIC_SOURCE, GameObject.FindGameObjectWithTag( Globals.Tags.MUSIC_SOURCE ).GetComponent<AudioSource>() );
	}

	// Update is called once per frame
	void Update()
	{
		if ( !pauseMenu.IsPaused() )
		{
		HandleMouseInput();
		HandleKeyboardInput();
		HandleFootSteps();
		}
	}

	void HandleMouseInput()
	{
		Vector2 input = new Vector2( Input.GetAxis( Globals.Misc.MOUSE_X ), -Input.GetAxis( Globals.Misc.MOUSE_Y ) );

		pitch += input.y * sensitivity;
		pitch = Mathf.Clamp( pitch, PITCH_MIN, PITCH_MAX );
		playerCamera.localEulerAngles = Vector3.right * pitch;

		transform.Rotate( Vector3.up * input.x * sensitivity );

		DisplayInteractionPrompts();
		ShowInteractionIcon();

		float scrollDelta = Input.mouseScrollDelta.y;
		if ( Mathf.Abs( scrollDelta ) >= zoomThreshold )
		{
			HandleZoom( scrollDelta );
		}

		if ( inventoryOpened )
		{
			int spin = (int)Input.mouseScrollDelta.y;
			if ( spin != 0 )
			{
				m_inventory.SpinInventory( spin );
			}
		}
	}

	void HandleZoom( float scrollDelta )
	{
		if ( m_zoomAnimating || inventoryOpened ) return;
		m_zoomAnimating = true;
		int newFov = scrollDelta > 0 ? zoomedFOV : defaultFOV;
		StartCoroutine( AnimateZoom( newFov ) );
	}

	private IEnumerator AnimateZoom( int newFOV )
	{
		float elapsedTime = 0.0f, percentComplete = 0.0f;
		Camera camera = playerCamera.gameObject.GetComponent<Camera>();
		float currentFOV = camera.fieldOfView;
		const float approximationDelta = 0.99f;
		const float animationDuration = 0.5f;

		while ( ( (int)currentFOV ) != newFOV )
		{
			elapsedTime += Time.deltaTime;
			percentComplete = elapsedTime / animationDuration;
			camera.fieldOfView = Mathf.Lerp( currentFOV, newFOV, percentComplete );
			// approximate for the last few values
			if ( percentComplete >= approximationDelta )
			{
				camera.fieldOfView = newFOV;
				break;
			}

			yield return new WaitForEndOfFrame();
		}

		m_zoomAnimating = false;
	}

	public bool IsGrounded()
	{
		float distToGround = 1;//m_collider.bounds.extents.y;

		// Determines whether a vector from the player's center pointing down is barely touching the floor.
		return Physics.Raycast( transform.position + new Vector3( 0, distToGround, 0 ),
			Vector3.down, distToGround + 0.1f );
	}

	void HandleKeyboardInput()
	{
		Vector3 input = new Vector3( Input.GetAxis( Globals.Misc.H_AXIS ), 0.0f, Input.GetAxis( Globals.Misc.V_AXIS ) );
		Vector3 velocity = transform.TransformDirection( input ) * speed;
		float yVelocity = m_playerBody.velocity.y;

		bool gr = IsGrounded();

		if ( Input.GetKeyDown( jumpKey ) && gr )
		{
			//Debug.Log( "Adding jump force" );
			m_playerBody.AddForce( Vector3.up * jumpForce, ForceMode.Impulse );
		}
		else if ( !gr )
		{
			yVelocity -= gravityAccel * Time.deltaTime;
		}
		m_playerBody.velocity = new Vector3( velocity.x, yVelocity, velocity.z );
	}

	void HandleEscape()
	{
		if ( pauseMenu.IsPaused() ) {
			pauseMenu.ResumeGame();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			pauseMenu.PauseGame();
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}
	}

	void SkipCurrentVoiceline()
	{
		// user presses INTERACT_KEY without looking at some interactable object => user is trying to skip voiceline
		if ( targetObject != null )
		{
			return;
		}
		AudioPlayer.StopCurrentClip( Globals.Tags.DIALOGUE_SOURCE );
	}

	void HandleFootSteps()
	{
		Vector3 input = new Vector3( Input.GetAxis( Globals.Misc.H_AXIS ), 0.0f, Input.GetAxis( Globals.Misc.V_AXIS ) );
		Vector3 velocity = transform.TransformDirection( input ) * speed;

		bool gr = IsGrounded();

		// Only do foot steps if the player is grounded.
		if ( gr )
		{
			if ( stepCounter <= 0 )
			{
				// Change pitch on right or left footstep
				if ( stepRightFoot )
				{
					m_footstepSource.pitch = 1.0f;
				}
				else
				{
					m_footstepSource.pitch = 0.85f;
				}
				m_footstepSource.PlayOneShot( m_footstepSource.clip );
				stepCounter = FOOT_STEP_INTERVAL;
			}
			else
			{
				if ( velocity.magnitude < 0.05 )
				{
					// The player stopped moving. Reset their foot forward to be right.
					stepRightFoot = true;
				}
				stepCounter -= Time.deltaTime * velocity.magnitude;
			}
		}
		else
		{
			// Reset the counter so they step as soon as they're grounded again.
			stepCounter = 0;
		}
	}

	void HandleInteractPress()
	{
		if ( targetObject != null )
		{
			if ( targetObject.GetItemType() == InteractableAbstract.ItemType.PICKUP )
			{
				PickupItem item = (PickupItem)targetObject;
				ItemPickupResult res = Inventory.GetInstance().PickupItem( ref item );
				if ( res != ItemPickupResult.SUCCESS )
				{
					Debug.Log( "Inventory failed to store item" );
				}
			}
			else
			{
				targetObject.ActivateItem();
			}
		}
	}

	void HandleUseItemPress()
	{
		if ( targetObject != null && targetObject.interactable && targetObject.WillAcceptItem() )
		{
			targetObject.ActivateUseItem( m_inventory.GetSelectedItem() );
		}
	}

	void HandleOpenInventory()
	{
		m_inventory.openInventory();
		inventoryOpened = true;
	}
	void HandleCloseInventory()
	{
		m_inventory.CloseInventory();
		inventoryOpened = false;
	}

	IEnumerator LookAtObject()
	{
		while ( true )
		{
			RaycastHit hitRes;
			bool hit = Physics.Raycast( playerCamera.position, playerCamera.forward, out hitRes, Globals.Misc.MAX_INTERACT_DISTANCE );
			if ( hit && hitRes.collider.gameObject.GetComponent<InteractableAbstract>() != null )
			{
				targetObject = hitRes.collider.gameObject.GetComponent<InteractableAbstract>();
			}
			else
			{
				targetObject = null;
			}

			yield return new WaitForSecondsRealtime( 1 / RAYCAST_RATE );
		}
	}

	void DisplayInteractionPrompts()
	{
		if ( targetObject != null && targetObject.interactable )
		{
			if ( targetObject.WillDisplayPrompt() )
			{
				bp.SetButton( 'f' );
				bp.showPrompt( targetObject.GetPromptText() );
			}
			else
			{
				bp.hidePrompt();
			}
			string selectedItem = m_inventory.GetSelectedItem();
			if (targetObject.thisIsAMirror) {
				bp2.SetButton('e');
				bp2.showPrompt("Reflect across Mirror");
			} else if ( targetObject.WillAcceptItem() && selectedItem != "" )
			{
				bp2.SetButton('e');
				bp2.showPrompt(targetObject.GetItemText(selectedItem));
			}
			else
			{
				bp2.hidePrompt();
			}
		}
		else
		{
			bp.hidePrompt();
			bp2.hidePrompt();
		}
	}
	void ShowInteractionIcon()
	{
		if ( targetObject != null )
		{
			if ( targetObject.GetItemType() == InteractableAbstract.ItemType.INTERACT )
			{
				interactionIcon.ShowEyeIcon();
			}
			else
			{
				interactionIcon.ShowHandIcon();
			}
		}
		else
		{
			interactionIcon.HideIcon();
		}
	}
	void HandleDrop()
	{
		//Deprecated??
		if ( !Physics.Raycast( playerCamera.position, playerCamera.forward, dropDistance - 0.1f )
				&& Inventory.GetInstance().DropItem( playerCamera.position + playerCamera.forward * dropDistance ) )
		{
			Debug.Log( "Dropped Item" );
		}
		else
		{
			Debug.Log( "Failed to drop item" );
		}
	}

	void SwitchHeightOnTeleport()
	{
		if ( GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD ) )
		{
			transform.localScale = new Vector3( transform.localScale.x, PRESENT_HEIGHT, transform.localScale.z );
		}
		else
		{
			transform.localScale = new Vector3( transform.localScale.x, PAST_HEIGHT, transform.localScale.z );
		}
	}

	void ChangeAudioOnTeleport()
	{
		if ( GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD ) )
		{
			AudioPlayer.Play( Globals.AudioFiles.Ambience.PRESENT_AMBIENCE, Globals.Tags.AMBIENCE_SOURCE );
		}
		else
		{
			AudioPlayer.Play( Globals.AudioFiles.Ambience.PAST_AMBIENCE, Globals.Tags.AMBIENCE_SOURCE );
		}
	}
}
