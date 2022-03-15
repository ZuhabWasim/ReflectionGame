using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BookShelfPuzzle : MonoBehaviour
{

	public uint numBookSlots = 4;
	private List<BookSlot> m_bookslots;

	private const string WHITE_BOOK = "WhiteBook";
	private const string GREEN_BOOK = "GreenBook";

	void Start()
	{
		m_bookslots = new List<BookSlot>();
		GameObject[] bookslots = GameObject.FindGameObjectsWithTag( Globals.Tags.BOOK_SLOT );
		Assert.IsTrue( bookslots.Length == numBookSlots );

		foreach ( GameObject bookslot in bookslots )
		{
			m_bookslots.Add( bookslot.GetComponent<BookSlot>() );
		}

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
		foreach ( BookSlot slot in m_bookslots )
		{
			if ( slot.targetBook != slot.currentBook )
			{
				return;
			}
		}

		OnPuzzleSolved();
	}

	void OnPuzzleSolved()
	{
		Debug.Log( "Solved" );
	}

	void FixBookTransform( GameObject book )
	{
		book.transform.localEulerAngles = new Vector3(83.0208511f,83.0759659f,264.555115f);
	}
}
