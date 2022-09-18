using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data", menuName ="Character States/Data")]

public class CharacterData_SO : ScriptableObject
{
    [Header("States Info")] //可以发展想象力添加所有属性
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
    public float defendRange;

    [Header("Kill")]
    public int killPoint; //被杀后给到的经验值，主要是敌人有这个属性

    [Header("Level")]
    public int currentLevel; //当前等级

    public int maxLevel; //最高等级

    public int baseExp; // 基础经验值

    public int currentExp; // 当前经验值

    public float levelBuff; //升级之后经验升级的百分比

    public float LevelMultiplier //属性
    {   //随着等级的提升增加的经验越来越多
        get { return 1 + ( currentLevel - 1 ) * levelBuff; }
    }

    public void UpdateExp(int point) //怪物死亡后得到的经验
    {
        currentExp += point;

        if (currentExp >= baseExp) //当前经验值大于基础经验值就升级
            LevelUp();
    }

    private void LevelUp()
    {
        //等级提升
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        //下一个阶段需要的经验值
        baseExp += (int)(baseExp * LevelMultiplier);

        //血量增加
        maxHealth = (int)(maxHealth * LevelMultiplier);
        //血量回复
        currentHealth = maxHealth;


       // Debug.Log("LEVEL UP!" + currentLevel + "Max Health" + maxHealth);
    }
}

