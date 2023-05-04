using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tile : MonoBehaviour
{
    public Color baseColor, offsetColor, highlightColor, rangeColor, rangeHighlightColor;
    [HideInInspector] public Renderer _renderer;
    [HideInInspector] public Color normalColor;


    public bool isOccupied;
    private LayerMask layer;

    //public static bool canMove = true; // okresla czy postac moze wykonac ruch

    public void Init(bool isOffset)
    {
        _renderer.material.color = isOffset ? offsetColor : baseColor;
        normalColor = _renderer.material.color;
        layer = LayerMask.GetMask("Character");

        // Ustala kolor podswietlonych pol na taki sam jak normalny ale z opacity na poziomie 80%
        rangeColor = _renderer.material.color;
        rangeColor.a = 0.8f;
        rangeHighlightColor = highlightColor;
        rangeHighlightColor.a = 0.8f;
    }

    void Update()
    {
        // Sprawdza czy na polu stoi jakas postac
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.1f, layer);
        isOccupied = (collider != null) ? true : false;
    }

    void OnMouseEnter()
    {
        //podswietlenie pola
        if (MovementManager.canMove == true)
        {
            // Jezeli alpha pola jest rowne 1 (czyli nie jest to pole w zasiegu ruchu) to ustala standardowy kolor podswietlenia, w przeciwnym razie ustala rangeHighlightColor
            if (_renderer.material.color.a == 1f)
                _renderer.material.color = highlightColor;
            else
                _renderer.material.color = rangeHighlightColor;
        }
    }

    void OnMouseExit()
    {
        // Przywrócenie normalnego koloru, ale tylko jeśli obecny kolor nie jest równy rangeHighlightColor
        if (_renderer.material.color != rangeHighlightColor && _renderer.material.color == highlightColor)
            _renderer.material.color = normalColor;
        else if (MovementManager.canMove == true)
            _renderer.material.color = rangeColor;
    }

    void OnMouseDown()
    {
        // Jeżeli jesteśmy w kreatorze pola bitwy to funkcja OnMouseDown jest nieaktywna
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (GridManager.treeAdding)
            {
                GameObject.Find("Grid").GetComponent<GridManager>().AddObstacle(this.transform.position, "Tree", false);
            }
            else if (GridManager.rockAdding)
            {
                GameObject.Find("Grid").GetComponent<GridManager>().AddObstacle(this.transform.position, "Rock", false);
            }
            return;
        }


        MovementManager movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();

        // Ustala jaka postac ma sie ruszyc
        GameObject character = Character.selectedCharacter;

        // wywoluje akcje ruchu wewnatrz klasy MovementManager
        if(character != null && !GameManager.PanelIsOpen && MovementManager.canMove)
            movementManager.MoveSelectedCharacter(this.gameObject, character);    
    }
}