using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicManager : MonoBehaviour
{
    private int powerLevel;
    private GameObject wizard;

    private MessageManager messageManager;

    void Start()
    {
        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
    }

    // Splatanie magii
    public void ChannelingMagic()
    {
        wizard = CharacterManager.GetSelectedCharacter();

        int rollResult = Random.Range(1, 101);

        if (wizard.GetComponent<Stats>().SW >= rollResult)
        {
            powerLevel += wizard.GetComponent<Stats>().Mag;
            messageManager.ShowMessage($"Wynik rzutu: {rollResult}. Splatanie magii zakończone sukcesem.", 4f);
            Debug.Log($"Wynik rzutu: {rollResult}. Splatanie magii zakończone sukcesem.");
        }
        else
        {
            powerLevel = 0;
            messageManager.ShowMessage($"Wynik rzutu: {rollResult}. Splatanie magii zakończone niepowodzeniem.", 4f);
            Debug.Log($"Wynik rzutu: {rollResult}. Splatanie magii zakończone niepowodzeniem.");
        }
    }

    // Rzucenie zaklęcia
    public void CastASpell()
    {
        // Ustalenie kto rzuca zaklęcie
        wizard = CharacterManager.GetSelectedCharacter();

        // Lista i słownik wszystkich wyników rzutów, potrzebne do sprawdzenia wystąpienia manifestacji chaosu
        List<int> allRollResults = new List<int>();
        Dictionary<int, int> doubletCount = new Dictionary<int, int>();

        // Rzuty na poziom mocy w zależności od wartości Magii
        for (int i = 0; i < wizard.GetComponent<Stats>().Mag; i++)
        {
            int rollResult = Random.Range(1, 11);
            allRollResults.Add(rollResult);
            powerLevel += rollResult;

            messageManager.ShowMessage($"Wynik rzutu na poziom mocy, kość {i + 1}: {rollResult}", 6f);
            Debug.Log($"Wynik rzutu na poziom mocy, kość {i+1}: {rollResult}");
        }
        messageManager.ShowMessage($"Uzyskany poziom mocy: <color=red>{powerLevel}</color>", 6f);
        Debug.Log($"Uzyskany poziom mocy: <color=red>{powerLevel}</color>");


        // Liczenie dubletów
        foreach (int rollResult in allRollResults)
        {
            if (doubletCount.ContainsKey(rollResult))
                doubletCount[rollResult] += 1; // jeśli wartość istnieje w słowniku, zwiększamy jej licznik
            else
                doubletCount.Add(rollResult, 1); // jeśli wartość nie istnieje w słowniku, dodajemy ją i ustawiamy licznik na 1
        }

        // Rzuty na manifestację w zależności od ilości wyników, które się powtórzyły
        foreach (KeyValuePair<int, int> kvp in doubletCount)
        {
            int value = kvp.Key;
            int count = kvp.Value;
            if (count == 2)
            {
                int rollResult = Random.Range(1, 101);
                messageManager.ShowMessage($"Wartość {value} występuje {count} razy.", 6f);
                Debug.Log($"Wartość {value} występuje {count} razy.");
                messageManager.ShowMessage($"<color=red>POMNIEJSZA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}", 6f);
                Debug.Log($"<color=red>POMNIEJSZA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}");
            }
            else if (count == 3)
            {
                int rollResult = Random.Range(1, 101);
                messageManager.ShowMessage($"Wartość {value} występuje {count} razy.", 6f);
                Debug.Log($"Wartość {value} występuje {count} razy.");
                messageManager.ShowMessage($"<color=red>POWAŻNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}", 6f);
                Debug.Log($"<color=red>POWAŻNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}");
            }
            else if (count > 3)
            {
                int rollResult = Random.Range(1, 101);
                messageManager.ShowMessage($"Wartość {value} występuje {count} razy.", 6f);
                Debug.Log($"Wartość {value} występuje {count} razy.");
                messageManager.ShowMessage($"<color=red>KATASTROFALNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}", 6f);
                Debug.Log($"<color=red>KATASTROFALNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}");
            }
        }
        // Zresetowanie poziomu mocy
        powerLevel = 0;
    }
}
