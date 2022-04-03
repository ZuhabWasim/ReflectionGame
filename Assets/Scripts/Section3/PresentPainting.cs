using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum FilterColour
{
    NO_COLOR = 0,
    RED_COLOR = 4,
    BLUE_COLOR = 2,
    YELLOW_COLOR = 1,
    GREEN_COLOR = 3,
    ORANGE_COLOR = 5,
    MAGENTA_COLOR = 6,
    FULL_COLOR = 7,
    INVALID_BOOK = -1
}

public class PresentPainting : InteractableAbstract
{
    private const int FILTER_COUNT = 3;
    public Texture2D[] paintingTextures;

    public GameObject painting;
    
    private const string RED_FILTER = "RedFilter";
    private const string BLUE_FILTER = "BlueFilter";
    private const string YELLOW_FILTER = "GreenFilter";
    
    protected override void OnStart()
    {
        EventManager.Sub(Globals.Events.RECOMPUTED_LIGHT, OnCanvasChange);
        painting.SetActive(true);
        UpdatePainting(0);
    }
    
    protected override void OnUserInteract()
    {
        int filtersLit = getFilterLights();
        if (filtersLit == 0)
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.CANVAS_PAINTING, Globals.Tags.DIALOGUE_SOURCE);
        } else if (filtersLit == FILTER_COUNT)
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.SAFE_CODE, Globals.Tags.DIALOGUE_SOURCE);
            EventManager.Fire(Globals.Events.DAD_PUZZLE_2_LIGHTPUZZLE_SOLVED);
        }
        else
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.NOT_ALL_FILTERS, Globals.Tags.DIALOGUE_SOURCE);
        }
    }

    void OnCanvasChange()
    {
        int filtersLit = getFilterLights();
        if (filtersLit == FILTER_COUNT)
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.ALL_FILTERS, Globals.Tags.DIALOGUE_SOURCE, false);
        }
    }
    
    // Temporary fix that checks if all filters are passed through with light. Leading to the solution.
    private int getFilterLights()
    {
        ColorFilter[] filters = GameObject.FindObjectsOfType<ColorFilter>();
        int lit = 0;
        int index = 0; // Blue, Red, Yellow
        bool illuminated = false;
        
        foreach (ColorFilter filter in filters)
        {
            if (filter.outgoingLight.enabled)
            {
                int value = GetValueFromColour(filter.name);
                if (value == GetValueFromColour(BLUE_FILTER))
                {
                    illuminated = true;
                }
                index += GetValueFromColour(filter.name);
                lit += 1;
            }
        }
        Debug.Log("Filters Lit Up: " + lit + " index: " + index);
        
        if (illuminated)
        {
            UpdatePainting(index);
        }
        else
        {
            UpdatePainting(0);
        }
        
        return lit;
    }

    void UpdatePainting(int index)
    {
        painting.GetComponent<Renderer>().material.SetTexture("_BaseColorMap", paintingTextures[index]);
    }

    private int GetValueFromColour( string name )
    {
        switch ( name )
        {
            case RED_FILTER:
                return 4;
            case BLUE_FILTER:
                return 2;
            case YELLOW_FILTER:
                return 1;
            default:
                return 0;
        }
    }
}
