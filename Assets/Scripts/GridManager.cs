using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public int width, height;

    [SerializeField] private Tile tilePrefab;

    private Dictionary<Vector3, Tile> tiles;

    private List<GameObject> tilesInRange;

    [SerializeField] private Slider sliderX;
    [SerializeField] private Slider sliderY;

    void Start()
    {
        GenerateGrid();
    }

    public void ChangeGridSize()
    {
        // Ustala szerokoœæ i wysokoœæ w zale¿noœci od wartoœci Sliderów
        width = (int)sliderX.value; 
        height = (int)sliderY.value;

        // Usuwa poprzednia siatke
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in tiles)
            Destroy(tile);

        // Generuje now¹ siatkê ze zmienionymi wartoœciami
        GenerateGrid();
    }

    void GenerateGrid()
    {
        this.transform.position = new Vector3(0, 0, 0);

        tiles = new Dictionary<Vector3, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            { 
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 1), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.parent = GameObject.Find("Grid").transform;

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);


                tiles[new Vector3(x, y, 1)] = spawnedTile;
            }
        }

        //przesuwa obiekt grid, ktory jest rodzicem wszystkich tile w taka pozycje, aby siatka bitewna byla na srodku ekranu
        this.transform.position = new Vector3(-(width / 2), -(height / 2), 1);
    }

    public void ResetTileColors()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in tiles)
            tile.GetComponent<Tile>()._renderer.material.color = tile.GetComponent<Tile>().normalColor;
    }
}