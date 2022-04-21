using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class BookShelfPuzzle : MonoBehaviour
{

	public uint numBookSlots = 4;
	public AudioClip wrongOrderVoiceLine;
	public AudioClip rightOrderVoiceLine;
	public AudioClip presentOrderVoiceLine;

	private List<BookSlot> m_bookslots;

	private const string WHITE_BOOK = "WhiteBook";
	private const string YELLOW_BOOK = "YellowBook";

	void Start()
	{
		m_bookslots = new List<BookSlot>();
		GameObject[] bookslots = GameObject.FindGameObjectsWithTag( Globals.Tags.BOOK_SLOT );
		Assert.IsTrue( bookslots.Length == numBookSlots );

		foreach ( GameObject bookslot in bookslots )
		{
			m_bookslots.Add( bookslot.GetComponent<BookSlot>() );
		}

		EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.DEBUG_TRIGGER ), OnPuzzleSolved );
		EventManager.Sub( Globals.Events.BOOKSHELF_BOOK_PLACED, CheckPuzzleSolved );
		// hack to fix transform for book
		EventManager.Sub( Globals.Events.BOOKSHELF_BOOK_PICKED_UP, ( GameObject book ) =>
		{
			if ( book.name == WHITE_BOOK )
			{
				FixBookTransform( book );
			}
		} );
	}

	void CheckPuzzleSolved()
	{
		bool isPastOrder = true;
		bool isPresentOrder = true;

		foreach ( BookSlot slot in m_bookslots )
		{
			// If the book is INVALID, the code cannot possibly be correct, we don't check.
			if ( slot.currentBook == BookType.INVALID_BOOK )
			{
				return;
			}
			// Present code checking.
			if ( slot.currentBook != slot.targetBook )
			{
				isPastOrder = false;
			}
			// Past code checking.
			if ( slot.currentBook != slot.incorrectBook )
			{
				isPresentOrder = false;
			}
		}

		if ( isPresentOrder )
		{
			// Dialog
			AudioPlayer.Play( presentOrderVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
			return;
		}

		if ( isPastOrder )
		{
			OnPuzzleSolved();
			AudioPlayer.Play( rightOrderVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
			return;
		}

		AudioPlayer.Play( wrongOrderVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
	}

	void OnPuzzleSolved()
	{
		Debug.Log( "Bookshelf puzzle solved" );

		EventManager.Fire( Globals.Events.DAD_PUZZLE_1_BOOKSHELF_SOLVED );
		AudioPlayer.Play( Globals.AudioFiles.Section3.BOOKSHELF_SEQUENCE, Globals.Tags.MAIN_SOURCE );
		// Disables the pick up for all of these objects via disabling their box colliders.
		foreach ( BookSlot slot in m_bookslots )
		{
			slot.interactable = false;
		}

		// Disables pick up for these books except Alice in Wonderland if the player wants to take it
		// Disables the pick up for all of these objects via disabling their box colliders.
		GameObject[] interactableBooks = GameObject.FindGameObjectsWithTag( Globals.Tags.INTERACTABLE_BOOK );
		foreach ( GameObject book in interactableBooks )
		{
			PickupItem pickUpScript = book.GetComponent<PickupItem>();
			if ( pickUpScript != null && pickUpScript.itemName != Globals.UIStrings.ALICE_IN_WONDERLAND )
			{
				book.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}

	void FixBookTransform( GameObject book )
	{
		//book.transform.localEulerAngles = new Vector3( 83.0208511f, 83.0759659f, 264.555115f ); // Old Shelf
		book.transform.localEulerAngles = new Vector3(96.979f, 186.924f, 275.44501f);
		book.transform.localScale = new Vector3(86.56586f, 83.93706f, 81.70756f);
	}
}
