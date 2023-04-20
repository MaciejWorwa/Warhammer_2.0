using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTeam : MonoBehaviour
{

    [SerializeField] private GameObject playerObject;
    GameObject newPlayer;
    private Vector2 position;

    public static int playersAmount;

    void Start()
    {
        playersAmount = 0;
    }

    public void CreateNewPlayer()
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
