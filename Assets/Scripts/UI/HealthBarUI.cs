using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab; // 事先生成的Prefab
    public Transform barPoint; // 血条生成的位置
    public bool isAlwaysVisible; //是否永久可见

    public float visibleTime; // 持续的显示时间
    float timeLeft; //剩余的显示事件

    Image healthSlider;
    Transform UIbar;
    Transform cam; // 希望血条永远面朝摄像机

    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable() //当gameObject启动时调用
    {
        //启动时将获取到相机的位置
        cam = Camera.main.transform;

        foreach(Canvas canvas in FindObjectsOfType<Canvas>()) //寻找所有Canvas组件
        {
            if(canvas.renderMode == RenderMode.WorldSpace) 
                //如果canvas组建的渲染模式是世界范围的，也可以用名字的方式去查找,这个游戏只有一个canvas是设置为世界坐标的，所以可以直接使用这种方式
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;//将血条预制体生成在指定的canvas的位置，获得其坐标
                healthSlider = UIbar.GetChild(0).GetComponent<Image>(); //获得滑动的Image
                UIbar.gameObject.SetActive(isAlwaysVisible); //根据是否是一直显示激活UIbar
            }

        }
    }

    private void UpdateHealthBar(int curHeath, int maxHealth)
    {
        if(curHeath <= 0)
             Destroy(UIbar.gameObject);
          

        UIbar.gameObject.SetActive(true); //gameObject是自带的？
        timeLeft = visibleTime;
        //获得目前血量相对于总血量的百分比
        float slidPersent = (float)curHeath / maxHealth;
        healthSlider.fillAmount = slidPersent;
    }

    //该帧渲染之后才执行
    private void LateUpdate()
    {
        if(UIbar!=null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;
            if (timeLeft <= 0 && !isAlwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }

        
    }

}
