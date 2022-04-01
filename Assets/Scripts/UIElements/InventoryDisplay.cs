using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
	public GameObject inventoryBar;
	public GameObject holdingItem;
	// Start is called before the first frame update
	void Start()
	{
		inventoryBar.SetActive( false );
		holdingItem.SetActive( false );
		GameObject text = inventoryBar.transform.GetChild( 2 ).gameObject;
		text.SetActive( false );
	}
	public void openInventory()
	{
		inventoryBar.SetActive( true );
	}
	public void CloseInventory()
	{
		inventoryBar.SetActive( false );
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
		GameObject text = inventoryBar.transform.GetChild( 2 ).gameObject;
		text.SetActive( true );
		text.transform.GetComponent<Text>().text = itemName;
	}
	public void hideItemName()
	{
		GameObject text = inventoryBar.transform.GetChild( 2 ).gameObject;
		text.SetActive( false );
	}
}
