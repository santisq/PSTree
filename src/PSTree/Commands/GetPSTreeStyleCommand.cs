using System.Management.Automation;
using PSTree.Style;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTreeStyle")]
[OutputType(typeof(TreeStyle))]
[Alias("treestyle")]
public sealed class GetPSTreeStyleCommand : PSCmdlet
{
    protected override void EndProcessing() => WriteObject(TreeStyle.Instance);
}
