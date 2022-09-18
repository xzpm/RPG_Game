using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class GameManager : Singleton<GameManager>
{
    //其他所有代码访问characterStats的时候都通过gamemanager来访问
    public CharacterStats playerStats;

    CinemachineFreeLook followCamera;

    //创建一个列表记录有在IEndGameObserver接口下的所有实体
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }


    //用观察者模式反向注册的方式让player生成的时候告诉gamemanager
    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();

        if(followCamera!=null)
        {
            followCamera.LookAt = playerStats.transform.GetChild(2);
            followCamera.Follow = playerStats.transform.GetChild(2);
        }
    }

    //反向注册的方式将endGameOberver全部添加到列表中
    public void AddObserver(IEndGameObserver observer)
    {
        //没有判断重复，因为只有敌人创建时才加入该列表，一定不会重复
        endGameObservers.Add(observer);
    }

    //从列表中移除对象
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //实现广播
    public void NotifyObservers()
    {
        foreach(var observer in endGameObservers)
        {//列表中的所有敌人实体都执行EndNotify();
            observer.EndNotify();
        }
    }

 

}
