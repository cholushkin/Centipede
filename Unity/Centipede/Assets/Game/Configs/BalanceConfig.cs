﻿using UnityEngine;

[CreateAssetMenu(fileName = "BalanceConfig_", menuName = "Create Balance Config", order = 1)]
public class BalanceConfig : ScriptableObject
{
    [Header("----- Map parameters")]
    // ------------------------------------------------------
    public Vector2Int GridSize;
    // todo: mushroom propagation density (curve)

    [Range(0.05f, 0.85f)]
    public float ActiveAreaOffsetPercent;

    [Range(10, 1000)]
    public int MushroomsAmount;

    [Range(1, 10)]
    [Tooltip("One laser hit = one hit point")]
    public int MushroomsHitpoints;

    [Header("----- Player parameters")]
    // ------------------------------------------------------
    [Range(1, 10)]
    public float PlayerSpeed;

    [Range(1, 10)]
    public float PlayerBulletSpeed;

    [Header("----- Centipede parameters")]
    // ------------------------------------------------------
    [Range(1, 100)]
    public int CentipedeSegmentsAmount;

    [Range(0.01f, 1f)]
    public float CentipedeSpeed;

    [Header("----- Spider parameters")]
    // ------------------------------------------------------
    public float SpiderSpeed;
    public float NextSpiderTimeout;
}
