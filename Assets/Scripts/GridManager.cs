using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    public static int width = 17;
    public static int height = 8;

    [SerializeField] private Tile tilePrefab;

    private Dictionary<Vector3, Tile> tiles;

    private List<GameObject> tilesInRange;

    [SerializeField] private Slider sliderX;
    [SerializeField] private Slider sliderY;

    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject treePrefab;

    public static bool treeAdding;
    public static bool rockAdding;
    public static bool obstacleRemoving;

    void Start()
    {
        treeAdding = false;
        rockAdding = false;
        obstacleRemoving = false;

        //Debug.Log("height " + height);
        //Debug.Log("width " + width);
        //sliderY.value = height;
        //sliderX.value = width;
        //Debug.Log("sliderY.value " + sliderY.value);
        //Debug.Log("sliderX.value " + sliderX.value);

        GenerateGrid();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && obstacleRemoving)
        {
            RemoveObstacle();
        }
    }

    public void ChangeGridSize()
    {
        // Ustala szerokoœæ i wysokoœæ w zale¿noœci od wartoœci Sliderów
        width = (int)sliderX.value; 
        height = (int)sliderY.value;

        // Generuje now¹ siatkê ze zmienionymi wartoœciami
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        // Usuwa poprzednia siatke
        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in allTiles)
        {
            Destroy(tile);
        }

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


    #region Adding and removing obstacles
    public void AddingObstacle(GameObject button)
    {
        Color newColor = button.GetComponent<Image>().color;
        newColor.a = 0.5f;
        button.GetComponent<Image>().color = newColor;

        if (button.name == "AddTreeButton")
        {
            treeAdding = true;
            rockAdding = false;
            newColor.a = 1f;
            GameObject.Find("AddRockButton").GetComponent<Image>().color = newColor;

        }
        else if (button.name == "AddRockButton")
        {
            rockAdding = true;
            treeAdding = false;
            newColor.a = 1f;
            GameObject.Find("AddTreeButton").GetComponent<Image>().color = newColor;
        }

        // Zresetowanie funkji i przycisku usuwania przeszkód
        obstacleRemoving = false;
        GameObject.Find("RemoveObstacleButton").GetComponent<Image>().color = new Color(1f, 0.398f, 0.392f, 1f);
    }

    public void RemovingObstacle()
    {
        obstacleRemoving = true;
        Color removeColor = GameObject.Find("RemoveObstacleButton").GetComponent<Image>().color;
        removeColor.a = 0.5f;
        GameObject.Find("RemoveObstacleButton").GetComponent<Image>().color = removeColor;

        treeAdding = false;
        rockAdding = false;
        GameObject.Find("AddRockButton").GetComponent<Image>().color = new Color(0.392f, 0.906f, 1f, 1f);
        GameObject.Find("AddTreeButton").GetComponent<Image>().color = new Color(0.392f, 0.906f, 1f, 1f);
    }

    public void AddObstacle(Vector3 position, string tag, bool loadGame)
    {
        position.z = 1;

        if(loadGame)
        {
            if (tag == "Tree")
                Instantiate(treePrefab, position, Quaternion.identity).AddComponent<Obstacle>();
            else if (tag == "Rock")
                Instantiate(rockPrefab, position, Quaternion.identity).AddComponent<Obstacle>();

            return;
        }


        Collider2D collider = Physics2D.OverlapCircle(position, 0.1f);
        if (collider != null && collider.gameObject.tag == "Tile")
        {
            if (tag == "Tree")
                Instantiate(treePrefab, position, Quaternion.identity).AddComponent<Obstacle>();
            else if (tag == "Rock")
                Instantiate(rockPrefab, position, Quaternion.identity).AddComponent<Obstacle>(); 
        }
        else
            Debug.Log("Nie moge tu postawic przeszkody.");
    }

    public void RemoveObstacle()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && (hit.collider.CompareTag("Tree") || hit.collider.CompareTag("Rock")))
        {
            Destroy(hit.collider.gameObject);
        }
    }
    #endregion
}