using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    //记录石头的状态
    public enum RockState {HitPlayer, HitEnemy,HitNothing,HitDefend};

    private Rigidbody rb;

    [Header("Basic Settings")]
    public float force;

    public int damage;

    public GameObject breakEffect;

    [HideInInspector]
    public GameObject target;

    private Vector3 direction;

    [HideInInspector]
    public RockState rockState;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockState = RockState.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        //sqrMagnitude将向量换为距离
        if(rb.velocity.sqrMagnitude < 0.1f)
        {
            rockState = RockState.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        //当玩家刚好脱战但石头还没飞出来时，就直接找到玩家
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;


        Vector3 attackDistance = target.transform.position - transform.position;

        float tmp = attackDistance.sqrMagnitude;

        direction = (attackDistance).normalized; //+ Vector3.up * tmp * 0.02f

        //除了朝向玩家的方向，先让方向朝上一点所以加一个vector3.up,为（0,0,1）
        rb.AddForce(direction * force, ForceMode.Impulse);
        //给刚体一个力，力的类型为Impule就是突发的力

    }

    //Unity 自带函数，帮助我们判断碰撞体是什么，然后做出对应的反应
    private void OnCollisionEnter(Collision other)
    {
        GameObject tmpTarget = other.gameObject;
        switch ( rockState)
        {
            case RockState.HitPlayer:
                if (tmpTarget.CompareTag("Player"))
                {
                    if (!tmpTarget.GetComponent<CharacterStats>().isDefend) //没在防御状态就击中
                    {
                        float outDistance = tmpTarget.GetComponent<NavMeshAgent>().stoppingDistance;

                        tmpTarget.GetComponent<NavMeshAgent>().destination = tmpTarget.GetComponent<Transform>().position
                                                         + new Vector3(outDistance + 0.1f, outDistance + 0.1f, outDistance + 0.1f);
                        tmpTarget.GetComponent<NavMeshAgent>().isStopped = true;
                        tmpTarget.GetComponent<NavMeshAgent>().velocity = direction * force * 0.5f;
                        tmpTarget.GetComponent<Animator>().SetTrigger("Dizzy");
                        tmpTarget.GetComponent<CharacterStats>().TakeDamage(damage, tmpTarget.GetComponent<CharacterStats>());

                        Instantiate(breakEffect, transform.position, Quaternion.identity);
                        Destroy(gameObject);
                    }
                    else //否则直接使其失效
                    {
                       
                        rb.AddForce(- (direction - Vector3.down* 0.5f) * force, ForceMode.Impulse);
                        rockState = RockState.HitDefend;

                    }
                    
                }
                else if(tmpTarget.CompareTag("Ground"))
                {
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockState.HitEnemy:
                if (tmpTarget.GetComponent<Golem>()) //getComponent返回的是bool值是否存在
                {
                    tmpTarget.GetComponent<CharacterStats>().TakeDamage(damage, tmpTarget.GetComponent<CharacterStats>());
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockState.HitNothing:
                break;
            case RockState.HitDefend:
                if (tmpTarget.CompareTag("Ground"))
                    rockState = RockState.HitNothing;
                if (tmpTarget.GetComponent<Golem>()) //getComponent返回的是bool值是否存在
                {
                    tmpTarget.GetComponent<CharacterStats>().TakeDamage(damage, tmpTarget.GetComponent<CharacterStats>());
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
