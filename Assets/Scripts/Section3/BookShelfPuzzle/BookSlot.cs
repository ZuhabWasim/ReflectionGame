using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableBooks
{
    WHITE_BOOK = 0,
    RED_BOOK,
    BLUE_BOOK,
    GREEN_BOOK,
    INVALID_BOOK = -1
}

public class BookSlot : InteractableAbstract
{
    public InteractableBooks book = InteractableBooks.INVALID_BOOK;

    private const string WHITE_BOOK = "WhiteBook";
    private const string RED_BOOK = "RedBook";
    private const string BLUE_BOOK = "BlueBook";
    private const string GREEN_BOOK = "GreenBook";
    
    protected override void OnStart()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private InteractableBooks GetBookColorFromName( string name )
    {
        switch ( name )
        {
            case WHITE_BOOK:
                return InteractableBooks.WHITE_BOOK;
            case RED_BOOK:
                return InteractableBooks.RED_BOOK;
            case BLUE_BOOK:
                return InteractableBooks.BLUE_BOOK;
            case GREEN_BOOK:
                return InteractableBooks.GREEN_BOOK;
            default:
                return InteractableBooks.INVALID_BOOK;
        }
    }

	protected override void OnUserInteract()
	{
		Inventory inventory = Inventory.GetInstance();
        PickupItem selectedItem = inventory.GetSelectedPickupItem();
        if ( selectedItem != null && selectedItem.gameObject.tag == Globals.Tags.INTERACTABLE_BOOK )
        {
            book = GetBookColorFromName( selectedItem.gameObject.name );
        }
	}
}
