using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor, highlightColor;
    [SerializeField] protected Renderer _renderer;
    [SerializeField] private GameObject highlight;
    private Color normalColor;

    public bool isOccupied;
    private LayerMask layer;

    //public static bool canMove = true; // okresla czy postac moze wykonac ruch

    public void Init(bool isOffset)
    {
        _renderer.material.color = isOffset ? offsetColor : baseColor;
        normalColor = _renderer.material.color;
        layer = LayerMask.GetMask("Character");
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
            _renderer.material.color = highlightColor;
    }

    void OnMouseExit()
    {
        //przywrocenie normalnego koloru
        if (MovementManager.canMove == true)
            _renderer.material.color = normalColor;
    }

    void OnMouseDown()
    {
        MovementManager movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();

        // wywo³uje akcje ruchu wewn¹trz klasy MovementManager
        movementManager.MoveSelectedCharacter(this.gameObject);    
    }
}