using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTeam : MonoBehaviour
{

    [SerializeField] private GameObject playerObject;

    private Vector2 position;

    public static int playersAmount;

    void Start()
    {
        playersAmount = 0;
    }

    public void CreateNewPlayer()
    {
        //generuje losowa pozycje na mapie
        int x = Random.Range(-8, 8);
        int y = Random.Range(-4, 4);
        position = new Vector2(x, y);

        //sprawdza czy dane pole jest wolne czy zajete
        Collider2D searchForColliders = Physics2D.OverlapCircle(position, 0.1f);

        if (searchForColliders == null || searchForColliders.tag == "Tile")
        {
            //tworzy nowego bohatera gracza w losowej pozycji i nadaje mu odpowiednia nazwe
            GameObject newPlayer = Instantiate(playerObject, position, Quaternion.identity);
            playersAmount++;
            newPlayer.name = ("Player " + playersAmount);

            newPlayer.GetComponent<Player>();
            newPlayer.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
        }
        else
        {
            //ponawia tworzenie nowego gracza ale tym razem losuje inna pozycje do momentu az pole bedzie wolne
            CreateNewPlayer();
        }
    }
}
