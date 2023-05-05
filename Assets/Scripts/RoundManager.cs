using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public static int roundNumber;
    public AutoCombat autoCombat;

    [SerializeField] private TMP_Text roundNumberDisplay;

    void Start()
    {
        roundNumber = 1;
        autoCombat = autoCombat.gameObject.GetComponent<AutoCombat>();
    }

    void Update()
    {
        roundNumberDisplay.text = "Runda: " + roundNumber;
    }

    public void NextRound()
    {
        if(AutoCombat.AutoCombatOn)
        {
            autoCombat.AutomaticActions();

            // Ponowne usunięcie postaci, które są w stanie krytycznym, dlatego że wewnątrz AutoCombatu z jakiegoś powodu nie zawsze to działa dobrze
            Stats[] characters = FindObjectsOfType<Stats>();
            foreach (var character in characters)
            {
                if (character.GetComponent<Stats>().tempHealth < 0)
                    Destroy(character.gameObject);
            }
        }

        roundNumber++;

        Stats[] allObjectsWithStats = FindObjectsOfType<Stats>();

        //przywracanie parowania i/lub uników każdej postaci wraz z nową rundą
        foreach (Stats obj in allObjectsWithStats)
        {
            obj.GetComponent<Stats>().ResetParryAndDodge();
            obj.GetComponent<Stats>().ResetActionsNumber();
        }

        Debug.Log($"======================= RUNDA {roundNumber} =======================");
    }
}
