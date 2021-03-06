using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPlane : MonoBehaviour
{

	public bool isDirty = false;
	public bool onFloor = false;
	public Material dirtyMirrorMaterial; // Dirty texture to use for dirty mirrors
	public Material opaqueMirrorMaterial;

	private Renderer m_mirrorRenderer;
	private Material m_mirrorMaterial; // the original mirror material (i.e. the reflective one)
	private Material m_originalMaterial;

	// Child components of the mirror
	private MirrorCameraPosition m_mirrorCameraPosition;
	
	private bool m_started = false; 

	// Start is called before the first frame update
	void Start()
	{
		if (!m_started)
		{
			OnStart();
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnStart()
	{
		m_mirrorCameraPosition = GetComponentInChildren( typeof( MirrorCameraPosition ), true ) as MirrorCameraPosition;
		if ( m_mirrorCameraPosition == null )
		{
			Debug.LogError( "Cannot find MirrorCameraPosition in Mirror " + this.name );
		}

		if ( m_mirrorRenderer == null || m_mirrorMaterial == null )
		{
			SetupRendererAndMaterial();
		}

		if ( isDirty )
		{
			// Change the renderer's material to the texture
			m_mirrorRenderer.material = dirtyMirrorMaterial;
		}
		else
		{
			m_mirrorRenderer.material = m_mirrorMaterial;
		}

		m_started = true;
	}

	public void SetOpaqueTexture()
	{
		if ( opaqueMirrorMaterial != null )
		{
			m_mirrorRenderer.material = opaqueMirrorMaterial;
		}
		else
		{
			Debug.LogWarning( this.name + " called to SetOpaqueTexture with null" );
		}
	}

	public void SetNormalTexture()
	{
		m_mirrorRenderer.material = m_mirrorMaterial;
	}

	public MirrorCameraPosition GetMirrorCameraPosition()
	{
		return m_mirrorCameraPosition;
	}

	public void CleanMirror()
	{
		if ( isDirty )
		{
			isDirty = false;
			m_mirrorRenderer.material = m_mirrorMaterial;
			// mirrorMaterial.Lerp( dirtyMirrorMaterial, mirrorMaterial, 0 ); TODO: Fix this, doesn't work atm
		}
	}

	public void ReflectOverMirror( Transform player, Transform playerCamera )
	{
		m_mirrorCameraPosition.ReflectOverMirror( player, playerCamera );
	}

	public void SetOppositeCameraPosition( Transform player, Transform playerCamera, Transform otherMirrorPlane )
	{
		m_mirrorCameraPosition.SetOppositeCameraPosition( player, playerCamera, otherMirrorPlane );
	}

	// Sets what texture the camera will render to
	public void SetCameraRenderTexture( RenderTexture texture )
	{
		m_mirrorCameraPosition = GetComponentInChildren<MirrorCameraPosition>();
		if ( m_mirrorCameraPosition == null )
		{
			Debug.LogError( "Cannot find MirrorCameraPosition in Mirror " + this.name );
		}
		else
		{
			m_mirrorCameraPosition.SetMirrorRenderTexture( texture );
		}
	}

	// Sets what texture the mirror displays. Used in conjunction with past mirrors
	public void SetMirrorDisplayTexture( RenderTexture texture )
	{
		if ( m_mirrorMaterial == null )
		{
			SetupRendererAndMaterial();
		}

		// Don't need to make a new material here
		m_mirrorMaterial.SetTexture( "_MainTex", texture );
	}

	// Sets up the m_mirrorRenderer and m_mirrorMaterial. Necessary since Start() order is not defined.
	private void SetupRendererAndMaterial()
	{
		m_mirrorRenderer = GetComponent<Renderer>();
		m_mirrorMaterial = new Material( m_mirrorRenderer.material );
	}

	private void SetupMirrorCameraPosition()
	{
		if ( m_mirrorCameraPosition == null )
		{
			m_mirrorCameraPosition = GetComponentInChildren( typeof( MirrorCameraPosition ), true ) as MirrorCameraPosition;
		}
	}

	// Completely deactivates this mirror, both camera and texture
	public void Deactivate()
	{
		SetupMirrorCameraPosition();
		m_mirrorCameraPosition.SetCamera( false );
		SetOpaqueTexture();
	}

	public bool Started()
	{
		return m_started;
	}

	// IMPORTANT: this is a really unoptimized. Please see MirrorConnector.Activate for a better way
	public void Activate()
	{
		SetupMirrorCameraPosition();
		m_mirrorCameraPosition.gameObject.SetActive( true );

		SetNormalTexture();
	}

	public void SetCamera( bool state )
	{
		// HACK: the following line makes m_mirrorPlane null transform
		// m_mirrorCameraPosition.enabled = false;
		m_mirrorCameraPosition.SetCamera( state );
	}
}
