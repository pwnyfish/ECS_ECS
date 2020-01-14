using System;
using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;

public class SpawnPhysicsBodies : MonoBehaviour
{
    public GameObject prefab;
    public float3 range;
    public int count;
    public GameObject targetObject;
    public float3 com = new float3(0, -1, 0);

    void OnEnable() { }
    public static void RandomPointsOnCircle(float3 center, float3 range, ref NativeArray<float3> positions, ref NativeArray<quaternion> rotations)
    {
        var count = positions.Length;

        // initialize the seed of the random number generator 
        Unity.Mathematics.Random random = new Unity.Mathematics.Random();
        random.InitState(10);
        quaternion qt = new quaternion(0, 1, 0, 0);
        for (int i = 0; i < count; i++)
        {
            positions[i] = (center + random.NextFloat3(-range, range)) * new Vector3(1, 0, 1);
            //rotations[i] = random.NextQuaternionRotation();
            rotations[i] = qt;
        }
    }

    void Start()
    {
        if (!enabled) return;
        GameObject leader = GameObject.FindGameObjectWithTag("Target");
        // Create entity prefab from the game object hierarchy once
        Entity sourceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, World.Active);
        //Entity leaderEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(leader, World.Active);
        var entityManager = World.Active.EntityManager;

        var positions = new NativeArray<float3>(count, Allocator.Temp);
        var rotations = new NativeArray<quaternion>(count, Allocator.Temp);
        RandomPointsOnCircle(transform.position, range, ref positions, ref rotations);
        BlobAssetReference<Collider> sourceCollider = entityManager.GetComponentData<PhysicsCollider>(sourceEntity).Value;
        entityManager.AddComponentData(sourceEntity, new Skeleton { });
        //entityManager.AddComponentData(leaderEntity, new Leader { });
        //entityManager.AddComponentData(leaderEntity, new ArrivalRadius { });
        //entityManager.SetComponentData(sourceEntity, new PhysicsMass { CenterOfMass = com });

        for (int i = 0; i < count; i++)
        {
            var instance = entityManager.Instantiate(sourceEntity);
            entityManager.SetComponentData(instance, new Translation { Value = positions[i] });
            entityManager.SetComponentData(instance, new Rotation { Value = rotations[i] });
            entityManager.SetComponentData(instance, new PhysicsCollider { Value = sourceCollider });
            


        }
        entityManager.DestroyEntity(sourceEntity);
        positions.Dispose();
        rotations.Dispose();
    }
}
