using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BookShelfPuzzle : MonoBehaviour
{

	public uint numBookSlots = 4;
	private List<BookSlot> m_bookslots;

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
}
