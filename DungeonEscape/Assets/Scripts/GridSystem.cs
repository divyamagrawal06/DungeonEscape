using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;



public enum Paths
{
    Vertical,
    Horizontal,
    Diagonal
}
public enum TileState
{
    Selected,
    Path,
    None
}

struct Tile
{
    public bool isTile;
    public TileState state;
}
public class GridSystem : MonoBehaviour
{
    float tileSize;
    public int gridSize;

    public float mapSize;
    Tile[][] grid;

    int[] selectedCel = { -1, -1 };

    Color tileColor = Color.red;

    public Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        tileSize = mapSize / gridSize;
        grid = new Tile[gridSize][];
        for (int i = 0; i < gridSize; i++)
            grid[i] = new Tile[gridSize];

    }

    // Update is called once per frame
    void Update()
    {
        if (selectedCel[0] != -1)
            grid[selectedCel[0]][selectedCel[1]].state = TileState.Selected;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 tilePos = new Vector3(j + tileSize / 2, 0f, i + tileSize / 2);
                Vector3 size = new Vector3(tileSize, .1f, tileSize);

                switch (grid[i][j].state)
                {
                    case TileState.Selected:
                        tileColor = Color.green;
                        break;
                    case TileState.Path:
                        tileColor = Color.yellow;
                        break;
                    case TileState.None:
                        tileColor = Color.red;
                        break;

                }
                Debug.DrawRay(tilePos, Vector3.up, tileColor);
            }
        }

        tileSelection();
    }

    void tileSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {

                selectedCel[0] = (int)hit.point.z;
                selectedCel[1] = (int)hit.point.x;

            }

            pathMatrix(2, Paths.Vertical);

        }
    }

    void pathMatrix(int range, Paths path)
    {
        int dim = 2 * range + 1;

        for (int i = 0; i < gridSize; i++)
            for (int j = 0; j < gridSize; j++)
                grid[i][j].state = TileState.None;
        switch (path)
        {
            case Paths.Vertical:
                for (int i = selectedCel[0] - 2; i <= selectedCel[0] + 2; i++)
                {
                    if (i != selectedCel[0])
                        grid[i][selectedCel[1]].state = TileState.Path;

                }
                break;
            case Paths.Horizontal:
                for (int i = selectedCel[1] - 2; i <= selectedCel[1] + 2; i++)
                {
                    if (i != selectedCel[1])
                        grid[selectedCel[0]][i].state = TileState.Path;
                }
                break;

            case Paths.Diagonal:
                for (int i = selectedCel[0] - 2, j = selectedCel[1] - 2; i <= selectedCel[0] + 2; i++, j++)
                {
                    if (i != range)
                        grid[i][j].state = TileState.Path;


                }
                for (int i = selectedCel[0] - 2, j = selectedCel[1] + 2; i <= selectedCel[0] + 2; i++, j--)
                {
                    if (i != range)
                        grid[i][j].state = TileState.Path;


                }
                break;
        }

        string output = "";
        for (int i = selectedCel[0] - 2; i < selectedCel[0] + 2; i++)
        {
            for (int j = selectedCel[0] - 2; j < selectedCel[0] + 2; j++)
                output += ((int)grid[i][j].state).ToString() + " ";
            output = output + "\n";
        }
        Debug.Log(output);

    }

}
