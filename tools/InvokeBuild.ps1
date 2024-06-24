[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ProjectBuilder.ProjectInfo] $ProjectInfo
)

task Clean {
    $ProjectInfo.CleanRelease()
}

task BuildDocs {
    $helpParams = $ProjectInfo.Documentation.GetParams()
    $null = New-ExternalHelp @helpParams
}

task BuildManaged {
    $arguments = $ProjectInfo.GetBuildArgs()
    Push-Location -LiteralPath $ProjectInfo.Project.Source.FullName

    try {
        foreach ($framework in $ProjectInfo.Project.TargetFrameworks) {
            Write-Host "Compiling for $framework"
            dotnet @arguments --framework $framework

            if ($LASTEXITCODE) {
                throw "Failed to compiled code for $framework"
            }
        }
    }
    finally {
        Pop-Location
    }
}

task CopyToRelease {
    $ProjectInfo.Module.CopyToRelease()
    $ProjectInfo.Project.CopyToRelease()
}

task Package {
    $ProjectInfo.Project.ClearNugetPackage()
    $repoParams = $ProjectInfo.Project.GetPSRepoParams()

    if (Get-PSRepository -Name $repoParams.Name -ErrorAction SilentlyContinue) {
        Unregister-PSRepository -Name $repoParams.Name
    }

    Register-PSRepository @repoParams
    try {
        $publishModuleSplat = @{
            Path       = $ProjectInfo.Project.Release
            Repository = $repoParams.Name
        }
        Publish-Module @publishModuleSplat
    }
    finally {
        Unregister-PSRepository -Name $repoParams.Name
    }
}

task Analyze {
    if (-not $ProjectInfo.AnalyzerPath) {
        Write-Host 'No Analyzer Settings found, skipping'
        return
    }

    $pssaSplat = $ProjectInfo.GetAnalyzerParams()
    $results = Invoke-ScriptAnalyzer @pssaSplat

    if ($results) {
        $results | Out-String
        throw 'Failed PsScriptAnalyzer tests, build failed'
    }
}

task PesterTests {
    if (-not $ProjectInfo.Pester.PesterScript) {
        Write-Host 'No Pester tests found, skipping'
        return
    }

    $ProjectInfo.Pester.ClearResultFile()

    if (-not (dotnet tool list --global | Select-String coverlet.console -SimpleMatch)) {
        Write-Host 'Installing dotnet tool coverlet.console' -ForegroundColor Yellow
        dotnet tool install --global coverlet.console
    }

    coverlet $ProjectInfo.Pester.GetTestArgs($PSVersionTable.PSVersion)

    if ($LASTEXITCODE) {
        throw 'Pester failed tests'
    }
}

task Build -Jobs Clean, BuildManaged, CopyToRelease, BuildDocs, Package
task Test -Jobs BuildManaged, Analyze, PesterTests
