using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Zmniejszenie przezroczystosci przycisku po kliknieciu
    public void DecreaseOpacityAfterClick(GameObject selectedButton)
    {
        selectedButton.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
        StartCoroutine(ExecuteAfterTime(0.5f, selectedButton));
    }

    // Przywrocenie domyúlnej przeüroczystosci po 0.5 sekundy od klikniecia
    IEnumerator ExecuteAfterTime(float time, GameObject selectedButton)
    {
        yield return new WaitForSeconds(time);
        selectedButton.GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
    }
}
