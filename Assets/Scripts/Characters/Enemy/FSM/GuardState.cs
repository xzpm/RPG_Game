using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.isChase = false;
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.transform.position != enemy.OrgPos) //并不在初始原点的话
        {
            enemy.isWalk = true;
            enemy.agent.isStopped = false;
            enemy.agent.destination = enemy.OrgPos;

            if (Vector3.SqrMagnitude(enemy.OrgPos - enemy.transform.position) <= enemy.agent.stoppingDistance) // 已归位
            {
                enemy.isWalk = false;
                enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation, enemy.OrgRotation, 0.01f);
            }
                
        }

        //如果找到玩家，就切换到追击状态
        if (enemy.FoundPlayer())
        {
            //Debug.Log("found Player");
            enemy.SwitchStates(enemy.chaseState);
        }
    }
}
