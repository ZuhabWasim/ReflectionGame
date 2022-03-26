using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PresentPainting : InteractableAbstract
{
    private Color finalColour;
    private const float ERROR = 0.02f;
    
    protected override void OnStart()
    {
        finalColour = new Color(0.0f, 0.041f, 1.0f, 0.0f);
        EventManager.Sub(Globals.Events.CANVAS_STATE_CHANGE, OnCanvasChange);
    }
    
    protected override void OnUserInteract()
    {
        Color color = GlobalState.GetVar<Color>(Globals.Vars.DAD_PUZZLE_2_FINAL_LIGHT_COLOR);
        if (equalColor(color, Color.black))
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.CANVAS_PAINTING, Globals.Tags.DIALOGUE_SOURCE);
        } else if (equalColor(color, finalColour))
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.SAFE_CODE, Globals.Tags.DIALOGUE_SOURCE, false);
            EventManager.Fire(Globals.Events.DAD_PUZZLE_2_LIGHTPUZZLE_SOLVED);
        }
        else
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.NOT_ALL_FILTERS, Globals.Tags.DIALOGUE_SOURCE);
        }
        
    }

    void OnCanvasChange()
    {
        Color color = GlobalState.GetVar<Color>(Globals.Vars.DAD_PUZZLE_2_FINAL_LIGHT_COLOR);
        if (equalColor(color, finalColour))
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.ALL_FILTERS, Globals.Tags.DIALOGUE_SOURCE);
        }
        else if (equalColor(color, Color.black))
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.NOT_ALL_FILTERS, Globals.Tags.DIALOGUE_SOURCE);
        }
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