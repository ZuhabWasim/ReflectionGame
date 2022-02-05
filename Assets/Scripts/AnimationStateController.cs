using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Input = UnityEngine.Windows.Input;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;

   // private int isWalkingHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool forwardPress = Input.GetKey("w");
        
        Debug.Log(isWalking + "," + forwardPress);
        if (!isWalking && forwardPress)
        {
            animator.SetBool("isWalking", true);
        }
        
        if (isWalking && !forwardPress)
        {
            animator.SetBool("isWalking", false);
        }
    }
}
