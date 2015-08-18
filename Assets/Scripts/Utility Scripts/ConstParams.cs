using UnityEngine;
using System.Collections;

public static class ConstParams
{
    public const string HorizontalAxis = "Horizontal";
    public const string VerticalAxis = "Vertical";
    public const string PlayerTag = "Player";
    public const string FloorLayer = "Floor";
    public const string KillBoxLayer = "KillBox";
    public const string JumpButton = "Jump";
    public const string FireButton = "Fire1";
    public const string BreakButton = "Break";
    public const string PlayerLayer = "Players";
    public const string PhasedLayer = "Phased";
    public const string DifaultLayer = "Difault";
    public const string BoxPrefab = "BoxPrefab";

    public const string ObjectPoolManager = "ObjectPoolManager";

    public const float BreakDrag = 2f;
    public const float BaseDrag = 0.35f;
    public const float BaseMoveSpeed = 20f;
    public const float SlowedMoveSpeed = 1f;

    public const int MaxEffectCount = 5;

    public const float NetTransformSyncRate = 25; //Messages per second
}

public enum eObjectPoolNames
{
    Bullet = 0,   
    PowerupItem,
    LandMide,
}

