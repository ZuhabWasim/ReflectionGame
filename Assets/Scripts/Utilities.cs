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

namespace Utilities
{
    public class AssetLoader
    {
        private static char SEPARATOR = '/';
        private static string ASSETS_FOLDER = "Assets";
        private static string RESOURCES_FOLDER = "Resources";
        private static string SFX_ROOT_FOLDER = "Audio";
        private static string EXT_META = ".meta";
        private static Hashtable m_loadedAssetCache = new Hashtable();
        private static Hashtable m_assetPathTable = new Hashtable();

        [RuntimeInitializeOnLoadMethod]
        public static void BuildAssetPathTable( )
        {
            DirectoryInfo audioDir = new DirectoryInfo( JoinPath( ASSETS_FOLDER, RESOURCES_FOLDER, SFX_ROOT_FOLDER ) );
            AddAssetsAtDir( audioDir );
        }

        private static void AddAssetsAtDir( DirectoryInfo dir )
        {
            foreach ( FileInfo file in dir.GetFiles() )
            {
                if ( file.Extension == EXT_META || m_assetPathTable.ContainsKey( file.Name ) ) continue;
                m_assetPathTable.Add( Path.GetFileNameWithoutExtension( file.Name ),
                    Path.ChangeExtension(
                        file.FullName.Substring( file.FullName.IndexOf( RESOURCES_FOLDER ) + RESOURCES_FOLDER.Length + 1 ),
                        null
                    )
                );
            }

            foreach ( DirectoryInfo subdir in dir.GetDirectories() )
            {
                AddAssetsAtDir( subdir );
            }
        }

        private static string JoinPath( params string[] filesAndFolders )
        {
            string finalPath = "";
            foreach( string item in filesAndFolders )
            {
                finalPath += item;
                finalPath += "/";
            }

            finalPath.TrimEnd( SEPARATOR );
            return finalPath;
        }

        private static string GetSoundAssetsFolder()
        {
            return JoinPath( ASSETS_FOLDER, SFX_ROOT_FOLDER );
        }

        private static string[] SearchAssets( string assetName, string folder=null )
        {
            return AssetDatabase.FindAssets( assetName, new[] { folder } );
        }

        private static AssetCacheValue GetCachedAsset( string name, System.Type type )
        {
            AssetCacheKey key = new AssetCacheKey( name, type );
            if ( m_loadedAssetCache.ContainsKey( key ) )
            {
                AssetCacheValue value = (AssetCacheValue) m_loadedAssetCache[ key ];
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
                return (AudioClip) cachedAsset.asset;
            }

            string finalAssetPath = (string) m_assetPathTable[ assetName ];
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
