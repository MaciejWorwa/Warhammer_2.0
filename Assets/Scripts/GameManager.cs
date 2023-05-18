using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    StatsEditor statsEditor;
    MessageManager messageManager;

    public static bool PanelIsOpen = false;
    public static bool StandardMode;
    public static bool ManualMode;
    public static bool AutoMode;

    public static float MusicVolume = 0.5f;

    void Start()
    {
        Camera.main.GetComponent<AudioSource>().volume = MusicVolume;
        StandardMode = true;
        ManualMode = false;
        AutoMode = false;

        if (SceneManager.GetActiveScene().buildIndex == 0)
            GameObject.Find("SliderVolume").GetComponent<Slider>().value = MusicVolume;

        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            statsEditor = GameObject.Find("StatsEditor").GetComponent<StatsEditor>();
            // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
            messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
        }
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
        if (Input.GetKeyDown("escape") && SceneManager.GetActiveScene().buildIndex == 1)
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
        else if (Input.GetKeyDown("escape") && SceneManager.GetActiveScene().buildIndex == 0)
        {
            ShowOrHideMainMenuPanel();
        }

        //PRZEŁĄCZENIE MIEDZY TRYBEM PEŁNOEKRANOWYM A OKIENKOWYM
        // Sprawdź, czy klawisz Ctrl jest wciśnięty
        bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        // Sprawdź, czy klawisz Enter jest wciśnięty
        bool enterPressed = Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
        // Sprawdź, czy oba klawisze są wciśnięte jednocześnie
        if (ctrlPressed && enterPressed)
            Screen.fullScreen = !Screen.fullScreen ? true : false;
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

    public void SetSliderValue(Slider slider)
    {
        slider.value = MusicVolume;
    }

    public void ChangeMusicVolume(Slider slider)
    {
        MusicVolume = slider.value;
        Camera.main.GetComponent<AudioSource>().volume = slider.value;
    }
}
