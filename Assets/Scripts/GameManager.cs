using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    StatsEditor statsEditor;
    MessageManager messageManager;

    public static bool PanelIsOpen = false;
    public static bool StandardMode;
    public static bool ManualMode;
    public static bool AutoMode;

    void Start()
    {
        statsEditor = GameObject.Find("StatsEditor").GetComponent<StatsEditor>();

        StandardMode = true;
        ManualMode = false;
        AutoMode = false;

        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
    }

    #region Change game mode
    public void StandardModeOn()
    {
        StandardMode = true;
        ManualMode = false;
        AutoMode = false;

        messageManager.ShowMessage($"<color=#00FF9A>Został włączony tryb standardowy.</color>", 3f);
        Debug.Log($"Został włączony tryb standardowy.");
    }


    public void ManualModeOn()
    {
        ManualMode = true;
        StandardMode = false;
        AutoMode = false;

        messageManager.ShowMessage($"<color=#00FF9A>Został włączony tryb manualny.</color>", 3f);
        Debug.Log($"Został włączony tryb manualny.");
    }


    public void AutoModeOn()
    {
        AutoMode = true;
        StandardMode = false;
        ManualMode = false;

        messageManager.ShowMessage($"<color=#00FF9A>Został włączony tryb automatyczny.</color>", 3f);
        Debug.Log($"Został włączony tryb automatyczny.");
    }
    #endregion

    public void ShowOrHidePanel(GameObject panel)
    {
        if(!panel.activeSelf)
        {
            panel.SetActive(true);
            PanelIsOpen = true;
        }
        else
        {
            panel.SetActive(false);
            PanelIsOpen = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            GameObject[] panels = GameObject.FindGameObjectsWithTag("Panel");

            foreach (GameObject panel in panels)
            {
                if (panel.activeSelf && panel != mainMenuPanel)
                {
                    ShowOrHidePanel(panel);
                }

                if(panel.transform.IsChildOf(GameObject.Find("Canvas").transform))
                {
                    ShowOrHideMainMenuPanel();
                }

                if (panel.transform.IsChildOf(GameObject.Find("SetStatsCanvas").transform) && panel.name != "RollPanel" && panel.name != "StatsPanel")
                {
                    statsEditor.ShowGeneralPanel();
                }
            }

            if (panels.Length == 0)
                ShowOrHideMainMenuPanel();
        }
    }

    public void ShowOrHideMainMenuPanel()
    {
        if (!mainMenuPanel.activeSelf)
        {
            PanelIsOpen = true;
            mainMenuPanel.SetActive(true);
        }
        else
        {
            PanelIsOpen = false;
            mainMenuPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
