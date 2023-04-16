using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsPanel; // Panel z opcjami

    public void ShowOptionsPanel()
    {
        // Otwiera lub zamyka glowny panel
        if (!optionsPanel.activeSelf)
        {
            StatsEditor.EditorIsOpen = true; // odwolanie do statycznego boola, zeby nie moc zaznaczac postai podczas wlaczonego menu opcji
            optionsPanel.SetActive(true);
        }
        else
        {
            optionsPanel.SetActive(false);
            StatsEditor.EditorIsOpen = false; // odwolanie do statycznego boola, zeby nie moc zaznaczac postai podczas wlaczonego menu opcji
        }
    }
}
