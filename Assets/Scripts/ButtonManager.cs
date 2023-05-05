using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject actionsButtons;

    #region Show or hide actions buttons function
    public void ShowOrHideActionsButtons(GameObject character, bool visibility)
    {
        if(visibility == true)
        {
            GameObject.Find("ActionsButtons").transform.Find("Canvas").gameObject.SetActive(true); // Dezaktywuje jedynie Canvas przypisany do obiektu ActionsButton, a nie cały obiekt
            GameObject.Find("ActionsButtons").transform.position = character.transform.position;
        }
        else
        {
            GameObject.Find("ActionsButtons").transform.Find("Canvas").gameObject.SetActive(false); // Dezaktywuje jedynie Canvas przypisany do obiektu ActionsButton, a nie cały obiekt
        }

        ShowReloadOrChargeButton(character);
        ShowOrHideMagicButtons(character);
        RefreshDefensivePositionButton(character);
        RefreshAimButton(character);
    }
    #endregion

    #region Clicked button effect (reduce opacity)
    // Zmniejszenie przezroczystosci przycisku po kliknieciu
    public void DecreaseOpacityAfterClick(GameObject selectedButton)
    {
        selectedButton.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
        StartCoroutine(ExecuteAfterTime(0.5f, selectedButton));
    }

    // Przywrocenie domyślnej przeźroczystosci po 0.5 sekundy od klikniecia
    IEnumerator ExecuteAfterTime(float time, GameObject selectedButton)
    {
        yield return new WaitForSeconds(time);
        selectedButton.GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
    }
    #endregion

    #region Show or hide magic-related buttons function
    // Określa, czy są widoczne przyciski splatania magii i rzucania zaklęć
    public void ShowOrHideMagicButtons(GameObject character)
    {
        if (character.GetComponent<Stats>().Mag > 0)
        {
            if (GameObject.Find("ActionsButtons/Canvas/ChannelingButton") != null)
                GameObject.Find("ActionsButtons/Canvas/ChannelingButton").SetActive(true);
            if (GameObject.Find("ActionsButtons/Canvas/SpellButton") != null)
                GameObject.Find("ActionsButtons/Canvas/SpellButton").SetActive(true);
        }
        else
        {
            if (GameObject.Find("ActionsButtons/Canvas/ChannelingButton") != null)
                GameObject.Find("ActionsButtons/Canvas/ChannelingButton").SetActive(false);
            if (GameObject.Find("ActionsButtons/Canvas/SpellButton") != null)
                GameObject.Find("ActionsButtons/Canvas/SpellButton").SetActive(false);
        }   
    }

    public void ShowSpellButtons()
    {
        GameObject character = Character.selectedCharacter;

        if (GameObject.Find("SpellButtons/SpellButtonsCanvas") != null)
        {
            GameObject.Find("SpellButtons/SpellButtonsCanvas").SetActive(true);
            GameObject.Find("SpellButtons/SpellButtonsCanvas").transform.position = character.transform.position;
            if(character.GetComponent<Stats>().etherArmorActive)
                GameObject.Find("EtherArmorSpellButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
            else
                GameObject.Find("EtherArmorSpellButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
        }
    }

    public void HideSpellButtons()
    {
        if (GameObject.Find("SpellButtons/SpellButtonsCanvas") == true)
            GameObject.Find("SpellButtons/SpellButtonsCanvas").SetActive(false);   
    }
    #endregion

    #region Show or hide reload and charge buttons function
    // Określa, czy jest widoczny przycisk przeladowania broni, czy szarzy w zaleznosci od zasiegu broni, ktorej uzywa postac
    private void ShowReloadOrChargeButton(GameObject character)
    {
        if (character.GetComponent<Stats>().AttackRange > 1.5f)
        {
            if (GameObject.Find("ActionsButtons/Canvas/ReloadButton") != null)
                GameObject.Find("ActionsButtons/Canvas/ReloadButton").SetActive(true);
            if (GameObject.Find("ActionsButtons/Canvas/ChargeButton") != null)
                GameObject.Find("ActionsButtons/Canvas/ChargeButton").SetActive(false);
        }
        else
        {
            if (GameObject.Find("ActionsButtons/Canvas/ReloadButton") != null)
                GameObject.Find("ActionsButtons/Canvas/ReloadButton").SetActive(false);
            if (GameObject.Find("ActionsButtons/Canvas/ChargeButton") != null)
                GameObject.Find("ActionsButtons/Canvas/ChargeButton").SetActive(true);
        }
    }
    #endregion

    #region Refresh defensive position button
    // Odswieza przycisk pozycji obronnej dla kazdej wybranej postaci, w zaleznosci czy pozycja obronna jest u niego aktywna, czy nie
    private void RefreshDefensivePositionButton(GameObject character)
    {
        if (character.GetComponent<Stats>().defensiveBonus == 0)
        {
            if (GameObject.Find("ActionsButtons/Canvas/DefensivePositionButton") != null)
                GameObject.Find("ActionsButtons/Canvas/DefensivePositionButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
        }
        else
        {
            if (GameObject.Find("ActionsButtons/Canvas/DefensivePositionButton") != null)
                GameObject.Find("ActionsButtons/Canvas/DefensivePositionButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
        }
    }
    #endregion

    #region Refresh aim button
    // Odswieza przycisk przycelowania dla kazdej wybranej postaci, w zaleznosci czy przycelowanie jest u niego aktywna, czy nie
    private void RefreshAimButton(GameObject character)
    {
        if (character.GetComponent<Stats>().aimingBonus == 0 && GameObject.Find("ActionsButtons/Canvas/AimButton") != null)
        {
            GameObject.Find("ActionsButtons/Canvas/AimButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
        }
        else if (GameObject.Find("ActionsButtons/Canvas/AimButton") != null)
        {
            GameObject.Find("ActionsButtons/Canvas/AimButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
        }
    }
    #endregion

    #region Refresh weapon type buttons 
    // Odswieza przycisk przycelowania dla kazdej wybranej postaci, w zaleznosci czy przycelowanie jest u niego aktywna, czy nie
    public void RefreshWeaponTypeButtons(GameObject character)
    {
        if (character.GetComponent<Stats>().AttackRange > 1.5f)
        {
            GameObject.Find("distance_attack_button").GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.5f);
            GameObject.Find("melee_attack_button").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("melee_attack_button").GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.5f);
            GameObject.Find("distance_attack_button").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
    }
    #endregion
}
