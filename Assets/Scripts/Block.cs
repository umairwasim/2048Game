using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int blockValue;
    public bool isMerging;

    public Tile myCurrentTile;
    public Block mergingBlock;

    public Vector2 Pos => transform.position;

    [SerializeField] private SpriteRenderer blockSpriteRenderer;
    [SerializeField] private TextMeshPro blockText;

    public void Init(BlockType type)
    {
        blockValue = type.value;
        blockSpriteRenderer.color = type.color;
        blockText.text = type.value.ToString();
    }

    public void SetBlockOnTile(Tile newTile)
    {
        //Check if the current Tile is not already set and has its block occupied.
        //If so, make it null so that we can set newTile to currentTile and
        //now this block should be the currentTile's occupied block/set this block
        //as currentTile's occupied block
        if (myCurrentTile != null)
            myCurrentTile.occupiedBlock = null;

        myCurrentTile = newTile;

        //set this block as my current tile's occupied block
        myCurrentTile.occupiedBlock = this;
    }

    public void MergeBlock(Block baseBlock)
    {
        //set our mergingBlock equal to block to merge with
        mergingBlock = baseBlock;

        //Release currentTile occupied block
        myCurrentTile.occupiedBlock = null;

        baseBlock.isMerging = true;
    }

    public bool CanMerge(int value) => value == blockValue && !isMerging && mergingBlock == null;
}
