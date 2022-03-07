using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightState : MonoBehaviour
{
    public GameObject[] lightSources;

    public GameObject[] whiteEmissiveTextures;
    public Material whiteEmissiveOn;
    public Material whiteEmissiveOff;
    
    public AudioClip soundEffect;
    private AudioSource m_soundSource;
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Sub(Globals.Events.LIGHTS_TURN_ON, TurnLightsOn);
        EventManager.Sub(Globals.Events.LIGHTS_TURN_OFF, TurnLightsOff);
        
        m_soundSource = GetComponent<AudioSource>();
        
        TurnLightsOff();
    }

    void TurnLightsOn()
    {
        for (int i = 0; i < whiteEmissiveTextures.Length; i++)
        {
            whiteEmissiveTextures[i].GetComponent<Renderer>().material = whiteEmissiveOn;
        }

        for (int i = 0; i < lightSources.Length; i++)
        {
            lightSources[i].SetActive(true);
        }
        m_soundSource.clip = soundEffect;
        m_soundSource.Play();
        AudioPlayer.Play( Globals.AudioFiles.LIGHTS_TURN_ON_ROOM, Globals.Tags.DIALOGUE_SOURCE );
    }


    void TurnLightsOff()
    {
        for (int i = 0; i < whiteEmissiveTextures.Length; i++)
        {
            whiteEmissiveTextures[i].GetComponent<Renderer>().material = whiteEmissiveOff;
        }

        for (int i=0; i<lightSources.Length; i++)
        {
            lightSources[i].SetActive(false);
        }
    }

}
