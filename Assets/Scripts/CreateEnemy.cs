using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour

{
    [SerializeField] private GameObject enemyPrefab;

    GameObject newEnemy;

    private Vector2 position;

    public static int enemiesAmount;  //ilosc wszystkich wrogow

    private GridManager gridManager;

    void Start()
    {
        enemiesAmount = 0;
        gridManager = GameObject.Find("Grid").GetComponent<GridManager>();
    }
    public void CreateNewEnemy()
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
                Debug.Log("Nie można utworzyć nowego przeciwnika. Brak wolnych pól.");
                return;
            }
        }
        while (searchForColliders != null && searchForColliders.tag != "Tile");

        // Utworzenie nowego wroga w losowej pozycji i nadanie mu odpowiedniej nazwy
        newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemiesAmount++;
        newEnemy.name = "Enemy " + enemiesAmount;

        newEnemy.GetComponent<Enemy>();
        newEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

        // Wywołanie ustawienia poziomu postaci (opóźnienie jest po to, aby inne operacje zdążyły się wykonać)
        Invoke("SetCharacterLevel", 0.05f);
    }


    void SetCharacterLevel()
    {
        GameObject.Find("ExpManager").GetComponent<ExpManager>().SetCharacterLevel(newEnemy);
    }
}
