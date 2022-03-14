using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
public class Pin : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Throwable throwable;
    [SerializeField]
    SteamVR_Action_Boolean stvr_press;
    bool toggle = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (throwable.interactable.attachedToHand != null)
        {
            if (stvr_press.stateDown && toggle==false)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX| RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                toggle = true;
            }
            else if(stvr_press.stateDown && toggle == true)
            {
                rigidbody.constraints =  RigidbodyConstraints.None;
                toggle = false;
            }
        }
    }
}
