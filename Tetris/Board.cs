public class Board
{
    public Block[,] grid;
    readonly float timeBetweenUpdate = 0.2f;
    float timeTillNextUpdate;
    public Board()
    {
        grid = new Block[10, 20];
        timeTillNextUpdate = timeBetweenUpdate;

        grid[5, 2] = new Block(Color.BLACK);
        grid[6, 2] = new Block(Color.BLACK);
        grid[5, 3] = new Block(Color.BLACK);
        grid[5, 4] = new Block(Color.BLACK);
        grid[5, 5] = new Block(Color.BLACK);
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
            MoveActiveBlocks(playerMove);

        // Perform a "game tick"
        if (timeTillNextUpdate <= 0)
        {
            MoveActiveBlocks(Move.Down);
            timeTillNextUpdate = timeBetweenUpdate;
        }
    }
    private void MoveActiveBlocks(Move move)
    {
        Dictionary<(int, int), (int, int)> blocksToMove = new Dictionary<(int, int), (int, int)>(); // Lists of all blocks that has already been moved so we dont accidentally move same block twice

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null && grid[x, y].isActive)
                {
                    if (move == Move.Left)
                        blocksToMove.Add((x, y), (x - 1, y));
                    if (move == Move.Right)
                        blocksToMove.Add((x, y), (x + 1, y));
                    if (move == Move.Down)
                        blocksToMove.Add((x, y), (x, y + 1));
                }
            }
        }

        bool validMove = true;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (blocksToMove.ContainsKey((x, y)))
                {
                    if (blocksToMove[(x, y)].Item1 > grid.GetLength(0) - 1 || blocksToMove[(x, y)].Item1 < 0)
                    {
                        validMove = false;
                        continue;
                    }
                    if (blocksToMove[(x, y)].Item2 > grid.GetLength(1) - 1)
                    {
                        validMove = false;
                        PieceIsDone(blocksToMove);
                        grid[0, 0] = new Block(Color.GOLD);
                        return;
                    }
                    if (grid[blocksToMove[(x, y)].Item1, blocksToMove[(x, y)].Item2] != null && !grid[blocksToMove[(x, y)].Item1, blocksToMove[(x, y)].Item2].isActive)
                    {
                        validMove = false;
                        if (blocksToMove[(x, y)].Item2 > y)
                        {
                            PieceIsDone(blocksToMove);
                            grid[0, 0] = new Block(Color.GOLD);
                            grid[1, 0] = new Block(Color.GOLD);

                            return;
                        }
                    }
                }
            }
        }


        if (validMove)
        {
            foreach (var item in blocksToMove)
            {
                grid[item.Key.Item1, item.Key.Item2] = null;
            }
            foreach (var item in blocksToMove)
            {
                grid[item.Value.Item1, item.Value.Item2] = new Block(Color.BEIGE);
            }
        }
    }
    private void PieceIsDone(Dictionary<(int, int), (int, int)> completedPiece)
    {
        foreach (var item in completedPiece)
        {
            grid[item.Key.Item1, item.Key.Item2] = new Block(Color.BLACK) { isActive = false };
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
                if (grid[x, y] != null)
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
    Right,
    Down,
    InstaDown
}