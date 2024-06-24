using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectBuilder;

public sealed class Module
{
    public DirectoryInfo Root { get; }

    public FileInfo? Manifest { get; internal set; }

    public Version? Version { get; internal set; }

    public string Name { get; }

    public string PreRequisitePath { get; }

    private string? Release { get => _info.Project.Release; }

    private readonly UriBuilder _builder = new(_base);

    private const string _base = "https://www.powershellgallery.com";

    private const string _path = "api/v2/package/{0}/{1}";

    private readonly ProjectInfo _info;

    private Hashtable? _req;

    internal Module(
        DirectoryInfo directory,
        string name,
        ProjectInfo info)
    {
        Root = directory;
        Name = name;
        PreRequisitePath = InitPrerequisitePath(Root);
        _info = info;
    }

    public void CopyToRelease() => Root.CopyRecursive(Release);

    internal IEnumerable<string> GetRequirements(string path)
    {
        _req ??= ImportRequirements(path);

        if (_req is { Count: 0 })
        {
            return [];
        }

        List<ModuleDownload> modules = new(_req.Count);
        foreach (DictionaryEntry entry in _req)
        {
            modules.Add(new ModuleDownload
            {
                Module = entry.Key.ToString(),
                Version = LanguagePrimitives.ConvertTo<Version>(entry.Value)
            });
        }

        return DownloadModules([.. modules]);
    }

    private static Hashtable ImportRequirements(string path)
    {
        using PowerShell powerShell = PowerShell.Create(RunspaceMode.CurrentRunspace);
        return powerShell
            .AddCommand("Import-PowerShellDataFile")
            .AddArgument(path)
            .Invoke<Hashtable>()
            .FirstOrDefault();
    }

    private string[] DownloadModules(ModuleDownload[] modules)
    {
        List<Task<string>> tasks = new(modules.Length);
        List<string> output = new(modules.Length);

        foreach ((string module, Version version) in modules)
        {
            string destination = GetDestination(module);
            string modulePath = GetModulePath(module);

            if (Directory.Exists(modulePath))
            {
                output.Add(modulePath);
                continue;
            }

            Console.WriteLine($"Installing build pre-req '{module}'");
            _builder.Path = string.Format(_path, module, version);
            Task<string> task = GetModuleAsync(
                uri: _builder.Uri.ToString(),
                destination: destination,
                expandPath: modulePath);
            tasks.Add(task);
        }

        output.AddRange(WaitTask(tasks));
        return [.. output];
    }

    private static string[] WaitTask(List<Task<string>> tasks) =>
        WaitTaskAsync(tasks).GetAwaiter().GetResult();

    private static void ExpandArchive(string source, string destination) =>
        ZipFile.ExtractToDirectory(source, destination);

    private static async Task<string[]> WaitTaskAsync(
        List<Task<string>> tasks)
    {
        List<string> completedTasks = new(tasks.Count);
        while (tasks.Count > 0)
        {
            Task<string> awaiter = await Task.WhenAny(tasks);
            tasks.Remove(awaiter);
            string module = await awaiter;
            completedTasks.Add(module);
        }
        return [.. completedTasks];
    }

    private string GetDestination(string module) =>
        Path.Combine(PreRequisitePath, Path.ChangeExtension(module, "zip"));

    private string GetModulePath(string module) =>
        Path.Combine(PreRequisitePath, module);

    private static async Task<string> GetModuleAsync(
        string uri,
        string destination,
        string expandPath)
    {
        using (FileStream fs = File.Create(destination))
        {
            using HttpClient client = new();
            using Stream stream = await client.GetStreamAsync(uri);
            await stream.CopyToAsync(fs);
        }

        ExpandArchive(destination, expandPath);
        File.Delete(destination);
        return expandPath;
    }

    private static string InitPrerequisitePath(DirectoryInfo root)
    {
        string path = Path.Combine(root.Parent.FullName, "tools", "Modules");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }
}
