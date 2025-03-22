namespace Senti.Shared.Models.Quotes;

public class QuoteMini
{

    public int o { get; set; }
    public int c { get; set; }
    public int h { get; set; }
    public int l { get; set; }
    public int t { get; set; }

    public QuoteMini() { }
    public QuoteMini(RawQuote raw) 
    {
        o = decimal.ToInt32(raw.o * 100);
        c = decimal.ToInt32(raw.c * 100);
        h = decimal.ToInt32(raw.h * 100);
        l = decimal.ToInt32(raw.l * 100);
        t = decimal.ToInt32(raw.t / 1000);
    }
}


