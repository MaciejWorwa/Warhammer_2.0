using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject setStatsButton;

    public static GameObject GetSelectedCharacter()
    {
        if (Player.trSelect != null && Enemy.trSelect == null)
            return Player.selectedPlayer;
        else if (Enemy.trSelect != null && Player.trSelect == null)
            return Enemy.selectedEnemy;
        else
            return null;
    }

    void Update()
    {
        // Wylacza widocznosc przycisku zmiany statystyk, jesli sa wybrane dwie postacie, albo nie jest wybrana zadna
        if (Player.trSelect != null && Enemy.trSelect != null || Player.trSelect == null && Enemy.trSelect == null)
            setStatsButton.SetActive(false);
        else
            setStatsButton.SetActive(true);
    }
}
