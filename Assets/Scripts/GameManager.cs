using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject quitPanel;
    [SerializeField] GameObject rollPanel;
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
            else if (rollPanel.activeSelf)
                HideRollPanel();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowOrHideQuitGamePanel()
    {
        if (!quitPanel.activeSelf)
            quitPanel.SetActive(true);
        else
            quitPanel.SetActive(false);
    }

    public void ShowRollPanel()
    {
        if (!rollPanel.activeSelf)
        {
            PanelIsOpen = true;
            rollPanel.SetActive(true);
        }
        else
        {
            PanelIsOpen = false;
            rollPanel.SetActive(false);
        }
    }

    public void HideRollPanel()
    {
        if (rollPanel.activeSelf)
        {
            rollPanel.SetActive(false);
            PanelIsOpen = false;
        }
    }
}
