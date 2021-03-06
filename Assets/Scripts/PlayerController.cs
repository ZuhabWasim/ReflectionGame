// PREPROCESSOR FLAGS
#undef TEST_LEVEL // set this to #undef or comment out when running main-level. When defined, all audio stuff is disabled
#undef USING_ZOOM

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
#if USING_ZOOM
	public int zoomedFOV = 40;
	private bool m_zoomAnimating = false;
#endif // if USING_ZOOM
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
	private JournalDisplay journalDisplay;
	private AudioSource m_footstepSource;
	private AudioClip m_defaultFootstepSound;

	private ZoneManager m_zoneManager;

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
		journalDisplay = GameObject.Find( Globals.Misc.UI_Canvas ).GetComponent<JournalDisplay>();
		m_footstepSource = GameObject.Find( FOOTSTEP_AUDIO_SOURCE_NAME ).GetComponent<AudioSource>();
		m_defaultFootstepSound = m_footstepSource.clip;
		m_zoneManager = GameObject.FindObjectOfType<ZoneManager>();

		RegisterEventListeners();
#if !TEST_LEVEL
		RegisterAudioSources();

		AudioPlayer.Play( Globals.AudioFiles.Section1.MAIN_DOOR, Globals.Tags.MAIN_SOURCE );
		AudioPlayer.Play( Globals.VoiceLines.Section1.DARK_IN_HERE, Globals.Tags.DIALOGUE_SOURCE );
		AudioPlayer.Play( Globals.AudioFiles.Ambience.PRESENT_AMBIENCE, Globals.Tags.AMBIENCE_SOURCE );

		// Play the stems all at once to keep them synced
		AudioPlayer.Play( Globals.AudioFiles.Music.MILLIE_STEM, Globals.Tags.MILLIE_STEM );
		AudioPlayer.Play( Globals.AudioFiles.Music.MOM_STEM, Globals.Tags.MOM_STEM );
		AudioPlayer.Play( Globals.AudioFiles.Music.DAD_STEM, Globals.Tags.DAD_STEM );

		// Initially keep them mute.
		AudioPlayer.SetSourceVolume( Globals.Tags.MILLIE_STEM, 0f );
		AudioPlayer.SetSourceVolume( Globals.Tags.MOM_STEM, 0f );
		AudioPlayer.SetSourceVolume( Globals.Tags.DAD_STEM, 0f );


#endif // if TEST_LEVEL
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
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.USE_ITEM_KEY ), HandleDropItem );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.USE_ITEM_KEY ), HandleUseItemPress );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.REFLECT_KEY ), HandleReflectPress );
		// EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.DROP_KEY ), HandleDrop );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INVENTORY_KEY ), HandleOpenInventory );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.ESCAPE_KEY ), HandleEscape );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.TAB_KEY ), HandleJournalKey );

		// for cheat code
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.CHEAT_KEY_1 ), () => { InputManager.HandleCheatInput( "1" ); } );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.CHEAT_KEY_8 ), () => { InputManager.HandleCheatInput( "8" ); } );
		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.CHEAT_KEY_3 ), () => { InputManager.HandleCheatInput( "3" ); } );
		EventManager.Sub( Globals.Events.CHEAT_SUCCESS, () => { GetMusicBox().OpenMusicBox(); } );

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

		AudioPlayer.RegisterAudioPlayer( Globals.Tags.MILLIE_STEM, GameObject.FindGameObjectWithTag( Globals.Tags.MILLIE_STEM ).GetComponent<AudioSource>() );
		AudioPlayer.RegisterAudioPlayer( Globals.Tags.MOM_STEM, GameObject.FindGameObjectWithTag( Globals.Tags.MOM_STEM ).GetComponent<AudioSource>() );
		AudioPlayer.RegisterAudioPlayer( Globals.Tags.DAD_STEM, GameObject.FindGameObjectWithTag( Globals.Tags.DAD_STEM ).GetComponent<AudioSource>() );
	}

	// Update is called once per frame
	void Update()
	{
		if ( !pauseMenu.IsPaused() && !journalDisplay.IsOpened() )
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

		//DisplayInteractionPrompts();
		ShowInteractionIcon();

		float scrollDelta = Input.mouseScrollDelta.y;
#if USING_ZOOM
		if ( Mathf.Abs( scrollDelta ) >= zoomThreshold )
		{
			HandleZoom( scrollDelta );
		}
#endif // if USING_ZOOM

		if ( inventoryOpened )
		{
			int spin = (int)Input.mouseScrollDelta.y;
			if ( spin != 0 )
			{
				m_inventory.SpinInventory( spin );
			}
		}
	}

#if USING_ZOOM
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
#endif // if USING_ZOOM

	public bool IsGrounded()
	{
		float distToGround = 1;

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
		if ( pauseMenu.IsPaused() )
		{
			pauseMenu.ResumeGame();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if ( journalDisplay.IsOpened() )
		{
			journalDisplay.CloseJournal();
		}
		else
		{
			pauseMenu.PauseGame();
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}
	}

	void HandleJournalKey()
	{
		if ( !pauseMenu.IsPaused() && !journalDisplay.IsOpened() )
		{
			journalDisplay.OpenJournal();
		}
		else if ( journalDisplay.IsOpened() )
		{
			journalDisplay.CloseJournal();
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
		AudioClip footstepSound = m_zoneManager.GetCurrentFootstepSound();
		if ( footstepSound is null )
		{
			footstepSound = m_defaultFootstepSound;
		}

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
				m_footstepSource.PlayOneShot( footstepSound );
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
		if ( targetObject != null && targetObject.GetItemType() != InteractableAbstract.ItemType.PICKUP )
		{
			targetObject.OnUserInteractUnfiltered();
			targetObject.ActivateItem();
		}
	}

	void HandleReflectPress()
	{
		if ( targetObject != null && targetObject.WillReflect() )
		{
			targetObject.ActivateReflect();
		}
	}

	void HandleUseItemPress()
	{
		if ( targetObject.WillAcceptItem() && targetObject.GetItemType() == InteractableAbstract.ItemType.PICKUP )
		{
			targetObject.ActivateItem();
			PickupItem item = (PickupItem)targetObject;
			ItemPickupResult res = Inventory.GetInstance().PickupItem( ref item );
			if ( res != ItemPickupResult.SUCCESS )
			{
				Debug.Log( "Inventory failed to store item" );
			}
			else
			{
				targetObject.gameObject.SetActive( false );
			}
		}

		if ( targetObject != null )
		{
			targetObject.OnUseItemUnfiltered();
		}

		if ( targetObject != null && targetObject.interactable && targetObject.WillAcceptItem() )
		{
			targetObject.ActivateUseItem( m_inventory.GetSelectedItem() );
		}
	}

	void HandleDropItem()
	{
		// Check if the player is picking up an item, don't place a bucket down if so, it's annoying
		PickupItem pickupItemScript = targetObject as PickupItem;
		if ( pickupItemScript ) return;

		PickupItem inventoryItem = m_inventory.GetSelectedPickupItem();
		// To make this better, we can give PickUpItem a boolean for 'canDrop', everything aside from the bucket would be false
		if ( !inventoryItem || inventoryItem.itemName != Globals.Misc.EMPTY_BUCKET ) return;

		/*// Spawn the bucket right below the player, prop the player on top of the bucket, if they fall, they can climb back up.
		this.transform.position = this.transform.position + new Vector3(0, EmptyBucket.BUCKET_HEIGHT + 0.2f, 0);
		inventoryItem.OnDrop(new Vector3( this.transform.position.x , EmptyBucket.BUCKET_HEIGHT / 2 + 0.1f, this.transform.position.z ) );*/
		// Drops the item at eye level of the player. Note that empty bucket now handles it's dropping logic.
		inventoryItem.OnDrop( this.transform.position + ( this.transform.forward * dropDistance ) + new Vector3( 0, 1, 0 ) );
	}

	void HandleOpenInventory()
	{
		if ( !pauseMenu.IsPaused() && !journalDisplay.IsOpened() )
		{
			m_inventory.openInventory();
			inventoryOpened = true;
		}
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
				if ( targetObject is not null )
				{
					targetObject.OnUserLookAway();
				}
				targetObject = hitRes.collider.gameObject.GetComponent<InteractableAbstract>();
				targetObject.OnUserLooking();
			}
			else
			{
				if ( targetObject is not null )
				{
					targetObject.OnUserLookAway();
				}
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
			if ( targetObject.thisIsAMirror )
			{
				bp2.SetButton( 'e' );
				bp2.showPrompt( "Reflect across Mirror" );
			}
			else if ( targetObject.WillAcceptItem() && selectedItem != "" )
			{
				bp2.SetButton( 'e' );
				bp2.showPrompt( targetObject.GetItemText( selectedItem ) );
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
			interactionIcon.ShowIcons( targetObject.GetItemName(), targetObject.WillAcceptItem(),
				targetObject.WillDisplayPrompt(), targetObject.WillReflect() );
		}
		else
		{
			interactionIcon.HideIcons();
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

		// Change the state of the music box, note this is a way of getting a unique & possibly inactive game object.
		MusicBox musicBox;
		MusicBox[] musicBoxes = Resources.FindObjectsOfTypeAll<MusicBox>();
		if ( musicBoxes.Length > 0 )
		{
			musicBox = musicBoxes[ 0 ];
			musicBox.ChangeStems();
		}
	}

	MusicBox GetMusicBox()
	{
		// Change the state of the music box, note this is a way of getting a unique & possibly inactive game object.
		MusicBox musicBox = null;
		MusicBox[] musicBoxes = Resources.FindObjectsOfTypeAll<MusicBox>();
		if ( musicBoxes.Length > 0 )
		{
			musicBox = musicBoxes[ 0 ];
		}
		return musicBox;
	}

	public void SetSensitivity( float newSens )
	{
		this.sensitivity = newSens;
	}
}
