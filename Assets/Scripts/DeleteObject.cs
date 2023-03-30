using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    public void DestroySelectedObjects()
    {
        if (Player.trSelect != null)
            Destroy(Player.selectedPlayer);

        if (Enemy.trSelect != null)
            Destroy(Enemy.selectedEnemy);
    }
}
