using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTracker : MonoBehaviour
{
    public Rigidbody rig;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= 4)
        {
            rig.useGravity = false;
            
        }
        if(transform.position.y >= 9)
        {
            Destroy(this.gameObject);
        }
    }
}
