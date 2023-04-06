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

    [SerializeField] private GameObject chargeButton;
    [SerializeField] private GameObject runButton;

    public void SetCharge()
    {
        if (Charge != true)
        {
            Charge = true;
            chargeButton.GetComponent<Image>().color = Color.white;
            Run = false;
            runButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            Charge = false;
            chargeButton.GetComponent<Image>().color = Color.gray;
        }
    }

    public void SetRun()
    {
        if (Run != true)
        {
            Run = true;
            runButton.GetComponent<Image>().color = Color.white;
            Charge = false;
            chargeButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            Run = false;
            runButton.GetComponent<Image>().color = Color.gray;
        }
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

            //zmienia predkosc w zaleznosci, czy sa zaznaczone przyciski szarzy lub biegu
            if (Charge)
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz * 2;
            else if (Run)
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz * 3;
            else
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz;

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

            //zmienia predkosc w zaleznosci, czy sa zaznaczone przyciski szarzy liub biegu
            if (Charge)
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz * 2;
            else if (Run)
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz * 3;
            else
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz;

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

            Vector3 charPos = character.transform.position;
            Vector3 selectedTilePos = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);

            float distanceFromCharacterToSelectedTile = (Mathf.Abs(charPos.x - selectedTilePos.x)) + (Mathf.Abs(charPos.y - selectedTilePos.y));

            // Sprawdza czy wybrane pole jest w zasiegu ruchu postaci
            if (distanceFromCharacterToSelectedTile <= movementRange)
            {
                // wektor w prawo, lewo, góra, dół
                Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up, Vector3.down };

                // lista pol przylegajacych do postaci
                List<GameObject> adjacentTiles = new List<GameObject>();

                // Wykonuje pojedynczy ruch tyle razy ile wynosi zasieg ruchu postaci
                for (int i = 0; i < movementRange; i++)
                {
                    // Aktualizuje zmienne po kazdym pojedynczym ruchu
                    charPos = character.transform.position;
                    distanceFromCharacterToSelectedTile = (Mathf.Abs(charPos.x - selectedTilePos.x)) + (Mathf.Abs(charPos.y - selectedTilePos.y));

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
       

        /*
        //RUCH POSTACI. Na przyszlosc: rozkminic jak zrobic, zeby wyliczac zasieg ruchu uwzgledniajac inne postacie, ktore ma na linii ruchu i powinien je ominac
        if (selectedTile.GetComponent<Tile>().isOccupied)
            Debug.Log("To pole jest zajęte.");
        else
        {
            if (Player.trSelect != null && Enemy.trSelect == null && canMove)
            {
                //ustala aktualna szybkosc
                int movementRange = Player.selectedPlayer.GetComponent<Stats>().tempSz;

                //zbiera informacje o pozycji wybranej postaci oraz kliknietego pola (tile)
                Vector3 charPos = Player.selectedPlayer.transform.position;
                Vector3 tilePos = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);

                //sprawdza dystans do przebycia
                if ((Mathf.Abs(charPos.x - tilePos.x)) + (Mathf.Abs(charPos.y - tilePos.y)) <= movementRange)
                {
                    CheckForOpportunityAttack(Player.selectedPlayer, selectedTile.transform.position);
                    Player.selectedPlayer.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);
                }
                else
                    Debug.Log("Wybrane pole jest poza zasięgiem ruchu postaci.");
            }
            else if (Enemy.trSelect != null && Player.trSelect == null && canMove)
            {
                //ustala aktualna szybkosc
                int movementRange = Enemy.selectedEnemy.GetComponent<Stats>().tempSz;

                //zbiera informacje o pozycji wybranej postaci oraz kliknietego pola (tile)
                Vector3 charPos = Enemy.selectedEnemy.transform.position;
                Vector3 tilePos = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);

                //sprawdza dystans do przebycia
                if ((Mathf.Abs(charPos.x - tilePos.x)) + (Mathf.Abs(charPos.y - tilePos.y)) <= movementRange)
                {
                    CheckForOpportunityAttack(Enemy.selectedEnemy, selectedTile.transform.position);
                    Enemy.selectedEnemy.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);
                }
                else
                    Debug.Log("Wybrane pole jest poza zasięgiem ruchu postaci.");
            }

            // resetuje podswietlenie pol siatki w zasiegu ruchu postaci
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
            foreach (var tile in tiles)
            {
                tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().normalColor;
            }
        }
        */
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
                    // Wywo�anie ataku okazyjnego w klasie AttackManager
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
                    // Wywo�anie ataku okazyjnego w klasie AttackManager
                    AttackManager attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
                    attackManager.OpportunityAttack(player, Enemy.selectedEnemy);
                }
            }
        }
    }

    // Zmienia kolor wszystkich pol w zasiegu ruchu postaci
    public void HighlightTilesInMovementRange(GameObject character)
    {

        // Znajduje wszystkie pola na planszy
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (var tile in tiles)
        {
            // Sprawdza czy pole jest w zasiegu ruchu postaci
            if ((Mathf.Abs(character.transform.position.x - tile.transform.position.x)) + (Mathf.Abs(character.transform.position.y - tile.transform.position.y)) <= character.GetComponent<Stats>().tempSz)
            {
                // Zmienia kolor pola na highlightColor. Jesli pole juz jest podswietlone to przywraca domyslny kolor
                if (tile.GetComponent<Tile>()._renderer.material.color != tile.GetComponent<Tile>().rangeColor)
                    tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().rangeColor;
                else
                    tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().normalColor;
            }
        }
    }

}
