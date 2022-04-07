public class Board
{
    public Block[,] grid;
    readonly float timeBetweenUpdate = 0.7f;
    float timeTillNextUpdate;
    public Board()
    {
        grid = new Block[10, 20];
        timeTillNextUpdate = timeBetweenUpdate;

        grid[5, 2] = new Block(Color.BLACK);
    }
    public void Update()
    {
        timeTillNextUpdate -= Raylib.GetFrameTime();

        Move playerMove = Move.None;
        // Get player move
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_A))
            playerMove = Move.Left;
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_D))
            playerMove = Move.Right;

        if (playerMove != Move.None)
            if (CanDoMove(playerMove))
            {
                DoPlayerMove(playerMove);
            }


        // Perform a "game tick"
        if (timeTillNextUpdate <= 0)
        {
            MoveActiveBlocksDown();
            timeTillNextUpdate = timeBetweenUpdate;
        }
    }
    private bool CanDoMove(Move playerMove)
    {
        bool canDoMove = true;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null && grid[x, y].isActive)
                {
                    if (playerMove == Move.Left)
                    {
                        if (x - 1 < 0 || (grid[x - 1, y] != null && !grid[x - 1, y].isActive))
                            canDoMove = false;
                    }
                    if (playerMove == Move.Right)
                    {
                        if (x + 1 > grid.GetLength(0) - 1 || (grid[x + 1, y] != null && !grid[x + 1, y].isActive))
                            canDoMove = false;
                    }
                }
            }
        }
        return canDoMove;
    }
    private void DoPlayerMove(Move playerMove)
    {
        Console.WriteLine("pls");

        List<Block> alreadyMoved = new List<Block>(); // Lists of all blocks that has already been moved so we dont accidentally move same block twice

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null && grid[x, y].isActive && !alreadyMoved.Contains(grid[x, y]))
                {
                    if (playerMove == Move.Left)
                    {
                        alreadyMoved.Add(grid[x, y]);
                        grid[x - 1, y] = grid[x, y];
                        grid[x, y] = null;
                        Console.WriteLine(x);
                    }
                    if (playerMove == Move.Right)
                    {
                        alreadyMoved.Add(grid[x, y]);
                        grid[x + 1, y] = grid[x, y];
                        grid[x, y] = null;
                    }
                }
            }
        }

    }
    private void MoveActiveBlocksDown()
    {
        List<Block> alreadyMoved = new List<Block>(); // Lists of all blocks that has already been moved so we dont insta move every block basically

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null && grid[x, y].isActive && !alreadyMoved.Contains(grid[x, y]))
                {
                    alreadyMoved.Add(grid[x, y]);
                    grid[x, y + 1] = grid[x, y];
                    grid[x, y] = null;
                }
            }
        }
    }
    public void Draw()
    {
        int blockSize = Raylib.GetScreenHeight() / 24;

        // Draw arena
        Rectangle arena = new Rectangle((Raylib.GetScreenWidth() - blockSize * grid.GetLength(0)) * 0.5f, 0, blockSize * grid.GetLength(0), blockSize * grid.GetLength(1));
        Raylib.DrawRectangleLinesEx(arena, 5, Color.BLUE);

        // Draw individual blocks
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null && grid[x, y].isActive)
                {
                    Raylib.DrawRectangle(x * blockSize + (int)arena.x, y * blockSize + (int)arena.y, blockSize - 1, blockSize - 1, grid[x, y].color);
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
enum Move
{
    None,
    Left,
    Right
}