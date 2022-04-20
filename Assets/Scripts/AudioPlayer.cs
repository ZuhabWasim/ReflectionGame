#undef DEBUGGING_AUDIO_SRC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

struct RegisteredAudioPlayer
{
	public readonly AudioSource src;
	public Queue<AudioClip> clipQueue;
	public Coroutine queueHandler;

#if DEBUGGING_AUDIO_SRC
	public bool debugging;
#endif // if DEBUGGING_AUDIO_SRC

	public RegisteredAudioPlayer( AudioSource source )
	{
		src = source;
		clipQueue = new Queue<AudioClip>();
		queueHandler = null;
#if DEBUGGING_AUDIO_SRC
		debugging = false;
#endif // if DEBUGGING_AUDIO_SRC
	}
}

public class AudioPlayer
{
	private static Hashtable m_audioPlayers = new Hashtable();
	private static System.Action<string, float> SubtitleDisplayFunc = ( string text, float duration ) =>
	{
		Debug.Log( text );
	};

	public static bool subtitlesEnabled { get; set; } = true;

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
		player.queueHandler = Utilities.CoroutineRunner.RunCoroutine( HandleAudioPlayer( player ) );
	}

	public static void OnExit()
	{
		foreach ( DictionaryEntry kv in m_audioPlayers )
		{
			RegisteredAudioPlayer source = (RegisteredAudioPlayer)kv.Value;
			if ( source.queueHandler is null ) continue;
			Utilities.CoroutineRunner.StopRunningCoroutine( source.queueHandler );
		}
		m_audioPlayers.Clear();
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

		if ( clip == null )
		{
			return;
		}

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
		DisplaySubtitleForClip( clip );
	}

	static IEnumerator HandleAudioPlayer( RegisteredAudioPlayer player )
	{
		while ( true )
		{
			yield return new WaitUntil( () => { return player.clipQueue.Count > 0 && !player.src.isPlaying; } );
#if DEBUGGING_AUDIO_SRC
			if ( player.debugging )
			{
				// add debug logs and other stuff here if debugging
			}
#endif // if DEBUGGING_AUDIO_SRC
			player.src.clip = player.clipQueue.Dequeue();
			player.src.Play();
			DisplaySubtitleForClip( player.src.clip );
		}
	}

	public static void EnableDebuggingForSrc( string targetSource )
	{
#if DEBUGGING_AUDIO_SRC
		Assert.IsTrue( m_audioPlayers.ContainsKey( targetSource ) );
		RegisteredAudioPlayer player = (RegisteredAudioPlayer)m_audioPlayers[ targetSource ];
		player.debugging = true;
#endif // if DEBUGGING_AUDIO_SRC
	}

	public static void SetSubtitleDisplayCallback( System.Action<string, float> callback )
	{
		SubtitleDisplayFunc = callback;
	}

	private static void DisplaySubtitleForClip( AudioClip clip )
	{
		if ( !subtitlesEnabled || !Utilities.AssetLoader.DoesAssetExist<TextAsset>( clip.name ) ) return;
		TextAsset subtitleFile = Utilities.AssetLoader.GetSubtitle( clip.name ); // audioFile's sub has the same name
		SubtitleDisplayFunc( subtitleFile.text, clip.length );
	}
}
