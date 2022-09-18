using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //要获得Agent的控制权

[RequireComponent(typeof(NavMeshAgent))]// 当任务身上没有该组件时自动添加组件
[RequireComponent(typeof(CharacterStats))]


public class EnemyController : MonoBehaviour,IEndGameObserver
{
    [HideInInspector]//状态机变量
    EnemyBaseState currentState;

    [HideInInspector]//存储数据的变量
    public CharacterStats characterStats;

    [HideInInspector]//获取Enemy的Agent
    public NavMeshAgent agent;

    [HideInInspector]//获取挂载的animator controller
    public Animator anim;

    //Agent的视野范围
    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    public float speed; //记录速度
    public float lookAtTime; //记录在巡逻状态时在边界点查看多久

    [Header("Patrol States")]
    public float patrolRange; //设置敌人自由巡逻的范围

    [HideInInspector] //获取攻击目标
    public GameObject attackTarget; 
    [HideInInspector]//获取敌人的初始坐标 
    public Vector3 OrgPos;
    [HideInInspector]//获取敌人的初始朝向角度 
    public Quaternion OrgRotation;
    [HideInInspector] // 布尔值判断控制动画状态转换
    public bool isFollow, isWalk, isChase, isDead;
    [HideInInspector] // 攻击间隔
    public float lastAttackTime;

    bool playerDead;


    //状态机变量
    public PatrolState patrolState = new PatrolState();
    public ChaseState chaseState = new ChaseState();
    public GuardState guardState = new GuardState();
    public AttackState attackState = new AttackState();
    public DeathState deathState = new DeathState();

    void Awake()
    {
        //自由组件的赋值通常在Awake里面
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        speed = agent.speed;
        OrgPos = transform.position;
        OrgRotation = transform.rotation;
        lastAttackTime = 0;
        playerDead = false;
        characterStats.isDefend = false;
    }

    private void Start()
    {  
        if(isGuard) //站桩敌人进入守卫状态
        {
            SwitchStates(guardState);
        }
        else
        {
            //巡逻敌人进入巡逻状态
            SwitchStates(patrolState);
        }

        //FIXME: 在之后场景加载时放到OnEnable()里面
        GameManager.Instance.AddObserver(this);
    }

    //void onenable()
    //{
    //    //把观察对象加入列表
    //    gamemanager.instance.addobserver(this);
    //}

    void OnDisable() //OnDisable在销毁完成之后才调用
    {
        //如果GameManager没有被初始化，就直接return
        if (!GameManager.IsInitialized) return;
        //销毁时移除
        GameManager.Instance.RemoveObserver(this);
    }

    void Update()
    {
        if (characterStats.CurrentHealth == 0)
        {
            SwitchStates(deathState);
        }
        
        if (!playerDead)
        {
            currentState.OnUpdate(this); //在每一帧更新该模式下的OnUpdate函数
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    void SwitchAnimation() //在每一帧都将脚本中的变量赋给动画器里面的parameters
    {
        anim.SetBool("Death", isDead);
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Critical", characterStats.isCritical);
    }

    /// <summary>
    /// 状态转移
    /// </summary>
    /// <param name="state"></param>
    public void SwitchStates(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    /// <param name="position"></param>
    public void MoveToTarget(Vector3 position)
    {
        agent.destination = position;
    }
   
    /// <summary>
    /// 敌人寻找到玩家
    /// </summary>
    /// <returns></returns>
    public bool FoundPlayer()
    {
        //获取到所有与该敌人的视角重叠的collider
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        //对于每一个collider，如果是玩家，就将AttackTarget换成它
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    /// <summary>
    /// 判断是否在攻击范围或技能范围之内
    /// </summary>
    /// <returns></returns>
    public bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.AttackRange;
        else 
            return false;
    }

    public bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
           // Debug.Log(Vector3.Distance(attackTarget.transform.position, transform.position));
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.SkillRange;
        }    
        else
            return false;
    }

    /// <summary>
    /// 显示Gizmo方便调整
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        //在Scene中显示设定的Gizmos
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    /// <summary>
    /// 攻击的Animation Event
    /// </summary>
    void Hit()
    {
        //因为敌人攻击是被动攻击，所以当它攻击时可能玩家已经抛开，获取不到attackTarget，此时会报错，所以要先进行判断
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            CharacterStats targetstats = attackTarget.GetComponent<CharacterStats>();
            targetstats.TakeDamage(characterStats, targetstats);
        }

    }

    /// <summary>
    /// 这是敌人收到游戏结束通知时会执行的代码，当玩家死亡时，游戏结束
    /// </summary>
    public void EndNotify()
    {
        //获胜动作
        
        anim.SetBool("Win", true);
        isWalk = false;
        isFollow = false;
        isChase = false;
        attackTarget = null;
        playerDead = true;
        //停止移动
        //停止Agent
    }
}
