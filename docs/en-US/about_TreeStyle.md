# about_TreeStyle

## SHORT DESCRIPTION

Describes the available support for ANSI escape sequences in PSTree Module.

## LONG DESCRIPTION

PSTree v2.2.0 and above added support for coloring the hierarchy output from `Get-PSTree` and `Get-PStreeRegistry` cmdlets via the `TreeStyle` type. The type offers a subset of capabilities that the built-in [`PSStyle`][1] has.

The instance of this type can be accessed via the [`Get-PSTreeStyle`][2] cmdlet or the `[PSTree.Style.TreeStyle]::Instance` property:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/TreeStyle.png" alt="TreeStyle" width="35%" height="35%">
</div>

It has some useful methods to combine escape sequences as well as add accents, see next section for more details.

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

The `.EscapeSequence(string vt)` method can be used to see the escape sequence used to produce the color and accent, for example:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/EscapeSequence.png" alt="TreeStyle" width="45%" height="45%">
</div>

## CUSTOMIZING OUTPUT

### Get-PSTree

Similar to `PSStyle` you can update the properties of `TreeStyle` as well as add and remove coloring for different extensions.

> [!NOTE]
>
> - For now, customizing the output of files that are a __SymbolicLink__ is not supported.
> - The __Executable__ accent is only available for Windows Operating System.

For example, take the standard output:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Get-PSTree.Before.png" alt="Get-PSTree.Before" width="45%" height="45%">
</div>

We can make a few changes to the `PSTree.Style.FileSystemStyle` object:

```powershell
$style = Get-PSTreeStyle
$palette = $style.Palette
# update the .ps1 extension
$style.FileSystem.Extension['.ps1'] = $style.CombineSequence($palette.Foreground.White, $palette.Background.Red)
# add the .cs extension
$style.FileSystem.Extension['.cs'] = $style.ToItalic($style.ToBold($palette.ForeGround.BrightCyan))
# update the Directory style
$style.FileSystem.Directory = "`e[45m"
```

> [!TIP]
>
> - The `` `e `` escape character was added in PowerShell 6. __Windows PowerShell 5.1__ users can use `[char] 27` instead, for example from previous example, instead of ``"`e[45m"`` you can use `"$([char] 27)[45m"`.
 See [__about_Special_Characters__][3] for more details.
> - The `TreeStyle` type has 3 public methods that you can use to add accents or combine VT sequences, `ToItalic()`, `ToBold()` and `CombineSequence()`.
> - You can also reset the style instance to its initial state using `.ResetSettings()` however if you had the instance stored in a variable you will need to re-assign its value, i.e.: `$style.ResetSettings()` then `$style = treestyle`.

Then, if we re-run the same command we can see those changes in the `Get-PSTree` output:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Get-PSTree.After.png" alt="Get-PSTree.After" width="45%" height="45%">
</div>

### Get-PSTreeRegistry

PSTree v2.2.3 adds coloring support for this cmdlet, the `PSTree.Style.RegistryStyle` object allows you to customize the color for `TreeRegistryKey` and `TreeRegistryValue` instances.

The coloring for `TreeRegistryKey` instances is determined by the `.RegistryKey` property whereas the coloring for `TreeRegistryValue` instances is determined by the `RegistryValueKind` property, an object to which you can add key value pairs to determine the coloring based on `.Kind` property.

> [!NOTE]
> The keys must be of type [`RegistryValueKind`][4] or should be convertable to it, e.g.: ``.RegistryValueKind[2] = "`e[45m"`` or ``.RegistryValueKind['ExpandString'] = "`e[45m"`` is valid.

Take the standard output as an example:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Get-PSTreeRegistry.Before.png" alt="Get-PSTreeRegistry.Before" width="45%" height="45%">
</div>

We can add coloring for instances of Kind `String` and update the coloring for Registry Keys:

```powershell
$style = Get-PSTreeStyle
$palette = $style.Palette
# update the TreeRegistryKey style
$style.Registry.RegistryKey = $palette.Background.Red
# add style for `String` kind TreeRegistryValue
$style.Registry.RegistryValueKind['String'] = $palette.Foreground.BrightGreen
```

Then, if we re-run the same command we can see those changes in the `Get-PSTreeRegistry` output:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Get-PSTreeRegistry.After.png" alt="Get-PSTreeRegistry.After" width="45%" height="45%">
</div>

## DISABLING ANSI OUTPUT

Similarly to `PSStyle`, you can disable the ANSI rendering by updating the `OutputRendering` property:

```powershell
(Get-PSTreeStyle).OutputRendering = 'PlainText'
```

[1]: https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_ansi_terminals
[2]: ./Get-PSTreeStyle.md
[3]: https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_special_characters?view=powershell-7.4
[4]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.win32.registryvaluekind?view=net-9.0
