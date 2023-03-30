using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor, highlightColor;
    [SerializeField] protected Renderer _renderer;
    [SerializeField] private GameObject highlight;
    private Color normalColor;

    //public static bool canMove = true; // okresla czy postac moze wykonac ruch

    public void Init(bool isOffset)
    {
        _renderer.material.color = isOffset ? offsetColor : baseColor;
        normalColor = _renderer.material.color;
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