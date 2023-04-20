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
        if (character == Player.selectedPlayer)
            actionsButtons = GameObject.Find("ActionsButtonsPlayer");
        else
            actionsButtons = GameObject.Find("ActionsButtonsEnemy");

        if(visibility == true)
        {
            actionsButtons.transform.Find("Canvas").gameObject.SetActive(true); // Dezaktywuje jedynie Canvas przypisany do obiektu ActionsButton, a nie cały obiekt
            actionsButtons.transform.position = character.transform.position;
        }
        else
        {
            actionsButtons.transform.Find("Canvas").gameObject.SetActive(false); // Dezaktywuje jedynie Canvas przypisany do obiektu ActionsButton, a nie cały obiekt
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
        if(character.CompareTag("Enemy"))
        {
            if (character.GetComponent<Stats>().Mag > 0)
            {
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/ChannelingButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/ChannelingButton").SetActive(true);
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/SpellButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/SpellButton").SetActive(true);
            }
            else
            {
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/ChannelingButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/ChannelingButton").SetActive(false);
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/SpellButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/SpellButton").SetActive(false);
            }
        }
        else
        {
            if (character.GetComponent<Stats>().Mag > 0)
            {
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/ChannelingButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/ChannelingButton").SetActive(true);
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/SpellButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/SpellButton").SetActive(true);
            }
            else
            {
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/ChannelingButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/ChannelingButton").SetActive(false);
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/SpellButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/SpellButton").SetActive(false);
            }
        }
    }
    #endregion

    #region Show or hide reload and charge buttons function
    // Określa, czy jest widoczny przycisk przeladowania broni, czy szarzy w zaleznosci od zasiegu broni, ktorej uzywa postac
    private void ShowReloadOrChargeButton(GameObject character)
    {
        if (character.CompareTag("Enemy"))
        {
            if (character.GetComponent<Stats>().AttackRange > 1.5f)
            {
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/ReloadButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/ReloadButton").SetActive(true);
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/ChargeButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/ChargeButton").SetActive(false);
            }
            else
            {
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/ReloadButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/ReloadButton").SetActive(false);
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/ChargeButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/ChargeButton").SetActive(true);
            }
        }
        else
        {
            if (character.GetComponent<Stats>().AttackRange > 1.5f)
            {
                if(GameObject.Find("ActionsButtonsPlayer/Canvas/ReloadButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/ReloadButton").SetActive(true);
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/ChargeButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/ChargeButton").SetActive(false);
            }
            else
            {
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/ReloadButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/ReloadButton").SetActive(false);
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/ChargeButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/ChargeButton").SetActive(true);
            }
        }
    }
    #endregion

    #region Refresh defensive position button
    // Odswieza przycisk pozycji obronnej dla kazdej wybranej postaci, w zaleznosci czy pozycja obronna jest u niego aktywna, czy nie
    private void RefreshDefensivePositionButton(GameObject character)
    {
        if (character.CompareTag("Enemy"))
        {
            if (character.GetComponent<Stats>().defensiveBonus == 0)
            {
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/DefensivePositionButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/DefensivePositionButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
            }
            else
            {
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/DefensivePositionButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/DefensivePositionButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
            }
        }
        else
        {
            if (character.GetComponent<Stats>().defensiveBonus == 0)
            {
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/DefensivePositionButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/DefensivePositionButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
            }
            else
            {
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/DefensivePositionButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/DefensivePositionButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
            }
        }
    }
    #endregion

    #region Refresh aim button
    // Odswieza przycisk przycelowania dla kazdej wybranej postaci, w zaleznosci czy przycelowanie jest u niego aktywna, czy nie
    private void RefreshAimButton(GameObject character)
    {
        if (character.CompareTag("Enemy"))
        {
            if (character.GetComponent<Stats>().aimingBonus == 0)
            {
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/AimButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/AimButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
            }
            else
            {
                if (GameObject.Find("ActionsButtonsEnemy/Canvas/AimButton") != null)
                    GameObject.Find("ActionsButtonsEnemy/Canvas/AimButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
            }
        }
        else
        {
            if (character.GetComponent<Stats>().aimingBonus == 0)
            {
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/AimButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/AimButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
            }
            else
            {
                if (GameObject.Find("ActionsButtonsPlayer/Canvas/AimButton") != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas/AimButton").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
            }
        }
    }
    #endregion
}
