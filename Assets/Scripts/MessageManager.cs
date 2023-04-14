using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public TMP_Text Message;

    public void ShowMessage(string messageText, Color messageColor, float messageDuration)
    {
        Message.text = messageText;
        Message.color = messageColor;
        Message.gameObject.SetActive(true); // w³¹czenie obiektu

        StartCoroutine(HideMessage(messageDuration)); // uruchomienie korutyny
    }

    private IEnumerator HideMessage(float messageDuration)
    {
        // Czeka czas okreslony przez messageDuration
        yield return new WaitForSeconds(messageDuration);

        // Tworzy efektu zanikania tekstu
        float fadeTime = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            Message.color = new Color(Message.color.r, Message.color.g, Message.color.b, Mathf.Lerp(1, 0, elapsedTime / fadeTime));
            yield return null;
        }

        Message.gameObject.SetActive(false); // wy³¹czenie obiektu
    }
}

