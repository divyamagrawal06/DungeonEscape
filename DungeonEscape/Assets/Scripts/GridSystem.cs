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

    public int[] playerCell = { -1, -1 };


    int[] selectedCell = { -1, -1 };

    Color tileColor = Color.red;
    [SerializeField]
    private Transform playerPos;
    public Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        tileSize = mapSize / gridSize;
        grid = new Tile[gridSize][];
        for (int i = 0; i < gridSize; i++)
            grid[i] = new Tile[gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 origin = new Vector3(j * tileSize, 2f, i * tileSize);
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity))
                    grid[i][j].isTile = true;
                else
                    grid[i][j].isTile = false;
                //grid[i][j].isTile = true;

            }
        }
        if (playerPos)
            updatePlayercell();

    }
    public void updatePlayercell()
    {
        playerCell[0] = Mathf.RoundToInt(playerPos.position.z / tileSize);
        playerCell[1] = Mathf.RoundToInt(playerPos.position.x / tileSize);
        Vector3 tilePos = new Vector3(tileSize * (playerCell[1] + 1 / 2), playerPos.position.y, tileSize * (playerCell[0] + 1 / 2));
        playerPos.position = tilePos;
    }
    // Update is called once per frame
    void Update()
    {
        if (playerCell[0] != -1)
            grid[playerCell[0]][playerCell[1]].state = TileState.Selected;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 tilePos = new Vector3(tileSize * (j + 1 / 2), 0f, tileSize * (i + 1 / 2));
                if (grid[i][j].isTile)
                {

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

                }
                else
                    tileColor = Color.blue;
                Debug.DrawRay(tilePos, Vector3.up, tileColor);
            }
        }

        tileSelection();
    }

    public Vector3 tileSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {

                int i = selectedCell[0] = Mathf.RoundToInt(hit.point.z / tileSize);
                int j = selectedCell[1] = Mathf.RoundToInt(hit.point.x / tileSize);
                if (grid[i][j].state == TileState.Path)
                {
                    Vector3 tilePos = new Vector3(tileSize * (j + 1 / 2), playerPos.position.y, tileSize * (i + 1 / 2));
                    return tilePos;
                }
            }
        }
        return Vector3.up * 100f;
    }

    public void pathMatrix(int range, Paths path)
    {
        int dim = 2 * range + 1;

        for (int i = 0; i < gridSize; i++)
            for (int j = 0; j < gridSize; j++)
                grid[i][j].state = TileState.None;
        switch (path)
        {

            case Paths.Vertical:
                for (int i = playerCell[0] - 2; i <= playerCell[0] + 2; i++)
                {
                    if (i != playerCell[0] && grid[i][playerCell[1]].isTile)
                        grid[i][playerCell[1]].state = TileState.Path;

                }
                break;
            case Paths.Horizontal:
                for (int i = playerCell[1] - 2; i <= playerCell[1] + 2; i++)
                {
                    if (i != playerCell[1] && grid[playerCell[0]][i].isTile)
                        grid[playerCell[0]][i].state = TileState.Path;
                }
                break;

            case Paths.Diagonal:
                for (int i = playerCell[0] - 2, j = playerCell[1] - 2; i <= playerCell[0] + 2; i++, j++)
                {
                    if (i != range && grid[i][j].isTile)
                        grid[i][j].state = TileState.Path;


                }
                for (int i = playerCell[0] - 2, j = playerCell[1] + 2; i <= playerCell[0] + 2; i++, j--)
                {
                    if (i != range && grid[i][j].isTile)
                        grid[i][j].state = TileState.Path;


                }
                break;
        }

        string output = "";
        for (int i = playerCell[0] - 2; i < playerCell[0] + 2; i++)
        {
            for (int j = playerCell[0] - 2; j < playerCell[0] + 2; j++)
                output += ((int)grid[i][j].state).ToString() + " ";
            output = output + "\n";
        }
        Debug.Log(output);

    }

}
