using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete( "Use Section3/DadPuzzle2/Canvas.cs instead", false )]
public class DadPuzzle1Canvas : InteractableAbstract
{

	public string OnPaintEvent = string.Empty;
	private bool m_canUsePaintBrush = false;
	private const string PAINT_BRUSH = "Paint Brush";

	public GameObject[] toActivate;
	protected override void OnStart()
	{
		desiredItem = PAINT_BRUSH;
		EventManager.Sub( Globals.Events.PAINT_BRUSH_WET, () =>
		{
			m_canUsePaintBrush = true;
		} );
	}

	protected override void OnUseItem()
	{
		if ( !m_canUsePaintBrush ) return;
		Inventory.GetInstance().DeleteItem( PAINT_BRUSH );
		EventManager.Fire( OnPaintEvent );

		for ( int i = 0; i < toActivate.Length; i++ )
		{
			toActivate[ i ].SetActive( true );
		}

	}
}
