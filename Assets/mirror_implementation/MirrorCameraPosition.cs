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
    public bool mirrorPosition = false; // Marks whether this mirror is in the other world, and must be transformed carefully
    public MirrorCameraPosition mirroredCamera; // The associated mirror with this mirror

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
        if (!mirrorPosition)
        {
            transform.position = this.ReflectOverMirror(playerBody.position.x, playerBody.position.z);
            mirroredCamera.SetMirrorCameraPosition(transform.localPosition);
        }
        mirrorChild.LookAt(mirrorPlane);

        MirrorImp1();
    }

    // Finds reflection of (X1, Z1) in the line with slope m = dZ/dX that goes through point (X2, Z2)
    private static Vector2 RelfectOverLine(float X1, float Z1, float dX, float dZ, float X2, float Z2)
    {
        // Relative positions
        float relativeX = X2 - X1;
        float relativeZ = Z2 - Z1;

        // Reflected variables in global coordinates
        float reflectedX = 0;
        float reflectedZ = 0;
        if (Mathf.Abs(dX) < 0.0001) // The mirror is parallel to the z axis
        {
            reflectedX = X2 + relativeX;
            reflectedZ = Z2 - relativeZ;
        }
        else if (Mathf.Abs(dZ) < 0.0001) // The mirror is parallel to the x axis
        {
            reflectedX = X2 - relativeX;
            reflectedZ = Z2 + relativeZ;
        }
        else // Reflect the point
        {
            // y = mx+b of original slope
            float originalSlope = -dZ / dX;
            float originalIntercept = -originalSlope * X2 + Z2;

            // y = mx+b of inverse slope going through player (will be perpendicular to the above)
            float invSlope = dX / dZ;
            float invIntercept = -invSlope * X1 + Z1;

            // determine intersection point between the two slopes above
            float intersectionX = (invIntercept - originalIntercept) / (originalSlope - invSlope);
            float intersectionZ = invSlope * intersectionX + invIntercept;

            // Using midpoint = (x, y) = ((x1 + x2)/2, (y1 + y2)/2) to find reflected point
            reflectedX = 2 * intersectionX - X1;
            reflectedZ = 2 * intersectionZ - X2;
        }

        return new Vector2(reflectedX, reflectedZ);
    }

    // Reflects a point across the line formed by the mirror
    private Vector3 ReflectOverMirror(float inputX, float inputZ)
    {
        float YRotation = mirrorPlane.rotation.eulerAngles.y;
        float a = Mathf.Cos(YRotation * Mathf.Deg2Rad);  // change in x direction
        float b = Mathf.Sin(YRotation * Mathf.Deg2Rad);  // change in z direction

        Vector2 xz = RelfectOverLine(inputX, inputZ, a, b, mirrorPlane.position.x, mirrorPlane.position.z);
        return new Vector3(xz.x, _playerCameraTransform.position.y, xz.y);
    }

    // Sets the camera position based on the associated camera, using relative coordinates
    public void SetMirrorCameraPosition(Vector3 other)
    {
        transform.localPosition = other;

        float YRotation = mirrorPlane.rotation.eulerAngles.y;
        // This slope needs to be perpendicular to the mirror
        float a = -Mathf.Sin(YRotation * Mathf.Deg2Rad);  // change in x direction
        float b = Mathf.Cos(YRotation * Mathf.Deg2Rad);  // change in z direction

        Vector2 xz = RelfectOverLine(transform.position.x, transform.position.z, a, b, mirrorPlane.position.x, mirrorPlane.position.z);
        transform.position = new Vector3(xz.x, _playerCameraTransform.position.y, xz.y);
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
