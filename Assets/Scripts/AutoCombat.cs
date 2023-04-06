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

    void Start()
    {
        movementManager = movementManager.gameObject.GetComponent<MovementManager>();
    }

    public void SetAutoCombat()
    {
        if (AutoCombatOn)
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

    // Wykonuje automatyczne akcje za kazda postac
    public void AutomaticActions()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        players = GameObject.FindGameObjectsWithTag("Player");

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

            // Jeœli rywal jest w zasiegu ataku to wykonuje atak
            if (character.GetComponent<Stats>().AttackRange >= distance)
            {
                if (character.tag == "Enemy")
                {
                    // Wykonuje tyle ataków ile wynosi cecha Ataki postaci
                    for (int i = 0; i < character.GetComponent<Stats>().A; i++)
                    {
                        // Jesli bron nie wymaga naladowania to wykonuje atak, w przeciwnym razie wykonuje ladowanie
                        if (character.GetComponent<Stats>().reloadLeft == 0)
                            character.GetComponent<Enemy>().attackManager.Attack(character, closestOpponent);
                        else
                            character.GetComponent<Enemy>().attackManager.ReloadEnemy();
                    }

                }
                else
                {
                    // Wykonuje tyle ataków ile wynosi cecha Ataki postaci
                    for (int i = 0; i < character.GetComponent<Stats>().A; i++)
                    {
                        // Jesli bron nie wymaga naladowania to wykonuje atak, w przeciwnym razie wykonuje ladowanie
                        if (character.GetComponent<Stats>().reloadLeft == 0)
                            character.GetComponent<Player>().attackManager.Attack(character, closestOpponent);
                        else
                            character.GetComponent<Player>().attackManager.ReloadPlayer();
                    }
                }

            }
            else
            {
                // Postac wykonuje ruch w kierunku najblizszego rywala tyle razy ile wynosi jej aktualna szybkosc
                for (int i = 0; i < character.GetComponent<Stats>().tempSz; i++)
                {
                    int XorY = UnityEngine.Random.Range(0, 2);

                    // Ustala losowa kolejnosc sprawdzania roznicy dystansu (raz najpierw sprawdza w osi x, raz w osi y).
                    // Zapobiega to blokowaniu sie postaci w jednej osi, gdy pomiedzy atakujacym a celem stoi sojusznik atakujacego
                    if(XorY == 0 || closestOpponent.transform.position.y == character.transform.position.y)
                    {
                        // Sprawdza w ktorym kierunku powinien wykonac ruch.
                        // Jezeli wejdzie na pole zajete przez inna postac, szuka wolnego pola tu¿ obok.
                        if (closestOpponent.transform.position.x > character.transform.position.x)
                            character.transform.position = ChangeCharacterPosition(character, Vector3.right);
                        else if (closestOpponent.transform.position.x < character.transform.position.x)
                            character.transform.position = ChangeCharacterPosition(character, Vector3.left);
                    }
                    else if (XorY != 0 || closestOpponent.transform.position.x == character.transform.position.x)
                    {
                        // Sprawdza w ktorym kierunku powinien wykonac ruch.
                        // Jezeli wejdzie na pole zajete przez inna postac, szuka wolnego pola tu¿ obok.
                        if (closestOpponent.transform.position.y > character.transform.position.y)
                            character.transform.position = ChangeCharacterPosition(character, Vector3.up);
                        else if (closestOpponent.transform.position.y < character.transform.position.y)
                            character.transform.position = ChangeCharacterPosition(character, Vector3.down);
                    }
                }
            }

            // Usuwa z pola bitwy postacie, ktorych zywotnosc spadla ponizej 0
            if (closestOpponent.GetComponent<Stats>().tempHealth < 0)
                Destroy(closestOpponent);
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

    Vector3 FindFreeAdjacentPosition(Vector3 currentPosition, List<Vector3> otherPositions)
    {
        // Usuniecie pozycji atakujacego z listy zawierajacej pozycje wszystkich postaci
        otherPositions.Remove(currentPosition);

        // Zbiera wszystkie przylegle do postaci pola na ktore teoretycznie moglaby sie poruszyc
        Vector3[] adjacentPositions =
        {
        currentPosition + Vector3.right,
        currentPosition + Vector3.left,
        currentPosition + Vector3.up,
        currentPosition + Vector3.down
        };

        // Sortowanie elementow tablicy w losowy sposob, aby nie bylo tak, ze zawsze najpierw postac chce pojsc w prawo, potem w lewo itd.
        Vector3 tempVector3;
        for (int i = 0; i < adjacentPositions.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, adjacentPositions.Length);
            tempVector3 = adjacentPositions[rnd];
            adjacentPositions[rnd] = adjacentPositions[i];
            adjacentPositions[i] = tempVector3;
        }

        // Sprawdza czy pozycje przylegle do postaci sa zajete
        foreach (Vector3 position in adjacentPositions)
        {
            bool isFree = true;
            foreach (Vector3 otherPosition in otherPositions)
            {
                if (position == otherPosition)
                {
                    isFree = false;
                    break;
                }
            }
            if (isFree && position.x >= -8 && position.x <= 8 && position.y >= -4 && position.y <= 3) // w przyszlosci zamienic te stale wartosci na odniesienie do szerokosci i wysokosci grida
            {
                Debug.Log($"Zwracam nowa przylegla pozycje dla {currentPosition}: {position}");
                // Zwraca informacje o pierwszej wolnej przyleglej do postaci pozycji jesli jest wolna i znajduje sie w obrebie pola gry
                return position;
            }
        }
        // Jesli kazda pozycja przylegla do postaci jest zajeta to zwraca wektor zerowy
        return Vector3.zero;
    }

    Vector3 ChangeCharacterPosition(GameObject character, Vector3 direction)
    {
        Vector3 newPosition = character.transform.position + direction;
        bool canMove = true;

        foreach (GameObject ch in characters)
        {
            if (newPosition == ch.transform.position && ch != character)
            {
                canMove = false;
                break;
            }
        }

        if (!canMove)
        {
            newPosition = FindFreeAdjacentPosition(character.transform.position, characters.Select(ch => ch.transform.position).ToList());

            // PROBA WPROWADZENIA TUTAJ ATAKU OKAZYJNEGO, ALE FUNKCJA CHECKFOROPPORTUNITYATTACK NIESTETY TYLKO UWZGLEDNIA SELECTEDENEMY I SELECTEDPLAYER A NIE WSZYSTKICH
            //MovementManager moveMan = GameObject.Find("MovementManager").GetComponent<MovementManager>();
            //moveMan.CheckForOpportunityAttack(character, newPosition);
        }

        return newPosition;
    }
}