using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector2 Pos => transform.position;
    public Tile currentTile;
    public Node currentNode;
    public int blockValue;

    [SerializeField] private SpriteRenderer blockSpriteRenderer;
    [SerializeField] private TextMeshPro blockText;

    public void Init(BlockType type)
    {
        blockValue = type.value;
        blockSpriteRenderer.color = type.color;
        blockText.text = type.value.ToString();
    }

    public void SetBlock(Node newNode)
    {
        //check if the current node is already occupied, if so, clear its occupied block
        if (currentNode != null)
            currentNode.occupiedBlock = null;

        //set the node and assign this block to the new node's occupied block
        currentNode = newNode;
        currentNode.occupiedBlock = this;
    }

    public void SetBlockAtTile(Tile newTile)
    {
        if (currentTile != null)
            currentTile.occupiedBBlock = null;

        currentTile = newTile;
        currentTile.occupiedBBlock = this;
    }
}
