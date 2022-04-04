using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BookType
{
	WHITE_BOOK = 0,
	RED_BOOK,
	BLUE_BOOK,
	YELLOW_BOOK,
	INVALID_BOOK = -1
}

public class BookSlot : InteractableAbstract
{
	public BookType currentBook = BookType.INVALID_BOOK;
	public BookType targetBook;
	public BookType incorrectBook; // If the player enters the present puzzle

	public Vector3 bookPosition;
	public AudioClip missingBookVoiceLine;

	private const string WHITE_BOOK = "WhiteBook";
	private const string RED_BOOK = "RedBook";
	private const string BLUE_BOOK = "BlueBook";
	private const string YELLOW_BOOK = "YellowBook";

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
			case YELLOW_BOOK:
				return BookType.YELLOW_BOOK;
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

	public override void OnUseItemUnfiltered()
	{
		Inventory inventory = Inventory.GetInstance();
		PickupItem selectedItem = inventory.GetSelectedPickupItem();

		if ( EmptyBucket.IsOnBucket() )
		{
			if ( currentBook == BookType.INVALID_BOOK &&
				 ( selectedItem == null || GetBookTypeFromName( selectedItem.gameObject.name ) == BookType.INVALID_BOOK ) )
			{
				AudioPlayer.Play( missingBookVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
			}
			else
			{
				if ( selectedItem != null && selectedItem.gameObject.tag == Globals.Tags.INTERACTABLE_BOOK )
				{
					currentBook = GetBookTypeFromName( selectedItem.gameObject.name );
					selectedItem.OnDrop( bookPosition, true );
					inventory.DeleteItem( selectedItem.itemName );

					EventManager.Fire( Globals.Events.BOOKSHELF_BOOK_PLACED );
				}
			}
		}
		else
		{
			AudioPlayer.Play( nonInteractableVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
		}

	}
}
