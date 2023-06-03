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

    public static bool PanelIsOpen;
    public static bool StandardMode;
    public static bool ManualMode;
    public static bool AutoMode;

    [SerializeField] private Slider slider;
    public static float MusicVolume = 0.5f;

    void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
            Screen.fullScreen = true;
    }

    void Start()
    {
        Camera.main.GetComponent<AudioSource>().volume = MusicVolume;

        PanelIsOpen = false;

        // Wyszarza przycisk z aktualnie aktywnym trybem gry
        // Jeżeli w menu startowym nie został ustawiony tryb, to ustawiany jest tryb domyślny
        if (StandardMode != true && ManualMode != true && AutoMode != true && SceneManager.GetActiveScene().buildIndex == 2)
        {
            StandardMode = true;
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().DecreaseButtonOpacity(GameObject.Find("Canvas/OptionsPanel/StandardModeButton").GetComponent<Button>());
            GameObject.Find("Canvas/OptionsPanel").SetActive(false);
        }
        else if (ManualMode == true && SceneManager.GetActiveScene().buildIndex == 2)
        {
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().DecreaseButtonOpacity(GameObject.Find("Canvas/OptionsPanel/ManualModeButton").GetComponent<Button>());
            GameObject.Find("Canvas/OptionsPanel").SetActive(false);
        }
        else if (AutoMode == true && SceneManager.GetActiveScene().buildIndex == 2)
        {
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().DecreaseButtonOpacity(GameObject.Find("Canvas/OptionsPanel/AutoModeButton").GetComponent<Button>());
            GameObject.Find("Canvas/OptionsPanel").SetActive(false);
        }
        else if(StandardMode == true && SceneManager.GetActiveScene().buildIndex == 2)
        {
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().DecreaseButtonOpacity(GameObject.Find("Canvas/OptionsPanel/StandardModeButton").GetComponent<Button>());
            GameObject.Find("Canvas/OptionsPanel").SetActive(false);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 0)
            SetSliderValue(slider);

        if (SceneManager.GetActiveScene().buildIndex == 2)
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

        if(SceneManager.GetActiveScene().buildIndex == 2)
            messageManager.ShowMessage($"<color=#00FF9A>Został włączony tryb standardowy.</color>", 3f);           
        Debug.Log($"Został włączony tryb standardowy.");
    }


    public void ManualModeOn()
    {
        ManualMode = true;
        StandardMode = false;
        AutoMode = false;

        if (SceneManager.GetActiveScene().buildIndex == 2)
            messageManager.ShowMessage($"<color=#00FF9A>Został włączony tryb manualny.</color>", 3f);
        Debug.Log($"Został włączony tryb manualny.");
    }


    public void AutoModeOn()
    {
        AutoMode = true;
        StandardMode = false;
        ManualMode = false;

        if (SceneManager.GetActiveScene().buildIndex == 2)
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
        if (Input.GetKeyDown("escape") && (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 2))
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

                if(SceneManager.GetActiveScene().buildIndex != 0)
                {
                    if (panel.transform.IsChildOf(GameObject.Find("SetStatsCanvas").transform) && panel.name != "RollPanel" && panel.name != "StatsPanel")
                    {
                        statsEditor.ShowGeneralPanel();
                    }
                }
            }

            if (panels.Length == 0 && SceneManager.GetActiveScene().buildIndex != 0)
                ShowOrHideMainMenuPanel();
        }
        else if (Input.GetKeyDown("escape") && SceneManager.GetActiveScene().buildIndex == 1)
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
