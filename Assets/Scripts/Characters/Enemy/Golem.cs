using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 15;

    public GameObject rockPrefab; //获得石头的预制体
    public Transform handPos; //获得手部的坐标

    /// <summary>
    /// Animation Event KickOff
    /// </summary>
    public void KickOff()
    {
       
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            CharacterStats targetstats = attackTarget.GetComponent<CharacterStats>();
            targetstats.TakeDamage(characterStats, targetstats);
        }
        
        
        if(attackTarget!=null)
        {
            CharacterStats targetstats = attackTarget.GetComponent<CharacterStats>();
            //转向玩家
            transform.LookAt(attackTarget.transform);

            //从新设置一个在停止距离以外的点为目标点让agent动起来
            float outDistance = attackTarget.GetComponent<NavMeshAgent>().stoppingDistance;
            attackTarget.GetComponent<NavMeshAgent>().destination = attackTarget.GetComponent<Transform>().position 
                                                                   + new Vector3(outDistance + 0.1f, outDistance + 0.1f, outDistance + 0.1f);
            if (!targetstats.isDefend)
            {
                Vector3 dir = attackTarget.transform.position - transform.position;
                dir.Normalize();

                attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
                attackTarget.GetComponent<NavMeshAgent>().velocity = dir * kickForce;
                attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
            }
            else
            {
                Vector3 dir = attackTarget.transform.position - transform.position;
                dir.Normalize();
                attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
                attackTarget.GetComponent<NavMeshAgent>().velocity = dir * kickForce * 0.3f;
                attackTarget.transform.LookAt(transform);
            }
        }
    }

    /// <summary>
    /// Animation Event Throw Rock
    /// </summary>
    public void ThrowRock()
    {
        if(attackTarget!=null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, rockPrefab.transform.rotation);
            //将石头生成出来，生成的是石头预制体，位置是右手心的坐标，不需要旋转
            rock.GetComponent<Rock>().target = attackTarget;
            //将小石头的攻击对象设置为当前的攻击对象
        }
    }
}
