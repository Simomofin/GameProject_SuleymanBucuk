using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperClass
{
    public static class Tags
    {
        public static readonly string playerTag = "Player";
        public static readonly string platformTag = "Platform";
        public static readonly string rotatingPlatformTag = "RotatingPlatform";
        public static readonly string obstacleTag = "Obstacle";
        public static readonly string rotatingStick = "RotatingStick";
        public static readonly string barrier = "Barrier";
        public static readonly string wayPoints = "WayPoints";
        public static readonly string opponent = "Opponent";
        public static readonly string finishLine = "FinishLine";
        public static readonly string mainCamera = "MainCamera";
        public static readonly string fpsCameraHolder = "FPSCamHolder";
        public static readonly string paintableWalls = "PaintableWalls";
        public static readonly string upsideFence = "UpsideFence";
        public static readonly string cameraRotator = "CameraRotator";
        public static readonly string rotatingPlatformEntry = "RotatingPlatformEntry";
        public static readonly string rotatingPlatformExit = "RotatingPlatformExit";

    } // Tags

    public static class AnimationParameters
    {
        public static readonly string animIsRunningBoolean = "IsRunning";
        public static readonly string animIsHitByStick = "IsHitByStick";
        public static readonly string halfDonutTriggerRight = "HalfDonutTrigger_Right";
        public static readonly string halfDonutTriggerLeft = "HalfDonutTrigger_Left";
        public static readonly string winTheGame = "WinTheGame";
        public static readonly string loseTheGame = "LoseTheGame";
    } // AnimationParameters

} // Class

