using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor, highlightColor;
    [SerializeField] protected Renderer _renderer;
    [SerializeField] private GameObject highlight;
    private Color normalColor;

    public static bool canMove = true; // okresla czy postac moze wykonac ruch

    public void Init(bool isOffset)
    {
        _renderer.material.color = isOffset ? offsetColor : baseColor;
        normalColor = _renderer.material.color;
    }

    void OnMouseEnter()
    {
        //podswietlenie pola
        if (canMove == true)
            _renderer.material.color = highlightColor;
    }

    void OnMouseExit()
    {
        //przywrocenie normalnego koloru
        if (canMove == true)
            _renderer.material.color = normalColor;
    }

    void OnMouseDown()
    {
        //RUCH POSTACI. Na przysz�o��: rozkmini� jak zrobi�, �eby wylicza� zasi�g ruchu uwzgl�dniaj�c inne postacie, kt�re ma na linii ruchu i powinien je omin��

        if (Player.trSelect != null && Enemy.trSelect == null && canMove)
        {
            //zmienia predkosc w zalezno�ci, czy s� zaznaczone przyciski szar�y lub biegu
            if (MovementManager.Charge)
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz * 2;
            else if (MovementManager.Run)
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz * 3;
            else
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz;

            //ustala aktualn� szybkosc
            int movementRange = Player.selectedPlayer.GetComponent<Stats>().tempSz;

            //zbiera informacje o pozycji wybranej postaci oraz kliknietego pola (tile)
            Vector3 charPos = Player.selectedPlayer.transform.position;
            Vector3 tilePos = new Vector3(this.transform.position.x, this.transform.position.y, 0);

            //sprawdza dystans do przebycia
            if ((Mathf.Abs(charPos.x - tilePos.x)) + (Mathf.Abs(charPos.y - tilePos.y)) <= movementRange)
                Player.selectedPlayer.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
            else
                Debug.Log("Wybrane pole jest poza zasi�giem ruchu postaci.");
        }
        else if (Enemy.trSelect != null && Player.trSelect == null && canMove)
        {
            //zmienia predkosc w zalezno�ci, czy s� zaznaczone przyciski szar�y liub biegu
            if (MovementManager.Charge)
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz * 2;
            else if (MovementManager.Run)
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz * 3;
            else
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz;

            //ustala aktualn� szybkosc
            int movementRange = Enemy.selectedEnemy.GetComponent<Stats>().tempSz;

            //zbiera informacje o pozycji wybranej postaci oraz kliknietego pola (tile)
            Vector3 charPos = Enemy.selectedEnemy.transform.position;
            Vector3 tilePos = new Vector3(this.transform.position.x, this.transform.position.y, 0);

            //sprawdza dystans do przebycia
            if ((Mathf.Abs(charPos.x - tilePos.x)) + (Mathf.Abs(charPos.y - tilePos.y)) <= movementRange)
                Enemy.selectedEnemy.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
            else
                Debug.Log("Wybrane pole jest poza zasi�giem ruchu postaci.");
        }
        else if (Enemy.trSelect != null && Player.trSelect != null && canMove)
            Debug.Log("Nie mo�esz poruszy� si� dwoma postaciami jednocze�nie.");
    }        
}