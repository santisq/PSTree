# about_TreeStyle

## SHORT DESCRIPTION

Describes the support available for ANSI escape sequences for PSTree Module.

## LONG DESCRIPTION

PSTree v2.2.0 adds support for coloring the hierarchy output from `Get-PSTree` cmdlet via the `TreeStyle` type. The type offers a subset of capabilities that the built-in [`PSStyle`][1] has, more specifically, the [`FileInfoFormatting`][2] subset.

The instance of this type can be accessed via the [`Get-PSTreeStyle`][3] cmdlet or the `[PSTree.Style.TreeStyle]::Instance` property:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/TreeStyle.png" alt="TreeStyle" width="40%" height="40%">
</div>

## CUSTOMIZING OUTPUT

Similar to `PSStyle` you can update the properties of `TreeStyle` as well as add an remove coloring for different extensions.

> [!NOTE]
>
> For now, customizing the output of files that are a __SymbolicLink__ is not supported.

For example, take the standard output:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Example1.Before.png" alt="Example1.Before" width="40%" height="40%">
</div>

We can make a few changes to the style object:

```powershell
PS ..\PSTree> # update the .ps1 extension
PS ..\PSTree> $style.Extension['.ps1'] = $style.CombineSequence($palette.Foreground.White, $palette.Background.Red)
PS ..\PSTree> # add the .cs extension
PS ..\PSTree> $style.Extension['.cs'] = $style.ToItalic($style.ToBold($palette.ForeGround.BrightCyan))
PS ..\PSTree> # update the Directory style
PS ..\PSTree> $style.Directory = "`e[45m"
```

Then, if we re-run the same command we can see those changes in the PSTree output:

<div>
  &nbsp;&nbsp;&nbsp;
  <img src="../../assets/Example1.After.png" alt="Example1.Before" width="40%" height="40%">
</div>

> [!TIP]
>
> The `TreeStyle` type has 3 public methods that you can use to add accents or combine VT sequences, `ToItalic()`, `ToBold()` and `CombineSequence()`.
>
> You can also reset the style instance to its initial state using `.ResetSettings()` however if you had the instance stored in a variable you will need to re-assign its value, i.e.: `$style.ResetSettings()` then `$style = treestyle`.


## EXAMPLES

{{ Code or descriptive examples of how to leverage the functions described. }}

## NOTE

{{ Note Placeholder - Additional information that a user needs to know.}}

## TROUBLESHOOTING NOTE

{{ Troubleshooting Placeholder - Warns users of bugs}}

{{ Explains behavior that is likely to change with fixes }}

## SEE ALSO

{{ See also placeholder }}

{{ You can also list related articles, blogs, and video URLs. }}

## KEYWORDS

{{List alternate names or titles for this topic that readers might use.}}

- {{ Keyword Placeholder }}
- {{ Keyword Placeholder }}
- {{ Keyword Placeholder }}
- {{ Keyword Placeholder }}

[1]: https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_ansi_terminals
[2]: https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.psstyle.fileinfoformatting
[3]: ./Get-PSTreeStyle.md
