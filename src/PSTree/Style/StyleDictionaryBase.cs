using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSTree.Style;

public abstract class StyleDictionaryBase<TKey>(Dictionary<TKey, string> internalDictionary)
    where TKey : notnull
{
    private readonly Dictionary<TKey, string> _internalDictionary = internalDictionary;

    public ICollection<TKey> Keys { get => _internalDictionary.Keys; }

    public ICollection<string> Values { get => _internalDictionary.Values; }

    public int Count { get => _internalDictionary.Count; }

    protected abstract TKey Validate(TKey key);

    public string this[TKey key]
    {
        get => _internalDictionary[key];
        set => _internalDictionary[Validate(key)] = TreeStyle.ThrowIfInvalidSequence(value);
    }

    public bool TryGetValue(TKey key, [NotNullWhen(true)] out string? sequence) =>
        _internalDictionary.TryGetValue(key, out sequence);

    public bool ContainsKey(TKey key) => _internalDictionary.ContainsKey(key);

    public void Add(TKey key, string vt) =>
        _internalDictionary.Add(Validate(key), TreeStyle.ThrowIfInvalidSequence(vt));

    public bool Remove(TKey key) => _internalDictionary.Remove(Validate(key));

    public void Clear() => _internalDictionary.Clear();

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public string GetEscapedValues()
    {
        StringBuilder builder = new(_internalDictionary.Count);
        foreach (string value in _internalDictionary.Values)
        {
            builder.AppendLine(TreeStyle.Instance.EscapeSequence(value));
        }

        return builder.ToString();
    }

    public override string? ToString()
    {
        if (_internalDictionary.Count == 0)
        {
            return null;
        }

        StringBuilder builder = new(_internalDictionary.Count);
        string[] keys = [.. Keys.Select(static e => e is string str ? str : e.ToString()!)];
        int max = keys.Max(e => e.Length);
        int idx = 0;

        foreach (KeyValuePair<TKey, string> pair in _internalDictionary)
        {
            string key = keys[idx++];

            builder
                .Append(key.PadRight(max + 1))
                .Append(" → ")
                .AppendLine(TreeStyle.Instance.EscapeSequence(pair.Value));
        }

        return builder.ToString();
    }
}
