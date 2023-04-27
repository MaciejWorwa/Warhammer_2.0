using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    public void DestroySelectedObjects()
    {
        if (Character.trSelect)
        {
            Destroy(Character.selectedCharacter);

            // Wylacza przyciski akcji usuwanej postaci, jesli sa wlaczone
            if (GameObject.Find("ActionsButtons/Canvas") != null)
                GameObject.Find("ActionsButtons/Canvas").SetActive(false);

            // Wylacza wszystkie panele dotyczace niszczonej postaci, jesli sa otwarte
            GameObject.Find("StatsEditor").GetComponent<StatsEditor>().ShowGeneralPanel();
            GameObject.Find("StatsEditor").GetComponent<StatsEditor>().HideGeneralPanel();
            GameObject.Find("GameManager").GetComponent<GameManager>().HideRollPanel();
            GameObject.Find("OptionsMenu").GetComponent<OptionsMenu>().HideOptionsPanel();
        }
    }
}
