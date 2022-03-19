using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
	public GameObject inventoryBar;
	public GameObject inventoryIcon;
	// Start is called before the first frame update
	void Start()
	{
		inventoryBar.SetActive( false );
		GameObject text = inventoryBar.transform.GetChild( 3 ).gameObject;
		text.SetActive( false );
		GameObject journalNotice = inventoryBar.transform.GetChild( 4 ).GetChild(1).gameObject;
		journalNotice.SetActive( false );
	}
	public void openInventory()
	{
		inventoryBar.SetActive( true );
		inventoryIcon.SetActive( false );
	}
	public void CloseInventory()
	{
		inventoryBar.SetActive( false );
		inventoryIcon.SetActive( true );
	}
	public void addItem( PickupItem item, int idx )
	{
		GameObject slot = inventoryBar.transform.GetChild( 1 ).GetChild( idx ).gameObject;
		RawImage img = slot.transform.GetChild( 0 ).GetComponent<RawImage>();
		img.enabled = true;
		img.texture = item.img;
	}
	public void dropItem( int idx )
	{
		GameObject slot = inventoryBar.transform.GetChild( 1 ).GetChild( idx ).gameObject;
		RawImage img = slot.transform.GetChild( 0 ).GetComponent<RawImage>();
		img.enabled = false;
	}
	public void SpinInventory( int spin )
	{
		Transform background = inventoryBar.transform.GetChild( 1 );
		background.Rotate( 0, 0, spin * 45 );
	}
	public void showItemName( string itemName )
	{
		GameObject text = inventoryBar.transform.GetChild( 3 ).gameObject;
		text.SetActive( true );
		text.transform.GetComponent<Text>().text = itemName;
	}
	public void hideItemName()
	{
		GameObject text = inventoryBar.transform.GetChild( 3 ).gameObject;
		text.SetActive( false );
	}
	public void showNewEntryNotice()
	{
		Debug.Log("showing notice");
		GameObject journalNotice = inventoryBar.transform.GetChild( 4 ).GetChild(1).gameObject;
		journalNotice.SetActive( true );
	}
	public void hideNewEntryNotice()
	{
		GameObject journalNotice = inventoryBar.transform.GetChild( 4 ).GetChild(1).gameObject;
		journalNotice.SetActive( false );
	}
}
