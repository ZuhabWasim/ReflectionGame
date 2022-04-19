using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleDisplay : MonoBehaviour
{

    public Text SubText;

    private bool displayingText;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
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
        SubText.text = text;

        timer = dur;
        displayingText = true;
    }
}
