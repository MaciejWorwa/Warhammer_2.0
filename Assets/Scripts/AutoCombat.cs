using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCombat : MonoBehaviour
{
    private GameObject[] enemies;
    private GameObject[] players;

    public MovementManager movementManager;

    public static bool AutoCombatOn = true;

    void Start()
    {
        movementManager = movementManager.gameObject.GetComponent<MovementManager>();
    }

    public void SetAutoCombat()
    {
        if(AutoCombatOn)
        {
            AutoCombatOn = false;
            Debug.Log($"<color=green>Walka automatyczna zosta�a wy��czona.</color>");
        }
        else
        {
            AutoCombatOn = true;
            Debug.Log($"<color=green>Walka automatyczna zosta�a aktywowana.</color>");
        }

    }

    // Wykonuje automatyczne akcje za ka�d� posta�
    public void AutomaticActions()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject enemy in enemies)
        {
            Enemy.selectedEnemy = enemy;
            GameObject closestPlayer = GetClosestOpponent(players, enemy.transform);

            float distance = Vector3.Distance(closestPlayer.transform.position, enemy.transform.position);

            if (enemy.GetComponent<Stats>().AttackRange >= distance)
                enemy.GetComponent<Enemy>().attackManager.Attack(enemy, closestPlayer);
            else
            {
                // Tu bedzie wywoanie funkcji ruchu w kierunku najblizszego gracza
                //MoveSelectedCharacter(GameObject selectedTile) <--- MUSZ� ZNALEZC SPOSOB NA WLASCIWE USTALENIE NA KTORY KONKRETNIE TILE ENEMY POWINIEN SIE PRZESUNAC, PONIZEJ JEST POMOC OD CHATU-GPT
            }
        }
        foreach (GameObject player in players)
        {
            Player.selectedPlayer = player;
            GameObject closestEnemy = GetClosestOpponent(enemies, player.transform);

            player.GetComponent<Player>().attackManager.Attack(player, closestEnemy);
        }
    }

    GameObject GetClosestOpponent(GameObject[] characters, Transform currentCharacter)
    {
        GameObject closestCharacter = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = currentCharacter.position;
        foreach (GameObject character in characters)
        {
            float dist = Vector3.Distance(character.transform.position, currentPos);
            if (dist < minDist)
            {
                closestCharacter = character;
                minDist = dist;
            }
        }
        return closestCharacter;
    }
}

// KOD OD CHATU-GPT DO PRZEROBIENIA.
/*
// Oblicz kierunek do gracza
Vector2 direction = player.transform.position - transform.position;

// Je�li dystans do gracza jest wi�kszy ni� szybko��, to znajd� nowe pole z tagiem "Tile"
if (direction.magnitude > speed)
{
    // Normalizuj kierunek i przemn� go przez szybko��, aby uzyska� docelowe po�o�enie
    direction.Normalize();
    Vector2 targetPosition = transform.position + direction * speed;

    // Sprawd�, czy na docelowym polu znajduje si� ju� inny obiekt
    Collider2D[] colliders = Physics2D.OverlapBoxAll(targetPosition, Vector2.one * 0.9f, 0, tileLayer);
    Collider2D targetCollider = null;
    float minDistance = Mathf.Infinity;

    foreach (Collider2D collider in colliders)
    {
        // Sprawd�, czy to pole jest ju� zaj�te przez inny obiekt
        if (collider.gameObject != gameObject && collider.gameObject != player)
        {
            // Oblicz odleg�o�� od pola do przeciwnika
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < minDistance)
            {
                targetCollider = collider;
                minDistance = distance;
            }
        }
    }

    // Je�li znaleziono docelowe pole, to przemie�� przeciwnika na nie
    if (targetCollider != null)
    {
        transform.position = targetCollider.transform.position;
    }
}
*/