using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    //静态类只能声明静态变量
   private const float dotThreshold = 0.5f;
   public static bool IsFacingTarget(this Transform transform,Transform target)
   {
        //获得敌方的相对位置
        var VectorToTarget = target.position - transform.position;
        VectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, VectorToTarget);

        //点积大于.05就在面前120度扇区内
        return dot > dotThreshold;
   }
}
