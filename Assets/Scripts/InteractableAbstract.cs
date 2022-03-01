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
 *      b: Write an override 'OnUserItem' method on the inheriting script to handle the result of the player using the correct item
 *      
 *  4: If either prompts are active, make sure you set 'itemName' in inspector since this appears in the prompts
 */

public abstract class InteractableAbstract : MonoBehaviour
{

    public bool displayPrompt = true;
    public bool acceptItem = false;
    public string itemName;
    protected string desiredItem = "";
    public enum ItemType
    {
        INTERACT,
        MOVE,
        PICKUP,
        OPEN,
        CLOSE
    }
    public ItemType myType;

    public AudioClip voiceLine;


    public void SetType( ItemType typeIn )
    {
        myType = typeIn;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public string GetPromptText()
    {
        string t = "";
        switch (myType)
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

    public string GetItemText(string objectName)
    {
        return Globals.UIStrings.USE_ITEM_A + objectName + Globals.UIStrings.USE_ITEM_B + itemName;
    }

    public ItemType GetItemType()
    {
        return myType;
    }

    public bool WillDisplayPrompt()
    {
        return displayPrompt;
    }

    public bool WillAcceptItem()
    {
        return acceptItem;
    }

    public void ActivateItem()
    {
        if (voiceLine != null)
        {
            AudioPlayer.Play(voiceLine, Globals.Tags.DIALOGUE_SOURCE);
        }
        OnUserInteract();
    }

    public void ActivateUseItem(string objectName)
    {
        if (desiredItem == objectName)
        {
            OnUseItem();
        } else
        {
            //Millie says generic line like "I don't think I should use this here"
        }
    }

    protected virtual void OnUserInteract() {}

    protected virtual void OnUseItem() {}

}
