using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour

{
    [SerializeField] private GameObject enemyPrefab;

    GameObject newEnemy;

    private Vector2 position;

    public static int enemiesAmount;  //ilosc wszystkich wrogow

    void Start()
    {
        enemiesAmount = 0;
    }
    public void CreateNewEnemy()
    {
        // Liczba dostępnych pól
        int availableTiles = 17 * 8; // z założenia plansza o wymiarach 17x8

        // Sprawdzenie dostępności pól
        int attempts = 0;
        Collider2D searchForColliders;
        do
        {
            // Generowanie losowej pozycji na mapie
            int x = Random.Range(-8, 9);
            int y = Random.Range(-4, 4);
            position = new Vector2(x, y);

            // Sprawdzenie czy dane pole jest wolne czy zajęte
            searchForColliders = Physics2D.OverlapCircle(position, 0.1f);

            if (searchForColliders == null || searchForColliders.tag == "Tile")
            {
                // Zmniejszenie liczby dostępnych pól
                availableTiles--;
            }

            // Inkrementacja liczby prób
            attempts++;

            // Sprawdzenie, czy liczba prób nie przekracza maksymalnej liczby dostępnych pól
            if (attempts >= availableTiles)
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
