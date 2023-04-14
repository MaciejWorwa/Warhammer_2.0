using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    public void DestroySelectedObjects()
    {
        if (Player.trSelect != null && Enemy.trSelect == null)
        {
            Destroy(Player.selectedPlayer);

            // Wylacza przyciski akcji usuwanej postaci, jesli sa wlaczone
            if (GameObject.Find("ActionsButtonsPlayer/Canvas") != null)
                GameObject.Find("ActionsButtonsPlayer/Canvas").SetActive(false);
        }
        else if (Enemy.trSelect != null && Player.trSelect == null)
        {
            Destroy(Enemy.selectedEnemy);

            // Wylacza przyciski akcji usuwanej postaci, jesli sa wlaczone
            if (GameObject.Find("ActionsButtonsEnemy/Canvas") != null)
                GameObject.Find("ActionsButtonsEnemy/Canvas").SetActive(false);
        }
    }
}
