using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCheck : MonoBehaviour
{
    bool withinArea = false;
    public ImpactSound controller;
    [SerializeField]
    BoxCollider box;

    private void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Physics.IgnoreCollision(other,box);
            controller.withinArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            controller.withinArea = false;
        }

    }

}
