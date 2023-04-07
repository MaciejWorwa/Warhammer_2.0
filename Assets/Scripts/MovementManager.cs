using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class MovementManager : MonoBehaviour
{
    public static bool canMove = true; // okresla czy postac moze wykonac ruch
    public static bool Charge; // szarza
    public static bool Run; //bieg

    // Lista wszystkich pol w zasiegu ruchu postaci
    public List<GameObject> tilesInMovementRange = new List<GameObject>();

    [SerializeField] private GameObject chargeButton;
    [SerializeField] private GameObject runButton;

    public void SetCharge()
    {
        // Ustalenie ktora postac jest wybrana
        GameObject character = null;

        if (Player.trSelect != null && Enemy.trSelect == null)
            character = Player.selectedPlayer;
        else if (Enemy.trSelect != null && Player.trSelect == null)
            character = Enemy.selectedEnemy;

        if (Charge != true)
        {
            Charge = true;
            character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz * 2; // zmiana aktualnej predkosci
            chargeButton.GetComponent<Image>().color = Color.white;
            Run = false;
            runButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            Charge = false;
            character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz; // zmiana aktualnej predkosci
            chargeButton.GetComponent<Image>().color = Color.gray;
        }

        // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in tiles)
            tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().normalColor;

        // Aktualizacja podswietlonego zasiegu ruchu postaci
        HighlightTilesInMovementRange(character);
    }

    public void SetRun()
    {
        // Ustalenie ktora postac jest wybrana
        GameObject character = null;

        if (Player.trSelect != null && Enemy.trSelect == null)
            character = Player.selectedPlayer;
        else if (Enemy.trSelect != null && Player.trSelect == null)
            character = Enemy.selectedEnemy;

        if (Run != true)
        {
            Run = true;
            character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz * 3; // zmiana aktualnej predkosci
            runButton.GetComponent<Image>().color = Color.white;
            Charge = false;
            chargeButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            Run = false;
            character.GetComponent<Stats>().tempSz = character.GetComponent<Stats>().Sz; // zmiana aktualnej predkosci
            runButton.GetComponent<Image>().color = Color.gray;
        }

        // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in tiles)
            tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().normalColor;

        // Aktualizacja podswietlonego zasiegu ruchu postaci
        HighlightTilesInMovementRange(character);
    }

    //aktywuje mozliwosc ruchu
    public void ActiveMove()
    {
        canMove = true;

        //wylaczanie widocznosci buttonow akcji oraz odznaczanie drugiej zaznaczonej postaci, jezeli taka istnieje
        if (Player.trSelect != null && GameObject.Find("ActionsButtonsPlayer/Canvas") != null)
        {
            GameObject.Find("ActionsButtonsPlayer/Canvas").SetActive(false);
            if (Enemy.trSelect != null)
            {
                Enemy.trSelect = null;
                Enemy.selectedEnemy.transform.localScale = new Vector3(1f, 1f, 1f);
                Enemy.selectedEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            }

            HighlightTilesInMovementRange(Player.selectedPlayer);
        }
        if (Enemy.trSelect != null && GameObject.Find("ActionsButtonsEnemy/Canvas") != null)
        {
            GameObject.Find("ActionsButtonsEnemy/Canvas").SetActive(false);
            if (Player.trSelect != null)
            {
                Player.trSelect = null;
                Player.selectedPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
                Player.selectedPlayer.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            }

            HighlightTilesInMovementRange(Enemy.selectedEnemy);
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

            // Sprawdza czy wybrane pole jest w zasiegu ruchu postaci
            if (distanceFromCharacterToSelectedTile <= movementRange)
            {
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
                        // pozycja przylegajaca do postaci
                        Vector3 targetPos = character.transform.position + direction;

                        // znajdź kolider z tagiem "Tile" przylegajacy do postaci na ktorym nie stoi inna postac (gdy stoi to collider wykryje najpierw obiekt postaci i nie wykryje 'tile')
                        Collider2D collider = Physics2D.OverlapCircle(targetPos, 0.1f);
                        if (collider != null && collider.gameObject.tag == "Tile")
                            adjacentTiles.Add(collider.gameObject);
                    }

                    // Zamienia liste na tablice, zeby pozniej mozna bylo ja posortowac
                    GameObject[] adjacentTilesArray = adjacentTiles.ToArray();

                    // Sortuje przylegajace do postaci pola wg odleglosci od pola docelowego. Te ktore sa najblizej znajduja sie na poczatku tablicy
                    Array.Sort(adjacentTilesArray, (x, y) => Vector3.Distance(x.transform.position, selectedTilePos).CompareTo(Vector3.Distance(y.transform.position, selectedTilePos)));

                    // Ustawienie zmiennej tilePos na pozycje pierwszego elementu tablicy (czyli tego znajdujacego sie najblizej pola docelowego
                    Vector3 tilePos = adjacentTilesArray[0].transform.position;

                    // Pojedynczy ruch gracza na przylegajace do niego pole, ktore zbliza go w kierunku docelowym
                    character.transform.position = new Vector3(tilePos.x, tilePos.y, 0);
                }  
                
                // Jezeli postaci nie uda sie dotrzec na wybrane miejsce docelowe to jego pozycja jest resetowana do tej sprzed rozpoczenia ruchu
                if(character.transform.position != selectedTilePos)
                {
                    character.transform.position = startCharPos;
                    Debug.Log("Wybrane pole jest poza zasięgiem ruchu postaci.");
                }
            }
            else
                Debug.Log("Wybrane pole jest poza zasięgiem ruchu postaci.");

            // resetuje podswietlenie pol siatki w zasiegu ruchu postaci
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
            foreach (var tile in tiles)
            {
                tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().normalColor;
            }
        }
    }


    // Sprawdza czy ruch powoduje atak okazyjny
    public void CheckForOpportunityAttack(GameObject movingCharacter, Vector3 selectedTilePosition)
    {
        if (movingCharacter == Player.selectedPlayer)
        {
            // Stworzenie listy wszystkich wrogow
            GameObject[] nearEnemies;
            nearEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            // Atak okazyjny wywolywany dla kazdego wroga bedacego w zwarciu z bohaterem gracza
            foreach (GameObject enemy in nearEnemies)
            {
                // Sprawdzenie ilu wrogow jest w zwarciu z bohaterem gracza i czy ruch bohatera gracza powoduje oddalenie sie od nich (czyli atak okazyjny)
                float distanceFromOpponent = Vector3.Distance(Player.selectedPlayer.transform.position, enemy.transform.position);
                float distanceFromOpponentAfterMove = Vector3.Distance(selectedTilePosition, enemy.transform.position);

                if (distanceFromOpponent <= 1.8f && distanceFromOpponentAfterMove > 1.8f)
                {
                    // Wywolanie ataku okazyjnego w klasie AttackManager
                    AttackManager attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
                    attackManager.OpportunityAttack(enemy, Player.selectedPlayer);
                }
            }
        }
        if (movingCharacter == Enemy.selectedEnemy)
        {
            // Stworzenie listy wszystkich bohaterow graczy
            GameObject[] nearPlayers;
            nearPlayers = GameObject.FindGameObjectsWithTag("Player");

            // Atak okazyjny wywolywany dla kazdego bohatera gracza bedacego w zwarciu z wrogiem
            foreach (GameObject player in nearPlayers)
            {
                // Sprawdzenie ilu bohaterow gracza jest w zwarciu z wrogiem i czy ruch wroga powoduje oddalenie sie od nich (czyli atak okazyjny)
                float distanceFromOpponent = Vector3.Distance(Enemy.selectedEnemy.transform.position, player.transform.position);
                float distanceFromOpponentAfterMove = Vector3.Distance(selectedTilePosition, player.transform.position);

                if (distanceFromOpponent <= 1.8f && distanceFromOpponentAfterMove > 1.8f)
                {
                    // Wywolanie ataku okazyjnego w klasie AttackManager
                    AttackManager attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
                    attackManager.OpportunityAttack(player, Enemy.selectedEnemy);
                }
            }
        }
    }

    // Zmienia kolor wszystkich pol w zasiegu ruchu postaci
    public void HighlightTilesInMovementRange(GameObject character)
    {
        // Sprawdza zasieg ruchu postaci
        int movementRange = character.GetComponent<Stats>().tempSz;

        // Pozycja postaci przed zaczeciem wykonywania ruchu
        Vector3 startCharPos = character.transform.position;

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
                    // pozycja przylegajaca do postaci lub pola na ktorym postac stanie
                    Vector3 targetPos = tile.transform.position + direction;

                    // znajdź kolider z tagiem "Tile" przylegajacy do postaci na ktorym nie stoi inna postac (gdy stoi to collider wykryje najpierw obiekt postaci i nie wykryje 'tile')
                    Collider2D collider = Physics2D.OverlapCircle(targetPos, 0.1f);

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
            //else
               // tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().normalColor;
        }

        tilesInMovementRange.Clear();
    }
}
