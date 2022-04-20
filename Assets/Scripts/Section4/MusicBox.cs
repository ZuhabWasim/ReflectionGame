using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class MusicBox : InteractableAbstract
{

	private const string MUSICBOX_AUDIO_SOURCE = "MusicBoxAudioSource";
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

	public MirrorInteractable pastFireplaceMirror;
	public MirrorInteractable presentFireplaceMirror;
	
	protected override void OnStart()
	{
		EventManager.Sub( Globals.Events.LIGHTS_TURN_ON, OnLightsTurningOn );
		EventManager.Sub( Globals.Events.HAS_MILLIE_KEY, OnHavingMillieKey );
		EventManager.Sub( Globals.Events.HAS_MOM_KEY, OnHavingMomKey );
		EventManager.Sub( Globals.Events.HAS_DAD_KEY, OnHavingDadKey );
		EventManager.Sub( Globals.Events.FIRST_TELEPORT, OnFirstTeleport );
		
		// AudioPlayer.RegisterAudioPlayer(MUSICBOX_AUDIO_SOURCE, GetComponent<AudioSource>());

		discoveredBox = false;

		hasMillieKey = false;
		hasMomKey = false;
		hasDadKey = false;

		millieKeyInteracted = false;
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
			AudioPlayer.Play(Globals.VoiceLines.General.CANT_USE_ITEM, Globals.Tags.DIALOGUE_SOURCE);
			AudioPlayer.Play( Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
			return;
		}
		
		// If we got here, the player used a key.
		if (keys == 2)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.THATS_ONE, Globals.Tags.DIALOGUE_SOURCE);
		} else if (keys == 1)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.THATS_TWO, Globals.Tags.DIALOGUE_SOURCE);
		} else if (keys == 0)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.THATS_THREE, Globals.Tags.DIALOGUE_SOURCE);
			OpenMusicBox();
		}
		
	}

	void OpenMusicBox()
	{
		Debug.Log("Opening Music Box");
		AudioPlayer.Play(Globals.VoiceLines.Section4.IT_OPENED, Globals.Tags.DIALOGUE_SOURCE, false);

		// Stop all audio sources, preferably gradually.
		AudioPlayer.StopCurrentClip(Globals.Tags.MAIN_SOURCE);
		AudioPlayer.StopCurrentClip(Globals.Tags.AMBIENCE_SOURCE);
		
		// Stop the player movement
		PlayerController player = GameObject.FindGameObjectWithTag(Globals.Tags.PLAYER).GetComponent<PlayerController>();
		player.speed = 0f;
		player.sensitivity = 0f;
		
		// Open music box slowly but not enough to see the inside.
		InterpolateTransform it = lid.GetComponent<InterpolateTransform>();
		it.TriggerMotion();

		// Fade out at the speed it takes for the last two dialogue lines to play
		GameObject.Find(Globals.Misc.UI_Canvas).GetComponent<FadeToBlack>().defFadeTime = 5;
		GameObject.Find(Globals.Misc.UI_Canvas).GetComponent<FadeToBlack>().StartFadeOut(PlayFinalCutscene);
	}

	void PlayFinalCutscene()
    {
		//TODO right now this just takes the player back to the main menu
		//Instead, it should play final cutscene.
		GameObject.Find(Globals.Misc.UI_Canvas).GetComponent<PauseMenu>().ExitGame();
	}

	void OnLightsTurningOn()
	{
		pointLight.SetActive(true);
	}
	void OnHavingMillieKey()
	{
		if ( !hasMillieKey ) keys += 1;
		hasMillieKey = true;
		Debug.Log("Keys = " + keys);
		proximityTrigger.enabled = true;
	}

	void OnHavingMomKey()
	{
		if ( !hasMomKey ) keys += 1;
		hasMomKey = true;
		Debug.Log("Keys = " + keys);
		DisableFireplaceMirrorTeleport();
	}

	void OnHavingDadKey()
	{
		if ( !hasDadKey ) keys += 1;
		hasDadKey = true;
		Debug.Log("Keys = " + keys);
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
