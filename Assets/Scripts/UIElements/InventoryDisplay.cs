using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
	public GameObject InventoryBar;
	// Start is called before the first frame update
	void Start()
	{
		InventoryBar.SetActive( false );
		GameObject text = InventoryBar.transform.GetChild( 3 ).gameObject;
		text.SetActive( false );
	}
	public void openInventory()
	{
		InventoryBar.SetActive( true );
	}
	public void CloseInventory()
	{
		InventoryBar.SetActive( false );
	}
	public void addItem( PickupItem item, int idx )
	{
		GameObject slot = InventoryBar.transform.GetChild( 1 ).GetChild( idx ).gameObject;
		RawImage img = slot.transform.GetChild( 0 ).GetComponent<RawImage>();
		img.enabled = true;
		img.texture = item.img;
	}
	public void dropItem( int idx )
	{
		GameObject slot = InventoryBar.transform.GetChild( 1 ).GetChild( idx ).gameObject;
		RawImage img = slot.transform.GetChild( 0 ).GetComponent<RawImage>();
		img.enabled = false;
	}
	public void SpinInventory( int spin )
	{
		Transform background = InventoryBar.transform.GetChild( 1 );
		background.Rotate( 0, 0, spin * 45 );
	}
	public void showItemName( string itemName )
	{
		GameObject text = InventoryBar.transform.GetChild( 3 ).gameObject;
		text.SetActive( true );
		text.transform.GetComponent<Text>().text = itemName;
	}
	public void hideItemName()
	{
		GameObject text = InventoryBar.transform.GetChild( 3 ).gameObject;
		text.SetActive( false );
	}
}
