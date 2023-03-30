using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    private int roundNumber;

    [SerializeField] private TMP_Text roundNumberDisplay;

    void Start()
    {
        roundNumber = 1;
        roundNumberDisplay.text = "Runda: " + roundNumber;
    }

    public void NextRound()
    {
        roundNumber++;
        roundNumberDisplay.text = "Runda: " + roundNumber;

        Stats[] allObjectsWithStats = FindObjectsOfType<Stats>();

        //przywracanie parowania i/lub unik�w ka�dej postaci wraz z now� rund�
        foreach (Stats obj in allObjectsWithStats)
        {
            obj.GetComponent<Stats>().ResetParryAndDodge();
            //obj.GetComponent<Stats>().ResetActionsNumber();
        }

    }
}
