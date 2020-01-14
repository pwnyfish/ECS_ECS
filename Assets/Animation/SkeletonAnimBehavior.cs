using System;
using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;

//[UpdateBefore(typeof(UnityEngine.Experimental.PlayerLoop.FixedUpdate))]
public class SkeletonAnimBehavior : MonoBehaviour
{
    private Animator animator;
    //private SteeringCrowdUnit unit;
    private float speed;

    void Start()
    {
        var entityManager = World.Active.EntityManager;
        animator = GetComponentInChildren<Animator>();
        //unit = GetComponent<SteeringCrowdUnit>();
        //entityManager.GetComponentData<PhysicsVelocity>()
    }
    void FixedUpdate()
    {
        Vector3 velocity = new Vector3(1,1,1);
        speed = velocity.magnitude;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //if (Input.GetKeyDown(KeyCode.A))
        //{
            
        //}
        animator.SetFloat("speed", speed);
    }
}
