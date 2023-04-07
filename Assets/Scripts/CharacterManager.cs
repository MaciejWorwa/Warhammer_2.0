using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static GameObject GetSelectedCharacter()
    {
        if (Player.trSelect != null && Enemy.trSelect == null)
            return Player.selectedPlayer;
        else if (Enemy.trSelect != null && Player.trSelect == null)
            return Enemy.selectedEnemy;
        else
            return null;
    }
}
