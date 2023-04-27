using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class MovementManager : MonoBehaviour
{
    public static bool canMove = false; // okresla czy postac moze wykonac ruch
    public static bool Charge; // szarza
    public static bool Run; //bieg

    // Lista wszystkich pol w zasiegu ruchu postaci
    private List<GameObject> tilesInMovementRange = new List<GameObject>();

    private GridManager grid;

    private MessageManager messageManager;

    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<GridManager>();

        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
    }

    public void ResetChargeAndRun()
    {
        Charge = false;
        Run = false;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject[] characters = enemies.Concat(players).ToArray();

        foreach(var character in characters) character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz;

        // Zresetowanie koloru wszystkich pol
        grid.ResetTileColors();
    }

    public void SetCharge()
    {
        // Ustalenie ktora postac jest wybrana
        GameObject character = Character.selectedCharacter;
        if (character == null) return;

        if (Charge || Run)
            ResetChargeAndRun();
        else if (!Charge)
        {
            Charge = true;
            character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz * 2; // zmiana aktualnej predkosci
            Run = false;
        }

        // Zresetowanie koloru wszystkich pol
        grid.ResetTileColors();

        // Aktualizacja podswietlonego zasiegu ruchu postaci
        HighlightTilesInMovementRange(character);
    }

    public void SetRun()
    {
        if (Charge || Run)
            ResetChargeAndRun();

        // Ustalenie ktora postac jest wybrana
        GameObject character = Character.selectedCharacter;
        if (character == null) return;

        if (!Run)
        {
            Run = true;
            character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz * 3; // zmiana aktualnej predkosci
            Charge = false;
        }

        // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
        grid.ResetTileColors();

        // Aktualizacja podswietlonego zasiegu ruchu postaci
        HighlightTilesInMovementRange(character);
    }

    //aktywuje mozliwosc ruchu
    public void ActiveMove()
    {
        canMove = true;

        //wylaczanie widocznosci buttonow akcji
        if (Character.trSelect != null && GameObject.Find("ActionsButtons/Canvas") != null)
        {
            GameObject.Find("ActionsButtons/Canvas").SetActive(false);

            HighlightTilesInMovementRange(Character.selectedCharacter);
        }
    }

    public void MoveSelectedCharacter(GameObject selectedTile, GameObject character)
    {
        // Sprawdza, czy akcja wykonywania ruchu jest aktywna
        if(canMove)
        {
            // Sprawdza zasieg ruchu postaci
            int movementRange = character.GetComponent<Stats>().tempSz;

            // Pozycja postaci przed zaczeciem wykonywania ruchu
            Vector3 startCharPos = character.transform.position;

            // Pozycja pola wybranego jako cel ruchu
            Vector3 selectedTilePos = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);

            // Odleglosc od postaci do wybranego pola
            float distanceFromCharacterToSelectedTile = (Mathf.Abs(startCharPos.x - selectedTilePos.x)) + (Mathf.Abs(startCharPos.y - selectedTilePos.y));

            // Sprawdza czy wybrane pole jest w zasiegu ruchu postaci. Warunek ten nie jest konieczny w przypadku automatycznej walki, dlatego dochodzi drugi warunek.
            if (distanceFromCharacterToSelectedTile <= movementRange || AutoCombat.AutoCombatOn)
            {
                // Sprawdza, czy ruch spowoduje atak okazyjny
                CheckForOpportunityAttack(character, selectedTilePos);

                // Obecna pozycja postaci, aktualizowana po przejsciu kazdego pola. Poczatkowo przyjmuje wartosc pozycji startowej.
                Vector3 tempCharPos = startCharPos;

                // wektor w prawo, lewo, góra, dół
                Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up, Vector3.down };

                // lista pol przylegajacych do postaci
                List<GameObject> adjacentTiles = new List<GameObject>();

                // Wykonuje pojedynczy ruch tyle razy ile wynosi zasieg ruchu postaci
                for (int i = 0; i < movementRange; i++)
                {
                    // Aktualizuje zmienne po kazdym pojedynczym ruchu
                    tempCharPos = character.transform.position;
                    distanceFromCharacterToSelectedTile = (Mathf.Abs(tempCharPos.x - selectedTilePos.x)) + (Mathf.Abs(tempCharPos.y - selectedTilePos.y));

                    // Wykonuje ponizsze akcje tylko jezeli jeszcze nie osiagnal pola docelowego
                    if (distanceFromCharacterToSelectedTile < 1) break;

                    adjacentTiles.Clear();

                    // Szuka pol w każdym kierunku
                    foreach (Vector3 direction in directions)
                    {
                        // znajdź kolider z tagiem "Tile" przylegajacy do postaci na ktorym nie stoi inna postac (gdy stoi to collider wykryje najpierw obiekt postaci i nie wykryje 'tile')
                        Collider2D collider = Physics2D.OverlapCircle(character.transform.position + direction, 0.1f);
                        if (collider != null && collider.gameObject.tag == "Tile")
                            adjacentTiles.Add(collider.gameObject);
                    }

                    // Zamienia liste na tablice, zeby pozniej mozna bylo ja posortowac
                    GameObject[] adjacentTilesArray = adjacentTiles.ToArray();

                    // Sortuje przylegajace do postaci pola wg odleglosci od pola docelowego. Te ktore sa najblizej znajduja sie na poczatku tablicy
                    Array.Sort(adjacentTilesArray, (x, y) => Vector3.Distance(x.transform.position, selectedTilePos).CompareTo(Vector3.Distance(y.transform.position, selectedTilePos)));

                    // Pojedynczy ruch gracza na przylegajace do niego pole, ktore znajduje sie najblizej pola docelego
                    if (adjacentTilesArray.Length > 0)
                        character.transform.position = new Vector3(adjacentTilesArray[0].transform.position.x, adjacentTilesArray[0].transform.position.y, 0);
                }  
                
                // Jezeli postaci nie uda sie dotrzec na wybrane miejsce docelowe to jego pozycja jest resetowana do tej sprzed rozpoczenia ruchu.
                // Warunek ten jest wylaczony w przypadku automatycznej walki, zeby mogla dzialac poprawnie
                if(character.transform.position != selectedTilePos && !AutoCombat.AutoCombatOn)
                {
                    character.transform.position = startCharPos;
                    messageManager.ShowMessage("<color=red>Wybrane pole jest poza zasięgiem ruchu postaci.</color>", 4f);
                    Debug.Log("Wybrane pole jest poza zasięgiem ruchu postaci.");
                }
            }
            else
            {
                messageManager.ShowMessage("<color=red>Wybrane pole jest poza zasięgiem ruchu postaci.</color>", 4f);
                Debug.Log("Wybrane pole jest poza zasięgiem ruchu postaci.");
            }

            // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
            grid.ResetTileColors();

            // Przywraca widocznosc przyciskow akcji postaci po wykonaniu ruchu i ewentualnie resetuje bieg oraz szarze
            if(!AutoCombat.AutoCombatOn)
            {
                GameObject.Find("ButtonManager").GetComponent<ButtonManager>().ShowOrHideActionsButtons(character, true);
            }
            canMove = false;
            ResetChargeAndRun();
        }
    }


    // Sprawdza czy ruch powoduje atak okazyjny
    public void CheckForOpportunityAttack(GameObject movingCharacter, Vector3 selectedTilePosition)
    {
        if (movingCharacter.CompareTag("Player"))
        {
            // Stworzenie listy wszystkich wrogow
            GameObject[] nearEnemies;
            nearEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            // Atak okazyjny wywolywany dla kazdego wroga bedacego w zwarciu z bohaterem gracza
            foreach (GameObject enemy in nearEnemies)
            {
                // Sprawdzenie ilu wrogow jest w zwarciu z bohaterem gracza i czy ruch bohatera gracza powoduje oddalenie sie od nich (czyli atak okazyjny)
                float distanceFromOpponent = Vector3.Distance(movingCharacter.transform.position, enemy.transform.position);
                float distanceFromOpponentAfterMove = Vector3.Distance(selectedTilePosition, enemy.transform.position);

                if (distanceFromOpponent <= 1.8f && distanceFromOpponentAfterMove > 1.8f)
                {
                    // Wywolanie ataku okazyjnego w klasie AttackManager
                    AttackManager attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
                    attackManager.OpportunityAttack(enemy, movingCharacter);
                }
            }
        }
        if (movingCharacter.CompareTag("Enemy"))
        {
            // Stworzenie listy wszystkich bohaterow graczy
            GameObject[] nearPlayers;
            nearPlayers = GameObject.FindGameObjectsWithTag("Player");

            // Atak okazyjny wywolywany dla kazdego bohatera gracza bedacego w zwarciu z wrogiem
            foreach (GameObject player in nearPlayers)
            {
                // Sprawdzenie ilu bohaterow gracza jest w zwarciu z wrogiem i czy ruch wroga powoduje oddalenie sie od nich (czyli atak okazyjny)
                float distanceFromOpponent = Vector3.Distance(movingCharacter.transform.position, player.transform.position);
                float distanceFromOpponentAfterMove = Vector3.Distance(selectedTilePosition, player.transform.position);

                if (distanceFromOpponent <= 1.8f && distanceFromOpponentAfterMove > 1.8f)
                {
                    // Wywolanie ataku okazyjnego w klasie AttackManager
                    AttackManager attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
                    attackManager.OpportunityAttack(player, movingCharacter);
                }
            }
        }
    }

    // Zmienia kolor wszystkich pol w zasiegu ruchu postaci
    public void HighlightTilesInMovementRange(GameObject character)
    {
        // Sprawdza zasieg ruchu postaci
        int movementRange = character.GetComponent<Stats>().tempSz;

        // Wrzucajac do listy postac, dodajemy punkt poczatkowy, ktory jest potrzebny do pozniejszej petli wyszukujacej dostepne pozycje
        tilesInMovementRange.Add(character);

        // wektor w prawo, lewo, góra, dół
        Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up, Vector3.down };

        // Wykonuje pojedynczy ruch tyle razy ile wynosi zasieg ruchu postaci
        for (int i = 0; i < movementRange; i++)
        {
            // Lista pol, ktore bedziemy dodawac do listy wszystkich pol w zasiegu ruchu
            List<GameObject> tilesToAdd = new List<GameObject>();

            foreach (var tile in tilesInMovementRange)
            {
                // Szuka pol w każdym kierunku
                foreach (Vector3 direction in directions)
                {
                    // znajdź kolider z tagiem "Tile" przylegajacy do postaci na ktorym nie stoi inna postac (gdy stoi to collider wykryje najpierw obiekt postaci i nie wykryje 'tile')
                    Collider2D collider = Physics2D.OverlapCircle(tile.transform.position + direction, 0.1f);

                    // Jezeli collider to 'Tile' to dodajemy go do listy
                    if (collider != null && collider.gameObject.tag == "Tile")
                    {
                        tilesToAdd.Add(collider.gameObject);
                    }
                }
            }
            // Dodajemy do listy wszystkie pola, ktorych tam jeszcze nie ma
            foreach (var tile in tilesToAdd)
            {
                if(!tilesInMovementRange.Contains(tile))
                    tilesInMovementRange.Add(tile);
            }

            // Usuwamy postac z listy, bo nie jest ona 'Tile' :)
            tilesInMovementRange.Remove(character);
        }

        foreach (var tile in tilesInMovementRange)
        {
            // Zmienia kolor pola na highlightColor. Jesli pole juz jest podswietlone to przywraca domyslny kolor
            if (tile.GetComponent<Tile>()._renderer.material.color != tile.GetComponent<Tile>().rangeColor)
                tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().rangeColor;
        }

        tilesInMovementRange.Clear();
    }
}

