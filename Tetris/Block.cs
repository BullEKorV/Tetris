public class Block
{
    public Color color;
    public bool isActive;
    // public bool hasMovedAlready;
    public Block(Color colr)
    {
        this.color = colr;
        isActive = true;
    }
}