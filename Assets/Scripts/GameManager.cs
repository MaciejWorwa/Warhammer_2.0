using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    OptionsMenu optionsMenu;
    StatsEditor statsEditor;

    public static bool PanelIsOpen = false;

    void Start()
    {
        optionsMenu = GameObject.Find("OptionsMenu").GetComponent<OptionsMenu>();
        statsEditor = GameObject.Find("StatsEditor").GetComponent<StatsEditor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (!PanelIsOpen)
                ShowOrHideQuitGamePanel();
            else if (statsEditor.generalPanel.activeSelf) 
                GameObject.Find("StatsEditor").GetComponent<StatsEditor>().HideGeneralPanel();
            else if (optionsMenu.optionsPanel.activeSelf)
                GameObject.Find("OptionsMenu").GetComponent<OptionsMenu>().HideOptionsPanel();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowOrHideQuitGamePanel()
    {
        if (!panel.activeSelf)
            panel.SetActive(true);
        else
            panel.SetActive(false);
    }
}
