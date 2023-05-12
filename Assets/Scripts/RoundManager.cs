using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.TextCore.Text;
using System.Linq;

public class RoundManager : MonoBehaviour
{
    public static int roundNumber;
    public AutoCombat autoCombat;

    [SerializeField] private TMP_Text roundNumberDisplay;
    [SerializeField] private TMP_Text nextRoundButtonText;

    private CharacterManager characterManager;

    void Start()
    {
        roundNumber = 0;
        autoCombat = autoCombat.gameObject.GetComponent<AutoCombat>();

        // Odniesienie do menadżera postaci
        characterManager = GameObject.Find("CharacterManager").GetComponent<CharacterManager>();
    }

    void Update()
    {
        if (roundNumber == 0)
        {
            roundNumberDisplay.text = "Gotów?";
            nextRoundButtonText.text = "Start";
        }
        else
        {
            roundNumberDisplay.text = "Runda: " + roundNumber;
            nextRoundButtonText.text = "+";
        }
    }

    public void NextRound()
    {
        if (GameManager.AutoMode)
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

        // Ilość Enemies z cechą Straszny
        bool scaryEnemyExist = false;

        //przywracanie parowania i/lub uników każdej postaci wraz z nową rundą
        foreach (Stats obj in allObjectsWithStats)
        {
            characterManager.ResetParryAndDodge(obj.GetComponent<Stats>());
            characterManager.ResetActionsNumber(obj.GetComponent<Stats>());

            if (obj.isScary)
                scaryEnemyExist = true;
        }

        if(allObjectsWithStats.Length > 0)
            GameObject.Find("CharacterManager").GetComponent<CharacterManager>().SelectCharacterWithBiggestInitiative(scaryEnemyExist, allObjectsWithStats);

        GameObject.Find("MessageManager").GetComponent<MessageManager>().ShowMessage($"<color=#FFE100>RUNDA {roundNumber}</color>", 3f);
        Debug.Log($"======================= RUNDA {roundNumber} =======================");
    }

    public void EndSelectedCharacterTurn()
    {
        Character.selectedCharacter.GetComponent<Stats>().actionsLeft = 0;
    }
}
