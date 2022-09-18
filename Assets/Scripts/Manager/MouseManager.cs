using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//拖拽的方法对物体进行设置
//[System.Serializable]

//public class EventVector3 : UnityEvent<Vector3> { };
////一个继承了UnityEvent的类，类型应该是vector3？
////因为这个class不是继承monobehaviour的，所以必须被序列化才能显示出来

public class MouseManager : Singleton<MouseManager>
{
    ////单例模式需要先创建一个static的自身的变量，通常取名为Instance
    //public static MouseManager Instance;

    //获取鼠标的不同贴图
    public Texture2D point, doorway, attack, target, arrow;

    //创建一个事件
    public event Action<Vector3> OnMouseClicked;

    public event Action<GameObject> OnEnemyClicked;

    public event Action<GameObject> OnMousedPushed;

    //摄像机射线捕获到的数据
    RaycastHit hitinfo;

    ////设置一个EventVector3的变量点击鼠标；
    //public EventVector3 OnMouseClicked;


    // 在单例中重写Awake()
    protected override void Awake()
    {
        base.Awake(); //基于原有父类方法之上
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCurserTexture();
        MouseControl();
    }

    
    void SetCurserTexture()
    {
        //一条射线，从摄像机发出，方向沿着鼠标的位置发射
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray,out hitinfo))
        {
            //当鼠标移动到某些位置时改变鼠标贴图
            switch (hitinfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;

                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;

                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;

                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }

        }
    }

    void MouseControl()
    {
        //如果鼠标进行了点击，且射线碰撞到的碰撞体不为空
        if (Input.GetMouseButtonDown(0) && hitinfo.collider != null)
        {
            //如果碰撞到的物体的tag为地面
            if(hitinfo.collider.gameObject.CompareTag("Ground"))
            {
                //鼠标点击了的话，触发该事件
                OnMouseClicked?.Invoke(hitinfo.point);
                //就会执行注册在OnMouseClicked下面的所有事件
            }

            //如果碰撞的物体tag为Enemy
            if(hitinfo.collider.gameObject.CompareTag("Enemy"))
            {
                //执行注册在OnEnemyClicked下面的所有事件，
                //这里传进去的是鼠标射线碰撞到的collider的gameObject
                OnEnemyClicked?.Invoke(hitinfo.collider.gameObject);
            }

            //检测用
            //if(hitinfo.collider.gameObject.CompareTag("Player"))
            //{
            //    OnSelfClicked?.Invoke(hitinfo.collider.gameObject);
            //}

            //碰撞物体为石头等可攻击实体
            if(hitinfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitinfo.collider.gameObject);
            }

            //碰撞物体为传送门
            if (hitinfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitinfo.point);
              
            }
        }
    }
}
