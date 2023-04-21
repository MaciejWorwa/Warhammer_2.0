using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using System.Linq;

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

    public void ChangePanelIsOpenToFalse()
    {
        PanelIsOpen = false;
    }

    public void RestartGame()
    {
        // To niby restartuje scenê, ale nie da siê póŸniej z niewiadomych przyczyn zaznaczaæ postaci
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        /* Wiêc tymczasowo robimy tak:*/

        RoundManager.roundNumber = 1;

        if (Player.trSelect != null)
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().ShowOrHideActionsButtons(Player.selectedPlayer, false);
        if (Enemy.trSelect != null)
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().ShowOrHideActionsButtons(Enemy.selectedEnemy, false);
         
        List<Stats> allStats = new List<Stats>();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject[] characters = enemies.Concat(players).ToArray();

        foreach (var character in characters)
            Destroy(character);

        CreateTeam.playersAmount = 0;
        CreateEnemy.enemiesAmount = 0;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (!PanelIsOpen || mainMenuPanel.activeSelf)
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
