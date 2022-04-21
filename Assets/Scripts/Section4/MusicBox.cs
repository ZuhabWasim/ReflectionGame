using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.GlobalIllumination;

public class MusicBox : InteractableAbstract
{

	private const string MUSICBOX_AUDIO_SOURCE = "MusicBoxAudioSource";
	private const string PRESENT_FIREPLACE_SOURCE = "PresentFireplaceSource";
	public bool discoveredBox;

	public ProximityTrigger proximityTrigger;
	public GameObject pointLight;
	public GameObject lid;
	
	public AudioClip noReflectAudio;
	
	private bool hasMillieKey;
	private bool hasMomKey;
	private bool hasDadKey;
	private int keys = 0;

	private bool millieKeyInteracted;
	private bool epiphanyMom;
	private bool epiphanyDad;

	public MirrorInteractable pastFireplaceMirror;
	public MirrorInteractable presentFireplaceMirror;
	
	// Can't be bothered to do this correctly, today's the last day.
	public GameObject pastDadDoor;
	public GameObject presentDadDoor;
	public GameObject pastMomDoor;
	public GameObject presentMomDoor;

	private InterpolateInteractableWrapper pastDadIIW;
	private InterpolateInteractableWrapper presentDadIIW;
	private InterpolateInteractableWrapper pastMomIIW;
	private InterpolateInteractableWrapper presentMomIIW;
	
	protected override void OnStart()
	{
		EventManager.Sub( Globals.Events.LIGHTS_TURN_ON, OnLightsTurningOn );
		EventManager.Sub( Globals.Events.HAS_MILLIE_KEY, OnHavingMillieKey );
		EventManager.Sub( Globals.Events.LOCK_MOM_DOOR, OnLockingMomDoor );
		EventManager.Sub( Globals.Events.LOCK_DAD_DOOR, OnLockingDadDoor );
		EventManager.Sub( Globals.Events.HAS_MOM_KEY, OnHavingMomKey );
		EventManager.Sub( Globals.Events.HAS_DAD_KEY, OnHavingDadKey );
		EventManager.Sub( Globals.Events.FIRST_TELEPORT, OnFirstTeleport );
		EventManager.Sub( Globals.Events.EPIPHANY_MOM, () => { epiphanyMom = true; } );
		EventManager.Sub( Globals.Events.LOCK_PAST_DAD_SHELF, () => { epiphanyDad = true; ChangeStems(); } );
		
		AudioPlayer.RegisterAudioPlayer(PRESENT_FIREPLACE_SOURCE, presentFireplaceMirror.GetComponent<AudioSource>());

		discoveredBox = false;

		hasMillieKey = false;
		hasMomKey = false;
		hasDadKey = false;

		millieKeyInteracted = false;
		epiphanyMom = false;
		epiphanyDad = false;

		pastDadIIW = pastDadDoor.GetComponent<InterpolateInteractableWrapper>();
		presentDadIIW = presentDadDoor.GetComponent<InterpolateInteractableWrapper>();
		pastMomIIW = pastMomDoor.GetComponent<InterpolateInteractableWrapper>();
		presentMomIIW = presentMomDoor.GetComponent<InterpolateInteractableWrapper>();
	}

	protected override void OnUserInteract()
	{
		discoveredBox = true;
		
		// Has all three keys
		if (HasAllKeys())
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.ALL_THREE_KEYS, Globals.Tags.DIALOGUE_SOURCE);
		}
		// On completing Mom's closet
		else if (hasMomKey)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.FOUND_MOTHERS_KEY_LOOK_FOR_FATHERS, Globals.Tags.DIALOGUE_SOURCE);
		}
		// On completing Dad's closet
		else if (hasDadKey)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.OBTAINED_FATHERS_KEY_MOTHER_KEY_LEFT, Globals.Tags.DIALOGUE_SOURCE);
		}
		// Millie's key
		else if (hasMillieKey)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.HAVE_MY_KEY_NOW, Globals.Tags.DIALOGUE_SOURCE);
		}
		// Initial interaction dialogue
		else
		{
			AudioPlayer.Play(Globals.VoiceLines.Section1.ITS_MY_MUSICBOX, Globals.Tags.DIALOGUE_SOURCE);
		}
	}

	// Handling OnUseItem() differently than the class intended, since these items are used in a special way.
	public override void OnUseItemUnfiltered()
	{
		Inventory inventory = Inventory.GetInstance();
		PickupItem selectedItem = inventory.GetSelectedPickupItem();
		
		// Not holding anything
		if (selectedItem == null)
		{
			AudioPlayer.Play(Globals.VoiceLines.General.NOT_HOLDING_ANYTHING, Globals.Tags.DIALOGUE_SOURCE);
			AudioPlayer.Play( Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
			return;
		}
		
		// Check and use the keys.
		if (selectedItem.itemName == Globals.UIStrings.MUSICBOXMILLIE_ITEM)
		{
			if (!millieKeyInteracted)
			{
				AudioPlayer.Play(Globals.VoiceLines.Section1.BOX_NEEDS_TWO_OTHER_KEYS, Globals.Tags.DIALOGUE_SOURCE);
				AudioPlayer.Play(Globals.VoiceLines.Section1.WOAH_WHAT_JUST_HAPPENED_TO_THE_MIRROR, Globals.Tags.DIALOGUE_SOURCE, false);
				EventManager.Fire( Globals.Events.MILLIE_KEY_INTERACT );
				millieKeyInteracted = true;
				
				// Start playing Millie's stem faintly in the fireplace mirror.
				AudioPlayer.Play(Globals.AudioFiles.Music.MILLIE_STEM, PRESENT_FIREPLACE_SOURCE);
				
				Debug.Log( "Millie Key Interact" );
				return;
			}

			if (!HasAllKeys())
			{
				AudioPlayer.Play(Globals.VoiceLines.Section4.WAIT_FOR_ALL_THREE_KEYS, Globals.Tags.DIALOGUE_SOURCE);
				return;
			}
			
			keys -= 1;
			inventory.DeleteItem( selectedItem.itemName );
		}
		else if (selectedItem.itemName == Globals.UIStrings.MUSICBOXMOM_ITEM ||
		         selectedItem.itemName == Globals.UIStrings.MUSICBOXDAD_ITEM)
		{
			if (!HasAllKeys())
			{
				AudioPlayer.Play(Globals.VoiceLines.Section4.WAIT_FOR_ALL_THREE_KEYS, Globals.Tags.DIALOGUE_SOURCE);
				return;
			}
			keys -= 1;
			inventory.DeleteItem( selectedItem.itemName );
		}
		else
		{
			AudioPlayer.Play( Globals.VoiceLines.General.CANT_USE_ITEM, Globals.Tags.DIALOGUE_SOURCE );
			AudioPlayer.Play( Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
			return;
		}
		
		// If we got here, the player used a key.
		if (keys == 2)
		{
			AudioPlayer.Play( Globals.VoiceLines.Section4.THATS_ONE, Globals.Tags.DIALOGUE_SOURCE );
			AudioPlayer.Play( Globals.AudioFiles.Section4.MUSIC_BOX_USE_KEY_1, Globals.Tags.MAIN_SOURCE );
		} else if (keys == 1)
		{
			AudioPlayer.Play( Globals.VoiceLines.Section4.THATS_TWO, Globals.Tags.DIALOGUE_SOURCE );
			AudioPlayer.Play( Globals.AudioFiles.Section4.MUSIC_BOX_USE_KEY_2, Globals.Tags.MAIN_SOURCE );
		} else if (keys == 0)
		{
			AudioPlayer.Play( Globals.VoiceLines.Section4.THATS_THREE, Globals.Tags.DIALOGUE_SOURCE );
			AudioPlayer.Play( Globals.AudioFiles.Section4.MUSIC_BOX_USE_KEY_3, Globals.Tags.MAIN_SOURCE );
			OpenMusicBox();
		}
		
	}

	void OpenMusicBox()
	{
		Debug.Log("Opening Music Box");
		AudioPlayer.Play(Globals.VoiceLines.Section4.IT_OPENED, Globals.Tags.DIALOGUE_SOURCE, false);

		// Stop all audio sources, preferably gradually.
		AudioPlayer.StopCurrentClip(Globals.Tags.AMBIENCE_SOURCE);
		AudioPlayer.StopCurrentClip(PRESENT_FIREPLACE_SOURCE);
		
		// Stop the player movement
		PlayerController player = GameObject.FindGameObjectWithTag(Globals.Tags.PLAYER).GetComponent<PlayerController>();
		player.speed = 0f;
		player.sensitivity = 0f;
		
		// Play the box opening sound.
		AudioPlayer.Play(Globals.AudioFiles.Section4.MUSIC_BOX_CLICK, Globals.Tags.MAIN_SOURCE, false);
		
		// Open music box slowly but not enough to see the inside.
		InterpolateTransform it = lid.GetComponent<InterpolateTransform>();
		it.TriggerMotion();

		// Fade out at the speed it takes for the last two dialogue lines to play
		GameObject.Find(Globals.Misc.UI_Canvas).GetComponent<FadeToBlack>().defFadeTime = 5;
		GameObject.Find(Globals.Misc.UI_Canvas).GetComponent<FadeToBlack>().StartFadeOut(PlayFinalCutscene);
	}

	public void ChangeStems()
	{
		// If it's the present, mute all the sources.
		bool isPresent = GlobalState.GetVar<bool>(Globals.Vars.IS_PRESENT_WORLD);
		if ( isPresent )
		{
			AudioPlayer.SetSourceVolume(Globals.Tags.MILLIE_STEM, 0f);
			AudioPlayer.SetSourceVolume(Globals.Tags.MOM_STEM, 0f);
			AudioPlayer.SetSourceVolume(Globals.Tags.DAD_STEM, 0f);
			return;
		}
		
		// If it's the past, enable the stems based on how far the player has progressed.
		AudioPlayer.SetSourceVolume(Globals.Tags.MILLIE_STEM, Globals.Misc.STEM_VOLUME);

		if ( epiphanyMom )
		{
			AudioPlayer.SetSourceVolume(Globals.Tags.MOM_STEM, Globals.Misc.STEM_VOLUME);
		}

		if ( epiphanyDad )
		{
			AudioPlayer.SetSourceVolume(Globals.Tags.DAD_STEM, Globals.Misc.STEM_VOLUME);
		}
	}
	
	void PlayFinalCutscene()
    {
		//TODO right now this just takes the player back to the main menu
		//Instead, it should play final cutscene.
		SceneManager.LoadScene( Globals.ENDING_SCENE );
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Confined;
	}

	void OnLightsTurningOn()
	{
		pointLight.SetActive(true);
		
		// Update Voice lines for Present Doors
		AudioClip doorLockedLine= Utilities.AssetLoader.GetSFX( Globals.VoiceLines.Section1.LOCKED_FROM_THE_INSIDE );
		presentMomIIW.nonInteractableVoiceLine = doorLockedLine;
		presentDadIIW.nonInteractableVoiceLine = doorLockedLine;

		// Set all of the doors to have locked sound feedback.
		AudioClip doorLockedSound = Utilities.AssetLoader.GetSFX( Globals.AudioFiles.General.LOCKED_DOOR_JIGGLE );
		pastDadIIW.nonInteractableSound = doorLockedSound;
		presentDadIIW.nonInteractableSound = doorLockedSound;
		presentMomIIW.nonInteractableSound = doorLockedSound;
		pastMomIIW.nonInteractableSound = doorLockedSound;
	}

	void OnLockingMomDoor()
	{
		// Change the locked voice line to the short one.
		AudioClip doorLockedSoundShort = Utilities.AssetLoader.GetSFX( Globals.VoiceLines.Section2.AM_I_LOCKED_IN_SHORT );
		presentMomIIW.nonInteractableVoiceLine = doorLockedSoundShort;
		AudioPlayer.Play(Globals.AudioFiles.General.DOOR_SHUT, pastMomIIW.audioSourceName);
	}
	
	void OnLockingDadDoor()
	{		
		// Change the locked voice line to the short one.
		AudioClip doorLockedSoundShort = Utilities.AssetLoader.GetSFX( Globals.VoiceLines.Section3.AM_I_LOCKED_IN_SHORT );
		presentDadIIW.nonInteractableVoiceLine = doorLockedSoundShort;
		AudioPlayer.Play(Globals.AudioFiles.General.DOOR_SHUT, pastDadIIW.audioSourceName);
	}
	
	void OnHavingMillieKey()
	{
		if (hasMillieKey) return;
		
		keys += 1;
		hasMillieKey = true;
		Debug.Log("Keys = " + keys);
	}

	void OnHavingMomKey()
	{
		if (hasMomKey) return;
		
		keys += 1;
		hasMomKey = true;
		Debug.Log("Keys = " + keys);
		AudioPlayer.Play(Globals.AudioFiles.General.ACHIEVEMENT_REFLECTION_ONESHOT, Globals.Tags.MAIN_SOURCE, false);			
		// Playing Millie's stem faintly in the fireplace mirror.
		AudioPlayer.Play(Globals.AudioFiles.Music.MILLIE_STEM, PRESENT_FIREPLACE_SOURCE);
		DisableFireplaceMirrorTeleport();
	}

	void OnHavingDadKey()
	{
		if (hasDadKey) return;
		
		keys += 1;
		hasDadKey = true;
		Debug.Log("Keys = " + keys);
		AudioPlayer.Play(Globals.AudioFiles.General.ACHIEVEMENT_REFLECTION_ONESHOT, Globals.Tags.MAIN_SOURCE, false);
		// Playing Millie's stem faintly in the fireplace mirror.
		AudioPlayer.Play(Globals.AudioFiles.Music.MILLIE_STEM, PRESENT_FIREPLACE_SOURCE);
		DisableFireplaceMirrorTeleport();
	}

	void OnFirstTeleport()
	{
		// Removes the initial voice line.
		if (presentFireplaceMirror != null)
		{
			presentFireplaceMirror.teleportingVoiceLine = null;
		}
	}

	private void DisableFireplaceMirrorTeleport()
	{
		// Player can no longer go into the past after they collect all three keys.
		if (keys == 3 && presentFireplaceMirror != null)
		{
			presentFireplaceMirror.teleportable = false;
			presentFireplaceMirror.nonTeleportableVoiceLine = noReflectAudio;
		}
	}
	private bool HasAllKeys()
	{
		return hasMillieKey && hasDadKey && hasMomKey;
	}
}
