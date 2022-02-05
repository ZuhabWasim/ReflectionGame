using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCameraPosition : MonoBehaviour
{

    private Camera mirrorCam;
    //private Renderer mirrorRenderer;
    private float mirrorWidth;
    private float mirrorHeight;

    public Transform playerBody;
    public Transform mirrorPlane;
    public Transform mirrorChild;

    private Transform _playerCameraTransform;  // World transfrom of the player camera

    // Start is called before the first frame update
    void Start()
    {
        mirrorCam = mirrorChild.GetComponent<Camera>();
        _playerCameraTransform = playerBody.GetComponentInChildren<Camera>().transform;

        //mirrorRenderer = mirrorPlane.GetComponent<Renderer>();
        mirrorWidth = Mathf.Abs(mirrorPlane.localScale.x) * 10f;
        mirrorHeight = Mathf.Abs(mirrorPlane.localScale.z) * 10f;
    }

    // Update is called once per frame
    void Update()
    {
        //edit position and rotation
        float relativeX = mirrorPlane.position.x - playerBody.position.x;
        float relativeZ = mirrorPlane.position.z - playerBody.position.z;

        float YRotation = mirrorPlane.rotation.eulerAngles.y;
        float a = Mathf.Cos(YRotation * Mathf.Deg2Rad);  // x direction
        float b = Mathf.Sin(YRotation * Mathf.Deg2Rad);  // z direction

        // Reflected variables in global coordinates
        float reflectedX = 0;
        float reflectedZ = 0;
        if (Mathf.Abs(a) < 0.0001) // The mirror is parallel to the z axis
        {
            reflectedX = mirrorPlane.position.x + relativeX;
            reflectedZ = mirrorPlane.position.z - relativeZ;
        }
        else if (Mathf.Abs(b) < 0.0001) // The mirror is parallel to the x axis
        {
            reflectedX = mirrorPlane.position.x - relativeX;
            reflectedZ = mirrorPlane.position.z + relativeZ;
        }
        else // Reflect the point
        {
            // y = mx+b of mirror slope
            float mirrorSlope = -b / a;
            float mirrorIntercept = -mirrorSlope * mirrorPlane.position.x + mirrorPlane.position.z;

            // y = mx+b of inverse slope going through player (will be perpendicular to the above)
            float invSlope = a / b;
            float invIntercept = -invSlope * playerBody.position.x + playerBody.position.z;

            // determine intersection point between the two slopes above
            float intersectionX = (invIntercept - mirrorIntercept) / (mirrorSlope - invSlope);
            float intersectionZ = invSlope * intersectionX + invIntercept;

            // Using midpoint = (x, y) = ((x1 + x2)/2, (y1 + y2)/2) to find reflected point
            reflectedX = 2 * intersectionX - playerBody.position.x;
            reflectedZ = 2 * intersectionZ - playerBody.position.z;
        }

        transform.position = new Vector3(reflectedX, _playerCameraTransform.position.y, reflectedZ);
        mirrorChild.LookAt(mirrorPlane);

        MirrorImp1();
    }

    void MirrorImp1()
    {
        //For some reason this looks okay even though it makes no modifications to the mirror's aspect ratio

        //Using Vertical FOV axis
        //Edit FOV
        float h = Mathf.Abs(transform.position.z - mirrorPlane.position.z);
        float dc = Mathf.Abs(transform.position.x - mirrorPlane.position.x);
        float d = Mathf.Sqrt(h * h + dc * dc);
        mirrorCam.fieldOfView = Mathf.Rad2Deg * 2 * Mathf.Atan(mirrorHeight / (2 * d));
    }

    void MirrorImp2()
    {
        //UNFINISHED
        //This implementation was supposed to use more realistic physics

        //Using Vertical FOV axis
        //edit FOV
        //Using Vertical FOV axis
        //Edit FOV
        float h = Mathf.Abs(transform.position.z - mirrorPlane.position.z);
        float dc = Mathf.Abs(transform.position.x - mirrorPlane.position.x);
        float d = Mathf.Sqrt(h * h + dc * dc);
        mirrorCam.fieldOfView = Mathf.Rad2Deg * 2 * Mathf.Atan(mirrorHeight / (2 * d));

        float hyp = Mathf.Sqrt((h * h) + ((mirrorWidth/2 + dc) * (mirrorWidth / 2 + dc)));
        float h2 = Mathf.Sqrt((h * h) + ((dc - (mirrorWidth/2)) * (dc - (mirrorWidth / 2)))); 

        //edit Aspect Ratio of Mirror Texture
        //(unfinished, need to update mirror texture scale and offset to make it the most accurate resolution)
        float deFactoWidth = mirrorWidth;
        float newRes = Mathf.Min(1f, deFactoWidth / mirrorHeight);
        //mirrorRenderer.material.mainTextureScale = new Vector2(newRes, 1f);
        //mirrorRenderer.material.mainTextureOffset = new Vector2((1-newRes)/2, 1f);
    }

    void MirrorImp3()
    {
        //BAD, OLD implementation
        //Bad bc looking at the mirror close-up from the side results in a tiny FOV and zooms in like crazy
        
        //Using Horizontal FOV axis
        //edit FOV
        float h = Mathf.Abs(transform.position.z - mirrorPlane.position.z);
        float dc = Mathf.Abs(transform.position.x - mirrorPlane.position.x);
        float hyp = Mathf.Sqrt((h * h) + ((mirrorWidth / 2 + dc) * (mirrorWidth / 2 + dc)));
        float h2 = Mathf.Sqrt((h * h) + ((dc - (mirrorWidth / 2)) * (dc - (mirrorWidth / 2))));
        mirrorCam.fieldOfView = Mathf.Rad2Deg * Mathf.Acos(((h2 * h2) + (hyp * hyp) - (mirrorWidth * mirrorWidth)) / (2 * h2 * hyp));
    }


}
