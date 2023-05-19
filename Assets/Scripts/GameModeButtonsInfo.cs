using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameModeButtonsInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject standardModeButtonInfo;
    [SerializeField] private GameObject manualModeButtonInfo;
    [SerializeField] private GameObject autoModeButtonInfo;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.gameObject.name == "StandardModeButton")
            standardModeButtonInfo.SetActive(true);
        else if (this.gameObject.name == "ManualModeButton")
            manualModeButtonInfo.SetActive(true);
        else if (this.gameObject.name == "AutoModeButton")
            autoModeButtonInfo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.gameObject.name == "StandardModeButton")
            standardModeButtonInfo.SetActive(false);
        else if (this.gameObject.name == "ManualModeButton")
            manualModeButtonInfo.SetActive(false);
        else if (this.gameObject.name == "AutoModeButton")
            autoModeButtonInfo.SetActive(false);
    }
}
