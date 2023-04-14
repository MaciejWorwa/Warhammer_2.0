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
        if (Player.trSelect != null && GameObject.Find("ActionsButtonsPlayer/Canvas") != null)
            wizard = Player.selectedPlayer;
        else if (Enemy.trSelect != null && GameObject.Find("ActionsButtonsEnemy/Canvas") != null)
            wizard = Enemy.selectedEnemy;

        int rollResult = Random.Range(1, 101);

        if (wizard.GetComponent<Stats>().SW >= rollResult)
        {
            powerLevel += wizard.GetComponent<Stats>().Mag;
            Debug.Log($"Wynik rzutu: {rollResult}. Splatanie magii zako�czone sukcesem.");
        }
        else
            Debug.Log($"Wynik rzutu: {rollResult}. Splatanie magii zako�czone niepowodzeniem.");
    }

    // Rzucenie zakl�cia
    public void CastASpell()
    {
        // Ustalenie kto rzuca zakl�cie
        if (Player.trSelect != null && GameObject.Find("ActionsButtonsPlayer/Canvas") != null)
            wizard = Player.selectedPlayer;
        else if (Enemy.trSelect != null && GameObject.Find("ActionsButtonsEnemy/Canvas") != null)
            wizard = Enemy.selectedEnemy;

        // Lista i s�ownik wszystkich wynik�w rzut�w, potrzebne do sprawdzenia wyst�pienia manifestacji chaosu
        List<int> allRollResults = new List<int>();
        Dictionary<int, int> doubletCount = new Dictionary<int, int>();

        // Rzuty na poziom mocy w zale�no�ci od warto�ci Magii
        for (int i = 0; i < wizard.GetComponent<Stats>().Mag; i++)
        {
            int rollResult = Random.Range(1, 11);
            allRollResults.Add(rollResult);
            powerLevel += rollResult;
            Debug.Log($"Wynik rzutu na poziom mocy, ko�� {i+1}: {rollResult}");
        }
        Debug.Log($"Uzyskany poziom mocy: <color=red>{powerLevel}</color>");


        // Liczenie dublet�w
        foreach (int rollResult in allRollResults)
        {
            if (doubletCount.ContainsKey(rollResult))
                doubletCount[rollResult] += 1; // je�li warto�� istnieje w s�owniku, zwi�kszamy jej licznik
            else
                doubletCount.Add(rollResult, 1); // je�li warto�� nie istnieje w s�owniku, dodajemy j� i ustawiamy licznik na 1
        }

        // Rzuty na manifestacj� w zale�no�ci od ilo�ci wynik�w, kt�re si� powt�rzy�y
        foreach (KeyValuePair<int, int> kvp in doubletCount)
        {
            int value = kvp.Key;
            int count = kvp.Value;
            if (count == 2)
            {
                int rollResult = Random.Range(1, 101);
                Debug.Log($"Warto�� {value} wyst�puje {count} razy.");
                Debug.Log($"<color=red>POMNIEJSZA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestacj�: {rollResult}");
            }
            else if (count == 3)
            {
                int rollResult = Random.Range(1, 101);
                Debug.Log($"Warto�� {value} wyst�puje {count} razy.");
                Debug.Log($"<color=red>POWA�NA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestacj�: {rollResult}");
            }
            else if (count > 3)
            {
                int rollResult = Random.Range(1, 101);
                Debug.Log($"Warto�� {value} wyst�puje {count} razy.");
                Debug.Log($"<color=red>KATASTROFALNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestacj�: {rollResult}");
            }
        }
        // Zresetowanie poziomu mocy
        powerLevel = 0;
    }
}
