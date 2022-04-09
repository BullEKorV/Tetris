public class Piece // Only used outside of grid. Only for spawning and display
{
    public (int, int)[] blocks;
    public Color color;
    public Piece((int, int)[] blocks, Color color)
    {
        this.blocks = blocks;
        this.color = color;
    }
}