using System;
using UnityEngine;

[Serializable]
public class QTEActionData
{
    public int id;
    public string name;
    public int[] values;
    public HumanBone humanBone;

    public void Start()
    {
        var spineFrontBack = BodyDof.SpineFrontBack;
        var spineLeftRight = BodyDof.SpineLeftRight;
        var spineRollLeftRight = BodyDof.SpineRollLeftRight;
        //
        var chestFrontBack = BodyDof.ChestFrontBack;
        var chestLeftRight = BodyDof.ChestLeftRight;
        var chestRollLeftRight = BodyDof.ChestRollLeftRight;
        //
        var upperChestFrontBack = BodyDof.UpperChestFrontBack;
        var upperChestLeftRight = BodyDof.UpperChestLeftRight;
        var upperChestRollLeftRight = BodyDof.UpperChestRollLeftRight;
        //
        //
        var neckFrontBack = HeadDof.NeckFrontBack;
        var neckLeftRight = HeadDof.NeckLeftRight;
        var neckRollLeftRight = HeadDof.NeckRollLeftRight;
        //
        var headFrontBack = HeadDof.HeadFrontBack;
        var headLeftRight = HeadDof.HeadLeftRight;
        var headRollLeftRight = HeadDof.HeadRollLeftRight;
        //
        var leftEyeDownUp = HeadDof.LeftEyeDownUp;
        var leftEyeInOut = HeadDof.LeftEyeInOut;
        //
        var rightEyeDownUp = HeadDof.RightEyeDownUp;
        var rightEyeInOut = HeadDof.RightEyeInOut;
        //
        var jawDownUp = HeadDof.JawDownUp;
        var jawLeftRight = HeadDof.JawLeftRight;
        //
        //
        var leftUpperLegFrontBack = LegDof.UpperLegFrontBack;
        var leftUpperLegInOut = LegDof.UpperLegInOut;
        var leftUpperLegRollInOut = LegDof.UpperLegRollInOut;
        //
        var leftLegCloseOpen = LegDof.LegCloseOpen;
        var leftLegRollInOut = LegDof.LegRollInOut;
        //
        var leftFootCloseOpen = LegDof.FootCloseOpen;
        var leftFootOnOut = LegDof.FootInOut;
        //
        var leftToesUpDown = LegDof.ToesUpDown;
        //
        //
        var rightUpperLegFrontBack = LegDof.UpperLegFrontBack;
        var rightUpperLegInOut = LegDof.UpperLegInOut;
        var rightUpperLegRollInOut = LegDof.UpperLegRollInOut;
        //
        var rightLegCloseOpen = LegDof.LegCloseOpen;
        var rightLegRollInOut = LegDof.LegRollInOut;
        //
        var rightFootCloseOpen = LegDof.FootCloseOpen;
        var rightFootOnOut = LegDof.FootInOut;
        //
        var rightToesUpDown = LegDof.ToesUpDown;
        //
        //
        var leftShoulderDownUp = ArmDof.ShoulderDownUp;
        var leftShoulderFrontBack = ArmDof.ShoulderFrontBack;
        //
        var leftArmDownUp = ArmDof.ArmDownUp;
        var leftArmFrontBack = ArmDof.ArmFrontBack;
        var leftArmRollInOut = ArmDof.ArmRollInOut;
        //
        var leftForeArmCloseOpen = ArmDof.ForeArmCloseOpen;
        var leftForeArmRollInOut = ArmDof.ForeArmRollInOut;
        //
        var leftHandDownUp = ArmDof.HandDownUp;
        var leftHandInOut = ArmDof.HandInOut;
        //
        //
        var rightShoulderDownUp = ArmDof.ShoulderDownUp;
        var rightShoulderFrontBack = ArmDof.ShoulderFrontBack;
        //
        var rightArmDownUp = ArmDof.ArmDownUp;
        var rightArmFrontBack = ArmDof.ArmFrontBack;
        var rightArmRollInOut = ArmDof.ArmRollInOut;
        //
        var rightForeArmCloseOpen = ArmDof.ForeArmCloseOpen;
        var rightForeArmRollInOut = ArmDof.ForeArmRollInOut;
        //
        var rightHandDownUp = ArmDof.HandDownUp;
        var rightHandInOut = ArmDof.HandInOut;
        //  
        //


    }
}

[Serializable]
public class QTEMotionData
{
    public int parameterId;
    public float startValue;
    public float endValue;
}
