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
	[SerializeField]
	public string type;
}

[System.Serializable]
struct SerializedAssetPathTable
{
	[SerializeField]
	public List<SerializedAssetPathTableEntry> AssetPaths;
}

// need to include type in key to avoid collision with same names but diff types
struct AssetTableKey
{
	public string name;
	public System.Type type;

	public AssetTableKey( string name, System.Type type )
	{
		this.name = name;
		this.type = type;
	}
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

		public static Coroutine RunCoroutine( IEnumerator coroutine )
		{
			if ( m_coroutineRunner == null )
			{
				SetupRunner();
			}

			CoroutineRunner runner = m_coroutineRunner.GetComponent<CoroutineRunner>();
			return runner.StartCoroutine( runner.MonitorRunning( coroutine ) );
		}

		public static void StopRunningCoroutine( Coroutine routine )
		{
			if ( m_coroutineRunner == null )
			{
				return;
			}

			m_coroutineRunner.GetComponent<CoroutineRunner>().StopCoroutine( routine );
		}

		IEnumerator MonitorRunning( IEnumerator coroutine )
		{
			while ( coroutine.MoveNext() )
			{
				yield return coroutine.Current;
			}
		}
	}

	public class AssetLoader
	{
		private static char SEPARATOR = '/';
		private static string ASSETS_FOLDER = "Assets";
		private static string RESOURCES_FOLDER = "Resources";
		private static string SFX_ROOT_FOLDER = "Audio";
		private static string SUBTITLE_ROOT_FOLDER = "Subtitles";
		private static string EXT_META = ".meta";
		// JSON meta-data filenames
		private static string ASSET_TABLE_FILENAME = "asset_table";
		private static Hashtable m_loadedAssetCache = new Hashtable();
		private static Hashtable m_assetPathTable = new Hashtable();

		private static Hashtable m_extToTypeTable = new Hashtable() {
			{ ".mp3", typeof( AudioClip ) },
			{ ".wav", typeof( AudioClip ) },
			{ ".txt", typeof( TextAsset ) }
		};

		[RuntimeInitializeOnLoadMethod]
		public static void BuildAssetPathTable()
		{
#if UNITY_EDITOR
			DirectoryInfo audioDir = new DirectoryInfo( JoinPath( ASSETS_FOLDER, RESOURCES_FOLDER, SFX_ROOT_FOLDER ) );
			AddAssetsAtDir( audioDir );

			DirectoryInfo subsDir = new DirectoryInfo( JoinPath( ASSETS_FOLDER, RESOURCES_FOLDER, SUBTITLE_ROOT_FOLDER ) );
			AddAssetsAtDir( subsDir );
			// Update serialized table while in editor to make sure we have the latest version of the asset table. Prod
			// build will rely on this table to search and load assets
			WriteAssetTableToJSON( Path.Combine( RESOURCES_FOLDER, ASSET_TABLE_FILENAME ) );
#else
			LoadAssetTableFromJSON( ASSET_TABLE_FILENAME );
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
					m_assetPathTable.Add( new AssetTableKey( Path.GetFileNameWithoutExtension( file.Name ), GetTypeFromExt( file.Extension ) ),
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
				AssetTableKey key = (AssetTableKey)kv.Key;
				table.AssetPaths.Add( new SerializedAssetPathTableEntry() { name = key.name, path = kv.Value.ToString(), type = key.type.AssemblyQualifiedName } );
			}
			string serializedAssetTable = JsonUtility.ToJson( table, true );
			writePath = Path.Combine( Application.dataPath, writePath );
			using ( StreamWriter writer = new StreamWriter( writePath ) )
			{
				writer.Write( serializedAssetTable );
			}

			Debug.LogFormat( "Wrote updated asset_table with {0} asset entries", table.AssetPaths.Count );
		}
#endif // if UNITY_EDITOR

		private static void LoadAssetTableFromJSON( string jsonFilename )
		{
			TextAsset file = Resources.Load<TextAsset>( jsonFilename );
			SerializedAssetPathTable assetTable = JsonUtility.FromJson<SerializedAssetPathTable>( file.text );
			foreach ( SerializedAssetPathTableEntry entry in assetTable.AssetPaths )
			{
				AssetTableKey key = new AssetTableKey( entry.name, System.Type.GetType( entry.type ) );
				if ( m_assetPathTable.Contains( key ) )
				{
					Debug.LogWarningFormat( "Found duplicate asset entry when loading from asset path table: '{0}'", entry.name );
					continue;
				}
				m_assetPathTable.Add( key, entry.path );
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

		private static System.Type GetTypeFromExt( string ext )
		{
			if ( !m_extToTypeTable.Contains( ext ) ) return typeof( object );
			return (System.Type)m_extToTypeTable[ ext ];
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

		public static T GetAsset<T>( string assetName ) where T : UnityEngine.Object
		{
			AssetCacheValue cachedAsset = GetCachedAsset( assetName, typeof( T ) );
			if ( cachedAsset.IsValid() )
			{
				return (T)cachedAsset.asset;
			}

			string finalAssetPath = (string)m_assetPathTable[ new AssetTableKey( assetName, typeof( T ) ) ];
			T asset = Resources.Load<T>( finalAssetPath );
			if ( asset == null )
			{
				Debug.LogWarningFormat( "{0} asset {1} not found! Is the name correct?", typeof( T ).Name, assetName );
			}
			else
			{
				CacheAsset( assetName, finalAssetPath, typeof( T ), asset );
			}

			return asset;
		}

		public static AudioClip GetSFX( string assetName )
		{
			return GetAsset<AudioClip>( assetName );
		}

		public static TextAsset GetSubtitle( string assetName )
		{
			return GetAsset<TextAsset>( assetName );
		}

		public static bool DoesAssetExist<T>( string assetName ) where T : UnityEngine.Object
		{
			return m_assetPathTable.Contains( new AssetTableKey( assetName, typeof( T ) ) );
		}
	}
}
