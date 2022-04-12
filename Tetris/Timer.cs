public class Timer
{
    readonly static float timeBetweenAutoDrop = 0.7f;
    public static float autoDrop = timeBetweenAutoDrop;
    public static float keyTimer;
    readonly static float autoRepeatDelay = 0.25f;
    readonly static float autoRepeatSpeed = 0.05f;
    readonly public static float placePieceTimeMax = 0.8f;
    // Gives the player extra time before the piece gets placed
    public static float placePiece = placePieceTimeMax;
    static bool pieceTouchingBlock = false;

    public static void Update()
    {
        if (pieceTouchingBlock)
            placePiece -= Raylib.GetFrameTime();

        autoDrop -= Raylib.GetFrameTime();
        keyTimer -= Raylib.GetFrameTime();
    }
    public static void ResetAutoDrop()
    {
        autoDrop = timeBetweenAutoDrop;
    }
    public static void KeyTimerDelay()
    {
        keyTimer = autoRepeatDelay;
    }
    public static void KeyTimerSpeed()
    {
        keyTimer = autoRepeatSpeed;
    }
    public static bool CanAutoRepeat()
    {
        if (keyTimer <= 0) return true;
        return false;
    }
    public static void StartPlacePieceTimer()
    {
        if (pieceTouchingBlock == true) return;
        placePiece = placePieceTimeMax;
        pieceTouchingBlock = true;
    }
    public static void StopPlacePieceTimer()
    {
        if (pieceTouchingBlock == false) return;
        placePiece = placePieceTimeMax;
        pieceTouchingBlock = false;
    }
}