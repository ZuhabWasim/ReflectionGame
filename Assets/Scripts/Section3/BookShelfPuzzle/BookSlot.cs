using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BookType
{
	WHITE_BOOK = 0,
	RED_BOOK,
	BLUE_BOOK,
	GREEN_BOOK,
	INVALID_BOOK = -1
}

public class BookSlot : InteractableAbstract
{
	public BookType currentBook = BookType.INVALID_BOOK;
	public BookType targetBook;
	public Vector3 bookPosition;
	
	private const string WHITE_BOOK = "WhiteBook";
	private const string RED_BOOK = "RedBook";
	private const string BLUE_BOOK = "BlueBook";
	private const string GREEN_BOOK = "GreenBook";

	private const float FLOOR_HEIGHT = 0.27f;
	private const float BUCKET_HEIGHT = 1.25f;
	private const float HEIGHT_ERROR = 0.02f;
	
	protected override void OnStart()
	{
		EventManager.Sub( Globals.Events.BOOKSHELF_BOOK_PICKED_UP, OnBookPickedUp );
	}

	private BookType GetBookTypeFromName( string name )
	{
		switch ( name )
		{
			case WHITE_BOOK:
				return BookType.WHITE_BOOK;
			case RED_BOOK:
				return BookType.RED_BOOK;
			case BLUE_BOOK:
				return BookType.BLUE_BOOK;
			case GREEN_BOOK:
				return BookType.GREEN_BOOK;
			default:
				return BookType.INVALID_BOOK;
		}
	}

	void OnBookPickedUp( GameObject book )
	{
		BookType bookType = GetBookTypeFromName( book.name );
		UnityEngine.Assertions.Assert.AreNotEqual<BookType>( bookType, BookType.INVALID_BOOK );

		if ( bookType == currentBook )
		{
			currentBook = BookType.INVALID_BOOK;
		}
	}

	protected override void OnUserInteract()
	{
		Inventory inventory = Inventory.GetInstance();
		PickupItem selectedItem = inventory.GetSelectedPickupItem();
		
		if (IsOnBucket())
		{
			if ( selectedItem != null && selectedItem.gameObject.tag == Globals.Tags.INTERACTABLE_BOOK )
			{
				currentBook = GetBookTypeFromName( selectedItem.gameObject.name );
				selectedItem.OnDrop( bookPosition, true );
				inventory.DeleteItem( selectedItem.itemName );

				EventManager.Fire( Globals.Events.BOOKSHELF_BOOK_PLACED );
			}
		}
		else
		{
			AudioPlayer.Play(Globals.VoiceLines.General.PLACEHOLDER_3, Globals.Tags.DIALOGUE_SOURCE);
		}

	}

	public static bool IsOnBucket()
	{
		Vector3 playerPosition = GameObject.FindGameObjectWithTag(Globals.Tags.PLAYER).transform.position;
		return playerPosition.y >= BUCKET_HEIGHT - HEIGHT_ERROR;
	}
}
