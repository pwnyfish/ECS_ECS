using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Physics;

public class UnitHandlerSystem : ComponentSystem
{
    float distance = 3;
    public GameObject targetObject;
    private float slowingDistance = 10;
    float moveSpeed = 10;
    Vector3 tempTarget = new Vector3(0, 0, 0);
    protected override void OnUpdate()
    {

        if (targetObject == null)
        {
            targetObject = GameObject.FindGameObjectWithTag("Target");
        }
        tempTarget = targetObject.transform.position;
        float3 sepVelocity = new float3(0, 0, 0);
        float3 alignVelocity = new float3(0, 0, 0);
        float3 arriveVelocity = new float3(0, 0, 0);
        float3 velocitySum = new float3(0, 0, 0);

        Entities.WithAll<Skeleton>().ForEach((Entity entity, ref Translation skeletonTranslation, ref PhysicsVelocity velocity, ref Rotation rotation) =>
        {
            //Code to run on all entities with the "Skeleton" tag
            NativeList<Entity> closestTargetsEntityList = new NativeList<Entity>(Allocator.Temp);
            NativeList<float3> closestTargetsPositionList = new NativeList<float3>(Allocator.Temp);
            NativeList<Vector3> closestTargetsVelocityList = new NativeList<Vector3>(Allocator.Temp);
            float3 skeletonPosition = skeletonTranslation.Value;
            float3 closestTargetPosition = float3.zero;


            //Cycle through all other skeletons units to find the ones in neighbour distance
            Entities.WithAll<Skeleton>().ForEach((Entity targetEntity, ref Translation targetTranslation, ref PhysicsVelocity targetVelocity) =>
            {
                if (entity != null)
                {
                    if (entity != targetEntity)
                    {
                        if (math.lengthsq(targetTranslation.Value - skeletonPosition) < (distance * distance))
                        {
                            closestTargetsEntityList.Add(targetEntity);
                            closestTargetsPositionList.Add(targetTranslation.Value);
                            closestTargetsVelocityList.Add(targetVelocity.Angular);
                        }
                    }
                }
            });
            sepVelocity = PerformSeparationBehavior(closestTargetsEntityList, closestTargetsPositionList, skeletonPosition, moveSpeed, velocity.Linear);
            arriveVelocity = PerformArrivalBehavior(tempTarget, skeletonPosition, moveSpeed, velocity.Linear);
            velocitySum = (sepVelocity + arriveVelocity + alignVelocity) * new float3(1, 0, 1);

            if (!(velocitySum.Equals(float3.zero)))
            {
                velocity.Linear += math.normalize(velocitySum);
            }

            closestTargetsEntityList.Dispose();
            closestTargetsPositionList.Dispose();
            closestTargetsVelocityList.Dispose();
        });
    }

    public Vector3 PerformSeparationBehavior(NativeList<Entity> closestTargetsEntityList, NativeList<float3> closestTargetsPositionList, float3 skeletonPosition, float speed, Vector3 velocity)
    {
        Vector3 desiredSepVelocity = Vector3.zero;

        //Calc separation force
        Vector3 separationForce = Vector3.zero;
        Vector3 steeringForce;
        Vector3 sepForce = Vector3.zero;
        if (closestTargetsEntityList.Length != 0)
        {

            for (int i = 0; i < closestTargetsEntityList.Length; i++)
            {
                sepForce = skeletonPosition - closestTargetsPositionList[i];
                sepForce *= 1 - Mathf.Min(sepForce.sqrMagnitude / (distance * distance), 1);
                separationForce += sepForce;
            }

        }
        else
        {
            return Vector3.zero;
        }

        separationForce /= closestTargetsEntityList.Length;
        desiredSepVelocity = separationForce.normalized * speed;
        steeringForce = desiredSepVelocity - velocity;
        Debug.DrawLine(skeletonPosition, steeringForce, Color.yellow);
        return steeringForce;
    }


    public Vector3 PerformArrivalBehavior(Vector3 targetPosition, Vector3 position, float speed, Vector3 velocity)
    {

        Vector3 desiredArriveVelocity = Vector3.zero;
        if (targetObject == null)
        {
            return Vector3.zero;
        }

        // Calculate stopping factor
        float targetDistance = (targetPosition - position).magnitude;
        float stoppingFactor;

        if (slowingDistance > 0)
        {
            stoppingFactor = Mathf.Clamp(targetDistance / slowingDistance, 0.0f, 1.0f);
        }
        else
        {
            stoppingFactor = Mathf.Clamp(targetDistance, 0.0f, 1.0f);
        }

        desiredArriveVelocity = (targetPosition - position).normalized * speed * stoppingFactor;

        // Calculate steering force
        Vector3 steeringForce = desiredArriveVelocity - velocity;
        return steeringForce;
    }
}
