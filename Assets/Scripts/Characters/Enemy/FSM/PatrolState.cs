using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyBaseState
{
    Vector3 wayPoint; //记录Patrol状态时随机到的移动目标点
    Vector3 OrgPos;

    float patrolRange; //敌人巡逻的范围
    private float remainLookAtTime;
    public override void EnterState(EnemyController enemy)
    { 
        GetNewWayPoint(enemy); //一开始给一个巡逻的随机目标点
        enemy.agent.speed = enemy.speed * 0.5f; //将agent的速度设置为原本速度的一半 Unity乘法开销小
        patrolRange = enemy.patrolRange; //获取到巡逻的半径
        enemy.isChase = false; //没有进行追击 设置追击为false
        OrgPos = enemy.OrgPos;
        remainLookAtTime = enemy.lookAtTime;
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if(Vector3.Distance(wayPoint, enemy.transform.position) <= enemy.agent.stoppingDistance)
        {
            //如果人物距离目标点已经在stoppingdistancce之内，就从新找到新的点
            enemy.isWalk = false;
            if (remainLookAtTime > 0) //停下来巡逻一下
                remainLookAtTime -= Time.deltaTime;
            else
                GetNewWayPoint(enemy);
        }
        else
        {
            //如果还没到，那么敌人就会向随机点移动，同时播放walk的动画
           // Debug.Log(wayPoint);
            enemy.isWalk = true;
            enemy.agent.destination = wayPoint;
        }


        //如果找到玩家，就切换到追击状态
        if (enemy.FoundPlayer())
        {
            //Debug.Log("found Player");
            enemy.SwitchStates(enemy.chaseState);
        }
    }



    /// <summary>
    /// 获取敌人下一次自由移动的目标点
    /// </summary>
    void GetNewWayPoint(EnemyController enemy)
    {
        remainLookAtTime = enemy.lookAtTime;
        //保持y轴不变，在规定范围内得到一个随机的点
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(OrgPos.x + randomX, enemy.transform.position.y,OrgPos.z + randomZ);


        //wayPoint = randomPoint;
        //如果直接这样获取到移动的目标点。可能会出现目标点选在non walkable的区域内，这样就永远移动不到那里，所以我们要避开这样的点
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)? hit.position : enemy.transform.position;

    }
}
