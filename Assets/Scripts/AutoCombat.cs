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

    public static bool AutoCombatOn = false;

    private MessageManager messageManager;

    void Start()
    {
        movementManager = movementManager.gameObject.GetComponent<MovementManager>();

        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
    }

    public void SetAutoCombat()
    {
        if (AutoCombatOn)
        {
            AutoCombatOn = false;
            messageManager.ShowMessage($"<color=#00FF9A>Walka automatyczna została wyłączona.</color>", 3f);
            Debug.Log($"Walka automatyczna została wyłączona.");
        }
        else
        {
            AutoCombatOn = true;
            messageManager.ShowMessage($"<color=#00FF9A>Walka automatyczna została aktywowana.</color>", 3f);
            Debug.Log($"Walka automatyczna została aktywowana.");
        }

    }

    // Wykonuje automatyczne akcje za kazda postac
    public void AutomaticActions()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        players = GameObject.FindGameObjectsWithTag("Player");

        // Gdy na polu bitwy wystepuja tylko postacie Enemy albo tylko postacie Player to automatyczna akcja nie jest wykonywana
        if (enemies.Length == 0 || players.Length == 0)
            return;

        // Polaczenie tablicy enemies z tablica players. Zbior wszystkich postaci.
        // Potem sortuje tablice wg inicjatywy. Domyslnie jest rosnaco, dlatego pozniej jeszcze obracamy kolejnosc przy pomocy Reverse()
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

            // Jesli rywal jest w zasiegu ataku to wykonuje atak
            if (character.GetComponent<Stats>().AttackRange >= distance)
            {
                // Przekazuje informacje o atakujacym i ilosci atakow, ktore moze wykonac w jednej rundzie
                AutomaticAttack(character, closestOpponent, character.GetComponent<Stats>().A);
            }
            else
            {
                // Wlaczenie opcji poruszania
                MovementManager.canMove = true;

                // wektor we wszystkie osiem kierunkow dookola pola z postacia bedaca celem ataku
                Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, new Vector3(1, 1, 0), new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0) };

                // lista pol przylegajacych do postaci
                List<GameObject> adjacentTiles = new List<GameObject>();

                // Szuka pol w każdym kierunku
                foreach (Vector3 direction in directions)
                {
                    // znajdz wszystkie kolidery w miejscach przylegajacych do postaci bedacej celem ataku
                    // jesli w jednym miejscu wystepuje wiecej niz jeden collider to oznacza, ze pole jest zajete przez postac, wtedy nie dodajemy tego collidera do listy adjacentTiles
                    Collider2D[] collider = Physics2D.OverlapCircleAll(closestOpponent.transform.position + direction, 0.1f);
                    if (collider != null && collider.Length == 1 && collider[0].CompareTag("Tile"))
                    {
                        adjacentTiles.Add(collider[0].gameObject);
                    }
                }

                // Zamienia liste na tablice, zeby pozniej mozna bylo ja posortowac
                GameObject[] adjacentTilesArray = adjacentTiles.ToArray();

                // Sortuje przylegajace do postaci pola wg odleglosci od atakujacego. Te ktore sa najblizej znajduja sie na poczatku tablicy
                Array.Sort(adjacentTilesArray, (x, y) => Vector3.Distance(x.transform.position, character.transform.position).CompareTo(Vector3.Distance(y.transform.position, character.transform.position)));


                // Sprawdza dystans do pola docelowego
                float distanceBetweenOpponents = 0f;
                if (adjacentTilesArray.Length > 0)
                    distanceBetweenOpponents = (Mathf.Abs(character.transform.position.x - adjacentTilesArray[0].transform.position.x)) + (Mathf.Abs(character.transform.position.y - adjacentTilesArray[0].transform.position.y));

                if (distanceBetweenOpponents > character.GetComponent<Stats>().tempSz * 2) // Jesli jest poza zasiegiem szarzy
                {
                    GameObject.Find("MovementManager").GetComponent<MovementManager>().MoveSelectedCharacter(adjacentTilesArray[0], character);
                    Physics2D.SyncTransforms(); // Synchronizuje collidery (inaczej Collider2D nie wykrywa zmian pozycji postaci)
                }
                else if (distanceBetweenOpponents >= 3 && distanceBetweenOpponents <= character.GetComponent<Stats>().tempSz * 2) // Jesli jest w zasiegu szarzy
                {
                    //Wykonanie szarzy
                    if (adjacentTilesArray.Length > 0)
                    {
                        MovementManager.Charge = true;
                        character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz * 2;

                        GameObject.Find("MovementManager").GetComponent<MovementManager>().MoveSelectedCharacter(adjacentTilesArray[0], character);
                        Physics2D.SyncTransforms(); // Synchronizuje collidery (inaczej Collider2D nie wykrywa zmian pozycji postaci)

                        AutomaticAttack(character, closestOpponent, 1); // Wykonywany jest jeden atak z bonusem +10, bo to szarza

                        // Zresetowanie szarzy
                        character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz;
                        MovementManager.Charge = false;
                    }
                }
                else if (distanceBetweenOpponents < 3) // Jesli jest zbyt blisko na szarze
                {
                    if (adjacentTilesArray.Length > 0)
                    {
                        GameObject.Find("MovementManager").GetComponent<MovementManager>().MoveSelectedCharacter(adjacentTilesArray[0], character);
                        Physics2D.SyncTransforms(); // Synchronizuje collidery (inaczej Collider2D nie wykrywa zmian pozycji postaci)

                        AutomaticAttack(character, closestOpponent, 1); // Wykonywany jest jeden atak, bo wczesniej zostal wykonany ruch
                    }
                }
            }

            // Usuwa z pola bitwy postacie, ktorych zywotnosc spadla ponizej 0
            if (closestOpponent.GetComponent<Stats>().tempHealth < 0)
            {
                Destroy(closestOpponent);
                GameObject.Find("ButtonManager").GetComponent<ButtonManager>().ShowOrHideActionsButtons(closestOpponent, false);
            }
                
        }
    }

    void AutomaticAttack(GameObject character, GameObject closestOpponent, int attacksAmount)
    {
        if (character.tag == "Enemy")
        {
            // Wykonuje tyle atakow ile wynosi cecha Ataki postaci
            for (int i = 0; i < attacksAmount; i++)
            {
                // Jesli bron nie wymaga naladowania to wykonuje atak, w przeciwnym razie wykonuje ladowanie
                if (character.GetComponent<Stats>().reloadLeft == 0)
                    character.GetComponent<Enemy>().attackManager.Attack(character, closestOpponent);
                else if (character.GetComponent<Stats>().reloadLeft == 1)
                {
                    character.GetComponent<Enemy>().attackManager.Reload();
                    character.GetComponent<Enemy>().attackManager.Attack(character, closestOpponent);
                    return;
                }
                else if (character.GetComponent<Stats>().reloadLeft > 1)
                {
                    character.GetComponent<Enemy>().attackManager.Reload();
                    character.GetComponent<Enemy>().attackManager.Reload();
                    return;
                }
            }
        }
        else
        {
            // Wykonuje tyle atakow ile wynosi cecha Ataki postaci
            for (int i = 0; i < attacksAmount; i++)
            {
                // Jesli bron nie wymaga naladowania to wykonuje atak, w przeciwnym razie wykonuje ladowanie
                if (character.GetComponent<Stats>().reloadLeft == 0)
                    character.GetComponent<Player>().attackManager.Attack(character, closestOpponent);
                else if (character.GetComponent<Stats>().reloadLeft == 1)
                {
                    character.GetComponent<Player>().attackManager.Reload();
                    character.GetComponent<Player>().attackManager.Attack(character, closestOpponent);
                    return;
                }
                else if (character.GetComponent<Stats>().reloadLeft > 1)
                {
                    character.GetComponent<Player>().attackManager.Reload();
                    character.GetComponent<Player>().attackManager.Reload();
                    return;
                }
            }
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