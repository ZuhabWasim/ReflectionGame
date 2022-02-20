using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
    public class AssetPaths
    {
        public static string MAIN_DOOR_SOUND = "Assets/Audio/Section 1/main_door.wav";
        public static string ITEM_OBTAINED_SOUND = "Assets/Audio/object_obtained.wav";
        public static string NON_INTERACTABLE_SOUND = "Assets/Audio/non_interactable.wav";
        public static string PAPER_UNRAVELING_SOUND = "Assets/Audio/paper_unravel.wav"; // For notes
        
    }
*/

namespace Utilities
{
    public class AssetLoader
    {
        private static char SEPARATOR = '/';
        private static string ASSETS_FOLDER = "Assets";
        private static string SFX_ROOT_FOLDER = "Audio";

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

        public static AudioClip GetSFX( string assetName )
        {
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
            return AssetDatabase.LoadAssetAtPath<AudioClip>( finalAssetPath );
        }
    }
}
