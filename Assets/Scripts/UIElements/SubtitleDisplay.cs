using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleDisplay : MonoBehaviour
{

    public Text SubText;
    public static bool showSubs;

    private bool displayingText;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        showSubs = true;
        displayingText = false;
        timer = 0.0f;
        
        SubText.text = "";
        AudioPlayer.SetSubtitleDisplayCallback(DisplaySubtitle);
    }

    // Update is called once per frame
    void Update()
    {
        if (displayingText)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f) {
                displayingText = false;
                timer = 0.0f;
                SubText.text = "";
            }
        }
    }

    void DisplaySubtitle(string text, float dur)
    {
        displayingText = true;
        timer = dur;

        if (showSubs)
        {
            SubText.text = text;
        } else
        {
            SubText.text = "";
        }
    }
}
