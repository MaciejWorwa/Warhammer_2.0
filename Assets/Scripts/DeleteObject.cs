using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    public void DestroySelectedObjects()
    {
        if (Player.trSelect != null)
        {
            Destroy(Player.selectedPlayer);
            //actionsButtons = GameObject.Find("ActionsButtonsPlayer");

            //// Wylacza przyciski akcji usuwanej postaci, jesli sa wlaczone
            //if (actionsButtons.activeSelf && actionsButtons != null)
            //    actionsButtons.SetActive(false);
        }
        if (Enemy.trSelect != null)
        {
            Destroy(Enemy.selectedEnemy);
            //actionsButtons = GameObject.Find("ActionsButtonsEnemy");

            //// Wylacza przyciski akcji usuwanej postaci, jesli sa wlaczone
            //if (actionsButtons.activeSelf && actionsButtons != null)
            //    actionsButtons.SetActive(false);
        }
    }
}
