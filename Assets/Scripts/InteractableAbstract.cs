#define USING_INTERACTABLE_HIGHLIGHTING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* HOW TO USE THIS CLASS
 * 
 *  1: add a script inheriting from this to any object that can either be interacted with directly or have an item used on it
 *  
 *  2: If you want an interact prompt to appear, set 'displayPrompt' to true in inspector
 *      a: Set 'myType' for the interaction type. This mainly just affects what text appears in the prompt
 *          i: 'myType' set to open/close on an InterpTransform will swap to the opposite open/close with each interaction
 *          ii: 'myType' is automatically set to PICKUP by the inheriting PickupItem class.
 *      b: If you want, set a voiceline for millie to say when interacting with the item
 *      c: Write an override 'OnUserInteract' method on the inheriting script to handle the result of interaction
 *      
 *  3: If you want a use item prompt to appear, set 'acceptItem' to true in inspector
 *      a: have the inheriting script set 'desiredItem' to the item it wants the player to use (see UseHandkerchief for example)
 *      b: Write an override 'OnUseItem' method on the inheriting script to handle the result of the player using the correct item
 *      
 *  4: If either prompts are active, make sure you set 'itemName' in inspector since this appears in the prompts
 */

public abstract class InteractableAbstract : MonoBehaviour
{

	public bool interactable = false;
	[Tooltip( "Event which should set this object to become interactable" )]
	public string makeInteractableEvent = "";
	public string makeNonInteractableEvent = "";

	public bool displayPrompt = true;
	public bool isPickup = false;
	protected bool displayReflect = false;
	public bool acceptItem = false;
	public bool deleteItem = false;
	public string itemName;
	protected string desiredItem = "";

	public string firstInteractEvent;
	bool firstInteracted;

	private int originalLayer;

	public enum ItemType
	{
		INTERACT,
		MOVE,
		PICKUP,
		OPEN,
		CLOSE
	}
	public ItemType myType;

#if USING_INTERACTABLE_HIGHLIGHTING
	[Header( "Object Highlighting" )]
	public bool highlightOnLook = true;
	public bool highlightChildObjects = false;
#endif

	private Inventory m_inventory;
	[HideInInspector]
	public bool thisIsAMirror = false;

	[Header( "Audio" )]
	public AudioClip voiceLine;
	public AudioClip nonInteractableVoiceLine;
	public AudioClip nonInteractableSound;
	public AudioClip nonUseableVoiceLine;
	public AudioClip useVoiceLine;
	public AudioClip useSound;


	public void Start()
	{
		if ( makeInteractableEvent != string.Empty )
		{
			EventManager.Sub( makeInteractableEvent, () => { interactable = true; } );
		}

		if ( makeNonInteractableEvent != string.Empty )
		{
			EventManager.Sub( makeNonInteractableEvent, () => { interactable = false; } );
		}

		m_inventory = Inventory.GetInstance();

		originalLayer = gameObject.layer;
		firstInteracted = ( firstInteractEvent == string.Empty );

		OnStart();

	}

	public void SetType( ItemType typeIn )
	{
		myType = typeIn;
	}

	public string GetItemName()
	{
		return itemName;
	}

	public void SetInteractable( bool v )
	{
		interactable = v;
	}

	public string GetPromptText()
	{
		string t = "";
		switch ( myType )
		{
			case ItemType.MOVE:
				t += Globals.UIStrings.MOVE_ITEM;
				break;

			case ItemType.OPEN:
				t += Globals.UIStrings.OPEN_ITEM;
				break;

			case ItemType.CLOSE:
				t += Globals.UIStrings.CLOSE_ITEM;
				break;

			case ItemType.PICKUP:
				t += Globals.UIStrings.PICKUP_ITEM;
				break;

			default:
				t += Globals.UIStrings.INTERACT_ITEM;
				break;

		}

		return t + itemName;
	}

	public string GetItemText( string objectName )
	{
		return Globals.UIStrings.USE_ITEM_A + objectName + Globals.UIStrings.USE_ITEM_B + itemName;
	}

	public ItemType GetItemType()
	{
		return myType;
	}

	public bool WillDisplayPrompt()
	{
		return displayPrompt && !isPickup;
	}

	public bool WillReflect()
	{
		return displayReflect;
	}

	public bool WillAcceptItem()
	{
		return acceptItem || isPickup;
	}

	public void ActivateItem()
	{

		if ( !interactable )
		{
			if ( nonInteractableVoiceLine != null )
			{
				AudioPlayer.Play( nonInteractableVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
			}
			if ( nonInteractableSound != null )
			{
				AudioPlayer.Play( nonInteractableSound, Globals.Tags.MAIN_SOURCE );
			}
			return;
		}

		if ( !firstInteracted )
		{
			EventManager.Fire( firstInteractEvent );
			firstInteracted = true;
		}

		if ( voiceLine != null )
		{
			AudioPlayer.Play( voiceLine, Globals.Tags.DIALOGUE_SOURCE );
		}
		OnUserInteract();
	}

	public void ActivateReflect()
	{
		OnUserReflect();
	}

	public void ActivateUseItem( string objectName )
	{
		// Correctly picking item to use on object
		if ( desiredItem == objectName )
		{
			OnUseItem();
			if ( useVoiceLine != null )
			{
				AudioPlayer.Play( useVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
			}
			if (useSound != null)
			{
				AudioPlayer.Play( useSound, Globals.Tags.MAIN_SOURCE);
			}
			if ( deleteItem )
			{
				m_inventory.DeleteItem( desiredItem );
			}
		}
		// Temporary fix to allow player to Reflect with items selected
		else if ( desiredItem == "" )
		{
			OnUseItem();
		}
		// If no item is selected.
		else if ( objectName == "" )
		{
			if ( nonUseableVoiceLine != null )
			{
				AudioPlayer.Play( nonUseableVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
			}
			else
			{
				AudioPlayer.Play( Globals.VoiceLines.General.NOT_HOLDING_ANYTHING, Globals.Tags.DIALOGUE_SOURCE );
			}
			AudioPlayer.Play( Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
		}
		// If the wrong item is selected.
		else
		{
			if ( nonUseableVoiceLine != null )
			{
				AudioPlayer.Play( nonUseableVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
			}
			else
			{
				AudioPlayer.Play( Globals.VoiceLines.General.CANT_USE_ITEM, Globals.Tags.DIALOGUE_SOURCE );
			}
			AudioPlayer.Play( Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
		}
	}


#if USING_INTERACTABLE_HIGHLIGHTING
	private void ApplyLayer( GameObject obj, int layer, bool applyToChildren = false )
	{
		obj.layer = layer;
		if ( !applyToChildren ) return;

		foreach ( Transform child in obj.transform )
		{
			ApplyLayer( child.gameObject, layer, applyToChildren );
		}
	}
#endif // if USING_INTERACTABLE_HIGHLIGHTING

	public void OnUserLooking()
	{
#if USING_INTERACTABLE_HIGHLIGHTING
		if ( !highlightOnLook ) return;
		ApplyLayer( this.gameObject, Globals.Misc.HIGHLIGHT_LAYER, this.highlightChildObjects );
#endif
	}

	public void OnUserLookAway()
	{
#if USING_INTERACTABLE_HIGHLIGHTING
		if ( !highlightOnLook ) return;
		ApplyLayer( this.gameObject, originalLayer, this.highlightChildObjects );
#endif
	}

	protected virtual void OnUserInteract() { }

	protected virtual void OnUserReflect() { }

	protected virtual void OnUseItem() { }

	protected virtual void OnStart() { }

	// fired on the object everytime without conditions blocking
	public virtual void OnUseItemUnfiltered() { }
	public virtual void OnUserInteractUnfiltered() { }

}
