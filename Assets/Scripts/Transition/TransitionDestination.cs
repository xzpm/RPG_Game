using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    { //传送门终点的标签，入口Enter或是A点，B点等
        ENTER,A,B
    }

    public DestinationTag destinationTag;
}
