public class Board
{
    public Block[,] grid;
    readonly float timeBetweenUpdate = 0.2f;
    float timeTillNextUpdate;
    public Board()
    {
        grid = new Block[10, 21];
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
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_S))
            playerMove = Move.Down;
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
            playerMove = Move.InstaDown;

        // DEBUG
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_C))
            SpawnPiece();

        // Commit to player move
        MoveActiveBlocks(playerMove);

        // Perform a "game tick"
        if (timeTillNextUpdate <= 0)
        {
            MoveActiveBlocks(Move.Down);
            timeTillNextUpdate = timeBetweenUpdate;
        }
    }
    #region Spawning
    private void SpawnPiece()
    {
        (int, int)[] piece = GetPiece();
        Color color = Color.BEIGE;

        int pieceCenterX = (int)MathF.Ceiling((float)GetPieceWidth(piece) / 2);

        // Actually place block
        foreach ((int, int) block in piece)
        {
            grid[(grid.GetLength(0) / 2) - pieceCenterX + block.Item1, block.Item2] = new Block(color);
        }

        timeTillNextUpdate = timeBetweenUpdate;
    }
    private int GetPieceWidth((int, int)[] piece)
    {
        int width = 0;
        foreach ((int, int) block in piece)
        {
            width = block.Item1 > width ? block.Item1 : width;
        }
        return width + 1; // + 1 because array ints start at 0
    }
    private int GetPieceHeight((int, int)[] piece)
    {
        int height = 0;
        foreach ((int, int) block in piece)
        {
            height = block.Item2 > height ? block.Item2 : height;
        }
        return height;
    }

    private (int, int)[] GetPiece()
    {
        (int, int)[][] allPieces = {
           new (int,int)[] // line piece
           {(0,0),
            (1,0),
            (2,0),
            (3,0)},
            new (int,int)[] // square piece
           {(0,0),
            (1,0),
            (0,1),
            (1,1)},
            new (int,int)[] // L piece right
           {(0,1),
            (1,1),
            (2,1),
            (2,0)},
            new (int,int)[] // L piece left
           {(0,0),
            (0,1),
            (1,1),
            (2,1)},
            new (int,int)[] // Squiggly piece left
           {(0,0),
            (1,0),
            (1,1),
            (2,1)},
            new (int,int)[] // Squiggly piece right
           {(0,1),
            (1,1),
            (1,0),
            (2,0)}
        };

        Random rnd = new Random();
        (int, int)[] piece = allPieces[rnd.Next(0, allPieces.Length)];


        return piece;
    }
    #endregion
    #region Moving blocks
    private void MoveActiveBlocks(Move move)
    {
        Dictionary<(int, int), (int, int)> blocksToMove = new Dictionary<(int, int), (int, int)>(); // Lists of all blocks that has already been moved so we dont accidentally move same block twice

        // Get the desired new location for all of the blocks
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

        // For the insta down move
        if (move == Move.InstaDown)
            blocksToMove = GetPiecePositionInstaDown();

        // Find color of the piece
        Color pieceColor = new Color();
        foreach (Block block in grid)
        {
            if (block != null && block.isActive)
            {
                pieceColor = block.color;
                break; /// ???? DOES IT REALLY BREAK?!?!=!?!
            }
        }

        // Replace old blocks with the new blocks aka move them
        if (ValidMove(blocksToMove))
        {
            foreach (var item in blocksToMove)
            {
                grid[item.Key.Item1, item.Key.Item2] = null;
            }
            foreach (var item in blocksToMove)
            {
                grid[item.Value.Item1, item.Value.Item2] = new Block(pieceColor);
            }
        }
    }
    private Dictionary<(int, int), (int, int)> GetPiecePositionInstaDown()
    {
        Dictionary<(int, int), (int, int)> blocksToMove = new Dictionary<(int, int), (int, int)>(); // Lists of all blocks that has already been moved so we dont accidentally move same block twice

        // Find y distance to move all blocks
        int smallestDeltaY = grid.GetLength(1);
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null && grid[x, y].isActive)
                {
                    if (DeltaYToBlockUnder((x, y)) < smallestDeltaY)
                        smallestDeltaY = DeltaYToBlockUnder((x, y));
                }
            }
        }

        // Find blocks and new coords
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null && grid[x, y].isActive)
                {
                    blocksToMove.Add((x, y), (x, y + smallestDeltaY));
                }
            }
        }
        return blocksToMove;
    }
    private int DeltaYToBlockUnder((int, int) block)
    {
        for (int y = block.Item2; y < grid.GetLength(1); y++)
        {
            if (grid[block.Item1, y] != null && !grid[block.Item1, y].isActive)
                return y - block.Item2 - 1; // Find blocks BETWEEN this and block under
        }
        return grid.GetLength(1) - block.Item2 - 1; // If no blocks under all, just find distance to floor
    }
    private bool ValidMove(Dictionary<(int, int), (int, int)> blocksToMove)
    {
        // Check if the blocks can be moved
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (blocksToMove.ContainsKey((x, y)))
                {
                    // Check if colliding with walls
                    if (blocksToMove[(x, y)].Item1 > grid.GetLength(0) - 1 || blocksToMove[(x, y)].Item1 < 0)
                    {
                        return false;
                    }
                    // Check if at bottom
                    if (blocksToMove[(x, y)].Item2 > grid.GetLength(1) - 1)
                    {
                        PieceIsDone(blocksToMove);
                        return false;
                    }
                    // Check if inactive block at location
                    if (grid[blocksToMove[(x, y)].Item1, blocksToMove[(x, y)].Item2] != null && !grid[blocksToMove[(x, y)].Item1, blocksToMove[(x, y)].Item2].isActive)
                    {
                        // Piece is done if inactive block underneath
                        if (blocksToMove[(x, y)].Item2 > y)
                            PieceIsDone(blocksToMove);
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private void PieceIsDone(Dictionary<(int, int), (int, int)> completedPiece)
    {
        foreach (Block block in grid)
        {
            if (block != null)
                block.isActive = false;
        }
    }
    #endregion
    #region Drawing
    public void Draw()
    {
        int blockSize = Raylib.GetScreenHeight() / 24;

        // Draw arena
        Rectangle arena = new Rectangle((Raylib.GetScreenWidth() - blockSize * grid.GetLength(0)) * 0.5f, -blockSize * 0.5f, blockSize * grid.GetLength(0), blockSize * grid.GetLength(1));
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
    #endregion   
}
enum Move
{
    None,
    Left,
    Right,
    Down,
    InstaDown
}