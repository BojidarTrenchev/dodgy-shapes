using UnityEngine;

public class LoopController : MonoBehaviour
{
    private Transform player;
    private Transform objTransform;
    private float offset;

    void Start()
    {
        this.objTransform = this.GetComponent<Transform>();
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        this.offset = this.objTransform.position.y - this.player.position.y;
    }

    void Update()
    {
        var pos = this.objTransform.position;
        pos.y = this.player.position.y + offset;
        this.objTransform.position = pos;
    }
}
