using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TippingBucket : InterpolateInteractableWrapper
{
    private const float SMALL_BUCKET_DISTANCE = 0.6f;
    private const float SMALL_BUCKET_ANGLE = 50.0f;

    private bool toppled = false;

    public AudioClip tippingVoiceLine;

    public GameObject mirrorPast;
    public GameObject mirrorPresent;

    public GameObject booksOnBustCase;
    
    void Update()
    {
        if (!toppled)
        {
            Vector3 playerPosition = GameObject.FindGameObjectWithTag(Globals.Tags.PLAYER).transform.position;
            float distance = Vector3.Distance(playerPosition, this.transform.position);
            Vector3 direction = (this.transform.position - playerPosition).normalized;

            if (distance <= SMALL_BUCKET_DISTANCE && Vector3.Angle(direction, Vector3.down) <= SMALL_BUCKET_ANGLE)
            {
                Topple();
            } 
        }
    }
    
    void Topple()
    {
        InterpolateTransform it = GetComponent<InterpolateTransform>();
        it.TriggerMotion();

        interactable = false;
        toppled = true;

        InterpolateTransform itMirrorPast = mirrorPast.GetComponent<InterpolateTransform>();
        itMirrorPast.TriggerMotion();
        //mirrorPast.SetActive(true);
        
        InterpolateTransform itMirrorPresent = mirrorPresent.GetComponent<InterpolateTransform>();
        itMirrorPresent.TriggerMotion();

        booksOnBustCase.GetComponent<InspectableObject>().interactable = false;
        
        AudioPlayer.Play(tippingVoiceLine, Globals.Tags.DIALOGUE_SOURCE);
        AudioPlayer.Play(Globals.AudioFiles.Section3.PAINT_CAN_SPILL_REFLECTION_ONESHOT, Globals.Tags.MAIN_SOURCE);
        
        // Fire event to enable ground mirror.
    }
}
