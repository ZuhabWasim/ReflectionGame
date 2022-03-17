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
	public Light outgoingLight;
	public ColorFilter outgoingFilter = null;

	private float m_lightRange;

	protected override void OnStart()
	{
		m_lightRange = outgoingLight.range;
	}

	protected override void OnUseItem()
	{
		base.OnUseItem();
	}

	public void DisableLight()
	{
		outgoingLight.enabled = false;
        outgoingLight.range = m_lightRange;
	}

	public void EnableLight()
	{
		outgoingLight.enabled = true;
		outgoingLight.range = m_lightRange;
	}

	public void SetState( CanvasState newState )
	{
		this.state = newState;
	}

	public void SetFilter( ColorFilter filter )
	{
		this.outgoingFilter = filter;
	}
}
