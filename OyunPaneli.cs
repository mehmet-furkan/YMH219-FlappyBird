namespace FlappyBird;

/// <summary>
/// Oyunun çizildiği panel. DoubleBuffered açık olduğu için her karede
/// önce bellekte çizilir, sonra tek seferde ekrana aktarılır: titreme olmaz.
/// </summary>
public class OyunPaneli : Panel
{
    public OyunPaneli()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }
}
