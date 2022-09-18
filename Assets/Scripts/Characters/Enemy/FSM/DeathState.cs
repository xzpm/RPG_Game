using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.isDead = true;
    }

    public override void OnUpdate(EnemyController enemy)
    {
        enemy.GetComponent<Collider>().enabled = false;
        //enemy.agent.enabled = false; //将agent关闭,会导致其他动画状态转到这个状态时因为又访问了一次agent导致报错
        enemy.agent.radius = 0; //将半径设置为0
        Object.Destroy(enemy.gameObject, 2f); //销毁敌人的物体，延迟两秒
    }
}
