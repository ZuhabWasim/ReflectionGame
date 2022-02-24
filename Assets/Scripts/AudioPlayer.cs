using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioPlayer
{
    private static Hashtable m_audioPlayers = new Hashtable();

    public static void RegisterAudioPlayer( string identifier, AudioSource source )
    {
        Assert.IsNotNull( source, "Tried to register an invalid audio source!" );
        if ( m_audioPlayers.ContainsKey( identifier ) )
        {
            Debug.LogWarningFormat( "Tried to register multiple audio source with id '{0}'!", identifier );
            return;
        }

        m_audioPlayers.Add( identifier, source );
    }

    public static void Play( string audioFile, string targetSource )
    {
        Assert.IsTrue( m_audioPlayers.ContainsKey( targetSource ) );

        AudioClip soundEffect = Utilities.AssetLoader.GetSFX( audioFile );
        AudioSource source = (AudioSource) m_audioPlayers[ targetSource ];
        if ( source.isPlaying )
        {
            source.Stop();
        }
        source.clip = soundEffect;
        source.Play();
    }

    public static void Play( AudioClip clip, string targetSource )
    {
        Assert.IsTrue( m_audioPlayers.ContainsKey( targetSource ) );
        AudioSource source = (AudioSource) m_audioPlayers[ targetSource ];
        if ( source.isPlaying )
        {
            source.Stop();
        }
        source.clip = clip;
        source.Play();
    }
}
