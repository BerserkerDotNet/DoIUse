public record TypeReference(string File, string Type);

public interface IDataExporter
{
    void BeginScan(int totalFilesToScan);

    void EndScan();

    void Scan(string fileName);

    void WriteMatch(TypeReference reference);

    void WriteError(Exception ex);
}
