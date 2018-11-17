using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public void Awake()
    {
        Instantiate(PlayerInfo.info.allPlayerShapes[PlayerInfo.info.activePlayerShapeIndex]);
    }
}
