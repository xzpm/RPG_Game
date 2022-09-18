using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    public event Action<int, int> UpdateHealthBarOnAttack;

    public CharacterData_SO templateData;

    public CharacterData_SO characterData;

    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;
    [HideInInspector]
    public bool isDefend;

    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    #region Read from Data_SO
    public int MaxHealth 
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0;}
        set { characterData.maxHealth = value; }
        //也可以这么写
        //get => characterData = null ? 0 : characterData.maxHealth;
        //set => characterData.maxHealth = vcalue;
    } //可读，可写

    public int CurrentHealth
    {
        get => characterData == null ? 0 : characterData.currentHealth;
        set => characterData.currentHealth = value;
    }

    public int BaseDefence
    {
        get => characterData == null ? 0 : characterData.baseDefence;
        set => characterData.baseDefence  = value;
    }

    public int CurrentDefence
    {
        get => characterData == null ? 0 : characterData.currentDefence;
        set => characterData.currentDefence = value;
    }

    #endregion

    #region Read from AttackData_SO
    public float AttackRange
    {
        get => attackData == null ? 0 : attackData.attackRange;
        set => attackData.attackRange = value;
    }
    public float SkillRange
    {
        get => attackData == null ? 0 : attackData.skillRange;
        set => attackData.skillRange = value;
    }
    public float CoolDown
    {
        get => attackData == null ? 0 : attackData.coolDown;
        set => attackData.coolDown = value;
    }
    public int MinDamage
    {
        get => attackData == null ? 0 : attackData.maxDamage;
        set => attackData.minDamage = value;
    }
    public int MaxDamage
    {
        get => attackData == null ? 0 : attackData.maxDamage;
        set => attackData.maxDamage = value;
    }
    public float CriticalMultiplier
    {
        get => attackData == null ? 0 : attackData.criticalMultiplier;
        set => attackData.criticalMultiplier = value;
    }
    public float CriticalChance
    {
        get => attackData == null ? 0 : attackData.criticalChance;
        set => attackData.criticalChance = value;
    }
    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker,CharacterStats defender)
    {
        if (!defender.isDefend)
        {
            int damage = Mathf.Max(0, attacker.CurrentDamage() - defender.CurrentDefence);
            //要考虑到防御比攻击还要大的情况，会变成加血
            defender.CurrentHealth = Mathf.Max(0, defender.CurrentHealth - damage);

            if (attacker.isCritical) //如果暴击则防守方播放受伤动画,这样就不用在每个Controller里面都设置了
                defender.GetComponent<Animator>().SetTrigger("Hit");


            defender.UpdateHealthBarOnAttack?.Invoke(defender.CurrentHealth, defender.MaxHealth); //更新血条

            if (defender.CurrentHealth <= 0)
            {
                //把受攻击的人的killpoint加到攻击者的data中
                attacker.characterData.UpdateExp(defender.characterData.killPoint);
            }
        }
        else //盾反
        {
            int damage = Mathf.Max(0, attacker.CurrentDamage() - attacker.CurrentDefence);
            damage = (int)(damage * 0.5f); //返回百分之五十的伤害

            //攻击者受到伤害
            attacker.CurrentHealth = Mathf.Max(0, attacker.CurrentHealth - damage);

            attacker.GetComponent<Animator>().SetTrigger("Hit");


            attacker.UpdateHealthBarOnAttack?.Invoke(attacker.CurrentHealth, attacker.MaxHealth); //更新血条

            if (attacker.CurrentHealth <= 0)
            {
                defender.characterData.UpdateExp(attacker.characterData.killPoint);
            }
        }
    }

    //对方法进行重载实现石头攻击
    public void TakeDamage(int damage,CharacterStats defender)
    {
        //要考虑到防御比攻击还要大的情况，会变成加血
        int curDamage = Mathf.Max(0, damage - defender.CurrentDefence);
        defender.CurrentHealth = Mathf.Max(0, defender.CurrentHealth - curDamage);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth); //更新血条

        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        coreDamage *= (1 + 0.2f * characterData.currentLevel); //提升一级攻击力增加20%

        coreDamage = isCritical? coreDamage * CriticalMultiplier:coreDamage;

       // Debug.Log("伤害" + coreDamage);

        return (int)coreDamage;
    }

    #endregion
}
