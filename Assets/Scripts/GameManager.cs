using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameState gameState;

    [SerializeField] private int width = 4;
    [SerializeField] private int height = 4;

    [SerializeField] private Node nodePrefab;               //Container for our block
    [SerializeField] private Tile tilePrefab;               //Tile is a container for block
    [SerializeField] private Block blockPrefab;             //Actual block prefab with number and color
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private List<BlockType> blockTypes;

    private readonly List<Tile> tiles = new List<Tile>();
    private readonly List<Node> nodes = new List<Node>();
    private readonly List<Block> blocks = new List<Block>();

    private int round = 0;
    private float snapDuration = 0.2f;
    private readonly float boardOffset = 0.5f;

    private BlockType GetBlockByValue(int value) => blockTypes.Find(b => b.value == value);

    private Node GetNodeAtPosition(Vector2 position) => nodes.FirstOrDefault(n => n.Pos == position);

    private Tile GetTileAtPosition(Vector2 position) => tiles.FirstOrDefault(t => t.Pos == position);

    private void ChangeGameState(GameState currentState)
    {
        gameState = currentState;
        switch (currentState)
        {
            case GameState.GenerateGrid:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(round++ == 0 ? 2 : 1); //round++ will be 0 for the fist time. Generate 2 for the fist time and 1 every other time 
                break;
            case GameState.WaitForInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }
    }

    private void Start() => ChangeGameState(GameState.GenerateGrid);

    private void Update()
    {
        if (gameState != GameState.WaitForInput)
            return;
        //Arrow Keys Input
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ShiftBlocks(Vector2.left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ShiftBlocks(Vector2.right);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            ShiftBlocks(Vector2.up);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ShiftBlocks(Vector2.down);
    }

    private void GenerateGrid()
    {
        //center board value, store in a vector
        var center = new Vector2(width * 0.5f - boardOffset, height * 0.5f - boardOffset);

        //Instantiate board at the center of the screen, 
        var board = Instantiate(boardPrefab, center, Quaternion.identity);

        //define board size equal to our grid width and height
        board.size = new Vector2(width, height);

        //center the camera , -10 along z axis
        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

        #region Base Tiles Intantiate
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity);
                tiles.Add(tile);
            }
        }
        #endregion

        ChangeGameState(GameState.SpawningBlocks);
    }

    private void SpawnBlocks(int numberOfBlocks)
    {
        //Randomly choose tiles with no occupied block 
        var freeTiles = tiles.Where(t => t.occupiedBlock == null).OrderBy(t => Random.value).ToList();

        foreach (var freeTile in freeTiles.Take(numberOfBlocks))
        {
            //Spawn given number of blocks
            var spawnBlock = Instantiate(blockPrefab, freeTile.Pos, Quaternion.identity);

            //Add spawned block to the list of blocks
            blocks.Add(spawnBlock);

            //Set the relevant freeTile as Tile of the spawned block
            spawnBlock.SetBlockOnNewTile(freeTile);

            //Set the value of the spawned block 
            spawnBlock.Init(GetBlockByValue(Random.value > 0.8f ? 4 : 2));
        }

        if (freeTiles.Count() <= 1)
        {
            //GAME OVER
            ChangeGameState(GameState.Lose);
            print("you lost the game, call game over panel");
            return;
        }

        ChangeGameState(GameState.WaitForInput);
    }

    void ShiftBlocks(Vector2 direction)
    {
        var orderedBlocks = blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();

        //For opposite direction Inputs
        if (direction == Vector2.right || direction == Vector2.up)
            orderedBlocks.Reverse();

        foreach (var block in orderedBlocks)
        {
            //store block's current tile reference
            var currentTile = block.currentTile;
            do
            {
                //set block's own tile as current tile
                block.SetBlockOnNewTile(currentTile);

                //look for next possible tile at position (which is our current tile's position
                //and the direction it is moving in)
                var nextPossibleTile = GetTileAtPosition(currentTile.Pos + direction);

                //if we have a possible next tile
                if (nextPossibleTile != null)
                {
                    //if next possible tile's occupied block is not already taken 
                    if(nextPossibleTile.occupiedBlock == null)
                    {
                        //set our current tile to next possible tile
                        currentTile = nextPossibleTile;
                    }
                }

                //loop through until my currentTile and block's currentTile aren't same, 
            } while (currentTile != block.currentTile);

            //set block's position to it's current tile position (move block)
            block.transform.DOMove(block.currentTile.Pos, snapDuration);
           // block.transform.position = block.currentTile.Pos;
        }
    }

    void Shift(Vector2 direction)
    {
        Debug.Log(" Shifting in  direction " + direction);
        var orderedBlocks = blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        Debug.Log(" Ordered blocks " + orderedBlocks.Count);

        if (direction == Vector2.right || direction == Vector2.up)
            orderedBlocks.Reverse();

        foreach (var block in orderedBlocks)
        {
            // Debug.Log(" current block's Node" + block.currentNode);
            var currentNode = block.currentNode;

            do
            {
                block.SetBlock(currentNode);

                var possibleNode = GetNodeAtPosition(currentNode.Pos + direction);

                if (possibleNode != null)
                {
                    if (possibleNode.occupiedBlock == null)
                    {
                        currentNode = possibleNode;
                    }
                }
            }
            while (currentNode != block.currentNode);
            {
                //Debug.Log(" move block " + block + " from position" + block.transform.position + " to " + block.currentNode.Pos + " block.currentNode.Pos");
                block.transform.position = block.currentNode.Pos;
            }

        }

    }

}

[Serializable]
public struct BlockType
{
    public int value;
    public Color color;
}

public enum GameState
{
    GenerateGrid,
    SpawningBlocks,
    WaitForInput,
    Moving,
    Win,
    Lose
}


