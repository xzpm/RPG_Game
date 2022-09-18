using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    //获取组件
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    //攻击目标需要的变量
    private GameObject attackTarget;
    private float lastAttackTime; // 记录上一次攻击的时间，用于设置技能冷却
    private float stopDistance;
    private float orgSpeed;

    //被攻击
    private bool isDead;

    private bool isGuard;




    private void Awake() 
    {
        //自身的components在awake的时候调用，因为Awake会在游戏最开始的时候优先获得这些数值，
        //以免出现空引用的情况
        agent = GetComponent<NavMeshAgent>(); //agent就是NavMeshAgent的组件
        anim = GetComponent<Animator>(); //获取到Animator组件
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
        orgSpeed = agent.speed;


    }
    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget; //使用+=将MoveToTarget这一函数注册进OnMouseClicked事件中
        MouseManager.Instance.OnEnemyClicked += EventAttack; //将EventAttack函数注册到OnEnemyClicked事件下
        GameManager.Instance.RegisterPlayer(characterStats); //将玩家的数据传进去
    }
    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }
    private void OnDisable()
    {
        //如果MouseManager还没有实例化，就不执行
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }


    private void Update()
    {
        isGuard = Input.GetButton("Fire2");
        characterStats.isDefend = isGuard;
        isDead = characterStats.CurrentHealth == 0;
        if (isDead) //玩家死了就进行广播
            GameManager.Instance.NotifyObservers();

        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);//将animator中的peremeter “Speed”的数值设置为agent的速度，velocity通过使用sqrmagnitude转换为float类型的数值
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
        anim.SetBool("Defend", isGuard);
    }

    public void MoveToTarget(Vector3 target)
    {
        if (isDead) return;

        agent.stoppingDistance = stopDistance; //move to target的时候改回原来的停止距离
        agent.speed = orgSpeed;

        StopAllCoroutines(); //打断所有携程
        agent.isStopped = false; //下一次点击地面的时候启动agent
        agent.destination = target;//修改组件的身体那通的值为target  
           
    }

    //检测用
    //private void EventPushOff(GameObject target)
    //{
    //    //从新设置一个在停止距离以外的点为目标点让agent动起来
    //    agent.destination = transform.position + new Vector3(1, 1, 1);
    //    agent.isStopped = false; 
    //    Vector3 dir = new Vector3(1, 1, 1).normalized;
    //    agent.velocity = dir * 15;
    //    anim.SetTrigger("Dizzy");
    //}

    private void EventAttack(GameObject target) //一个事件调用的函数方法
    {
        if (isDead) return;

        //执行攻击之前先计算是否是暴击
        characterStats.isCritical = UnityEngine.Random.value < characterStats.CriticalChance;
        if (target!=null)//当敌人死亡时会失去目标，要进行判断
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget()); //开启移动到目标敌人的携程
        }
    }

    IEnumerator MoveToAttackTarget() //定义一个协程
    {

        agent.isStopped = false;//player动起来
        agent.speed = 1.5f * orgSpeed;

        transform.LookAt(attackTarget.transform); //transform内置的函数方法，可以让物体转向目标
        agent.stoppingDistance = characterStats.AttackRange; //将StoppingDistance设置大于敌人的Radius
        

        //当敌人位置和player当前位置的距离大于攻击距离时，就移动到敌人位置
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.AttackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null; //在下一帧再次执行下面的命令      
        }

        agent.isStopped = true;//player停下来

        //attack
        if(lastAttackTime < 0)//cd冷却结束 进行下一次攻击
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.CoolDown;
        }
    }

    //Animation event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            //如果可攻击对象是石头，就将石头的状态设置为打击敌人
            if(attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockState == Rock.RockState.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockState = Rock.RockState.HitEnemy;

                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;

                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);

            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }


}
