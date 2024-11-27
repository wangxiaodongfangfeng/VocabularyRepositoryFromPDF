namespace VocabularyRepositoryFromPDF.DictionaryStorage;

public class DictionaryOperator
{

    private readonly string _filePath;
    
    public DictionaryOperator(string dictionaryPath)
    {
        if(string.IsNullOrEmpty(dictionaryPath))
            throw new ArgumentNullException(nameof(dictionaryPath));
        if (!File.Exists(dictionaryPath)) File.Create(dictionaryPath);
        
        _filePath = dictionaryPath;
    }

    public async void AppendToDictionaryAsync( string key, string value)
    {
        await File.AppendAllLinesAsync(this._filePath,new List<string> { $"{key}={value}" });
    }

    public async Task<string?> FindWordAsync(string key)
    {
        using var reader = new StreamReader(_filePath);
        while (!reader.EndOfStream)
        { 
            var line = await reader.ReadLineAsync();
            if (line?.Contains(key) == true)
            {
                return line;
            }
        }
        return null;
    }
    
}