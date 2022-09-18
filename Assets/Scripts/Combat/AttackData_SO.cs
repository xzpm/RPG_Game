using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Attack Data", menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    //¹¥»÷·¶Î§£¬¼¼ÄÜ·¶Î§
    public float attackRange;
    public float skillRange;

    //cdÀäÈ´
    public float coolDown;

    //ÆÕÍ¨¹¥»÷
    public int minDamage;
    public int maxDamage;

    //±©»÷
    public float criticalMultiplier;
    public float criticalChance;
}
