using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {  //枚举只有两个变量，同场景和不同场景
        SameScene, DifferentScene
    }
    [Header("Transition Info")]
    public string sceneName; //设置传送的场景
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;

    private bool canTrans;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && canTrans) //如果按下E键，且当前状态为可以传送
        {
            SceneController.Instance.TransitionToDestination(this);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}
