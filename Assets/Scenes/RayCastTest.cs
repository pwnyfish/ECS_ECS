using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    private RaycastHit vision;
    public float rayLength;
    private bool isGrabbed;
    void Start()
    {
        rayLength = 4.0f;
        isGrabbed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * rayLength, Color.red, 1f);

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out vision, rayLength))
            {
                Debug.Log(vision.point);
                Debug.Log(vision.collider.name);
            }
        }
    }
}
