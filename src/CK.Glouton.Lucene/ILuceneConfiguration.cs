namespace CK.Glouton.Lucene
{
    public interface ILuceneConfiguration
    {
        string ActualPath { get; }
        string Directory { get; set; }
        int MaxSearch { get; set; }
        string Path { get; set; }
    }
}