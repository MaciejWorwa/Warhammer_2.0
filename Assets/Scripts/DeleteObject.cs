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
            foreach (GameObject panel in GameObject.FindGameObjectsWithTag("Panel"))
            {
                if (panel.activeSelf)
                {
                    GameObject.Find("GameManager").GetComponent<GameManager>().ShowOrHidePanel(panel);
                }
            }
        }
    }
}
