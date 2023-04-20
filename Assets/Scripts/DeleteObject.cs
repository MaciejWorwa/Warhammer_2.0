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


        // Wylacza wszystkie panele dotyczace niszczonej postaci, jesli sa otwarte
        GameObject.Find("StatsEditor").GetComponent<StatsEditor>().ShowGeneralPanel();
        GameObject.Find("StatsEditor").GetComponent<StatsEditor>().HideGeneralPanel();
        GameObject.Find("GameManager").GetComponent<GameManager>().HideRollPanel();
        GameObject.Find("OptionsMenu").GetComponent<OptionsMenu>().HideOptionsPanel();
    }
}
