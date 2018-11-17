using UnityEngine;
using System.Collections;

public class EnemyObjectController : ObjectController
{
    public override void Start()
    {
        base.Start();
        colorManager.ChangeEnemyColor(spriteRend);
    }
}
