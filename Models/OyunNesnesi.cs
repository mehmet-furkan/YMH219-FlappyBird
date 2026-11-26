namespace FlappyBird.Models;

/// <summary>
/// Oyundaki her görsel nesnenin ortak atası: konum, boyut ve kendini çizme.
/// Kuş ve engeller bu sınıftan türer (kalıtım), her biri Ciz metodunu kendine göre yazar (çok biçimlilik).
/// </summary>
public abstract class OyunNesnesi
{
    public float X { get; protected set; }
    public float Y { get; protected set; }
    public float Genislik { get; protected set; }
    public float Yukseklik { get; protected set; }

    /// <summary>Çarpışma kontrolünde kullanılan sınır dikdörtgeni.</summary>
    public virtual RectangleF Sinirlar => new(X, Y, Genislik, Yukseklik);

    public abstract void Ciz(Graphics g);
}
