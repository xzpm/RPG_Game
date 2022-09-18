using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyBaseState
{
    private float remainLookAtTime; // 脱战后停留的时间
    
    public override void EnterState(EnemyController enemy)
    {
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.speed;
        enemy.isWalk = false;
        enemy.isChase = true;
        remainLookAtTime = enemy.lookAtTime;
    }

    public override void OnUpdate(EnemyController enemy)
    {
             
        if (!enemy.FoundPlayer())
        {
            // 玩家跑脱范围就回到其他状态
            //Debug.Log("Escaped!");
            enemy.isFollow = false;
            enemy.agent.destination = enemy.transform.position;

            //先让它在原地呆一下
            if (remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime;
            else if (enemy.isGuard)//如果是守卫的敌人，就回到守卫状态
                enemy.SwitchStates(enemy.guardState);
            else  // 否则回到巡逻状态
                enemy.SwitchStates(enemy.patrolState);
        }
        else
        {
            // 追击敌人
            enemy.agent.destination = enemy.attackTarget.transform.position;
            enemy.isFollow = true;
        }

        if(enemy.TargetInAttackRange() || enemy.TargetInSkillRange()) // 如果追到攻击范围内就进入攻击状态
        {
            enemy.SwitchStates(enemy.attackState);
        }
    }


}
