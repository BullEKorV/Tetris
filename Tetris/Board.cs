public class Board
{
    public Block[,] grid;
    readonly float timeBetweenUpdate = 0.7f;
    float timeTillNextUpdate;
    public Board()
    {
        grid = new Block[10, 20];
        timeTillNextUpdate = timeBetweenUpdate;
    }
    public void Update()
    {
        timeTillNextUpdate -= Raylib.GetFrameTime();

        if (timeTillNextUpdate <= 0)
        {
            UpdateGrid();
            timeTillNextUpdate = timeBetweenUpdate;
        }
    }
    private void UpdateGrid()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null)
                {

                }
            }
        }
    }
    public void Draw()
    {
        int blockSize = Raylib.GetScreenHeight() / 20;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null && grid[x, y].isActive)
                {
                    Raylib.DrawRectangle(x * blockSize, y * blockSize, blockSize - 1, blockSize - 1, grid[x, y].color);
                }


            }
        }

    }

    public void PlaceNewPiece()
    {
        (int, int)[] shape = GetPiece();
        Color color = Color.BEIGE;

        foreach ((int x, int y) places in shape)
        {
            grid[places.y, places.x] = new Block(color);
        }
    }

    private (int, int)[] GetPiece()
    {
        (int, int)[] test = {
            (0,0),
            (0,1),
            (0,2),
            (0,3)
        };
        return test;
    }
}