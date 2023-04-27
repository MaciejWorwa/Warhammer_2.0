using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Referencje do prefabu Playera i Enemy")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject enemyObject;
    GameObject newCharacter;
    private Vector2 position;

    public static int playersAmount; // liczba wszystkich bohaterow graczy
    public static int enemiesAmount; // liczba wszystkich wrogów graczy

    private GridManager gridManager;

    [Header("Przyciski dotyczące zaznaczonej postaci")]
    [SerializeField] private GameObject setStatsButton;
    [SerializeField] private GameObject destroyButton;
    [SerializeField] private GameObject rollButton;

    void Start()
    {
        playersAmount = 0;
        enemiesAmount = 0;
        gridManager = GameObject.Find("Grid").GetComponent<GridManager>();
    }

    void Update()
    {
        // Wylacza widocznosc przycisku zmiany statystyk i usuwania postaci, jesli postac nie jest wybrana
        if (Character.trSelect == null)
        {
            destroyButton.SetActive(false);
            setStatsButton.SetActive(false);
            rollButton.SetActive(false);
        }
        else
        {
            destroyButton.SetActive(true);
            setStatsButton.SetActive(true);
            rollButton.SetActive(true);
        }
    }

    #region Create new character functions
    public void CreatePlayer(string characterName = "")
    {
        CreateNewCharacter("Player", characterName);
    }

    public void CreateEnemy(string characterName = "")
    {
        CreateNewCharacter("Enemy", characterName);
    }

    public void CreateNewCharacter(string tag, string characterName)
    {
        // Liczba dostępnych pól
        int availableTiles = gridManager.width * gridManager.height; // wymiary planszy

        bool xParzysty = (gridManager.width % 2 == 0) ? true : false; // zmienna potrzebna do prawidlowego generowania losowej pozycji postaci
        bool yParzysty = (gridManager.height % 2 == 0) ? true : false; // zmienna potrzebna do prawidlowego generowania losowej pozycji postaci

        // Sprawdzenie dostępności pól
        int attempts = 0;
        Collider2D searchForColliders;
        do
        {
            int x;
            int y;

            // Generowanie losowej pozycji na mapie
            if (xParzysty)
                x = Random.Range(-gridManager.width / 2, gridManager.width / 2);
            else
                x = Random.Range(-gridManager.width / 2, gridManager.width / 2 + 1);
            if (yParzysty)
                y = Random.Range(-gridManager.height / 2, gridManager.height / 2);
            else
                y = Random.Range(-gridManager.height / 2, gridManager.height / 2 + 1);

            position = new Vector2(x, y);

            if (gridManager.width == 1)
                position.x = 0;
            if (gridManager.height == 1)
                position.y = 0;

            // Sprawdzenie czy dane pole jest wolne czy zajęte
            searchForColliders = Physics2D.OverlapCircle(position, 0.1f);

            if (searchForColliders == null || searchForColliders.tag == "Tile" && availableTiles > 1)
            {
                // Zmniejszenie liczby dostępnych pól
                availableTiles--;
            }

            // Inkrementacja liczby prób
            attempts++;

            // Sprawdzenie, czy liczba prób nie przekracza maksymalnej liczby dostępnych pól
            if (attempts > availableTiles)
            {
                Debug.Log("Nie można utworzyć nowej postaci. Brak wolnych pól.");
                return;
            }
        }
        while (searchForColliders != null && searchForColliders.tag != "Tile");

        
        if (tag == "Player")
        {        
            //tworzy nową postać w losowej pozycji i nadaje mu odpowiednia nazwe
            newCharacter = Instantiate(playerObject, position, Quaternion.identity);

            playersAmount++;
            if( characterName == "")
                newCharacter.name = ("Player " + playersAmount);
            else
                newCharacter.name = characterName;

            newCharacter.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
        }
        else if (tag == "Enemy")
        {
            //tworzy nową postać w losowej pozycji i nadaje mu odpowiednia nazwe
            newCharacter = Instantiate(enemyObject, position, Quaternion.identity);

            enemiesAmount++;
            if( characterName == "")
                newCharacter.name = ("Enemy " + enemiesAmount);
            else
                newCharacter.name = characterName;

            newCharacter.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
        }

        newCharacter.GetComponent<Character>();
        
        // Wywoluje ustawienie poziomu postaci (opoznienie jest po to, aby inne operacje zdazyly sie wykonac)
        Invoke("SetCharacterLevel", 0.05f);
    }

    void SetCharacterLevel()
    {
        GameObject.Find("ExpManager").GetComponent<ExpManager>().SetCharacterLevel(newCharacter);
    }
    #endregion
}
