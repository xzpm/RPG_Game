using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;

    public SceneFader sceneFaderPrefab;

    GameObject player;

    NavMeshAgent agent;

    bool fadeFinish;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);//在加载时不销毁该脚本
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinish = true;
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch(transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                //启动协程，传送到加载好的场景中，TransitionDestination有相对应Tag的位置
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));

                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        
        //传送之前保存数据
        SaveManager.Instance.SavePlayerData();//保存同场景数据

        if (SceneManager.GetActiveScene().name != sceneName) // 传送到不同场景
        {
            SceneFader fade = Instantiate(sceneFaderPrefab);
            yield return StartCoroutine(fade.FadeOut(2.0f));
            yield return SceneManager.LoadSceneAsync(sceneName); //等待场景加载
            TransitionDestination destination = GetTransitionDestination(destinationTag);
            yield return Instantiate(playerPrefab, destination.transform.position,destination.transform.rotation); //生成player
            SaveManager.Instance.LoadPlayerData(); //加载玩家的数据
            yield return StartCoroutine(fade.FadeIn(2.0f));
            yield break; //从协程中跳出

        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject; //拿到player
            agent = player.GetComponent<NavMeshAgent>();
            agent.enabled = false; //把玩家agent关闭
            TransitionDestination destination = GetTransitionDestination(destinationTag);
            player.transform.SetPositionAndRotation(destination.transform.position,destination.transform.rotation);
            agent.enabled = true; //传送到位置再开启
            yield return null;
        }

    }

    //根据tag获得指定destination点的坐标
    private TransitionDestination GetTransitionDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        for ( int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }
        return null;
    }

    //TODO： 简化合并当前方法和上面那个方法
    //获得新场景中的入口坐标
    public Transform GetEntrance()
    {
        foreach (var point in FindObjectsOfType<TransitionDestination>())
        {   //寻找场景中的入口点，如果匹配了就返回该点的transform
            if (point.destinationTag == TransitionDestination.DestinationTag.ENTER)
                return point.transform;
        }
        return null;
    }
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Scene1"));
    }

    public void TransitionToLoadedGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    //加载第一个场景
    IEnumerator LoadLevel(string scene)
    {
        //生成出FadeCanvas实现渐出渐入
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if(scene!="")
        {
            yield return StartCoroutine(fade.FadeOut(2.0f));
            yield return SceneManager.LoadSceneAsync(scene);//加载场景
            Transform startTrans = GetEntrance();
            yield return Instantiate(playerPrefab, startTrans.position, startTrans.rotation);//生成人物

            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2.0f));
            yield break;

        }
    }

    //加载主菜单
    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2.0f));
        yield return SceneManager.LoadSceneAsync("MainMenu");
        yield return StartCoroutine(fade.FadeIn(2.0f));
        yield break;
    }

    public void EndNotify()
    {
        if(fadeFinish) //保证加载主菜单的协程只开启一次
        {
            fadeFinish = false;
            StartCoroutine(LoadMain());

        }
    }
}
