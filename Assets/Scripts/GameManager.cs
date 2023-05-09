using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    OptionsMenu optionsMenu;
    StatsEditor statsEditor;
    MessageManager messageManager;

    public static bool PanelIsOpen = false;
    public static bool StandardMode;
    public static bool ManualMode;
    public static bool AutoMode;

    void Start()
    {
        optionsMenu = GameObject.Find("OptionsMenu").GetComponent<OptionsMenu>();
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

    //public void RestartGame()
    //{
    //    // To niby restartuje scenę, ale nie da się póniej z niewiadomych przyczyn zaznaczac postaci
    //    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    //    /* Wiec tymczasowo robimy tak:*/

    //    RoundManager.roundNumber = 1;

    //    if (Character.trSelect != null)
    //        GameObject.Find("ButtonManager").GetComponent<ButtonManager>().ShowOrHideActionsButtons(Character.selectedCharacter, false);
         
    //    List<Stats> allStats = new List<Stats>();

    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    //    GameObject[] characters = enemies.Concat(players).ToArray();

    //    foreach (var character in characters)
    //        Destroy(character);

    //    CharacterManager.playersAmount = 0;
    //    CharacterManager.enemiesAmount = 0;
    //}

    // Update is called once per frame
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
