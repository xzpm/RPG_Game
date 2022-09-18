using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName = "Insert";

    //利用property取得磁盘中sceneName键值对应的值，也就是保存的场景的名字
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMain();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    void Save(Object data,string key)
    {
        var jsonData = JsonUtility.ToJson(data,true);
        PlayerPrefs.SetString(key,jsonData); //利用PlayerPrefs保存在磁盘上
        //磁盘上有一个位置保存当前场景的名字，每次根据sceneName这个关键字去磁盘中存取
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name); 
        PlayerPrefs.Save();
    }

    void Load(Object data, string key)
    {
        if(PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
