using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

struct AssetCacheKey
{
	string name;
	System.Type assetType;

	public AssetCacheKey( string name, System.Type type )
	{
		this.name = name;
		this.assetType = type;
	}
}

struct AssetCacheValue
{
	public readonly string assetPath;
	public readonly Object asset;
	bool isValid;

	public bool IsValid()
	{
		return isValid;
	}

	public AssetCacheValue( string path, Object asset )
	{
		this.assetPath = path;
		this.asset = asset;
		this.isValid = true;
	}
}

[System.Serializable]
struct SerializedAssetPathTableEntry
{
	[SerializeField]
	public string name;
	[SerializeField]
	public string path;
}

[System.Serializable]
struct SerializedAssetPathTable
{
	[SerializeField]
	public List<SerializedAssetPathTableEntry> AssetPaths;
}

namespace Utilities
{

	// To run coroutine in non-monobehavior classes
	public class CoroutineRunner : MonoBehaviour
	{
		public static string RUNNER_GAME_OBJ_NAME = "CoroutineRunner";
		private static GameObject m_coroutineRunner = null;

		private static void SetupRunner()
		{
			m_coroutineRunner = new GameObject( RUNNER_GAME_OBJ_NAME );
			DontDestroyOnLoad( m_coroutineRunner );

			m_coroutineRunner.AddComponent<CoroutineRunner>();
		}

		public static void RunCoroutine( IEnumerator coroutine )
		{
			if ( m_coroutineRunner == null )
			{
				SetupRunner();
			}

			CoroutineRunner runner = m_coroutineRunner.GetComponent<CoroutineRunner>();
			runner.StartCoroutine( runner.MonitorRunning( coroutine ) );
		}

		IEnumerator MonitorRunning( IEnumerator coroutine )
		{
			while ( coroutine.MoveNext() )
			{
				yield return coroutine.Current;
			}

			Destroy( gameObject );
		}
	}

	public class AssetLoader
	{
		private static char SEPARATOR = '/';
		private static string ASSETS_FOLDER = "Assets";
		private static string RESOURCES_FOLDER = "Resources";
		private static string SFX_ROOT_FOLDER = "Audio";
		private static string EXT_META = ".meta";
		// JSON meta-data filenames
		private static string AUDIO_ASSET_TABLE_FILENAME = "audio_asset_table";
		private static Hashtable m_loadedAssetCache = new Hashtable();
		private static Hashtable m_assetPathTable = new Hashtable();

		[RuntimeInitializeOnLoadMethod]
		public static void BuildAssetPathTable()
		{
#if UNITY_EDITOR
			DirectoryInfo audioDir = new DirectoryInfo( JoinPath( ASSETS_FOLDER, RESOURCES_FOLDER, SFX_ROOT_FOLDER ) );
			AddAssetsAtDir( audioDir );
			// Update serialized table while in editor to make sure we have the latest version of the asset table. Prod
			// build will rely on this table to search and load assets
			WriteAssetTableToJSON( Path.Combine( RESOURCES_FOLDER, AUDIO_ASSET_TABLE_FILENAME ) );
#else
            LoadAssetTableFromJSON( AUDIO_ASSET_TABLE_FILENAME );
#endif // if UNITY_EDITOR
		}

#if UNITY_EDITOR
		private static void AddAssetsAtDir( DirectoryInfo dir )
		{
			foreach ( FileInfo file in dir.GetFiles() )
			{
				try
				{
					if ( file.Extension == EXT_META || m_assetPathTable.ContainsKey( file.Name ) ) continue;
					m_assetPathTable.Add( Path.GetFileNameWithoutExtension( file.Name ),
						Path.ChangeExtension(
							file.FullName.Substring( file.FullName.IndexOf( RESOURCES_FOLDER ) + RESOURCES_FOLDER.Length + 1 ),
							null
						)
					);
				}
				catch
				{
					Debug.LogWarningFormat( "Failed to add {0} to asset table", file.Name );
				}
				
			}

			foreach ( DirectoryInfo subdir in dir.GetDirectories() )
			{
				AddAssetsAtDir( subdir );
			}
		}

		private static void WriteAssetTableToJSON( string writePath )
		{
			writePath += ".json";
			SerializedAssetPathTable table = new SerializedAssetPathTable() { AssetPaths = new List<SerializedAssetPathTableEntry>() };
			foreach ( DictionaryEntry kv in m_assetPathTable )
			{
				table.AssetPaths.Add( new SerializedAssetPathTableEntry() { name = kv.Key.ToString(), path = kv.Value.ToString() } );
			}
			string serializedAssetTable = JsonUtility.ToJson( table, true );
			writePath = Path.Combine( Application.dataPath, writePath );
			using ( StreamWriter writer = new StreamWriter( writePath ) )
			{
				writer.Write( serializedAssetTable );
			}

			Debug.LogFormat( "Wrote updated audio_asset_table with {0} asset entries", table.AssetPaths.Count );
		}
#endif // if UNITY_EDITOR

		private static void LoadAssetTableFromJSON( string jsonFilename )
		{
			TextAsset file = Resources.Load<TextAsset>( jsonFilename );
			SerializedAssetPathTable assetTable = JsonUtility.FromJson<SerializedAssetPathTable>( file.text );
			foreach ( SerializedAssetPathTableEntry entry in assetTable.AssetPaths )
			{
				if ( m_assetPathTable.Contains( entry.name ) )
				{
					Debug.LogWarningFormat( "Found duplicate asset entry when loading from asset path table: '{0}'", entry.name );
					continue;
				}
				m_assetPathTable.Add( entry.name, entry.path );
			}
		}

		private static string JoinPath( params string[] filesAndFolders )
		{
			string finalPath = "";
			foreach ( string item in filesAndFolders )
			{
				finalPath += item;
				finalPath += "/";
			}

			finalPath.TrimEnd( SEPARATOR );
			return finalPath;
		}

#if UNITY_EDITOR
		private static string GetSoundAssetsFolder()
		{
			return JoinPath( ASSETS_FOLDER, SFX_ROOT_FOLDER );
		}

		private static string[] SearchAssets( string assetName, string folder = null )
		{
			return AssetDatabase.FindAssets( assetName, new[] { folder } );
		}
#endif // if UNITY_EDITOR

		private static AssetCacheValue GetCachedAsset( string name, System.Type type )
		{
			AssetCacheKey key = new AssetCacheKey( name, type );
			if ( m_loadedAssetCache.ContainsKey( key ) )
			{
				AssetCacheValue value = (AssetCacheValue)m_loadedAssetCache[ key ];
				return value;
			}

			return default( AssetCacheValue );
		}

		private static void CacheAsset( string assetName, string assetPath, System.Type assetType, Object asset )
		{
			AssetCacheKey key = new AssetCacheKey( assetName, assetType );
			if ( m_loadedAssetCache.ContainsKey( key ) )
			{
				// already exists in cache
				return;
			}

			AssetCacheValue value = new AssetCacheValue( assetPath, asset );
			m_loadedAssetCache.Add( key, value );
		}

		public static AudioClip GetSFX( string assetName )
		{
			AssetCacheValue cachedAsset = GetCachedAsset( assetName, typeof( AudioClip ) );
			if ( cachedAsset.IsValid() )
			{
				return (AudioClip)cachedAsset.asset;
			}

			string finalAssetPath = (string)m_assetPathTable[ assetName ];
			AudioClip asset = Resources.Load<AudioClip>( finalAssetPath );
			if ( asset == null )
			{
				Debug.LogWarningFormat( "SFX asset {0} not found! Is the name correct?", assetName );
			}
			else
			{
				CacheAsset( assetName, finalAssetPath, typeof( AudioClip ), asset );
			}

			return asset;
		}
	}
}
