using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameButton;
    Button continueButton;
    Button quitButton;
    PlayableDirector director;

    private void Awake()
    {
        newGameButton = transform.GetChild(1).GetComponent<Button>();
        continueButton = transform.GetChild(2).GetComponent<Button>();
        quitButton = transform.GetChild(3).GetComponent<Button>();

        quitButton.onClick.AddListener(QuitGame);
        continueButton.onClick.AddListener(ContinueGame);
        newGameButton.onClick.AddListener(PlayStartTimeLine);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }

    void PlayStartTimeLine()
    {
        director.Play();
    }
    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();//删除所有记录
        SceneController.Instance.TransitionToFirstLevel();
       // Debug.Log("从新开始游戏");

    }

    void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadedGame();
        //继续游戏
       // Debug.Log("继续游戏");
    }

    void QuitGame()
    {
        Application.Quit();
      //  Debug.Log("退出游戏");
    }
}
