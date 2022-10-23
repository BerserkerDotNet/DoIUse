public sealed class NullExporter : IDataExporter
{
    public void BeginScan(int totalFilesToScan)
    {
    }

    public void EndScan()
    {
    }

    public void Scan(string fileName)
    {
    }

    public void WriteError(Exception ex)
    {
    }

    public void WriteMatch(TypeReference reference)
    {
    }
}
