using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Physics;
using Collider = UnityEngine.Collider;
using RaycastHit = UnityEngine.RaycastHit;
using Ray = UnityEngine.Ray;
using Unity.Entities;

public class LeaderControl : MonoBehaviour
{
    UnitHandlerSystem unitHandlerSystem;
    //Leader Attributes
    private Vector3 targetPosition;
    Vector3 lookAtTarget;
    Quaternion leaderRot;
    [Range(0, 40)]
    public float speed = 30f;
    [Range(0, 20)]
    public float rotSpeed = 10f;

    public bool moving = false;
    //public Collider[] unitsInsideArea;
    //public int unitsInsideAreaCount;
    int layerMask;
    public LayerMask groundLayerMask = 1 << 8;

    public float separationRadius_min = 2;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("w")|| Input.GetKey("a")|| Input.GetKey("s")|| Input.GetKey("d"))
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        if (Input.GetMouseButton(1))
        {
            SetTargetPosition();
        }

        if (moving)
        {
            Move();
        }

    }
    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical);
        gameObject.transform.Translate(direction.normalized * Time.deltaTime * speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, leaderRot, rotSpeed * Time.deltaTime);
        
    }
    public void SetTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        Debug.Log("click mouse");
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, groundLayerMask))
        {
            targetPosition = new Vector3(rayHit.point.x, rayHit.point.y + 0.5f, rayHit.point.z);
            lookAtTarget = new Vector3(targetPosition.x - transform.position.x, transform.position.y, targetPosition.z - transform.position.z);
            leaderRot = Quaternion.LookRotation(lookAtTarget);
            moving = true;
            Debug.Log(rayHit.collider);
        }
    }
}
