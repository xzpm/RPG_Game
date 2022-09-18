using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.isFollow = false;

    }

    public override void OnUpdate(EnemyController enemy)
    {
        enemy.agent.isStopped = true;
        if (enemy.FoundPlayer())
        {
            if (enemy.lastAttackTime < 0)
            {
                enemy.lastAttackTime = enemy.characterStats.CoolDown;

                enemy.characterStats.isCritical = Random.value < enemy.characterStats.CriticalChance;
                //执行攻击
                if (enemy.attackTarget != null)
                    Attack(enemy);
            }

            if (!enemy.TargetInAttackRange() && !enemy.TargetInSkillRange()) //不在攻击范围就回到追击状态
            {

                enemy.SwitchStates(enemy.chaseState);
            }
        }
        else
        {
            if (enemy.isGuard)
                enemy.SwitchStates(enemy.guardState);
            else
                enemy.SwitchStates(enemy.patrolState);
        }
    }

    void Attack(EnemyController enemy)
    {
        //面向方向转向攻击目标
        enemy.transform.LookAt(enemy.attackTarget.transform);

        if (enemy.TargetInAttackRange())//近战攻击
        {
            enemy.anim.SetTrigger("Attack");
        }

        if (enemy.TargetInSkillRange())//技能攻击
        {
            enemy.anim.SetTrigger("Skill");
        }

        

        
    }
}
