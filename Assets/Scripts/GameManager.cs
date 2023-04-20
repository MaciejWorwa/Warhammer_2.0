using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
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
                ShowOrHideMainMenuPanel();
            else if (statsEditor.generalPanel.activeSelf)
                statsEditor.HideGeneralPanel();
            else if (optionsMenu.optionsPanel.activeSelf)
            {
                optionsMenu.HideOptionsPanel();
                ShowOrHideMainMenuPanel();
            }
            else if (rollPanel.activeSelf)
                HideRollPanel();
        }
    }

    public void ShowOrHideMainMenuPanel()
    {
        if (!mainMenuPanel.activeSelf)
            mainMenuPanel.SetActive(true);
        else
            mainMenuPanel.SetActive(false);
    }

    public void ShowOrHideQuitGamePanel()
    {
        if (!quitPanel.activeSelf)
            quitPanel.SetActive(true);
        else
            quitPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
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
