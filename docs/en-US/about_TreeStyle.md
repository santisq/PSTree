# about_TreeStyle

## TOPIC

Customizing PSTree Output with TreeStyle.

## SHORT DESCRIPTION

The `TreeStyle` class enables customization of the hierarchical output for `Get-PSTree` and `Get-PSTreeRegistry` cmdlets in the PSTree module.

## LONG DESCRIPTION

PSTree version 2.2.0 and later introduces support for coloring the hierarchical output of the `Get-PSTree` and `Get-PSTreeRegistry` cmdlets using the `TreeStyle` class. This class provides a subset of features similar to those in PowerShell’s built-in [PSStyle][1].
You can access the singleton instance of `TreeStyle` through either the [Get-PSTreeStyle][2] cmdlet or the `[PSTree.Style.TreeStyle]::Instance` property:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/TreeStyle.png" alt="TreeStyle" width="35%" height="35%">
</div>

The `TreeStyle` class offers methods for combining escape sequences and applying text accents, such as bold or italic. See the next section for additional details.

Here are its members:

```powershell
   TypeName: PSTree.Style.TreeStyle

Name            MemberType Definition
----            ---------- ----------
CombineSequence Method     string CombineSequence(string left, string right)
Equals          Method     bool Equals(System.Object obj)
EscapeSequence  Method     string EscapeSequence(string vt)
GetHashCode     Method     int GetHashCode()
GetType         Method     type GetType()
ResetSettings   Method     void ResetSettings()
ToBold          Method     string ToBold(string vt)
ToItalic        Method     string ToItalic(string vt)
ToString        Method     string ToString()
FileSystem      Property   PSTree.Style.FileSystemStyle FileSystem {get;}
OutputRendering Property   PSTree.Style.OutputRendering OutputRendering {get;set;}
Palette         Property   PSTree.Style.Palette Palette {get;}
Registry        Property   PSTree.Style.RegistryStyle Registry {get;}
Reset           Property   string Reset {get;}
```

The `.EscapeSequence()` method reveals the escape sequence applied to generate specific colors or accents. For example:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/EscapeSequence.png" alt="TreeStyle" width="45%" height="45%">
</div>

## CUSTOMIZING OUTPUT

### Get-PSTree

You can customize the output of `Get-PSTree` by modifying the properties of the `TreeStyle` class, much like you would with PowerShell’s `PSStyle`. This allows you to update colors for directories, files, and specific file extensions, as well as add or remove color schemes for different file types.

> [!NOTE]
>
> - Customizing the output for files that are __Symbolic Links__ is not currently supported.
> - The __Executable__ accent is only available on Windows operating systems.

Consider the standard output of `Get-PSTree`:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Get-PSTree.Before.png" alt="Get-PSTree.Before" width="45%" height="45%">
</div>

You can adjust the appearance by modifying the `PSTree.Style.FileSystemStyle` object. Here’s an example of how to apply customizations:

```powershell
$style = Get-PSTreeStyle
$palette = $style.Palette

# Update the .ps1 extension to white text on a red background
$style.FileSystem.Extension['.ps1'] = $style.CombineSequence($palette.Foreground.White, $palette.Background.Red)

# Add the .cs extension with bold and italic bright cyan text
$style.FileSystem.Extension['.cs'] = $style.ToItalic($style.ToBold, $palette.Foreground.BrightCyan)

# Update the Directory style to use a magenta background
$style.FileSystem.Directory = "`e[45m"
```

> [!TIP]
>
> - PowerShell 6 and later support the `` `e `` escape character for VT sequences. For __Windows PowerShell 5.1__, use `[char] 27` instead. For example, replace ``"`e[45m"`` with `"$([char] 27)[45m"`. See [about_Special_Characters)[3] for more details.
> - The `TreeStyle` class provides methods like `.ToItalic()`, `.ToBold()`, and `.CombineSequence()` to apply text accents or combine VT sequences.
> - To reset the `TreeStyle` instance to its default state, use `.ResetSettings()`. If stored in a variable, reassign it afterward, e.g., `$style.ResetSettings()` followed by `$style = Get-PSTreeStyle`.

After applying these changes, re-running the same `Get-PSTree` command will display the updated styles:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Get-PSTree.After.png" alt="Get-PSTree.After" width="45%" height="45%">
</div>

### Get-PSTreeRegistry

Starting with PSTree version 2.2.3, the `Get-PSTreeRegistry` cmdlet supports customizable coloring via the `PSTree.Style.RegistryStyle` object. This allows you to define colors for both `TreeRegistryKey` and `TreeRegistryValue` instances.

- __TreeRegistryKey__: The color is set using the `.RegistryKey` property.
- __TreeRegistryValue__: The color is controlled by the `.RegistryValueKind` property, a dictionary that maps [RegistryValueKind][4] types to color settings based on the `.Kind` property of each registry value.

> [!NOTE]
> Keys in `.RegistryValueKind` must be of type [RegistryValueKind][4] or convertible to it. For example, both ``.RegistryValueKind[2] = "`e[45m"`` (where `2` corresponds to `ExpandString` enum value) and ``.RegistryValueKind['ExpandString'] = "`e[45m"`` are valid. Similarly, ``.RegistryValueKind[1] = "`e[45m"`` or ``.RegistryValueKind['String'] = "`e[45m"`` can be used for `String`. String keys must match the enum names exactly, such as `'String'`, `'ExpandString'`, `'Binary'`, etc.

Here’s the standard output of `Get-PSTreeRegistry` before customization:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Get-PSTreeRegistry.Before.png" alt="Get-PSTreeRegistry.Before" width="45%" height="45%">
</div>

You can customize the colors with the following code:

```powershell
$style = Get-PSTreeStyle
$palette = $style.Palette

# Set TreeRegistryKey instances to a red background
$style.Registry.RegistryKey = $palette.Background.Red

# Set TreeRegistryValue instances of 'String' kind to bright green foreground
$style.Registry.RegistryValueKind['String'] = $palette.Foreground.BrightGreen
```

After applying these changes, re-running `Get-PSTreeRegistry` reflects the updated styles:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Get-PSTreeRegistry.After.png" alt="Get-PSTreeRegistry.After" width="45%" height="45%">
</div>

## DISABLING ANSI OUTPUT

Just like PowerShell’s `PSStyle`, you can disable ANSI rendering in PSTree’s output by modifying the `.OutputRendering` property of the `TreeStyle` instance. Simply set it to `'PlainText'` using the following command:

```powershell
(Get-PSTreeStyle).OutputRendering = 'PlainText'
```

This disables all ANSI-based coloring and formatting, resulting in plain text output for commands like `Get-PSTree` and `Get-PSTreeRegistry`. It’s a straightforward way to simplify the display when you don’t need the extra visual styling.

[1]: https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_ansi_terminals
[2]: ./Get-PSTreeStyle.md
[3]: https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_special_characters?view=powershell-7.4
[4]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.win32.registryvaluekind?view=net-9.0
