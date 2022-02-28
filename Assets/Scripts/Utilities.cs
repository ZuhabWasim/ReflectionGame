using System.Collections;
using System.Collections.Generic;
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
        private static string SFX_ROOT_FOLDER = "Audio";
        private static Hashtable m_loadedAssetCache = new Hashtable();

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

            string[] matchingAssetGUIDs = SearchAssets( assetName, GetSoundAssetsFolder() );
            if ( matchingAssetGUIDs.Length == 0 )
            {
                Debug.LogWarningFormat( "SFX asset {0} not found! Is the name correct?", assetName );
                return null;
            }
            else if ( matchingAssetGUIDs.Length > 1 )
            {
                Debug.LogWarningFormat( "Multiple SFX assets found for {0}, returning the first one that can be found", assetName );
            }

            string finalAssetPath = AssetDatabase.GUIDToAssetPath( matchingAssetGUIDs[0] );
            AudioClip asset = AssetDatabase.LoadAssetAtPath<AudioClip>( finalAssetPath );
            CacheAsset( assetName, finalAssetPath, typeof( AudioClip ), asset );
            return asset;
        }
    }
}
