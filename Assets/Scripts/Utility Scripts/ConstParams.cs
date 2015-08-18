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
    public const string PlayerLayer = "PlayerLayer";
    public const string PhasedLayer = "Phased";
    public const string PlayerUICanvasTag = "Player GUI";
    public const string ObjectPoolManager = "ObjectPoolManager";
    public const string HealthTextObject = "HealthText";
    public const string BoxPrefab = "BoxPrefab";
    
    public const float SlowedMoveSpeed = 1;
    public const float BaseMoveSpeed = 20;
    public const float BreakDrag = 2f;
    public const float BaseDrag = 0.35f;

    public const int MaxEffectCount = 5;

    public const float NetTransformSyncRate = 15; //Messages per second
}

public enum eObjectPoolNames
{
    Bullet = 0,
    PowerupItem,
    LandMine,
}

