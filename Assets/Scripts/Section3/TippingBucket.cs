using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TippingBucket : InterpolateInteractableWrapper
{
    private const float SMALL_BUCKET_DISTANCE = 0.6f;
    private const float SMALL_BUCKET_ANGLE = 50.0f;

    private bool toppled = false;

    void Update()
    {
        if (!toppled)
        {
            Vector3 playerPosition = GameObject.FindGameObjectWithTag(Globals.Tags.PLAYER).transform.position;
            float distance = Vector3.Distance(playerPosition, this.transform.position);
            Vector3 direction = (this.transform.position - playerPosition).normalized;
            /*
            Debug.Log("playerPosition: " + playerPosition + 
                      ",      distance: " + distance + 
                      ",      direction: " + direction + 
                      ",      angle: " + Vector3.Angle(direction, Vector3.down) + " " + (distance <= SMALL_BUCKET_DISTANCE) + " " + (Vector3.Angle(direction, Vector3.down) <= SMALL_BUCKET_ANGLE));
                      */

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
            
        // Fire event to enable ground mirror.
    }
}
