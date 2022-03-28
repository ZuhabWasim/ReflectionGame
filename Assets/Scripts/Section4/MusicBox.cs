using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class MusicBox : InteractableAbstract
{

	private const string MUSICBOX_AUDIO_SOURCE = "MusicBoxAudioSource";
	private Inventory m_inventory;
	public AudioClip millieKeyVoiceline;
	public AudioClip motherKeyVoiceline;
	public AudioClip fatherKeyVoiceline;
	public bool discoveredBox;

	public ProximityTrigger proximityTrigger;
	public GameObject pointLight;

	private bool hasMillieKey;
	private bool hasMomKey;
	private bool hasDadKey;
	private int keys = 0;

	private bool millieKeyInteracted;
	
	protected override void OnStart()
	{
		EventManager.Sub( Globals.Events.LIGHTS_TURN_ON, OnLightsTurningOn );
		EventManager.Sub( Globals.Events.HAS_MILLIE_KEY, OnHavingMillieKey );
		EventManager.Sub( Globals.Events.HAS_MOM_KEY, OnHavingMomKey );
		EventManager.Sub( Globals.Events.HAS_DAD_KEY, OnHavingDadKey );

		m_inventory = Inventory.GetInstance();
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
			Debug.Log("I finally have all of them, i'm nervous to check what's inside, let's use them");
		}
		// On completing Mom's closet
		else if (hasMomKey)
		{
			Debug.Log("I have mom's key now, now to find dad's key");
		}
		// On completing Dad's closet
		else if (hasDadKey)
		{
			Debug.Log("I have dad's key now, now to find mom's key");
		}
		// Millie's key
		else if (hasMillieKey)
		{
			Debug.Log("I have my key now, let's see if I can use it ");
		}
		// Initial interaction dialogue
		else
		{
			Debug.Log("Oh it's my music box");
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
			Debug.Log("Not holding anything");
			return;
		}
		
		// Check and use the keys.
		if (selectedItem.itemName == Globals.UIStrings.MUSICBOXMILLIE_ITEM)
		{
			if (!millieKeyInteracted)
			{
				Debug.Log("Oh i forgot the music box needed all three keys, Mine, Mom , and Dad");
				AudioPlayer.Play( millieKeyVoiceline, Globals.Tags.DIALOGUE_SOURCE );
				EventManager.Fire( Globals.Events.MILLIE_KEY_INTERACT );
				millieKeyInteracted = true;
				Debug.Log( "Initially interacting with the Music Box with Millie's key" );
				return;
			}

			if (!HasAllKeys())
			{
				Debug.Log("I shouldn't try these until I have all three keys");
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
				Debug.Log("I shouldn't try these until I have all three keys");
			}
			keys -= 1;
			inventory.DeleteItem( selectedItem.itemName );
		}
		else
		{
			Debug.Log("I can't use this here");
			return;
		}
		
		// If we got here, the player used a key.
		if (keys == 2)
		{
			Debug.Log("That's one");// That's one
		} else if (keys == 1)
		{
			Debug.Log("That's two");
		} else if (keys == 0)
		{
			Debug.Log("That's three");
			OpenMusicBox();
		}
		
	}

	void OpenMusicBox()
	{
		Debug.Log("Opening Music Box");
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
	}

	void OnHavingDadKey()
	{
		if ( !hasDadKey ) keys += 1;
		hasDadKey = true;
		Debug.Log("Keys = " + keys);
	}

	private bool HasAllKeys()
	{
		return hasMillieKey && hasDadKey && hasMomKey;
	}
}
