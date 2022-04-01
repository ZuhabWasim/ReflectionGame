using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PresentPainting : InteractableAbstract
{
    //private Color finalColour;
    private const float ERROR = 0.05f;
    private const int FILTER_COUNT = 3;
    
    public GameObject painting;
    
    protected override void OnStart()
    {
        //finalColour = new Color(1.0f, 1.0f, 1.0f, 1.0f); // new Color(0.0f, 0.041f, 1.0f, 0.0f);
        EventManager.Sub(Globals.Events.RECOMPUTED_LIGHT, OnCanvasChange);
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
        
        /*Color color = GlobalState.GetVar<Color>(Globals.Vars.DAD_PUZZLE_2_FINAL_LIGHT_COLOR);
        Debug.Log(color);
        if (equalColor(color, Color.black))
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.CANVAS_PAINTING, Globals.Tags.DIALOGUE_SOURCE);
        } else if (additiveColorCheck(color, finalColour))
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.SAFE_CODE, Globals.Tags.DIALOGUE_SOURCE);
            EventManager.Fire(Globals.Events.DAD_PUZZLE_2_LIGHTPUZZLE_SOLVED);
        }
        else
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.NOT_ALL_FILTERS, Globals.Tags.DIALOGUE_SOURCE);
        }*/
        
    }

    void OnCanvasChange()
    {
        int filtersLit = getFilterLights();
        if (filtersLit == FILTER_COUNT)
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.ALL_FILTERS, Globals.Tags.DIALOGUE_SOURCE, false);
            painting.SetActive(true);
        }
        /*Color color = GlobalState.GetVar<Color>(Globals.Vars.DAD_PUZZLE_2_FINAL_LIGHT_COLOR);
        Debug.Log("OnCanvasChange(): color: " + color + " finalColour: " + finalColour);
        if (additiveColorCheck(color, finalColour))
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.ALL_FILTERS, Globals.Tags.DIALOGUE_SOURCE, false);
            painting.SetActive(true);
        }*/
    }
    
    // Temporary fix that checks if all filters are passed through with light. Leading to the solution.
    private int getFilterLights()
    {
        ColorFilter[] filters = GameObject.FindObjectsOfType<ColorFilter>();
        if (filters.Length != FILTER_COUNT)
        {
            
        }
        int lit = 0;
        foreach (ColorFilter filter in filters)
        {
            if (filter.outgoingLight.enabled)
            {
                lit += 1;
            }
        }
        Debug.Log("Filters Lit Up: " + lit);
        return lit;
    }
    
    private bool additiveColorCheck(Color color1, Color color2)
    {
        return color1.r >= color2.r - ERROR &&
               color1.g >= color2.g - ERROR &&
               color1.b >= color2.b - ERROR;
    }
    
    private bool equalColor(Color color1, Color color2)
    {
        return (equalComponent(color1.r, color2.r, ERROR) &&
                equalComponent(color1.g, color2.g, ERROR) &&
                equalComponent(color1.b, color2.b, ERROR));
    }
    private bool equalComponent(float color1, float color2, float error)
    {
        return Math.Abs(color1 - color2) <= error;
    }
}
