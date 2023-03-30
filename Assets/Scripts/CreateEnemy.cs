using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour

{
    [SerializeField] private GameObject enemyPrefab;

    private Vector2 position;

    public static int enemiesAmount;  //ilosc wszystkich wrogow

    void Start()
    {
        enemiesAmount = 0;
    }

    public void CreateNewEnemy()
    {
        //generuje losowa pozycje na mapie
        int x = Random.Range(-8, 8);
        int y = Random.Range(-4, 4);
        position = new Vector2(x, y);

        //sprawdza czy dane pole jest wolne czy zajete
        Collider2D searchForColliders = Physics2D.OverlapCircle(position, 0.1f);

        if (searchForColliders == null || searchForColliders.tag == "Tile")
        {
            //tworzy nowego wroga w losowej pozycji i nadaje mu odpowiednia nazwe
            GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemiesAmount++;
            newEnemy.name = "Enemy " + enemiesAmount;

            newEnemy.GetComponent<Enemy>();
            newEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
        }
        else
        {
            //ponawia tworzenie nowego przeciwnika ale tym razem losuje inna pozycje do momentu az pole bedzie wolne
            CreateNewEnemy();
        }
    }
}
