using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

public enum UnitStatus
{
    Idle,
    Moving,
    Arrived,
}
public struct Skeleton : IComponentData {
    public UnitStatus unitStatus;
}
public struct Target : IComponentData {
    Vector3 position;
}
public struct UnitSelected : IComponentData { }