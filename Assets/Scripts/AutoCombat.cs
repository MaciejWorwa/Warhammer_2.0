using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AutoCombat : MonoBehaviour
{
    private GameObject[] enemies;
    private GameObject[] players;

    private GameObject[] characters;

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
            Debug.Log($"<color=green>Walka automatyczna zosta³a wy³¹czona.</color>");
        }
        else
        {
            AutoCombatOn = true;
            Debug.Log($"<color=green>Walka automatyczna zosta³a aktywowana.</color>");
        }

    }

    // Wykonuje automatyczne akcje za ka¿d¹ postaæ
    public void AutomaticActions()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        players = GameObject.FindGameObjectsWithTag("Player");

        // Polaczenie tablicy enemies z tablica players. Zbior wszystkich postaci.
        // Potem sortuje tablice wg inicjatywy. Domyœlnie jest rosnaco, dlatego pozniej jeszcze obracamy kolejnosc przy pomocy Reverse()
        characters = enemies.Concat(players).ToArray();
        Array.Sort(characters, delegate (GameObject x, GameObject y) { return x.GetComponent<Stats>().Initiative.CompareTo(y.GetComponent<Stats>().Initiative); });
        Array.Reverse(characters);

        foreach (GameObject character in characters)
        {
            GameObject closestOpponent;

            if (character.tag == "Enemy")
            {
                Enemy.selectedEnemy = character;
                closestOpponent = GetClosestOpponent(players, character.transform);
            }
            else
            {
                Player.selectedPlayer = character;
                closestOpponent = GetClosestOpponent(enemies, character.transform);
            }

            float distance = Vector3.Distance(closestOpponent.transform.position, character.transform.position);

            if (character.GetComponent<Stats>().AttackRange >= distance)
            {
                if(character.tag == "Enemy")
                    character.GetComponent<Enemy>().attackManager.Attack(character, closestOpponent);
                else
                    character.GetComponent<Player>().attackManager.Attack(character, closestOpponent);
            }
            else
            {
                Debug.Log("Tutaj bedzie mechanika ruchu w kierunku postaci :)"); // TUTAJ MUSZE COS ROZKMINIC. PONIZEJ SA ZAKOMENTOWANE POPRZEDNIE PROBY
            }

            // Usuwa z pola bitwy postacie, ktorych zywotnosc spadla ponizej 0
            if (closestOpponent.GetComponent<Stats>().tempHealth < 0)
                Destroy(closestOpponent);
        }



        /*
        foreach (GameObject enemy in enemies)
        {
            Enemy.selectedEnemy = enemy;
            GameObject closestPlayer = GetClosestOpponent(players, enemy.transform);

            Debug.Log($"Inicjatywa {enemy} to {enemy.GetComponent<Stats>().Initiative}");

            float distance = Vector3.Distance(closestPlayer.transform.position, enemy.transform.position);

            if (enemy.GetComponent<Stats>().AttackRange >= distance)
                enemy.GetComponent<Enemy>().attackManager.Attack(enemy, closestPlayer);
            else
            {
                /*
                //TO PONIZEJ NIE DZIALA POPRAWNIE. TRZEBA NAD TYM POKMINIC. WROGOWIE CZASAMI STAJA NA INNE POLE ZAJETE PRZEZ INNEGO WROGA, A CZASAMI ZATRZYMUJA SIE I NIE IDA DALEJ
                

                enemy.layer = 0; // zresetowanie warstwy aktualnego Enemy na 'Default' po to, ¿eby sam siebie nie wykrywa³ jako collidera

                // Porusza wroga w stronê bohatera gracza w jednym z czterech kierunkow. Wykonuje pojedynczy ruch tyle razy ile wynosi aktualna szybkosc wroga.
                for (int i = 0; i < enemy.GetComponent<Stats>().tempSz; i++)
                {
                    Collider2D collider;
                    LayerMask layer = LayerMask.GetMask("Character"); // wartwa wspolna dla wrogow i bohaterow graczy

                    // kazdy ponizszy 'if' odpowiada za ruch w innym kierunku (prawo, lewo, gora, dol). Sa zdeterminowane pozycja atakujacego enemy i najbliszego celu (playera)
                    if (closestPlayer.transform.position.x > enemy.transform.position.x && distance > 1.5f)
                    {
                        collider = Physics2D.OverlapCircle(enemy.transform.position, 1f, layer);

                        if (collider == null || collider.transform.position.x < enemy.transform.position.x || collider.transform.position.y != enemy.transform.position.y)
                        {
                            enemy.transform.position += Vector3.right;                           
                        }
                        else
                        {
                            Debug.Log("ktos stoi na drodze");
                        }
                        // TE ZAKOMENTOWANE PONIZEJ TO NIEUDANE PROBY OMIJANIA POSTACI STOJACYCH NA DRODZE. POLEGAJA NA ZMIANIE RUCHU Z POZIOMEGO NA PIONOWY LUB ODWROTNIE, ZAWSZE W KIERUNKU CELU
                        //else
                        //{
                        //    if(enemy.transform.position.y < closestPlayer.transform.position.y)
                        //        enemy.transform.position += Vector3.up;
                        //    if (enemy.transform.position.y > closestPlayer.transform.position.y)
                        //        enemy.transform.position += Vector3.down;
                        //}

                        distance = Vector3.Distance(closestPlayer.transform.position, enemy.transform.position);
                    }
                    else if (closestPlayer.transform.position.x < enemy.transform.position.x && distance > 1.5f)
                    {
                        collider = Physics2D.OverlapCircle(enemy.transform.position, 1f, layer);

                        if (collider == null || collider.transform.position.x > enemy.transform.position.x || collider.transform.position.y != enemy.transform.position.y)
                        {
                            enemy.transform.position += Vector3.left;
                        }
                        else
                        {
                            Debug.Log("ktos stoi na drodze");
                        }
                        //else
                        //{
                        //    if (enemy.transform.position.y < closestPlayer.transform.position.y)
                        //        enemy.transform.position += Vector3.up;
                        //    if (enemy.transform.position.y > closestPlayer.transform.position.y)
                        //        enemy.transform.position += Vector3.down;
                        //}

                        distance = Vector3.Distance(closestPlayer.transform.position, enemy.transform.position);
                    }
                    else if (closestPlayer.transform.position.y > enemy.transform.position.y && distance > 1.5f)
                    {
                        collider = Physics2D.OverlapCircle(enemy.transform.position, 1f, layer);

                        if (collider == null || collider.transform.position.y < enemy.transform.position.y || collider.transform.position.x != enemy.transform.position.x)
                        {
                            enemy.transform.position += Vector3.up;
                        }
                        else
                        {
                            Debug.Log("ktos stoi na drodze");
                        }
                        //else
                        //{
                        //    if (enemy.transform.position.x < closestPlayer.transform.position.x)
                        //        enemy.transform.position += Vector3.right;
                        //    if (enemy.transform.position.x > closestPlayer.transform.position.x)
                        //        enemy.transform.position += Vector3.left;
                        //}

                        distance = Vector3.Distance(closestPlayer.transform.position, enemy.transform.position);
                    }
                    else if (closestPlayer.transform.position.y < enemy.transform.position.y && distance > 1.5f)
                    {
                        collider = Physics2D.OverlapCircle(enemy.transform.position, 1f, layer);

                        if (collider == null || collider.transform.position.y > enemy.transform.position.y || collider.transform.position.x != enemy.transform.position.x)
                        {
                            enemy.transform.position += Vector3.down;
                        }
                        else
                        {
                            Debug.Log("ktos stoi na drodze");
                        }
                        //else
                        //{
                        //    if (enemy.transform.position.x < closestPlayer.transform.position.x)
                        //        enemy.transform.position += Vector3.right;
                        //    if (enemy.transform.position.x > closestPlayer.transform.position.x)
                        //        enemy.transform.position += Vector3.left;
                        //}

                        distance = Vector3.Distance(closestPlayer.transform.position, enemy.transform.position);                       
                    }
                }

                enemy.layer = 6;
                
            }
        }
        foreach (GameObject player in players)
        {
            Player.selectedPlayer = player;
            GameObject closestEnemy = GetClosestOpponent(enemies, player.transform);

            player.GetComponent<Player>().attackManager.Attack(player, closestEnemy);
        }
        */
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

// Jeœli dystans do gracza jest wiêkszy ni¿ szybkoœæ, to znajdŸ nowe pole z tagiem "Tile"
if (direction.magnitude > speed)
{
    // Normalizuj kierunek i przemnó¿ go przez szybkoœæ, aby uzyskaæ docelowe po³o¿enie
    direction.Normalize();
    Vector2 targetPosition = transform.position + direction * speed;

    // SprawdŸ, czy na docelowym polu znajduje siê ju¿ inny obiekt
    Collider2D[] colliders = Physics2D.OverlapBoxAll(targetPosition, Vector2.one * 0.9f, 0, tileLayer);
    Collider2D targetCollider = null;
    float minDistance = Mathf.Infinity;

    foreach (Collider2D collider in colliders)
    {
        // SprawdŸ, czy to pole jest ju¿ zajête przez inny obiekt
        if (collider.gameObject != gameObject && collider.gameObject != player)
        {
            // Oblicz odleg³oœæ od pola do przeciwnika
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < minDistance)
            {
                targetCollider = collider;
                minDistance = distance;
            }
        }
    }

    // Jeœli znaleziono docelowe pole, to przemieœæ przeciwnika na nie
    if (targetCollider != null)
    {
        transform.position = targetCollider.transform.position;
    }
}
*/