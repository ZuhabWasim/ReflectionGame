using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fusebox : MonoBehaviour
{

    public bool[] switchPositions;
    public bool[] answer;

    public Material light_on;
    public Material light_off;
    public GameObject[] lights;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i < 6; i++)
        {
            UpdateLight(i);
        }
    }

    void CheckInput()
    {
        //Check if code input matches solution
        bool correct = true;
        for (int i=0; i < 6; i++)
        {
            if (switchPositions[i] != answer[i])
            {
                correct = false;
            }
        }
        if (correct)
        {
            //Turn on all lights
            Debug.Log("Code Entered Successfully");
        }
    }

    void UpdateLight(int n)
    {
        if (switchPositions[n])
        {
            //set light to on
            lights[n].GetComponent<Renderer>().material = light_on;
        } else
        {
            //set light to off
            lights[n].GetComponent<Renderer>().material = light_off;
        }
    }

    public void switchLight(int n)
    {
        switchPositions[n] = !switchPositions[n];
        UpdateLight(n);
        CheckInput();
    }
}
