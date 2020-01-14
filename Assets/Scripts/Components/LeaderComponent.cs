using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

public struct Leader : IComponentData { }

public struct LeaderStatus : IComponentData
{
    bool isMoving;
}
public struct ArrivalRadius : IComponentData
{
    float Value;
}
