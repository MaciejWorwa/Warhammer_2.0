using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private TMP_Text messagePrefab;
    List<TMP_Text> allMessages = new List<TMP_Text>();

    public void ShowMessage(string messageText, float messageDuration)
    {
        // Zmienia pozycje poprzednich wiadomosci, zeby nie pojawialy sie jedna na drugiej
        if (allMessages.Count > 0)
            foreach (var m in allMessages)
                m.gameObject.transform.Translate(new Vector3(0, (int)(Screen.height * 0.035f), 0));

        // Gdy ilosc wyswietlanych wiadomosci przekroczy okreslona ilosc to usuwa najwczesniejsza z nich
        if (allMessages.Count > 12)
        {
            Destroy(allMessages[0]);
            allMessages.RemoveAt(0);
        }

        // Tworzy nowa wiadomosc i dodaje ja do listy wszystkich wiadomosci
        TMP_Text message = Instantiate(messagePrefab, messagePrefab.transform.position, Quaternion.identity);
        allMessages.Add(message);

        // Ustala pozycje nowo powstalej wiadomosci i przypisuje ja do glownego Canvasa oraz ustawia go jako dziecko z indexem 0, żeby był wartwą na samym dole
        message.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform.transform);
        message.gameObject.transform.SetSiblingIndex(0);

        message.text = messageText;


        // Ustawienie rozmiaru pola tekstowego
        message.rectTransform.sizeDelta = new Vector2((int)Screen.width / 2.05f, message.rectTransform.sizeDelta.y);

        // Ustawienie rozmiaru czcionki
        message.fontSize = (int)(Screen.width * 0.015f);


        StartCoroutine(HideMessage(message, messageDuration)); // uruchomienie korutyny

        // Usuwa instancje obiektu message po odpowiednim czasie (czas wydluzony o 2 sekundy, zeby na spokojnie animacja zanikania z HideMessage mogla sie wykonac)
        if (message != null)
            Destroy(message.gameObject, messageDuration + 2f);
    }

    private IEnumerator HideMessage(TMP_Text message, float messageDuration)
    {
        // Czeka czas okreslony przez messageDuration
        yield return new WaitForSeconds(messageDuration);

        // Tworzy efektu zanikania tekstu
        float fadeTime = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            message.color = new Color(message.color.r, message.color.g, message.color.b, Mathf.Lerp(1, 0, elapsedTime / fadeTime));
            yield return null;
        }

        //Usuwa wiadomosc z listy
        if (allMessages.Contains(message))
            allMessages.Remove(message);
    }
}

