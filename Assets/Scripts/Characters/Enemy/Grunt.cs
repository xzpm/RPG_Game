using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;

    public void KickOff()
    {
        if (attackTarget!=null)
        {
            transform.LookAt(attackTarget.transform);

            //从新设置一个在停止距离以外的点为目标点让agent动起来
            float outDistance = attackTarget.GetComponent<NavMeshAgent>().stoppingDistance;
            attackTarget.GetComponent<NavMeshAgent>().destination = attackTarget.GetComponent<Transform>().position 
                                                                  + new Vector3(outDistance+0.1f, outDistance + 0.1f, outDistance + 0.1f);

            //获得朝向敌人的方向向量
            Vector3 dir = (attackTarget.transform.position - transform.position).normalized;
            //停止攻击对象的agent的运行
            attackTarget.GetComponent<NavMeshAgent>().isStopped = false;
            //通过获得一个沿着指定方向的力进行击飞

            attackTarget.GetComponent<NavMeshAgent>().velocity = dir * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
            
        }
    }
}
