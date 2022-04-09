public class Board
{
    public Block[,] grid;
    List<Piece> piecesUpNext = new List<Piece>();
    readonly float timeBetweenUpdate = 0.2f;
    float timeTillNextUpdate;
    public Board()
    {
        grid = new Block[10, 21];
        timeTillNextUpdate = timeBetweenUpdate;
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

        while (piecesUpNext.Count < 3)
            piecesUpNext.Add(GetPiece());

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
        Piece piece = piecesUpNext.First();
        piecesUpNext.Remove(piece);

        // Find center 
        int pieceCenterX = (int)MathF.Ceiling((float)GetPieceWidth(piece) / 2);

        // Actually place block
        foreach ((int, int) block in piece.blocks)
        {
            grid[(grid.GetLength(0) / 2) - pieceCenterX + block.Item1, block.Item2] = new Block(piece.color);
        }

        timeTillNextUpdate = timeBetweenUpdate;
    }
    private int GetPieceWidth(Piece piece)
    {
        int width = 0;
        foreach ((int, int) block in piece.blocks)
        {
            width = block.Item1 > width ? block.Item1 : width;
        }
        return width + 1; // +1 because array ints start at 0
    }
    private int GetPieceHeight(Piece piece)
    {
        int height = 0;
        foreach ((int, int) block in piece.blocks)
        {
            height = block.Item2 > height ? block.Item2 : height;
        }
        return height + 1; // +1 because array ints start at 0
    }

    private Piece GetPiece()
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
            (1,1)}
        //     new (int,int)[] // L piece right
        //    {(0,1),
        //     (1,1),
        //     (2,1),
        //     (2,0)},
        //     new (int,int)[] // L piece left
        //    {(0,0),
        //     (0,1),
        //     (1,1),
        //     (2,1)},
        //     new (int,int)[] // Squiggly piece left
        //    {(0,0),
        //     (1,0),
        //     (1,1),
        //     (2,1)},
        //     new (int,int)[] // Squiggly piece right
        //    {(0,1),
        //     (1,1),
        //     (1,0),
        //     (2,0)}
        };
        List<Color> allColors = new List<Color>() { Color.RED, Color.BLUE, Color.YELLOW, Color.PINK };

        Random rnd = new Random();
        Piece piece = new Piece(allPieces[rnd.Next(0, allPieces.Length)], allColors[rnd.Next(0, allColors.Count)]);


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

        // Move blocks to coords specified
        MoveBlocksToCoords(blocksToMove);
    }

    private void MoveBlocksToCoords(Dictionary<(int, int), (int, int)> blocksToMove)
    {
        // Find color of the piece
        Color pieceColor = GetActiveColor();

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
    private void DeleteFullRows()
    {
        int[] rowsToDelete = GetFullRows();

        if (rowsToDelete.Length == 0) return;

        Dictionary<(int, int), (int, int)> blocksToMove = new Dictionary<(int, int), (int, int)>();

        // Finds blocks above deleted rows to move down
        for (int y = 0; y < rowsToDelete.Max(); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] != null)
                    blocksToMove.Add((x, y), (x, y + rowsToDelete.Length));
            }
        }
        Console.WriteLine(blocksToMove.Count);
        // Delete rows
        for (int y = rowsToDelete.Min(); y < rowsToDelete.Min() + rowsToDelete.Length; y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, y] = null;
            }
        }

        MoveBlocksToCoords(blocksToMove);
    }
    private void PieceIsDone(Dictionary<(int, int), (int, int)> completedPiece)
    {
        foreach (Block block in grid)
        {
            if (block != null)
                block.isActive = false;
        }
        DeleteFullRows();
    }
    #endregion
    #region Data gathering
    private Color GetActiveColor()
    {
        foreach (Block block in grid)
        {
            if (block != null && block.isActive)
                return block.color;
        }
        return Color.WHITE;
    }
    private int[] GetFullRows()
    {
        List<int> rowNumbers = new List<int>();
        // full
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == null || grid[x, y].isActive)
                    break;
                if (x == grid.GetLength(0) - 1) rowNumbers.Add(y);
            }
        }
        return rowNumbers.ToArray();
    }
    #endregion
    #region Drawing
    public void Draw()
    {
        int blockSize = Raylib.GetScreenHeight() / 24;

        // Draw arena
        Rectangle arena = new Rectangle((Raylib.GetScreenWidth() - blockSize * grid.GetLength(0)) * 0.5f, -blockSize * 0.5f, blockSize * grid.GetLength(0), blockSize * grid.GetLength(1));
        Raylib.DrawRectangleLinesEx(new Rectangle(arena.x - 5, arena.y, arena.width + 10, arena.height + 5), 5, Color.BLUE);

        // Draw individual blocks
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null)
                {
                    Raylib.DrawRectangle(x * blockSize + (int)arena.x + 1, y * blockSize + (int)arena.y + 1, blockSize - 2, blockSize - 2, grid[x, y].color);
                }
            }
        }

        // Draw side up next 
        for (int i = 0; i < piecesUpNext.Count; i++)
        {
            for (int x = 0; x < piecesUpNext.ElementAt(i).blocks.Length; x++)
            {
                Raylib.DrawRectangle((int)arena.x + (int)arena.width + 30 + piecesUpNext.ElementAt(i).blocks[x].Item1 * blockSize, 50 + i * blockSize * 3 + piecesUpNext.ElementAt(i).blocks[x].Item2 * blockSize, blockSize - 1, blockSize - 1, piecesUpNext.ElementAt(i).color);
            }
        }

        // Draw highlight where block will land
        Dictionary<(int, int), (int, int)> highlightBlocks = new Dictionary<(int, int), (int, int)>();
        highlightBlocks = GetPiecePositionInstaDown();

        foreach (var block in highlightBlocks)
        {
            Rectangle outline = new Rectangle((int)arena.x + block.Value.Item1 * blockSize + 1, (int)arena.y + block.Value.Item2 * blockSize + 1, blockSize - 2, blockSize - 2);
            Raylib.DrawRectangleLinesEx(outline, 3, GetActiveColor());
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