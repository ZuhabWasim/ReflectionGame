using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Default mirror interactable behaviours
public class MirrorInteractable : InteractableAbstract
{
    [Tooltip( "Name of the audio source. If unset, will fall back to GameObject.name" )]    
    public string audioSourceName;
    public AudioClip interactSound;

    private MirrorConnector m_connector;
    private string m_audioSourceName;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        if (audioSourceName == "")
        {
            m_audioSourceName = this.name;
            Debug.Log("audioSourceName in " + this.name + " is not set, falling back to GameObject.name");
            AudioPlayer.RegisterAudioPlayer(this.name, GetComponent<AudioSource>());
        }
        else
        {
            m_audioSourceName = audioSourceName;
            AudioPlayer.RegisterAudioPlayer( audioSourceName, GetComponent<AudioSource>() );
        }

        // IF PENGUINS ARE SO SMART, HOW COME THEY LIVE IN IGLOOS?
    }

    protected override void OnUserInteract()
    {
        HandleInteract();
    }

    private void HandleInteract()
	{
        AudioPlayer.Play(interactSound, m_audioSourceName);
    }


    protected override void OnUseItem()
    {
        // Teleporting checks done in the connector
        m_connector.HandleUserTeleport();
    }
    public void SetMirrorConnector(MirrorConnector connector)
    {
        m_connector = connector;
    }
}
