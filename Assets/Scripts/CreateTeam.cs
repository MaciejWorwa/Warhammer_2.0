using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTeam : MonoBehaviour
{

    [SerializeField] private GameObject playerObject;
    GameObject newPlayer;
    private Vector2 position;

    public static int playersAmount;

    private GridManager gridManager;

    void Start()
    {
        playersAmount = 0;
        gridManager = GameObject.Find("Grid").GetComponent<GridManager>();
    }

    public void CreateNewPlayer()
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
                Debug.Log("Nie można utworzyć nowego bohatera gracza. Brak wolnych pól.");
                return;
            }
        }
        while (searchForColliders != null && searchForColliders.tag != "Tile");

        //tworzy nowego bohatera gracza w losowej pozycji i nadaje mu odpowiednia nazwe
        newPlayer = Instantiate(playerObject, position, Quaternion.identity);
        playersAmount++;
        newPlayer.name = ("Player " + playersAmount);

        newPlayer.GetComponent<Player>();
        newPlayer.GetComponent<Renderer>().material.color = new Color(0, 255, 0);

        // Wywoluje ustawienie poziomu postaci (opoznienie jest po to, aby inne operacje zdazyly sie wykonac)
        Invoke("SetCharacterLevel", 0.05f);

    }

    void SetCharacterLevel()
    {
        GameObject.Find("ExpManager").GetComponent<ExpManager>().SetCharacterLevel(newPlayer);
    }
}
