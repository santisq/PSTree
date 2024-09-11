# about_TreeStyle

## SHORT DESCRIPTION

Describes the support available for ANSI escape sequences for PSTree Module.

## LONG DESCRIPTION

PSTree v2.2.0 adds support for coloring the hierarchy output from `Get-PSTree` cmdlet via the `TreeStyle` type. The type offers a subset of capabilities that the built-in [`PSStyle`][1] has, more specifically, the [`FileInfoFormatting`][2] subset.

The instance of this type can be accessed via the [`Get-PSTreeStyle`][3] cmdlet or the `[PSTree.Style.TreeStyle]::Instance` property:

[![TreeStyle][4]][4]

Similarly to `PSStyle` you can update the

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
[4]: ../../assets/TreeStyle.png
