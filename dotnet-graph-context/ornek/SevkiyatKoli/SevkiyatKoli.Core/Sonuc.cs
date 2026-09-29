namespace SevkiyatKoli.Core;

/// <summary>Servis dönüş sözleşmesi. ArdCRM'deki ServiceResult ile aynı desen.</summary>
public class Sonuc
{
    public bool   Basarili { get; protected init; }
    public string Mesaj    { get; protected init; } = string.Empty;

    public static Sonuc Ok(string mesaj = "")   => new() { Basarili = true,  Mesaj = mesaj };
    public static Sonuc Hata(string mesaj)      => new() { Basarili = false, Mesaj = mesaj };
}

public sealed class Sonuc<T> : Sonuc
{
    public T? Veri { get; private init; }

    public static Sonuc<T> Ok(T veri, string mesaj = "")
        => new() { Basarili = true, Veri = veri, Mesaj = mesaj };

    public static new Sonuc<T> Hata(string mesaj)
        => new() { Basarili = false, Mesaj = mesaj };
}
