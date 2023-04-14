using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject setStatsButton;
    [SerializeField] private GameObject destroyButton;

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
        // Wylacza widocznosc przycisku zmiany statystyk i usuwania postaci, jesli postac nie jest wybrana
        if (Player.trSelect == null && Enemy.trSelect == null)
        {
            destroyButton.SetActive(false);
            setStatsButton.SetActive(false);
        }
        else
        {
            destroyButton.SetActive(true);
            setStatsButton.SetActive(true);
        }

    }
}
