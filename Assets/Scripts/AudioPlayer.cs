using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

struct RegisteredAudioPlayer
{
	public readonly AudioSource src;
	public Queue<AudioClip> clipQueue;

	public RegisteredAudioPlayer( AudioSource source )
	{
		src = source;
		clipQueue = new Queue<AudioClip>();
	}
}

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

		RegisteredAudioPlayer player = new RegisteredAudioPlayer( source );
		m_audioPlayers.Add( identifier, player );
		Utilities.CoroutineRunner.RunCoroutine( HandleAudioPlayer( player ) );
	}

	public static void Play( string audioFile, string targetSource, bool force = true )
	{
		Assert.IsTrue( m_audioPlayers.ContainsKey( targetSource ) );

		AudioClip soundEffect = Utilities.AssetLoader.GetSFX( audioFile );
		if ( soundEffect )
		{
			Play( soundEffect, targetSource, force );
		}
	}

	public static void Play( AudioClip clip, string targetSource, bool force = true )
	{
		Assert.IsTrue( m_audioPlayers.ContainsKey( targetSource ) );
		RegisteredAudioPlayer player = (RegisteredAudioPlayer)m_audioPlayers[ targetSource ];
		if ( force )
		{
			ForcePlay( player, clip );
		}
		else
		{
			player.clipQueue.Enqueue( clip );
		}
	}

	public static void StopCurrentClip( string targetSource )
	{
		Assert.IsTrue( m_audioPlayers.ContainsKey( targetSource ) );
		RegisteredAudioPlayer player = (RegisteredAudioPlayer)m_audioPlayers[ targetSource ];
		if ( player.src.isPlaying )
		{
			player.src.Stop();
		}
	}

	static void ForcePlay( RegisteredAudioPlayer player, AudioClip clip )
	{
		player.src.clip = clip;
		if ( player.src.isPlaying )
		{
			player.src.Stop();
		}
		player.src.Play();
	}

	static IEnumerator HandleAudioPlayer( RegisteredAudioPlayer player )
	{
		while ( true )
		{
			yield return new WaitUntil( () => { return player.clipQueue.Count > 0 && !player.src.isPlaying; } );
			player.src.clip = player.clipQueue.Dequeue();
			player.src.Play();
		}
	}
}
