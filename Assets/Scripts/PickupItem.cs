using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private static int _id;
    private int m_id;
    void Start()
    {
        m_id = _id++;
    }

    public void OnPickup()
    {
        this.gameObject.SetActive( false );
    }

    public void OnDrop( Vector3 dropPostion )
    {
        this.gameObject.SetActive( true );
        this.gameObject.transform.position = dropPostion;
    }
}
