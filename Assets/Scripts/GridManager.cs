using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    [SerializeField] private Tile tilePrefab;

    private Dictionary<Vector3, Tile> tiles;

    private List<GameObject> tilesInRange;

    void Start()
    {
        GenerateGrid();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (Tile.canMove == true)
    //    {
    //        tilesInRange = new List<GameObject>();

    //        //detect all objects inside the RangeSphere
    //        if ((!tilesInRange.Contains(other.gameObject) && (other.tag == "Tile")))
    //        {
    //            //If is a Tile add it to the list
    //            tilesInRange.Add(other.gameObject);
    //            foreach (var x in tilesInRange)
    //            {
    //                Debug.Log("Zawartoœæ listy: " + x.ToString());
    //            }
    //        }

    //    }

    //}

    void GenerateGrid()
    {
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
        this.transform.position = new Vector3(-(width/2), -(height/2), 1);

    }

}