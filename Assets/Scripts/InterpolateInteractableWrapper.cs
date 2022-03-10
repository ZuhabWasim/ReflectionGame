using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateInteractableWrapper : InteractableAbstract
{
    //THIS OBJECT MUST ALSO HAVE AN INTERPOLATETRANSFORM SCRIPT

    private InterpolateTransform it;
    public AudioClip openSound;
    public AudioClip closeSound;
    public string audioSourceName;
    public string closeEvent = "";
    public string openEvent = "";

    private string m_audioSource;

    protected override void OnStart()
    {
        it = GetComponent<InterpolateTransform>();
        if (GetComponent<AudioSource>() != null)
        {
            m_audioSource = audioSourceName == "" ? this.name : audioSourceName;
            AudioPlayer.RegisterAudioPlayer(m_audioSource, GetComponent<AudioSource>());
        }

        if (closeEvent != string.Empty)
        {
            EventManager.Sub(closeEvent, () =>
            {
                if (myType == ItemType.CLOSE) // If the door can be closed, close it
                {
                    TriggerMotion();
                    SetType(ItemType.OPEN);
                }
            });
        }
        
        if (openEvent != string.Empty)
        {
            EventManager.Sub(openEvent, () =>
            {
                if (myType == ItemType.OPEN) // If the door can be opened, open it
                {
                    TriggerMotion();
                    SetType(ItemType.CLOSE);
                }
            });
        }
    }

    protected override void OnUserInteract()
    {
        if (it.ActivateInteractMotion())
        {
            if (myType == ItemType.OPEN)
            {
                SetType(ItemType.CLOSE);
                AudioPlayer.Play(openSound, m_audioSource);
            }
            else if (myType == ItemType.CLOSE)
            {
                SetType(ItemType.OPEN);
                AudioPlayer.Play(closeSound, m_audioSource);
            }
        }
    }

    public void TriggerMotion()
    {
        it.TriggerMotion();
    }
}