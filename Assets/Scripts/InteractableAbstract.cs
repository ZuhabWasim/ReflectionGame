using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableAbstract : MonoBehaviour
{

    public bool displayPrompt = true;
    public string itemName;
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

    public ItemType GetItemType()
    {
        return myType;
    }

    public bool WillDisplayPrompt()
    {
        return displayPrompt;
    }

    public void ActivateItem()
    {
        if (voiceLine != null)
        {
            AudioPlayer.Play(voiceLine, Globals.Tags.DIALOGUE_SOURCE);
        }
        OnUserInteract();
    }
    public abstract void OnUserInteract();

}
