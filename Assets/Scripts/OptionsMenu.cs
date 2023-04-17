using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsPanel; // Panel z opcjami

    public void ShowOptionsPanel()
    {
        // Otwiera lub zamyka glowny panel
        if (!optionsPanel.activeSelf)
        {
            GameManager.PanelIsOpen = true;
            optionsPanel.SetActive(true);
        }
        else
        {
            optionsPanel.SetActive(false);
            GameManager.PanelIsOpen = false;
        }
    }

    public void HideOptionsPanel()
    {
        if (optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
            GameManager.PanelIsOpen = false;
        }
    }
}
