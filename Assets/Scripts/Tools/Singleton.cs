using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> //泛型约束了T的类型
{
    private static T instance; // 生成该类的单个实体

    public static T Instance //利用get外部访问
    {
        get { return instance; }
    }

    //希望继承该类型类的其他类都能更改内部的方法，所以使用protected
    protected virtual void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    // 返回单例类型是否生成
    public static bool IsInitialized
    {
        get { return instance != null;  }
    }

    // 销毁该单例类型的方式
    protected virtual void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }
}
