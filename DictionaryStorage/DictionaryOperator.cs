namespace VocabularyRepositoryFromPDF.DictionaryStorage;

public class DictionaryOperator
{
    private readonly string _filePath;

    private readonly Dictionary<string, bool> _dictionary = new();

    public DictionaryOperator(string dictionaryPath)
    {
        if (string.IsNullOrEmpty(dictionaryPath))
            throw new ArgumentNullException(nameof(dictionaryPath));

        this._filePath = Path.Combine(dictionaryPath, "Dictionary.pdf");

        if (!File.Exists(_filePath))
        {
            if (!Directory.Exists(dictionaryPath))
            {
                Directory.CreateDirectory(dictionaryPath);
            }

            File.Create(_filePath).Close();
        }
        else
        {
            this.LoadDictionaryAsync();
        }
    }

    public async void AppendToDictionaryAsync(string key, string value)
    {
        var exist = _dictionary.ContainsKey(key);
        if (exist) return;
        await File.AppendAllLinesAsync(this._filePath, new List<string> { $"{key}={value}" });
        _dictionary.Add(key, true);
    }

    private async void LoadDictionaryAsync()
    {
        using var reader = new StreamReader(this._filePath);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line == null) continue;
            var key = line.Split("=")[0].Trim();
            _dictionary.Add(key, true);
        }
    }

    private async Task<string?> FindWordAsync(string key)
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