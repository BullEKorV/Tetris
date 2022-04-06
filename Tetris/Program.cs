global using Raylib_cs;

Board board = new Board();


Raylib.InitWindow(800, 800, "Tetris");
Raylib.SetTargetFPS(60);

while (!Raylib.WindowShouldClose())
{
    board.Update();

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.WHITE);

    board.Draw();

    Raylib.EndDrawing();
}