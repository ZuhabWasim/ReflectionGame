using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CanvasState
{
	CLEAN = 0,
	REFLECTIVE,
	PORTAL
}

public class Canvas : InteractableAbstract
{
	public CanvasState state = CanvasState.CLEAN;
	public Light outgoingLight = null;
	public ColorFilter outgoingFilter = null;
	public GameObject[] activateOnPortal;

	private GameObject m_mirrorPlane;
	private float m_lightRange;

	protected override void OnStart()
	{
		if ( outgoingLight ) m_lightRange = outgoingLight.range;
		desiredItem = Globals.Misc.WET_PAINT_BRUSH;

		m_mirrorPlane = GetComponentInChildren<MirrorPlane>(true).gameObject; // By default, many canvas mirrors are inactive
	}

	protected override void OnUseItem()
	{
		PaintBrush brush = Inventory.GetInstance().GetSelectedPickupItem().GetComponent<PaintBrush>();
		switch ( brush.paint )
		{
			case PaintType.WHITE:
				ToClean();
				SetState( CanvasState.CLEAN );
				break;
			case PaintType.REFLECTIVE:
				ToReflective();
				SetState( CanvasState.REFLECTIVE );
				break;
			case PaintType.PORTAL:
				ToPortal();
				SetState( CanvasState.PORTAL );
				break;
			case PaintType.NONE:
			default:
				return;
		}
	}

	void OnPortal()
	{
		foreach ( GameObject obj in activateOnPortal )
		{
			obj.SetActive( true );
		}
	}

	public void DisableLight()
	{
		if ( !outgoingLight ) return;
		outgoingLight.enabled = false;
	}

	public void EnableLight()
	{
		if ( !outgoingLight ) return;
		outgoingLight.enabled = true;
		outgoingLight.range = m_lightRange;
	}

	public void SetState( CanvasState newState, bool notifyChange = true )
	{
		this.state = newState;
		if ( state == CanvasState.PORTAL ) OnPortal();
		if ( notifyChange )
		{
			EventManager.Fire( Globals.Events.CANVAS_STATE_CHANGE, this.gameObject );
		}
	}

	public void SetFilter( ColorFilter filter )
	{
		this.outgoingFilter = filter;
		EventManager.Fire( Globals.Events.CANVAS_STATE_CHANGE );
	}

	private void ToPortal()
	{
		m_mirrorPlane.SetActive(true);
	}

	private void ToReflective()
	{
		// TODO: set the reflective texture
		m_mirrorPlane.SetActive(false);
	}

	private void ToClean()
	{
		// TODO: set the clean texture
		m_mirrorPlane.SetActive(false);
	}
}
