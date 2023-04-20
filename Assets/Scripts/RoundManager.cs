using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    private int roundNumber;
    public AutoCombat autoCombat;

    [SerializeField] private TMP_Text roundNumberDisplay;

    void Start()
    {
        roundNumber = 1;
        roundNumberDisplay.text = "Runda: " + roundNumber;

        autoCombat = autoCombat.gameObject.GetComponent<AutoCombat>();
    }

    public void NextRound()
    {
        if(AutoCombat.AutoCombatOn)
            autoCombat.AutomaticActions();

        roundNumber++;
        roundNumberDisplay.text = "Runda: " + roundNumber;

        Stats[] allObjectsWithStats = FindObjectsOfType<Stats>();

        //przywracanie parowania i/lub uników każdej postaci wraz z nową rundą
        foreach (Stats obj in allObjectsWithStats)
        {
            obj.GetComponent<Stats>().ResetParryAndDodge();
            obj.GetComponent<Stats>().ResetActionsNumber();
        }

    }
}
