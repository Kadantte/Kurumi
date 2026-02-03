<#
.SYNOPSIS
    Generates a file inventory for the nhitomi project.

.DESCRIPTION
    Lists all non-build files in the repository and categorizes them by type.
    Excludes obj, bin, and other build artifact directories.

.PARAMETER OutputPath
    Optional path to save the inventory. If not specified, outputs to console.

.PARAMETER SaveToAI
    If specified, saves the inventory to the AI folder with timestamp.

.EXAMPLE
    .\Generate-FileInventory.ps1
    Lists all files to console.

.EXAMPLE
    .\Generate-FileInventory.ps1 -SaveToAI
    Saves inventory to AI/FILE-INVENTORY-{timestamp}.md
#>

param(
    [string]$OutputPath,
    [switch]$SaveToAI
)

$ErrorActionPreference = 'Stop'

# Get repository root (parent of AI folder)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Split-Path -Parent $scriptDir

Push-Location $root
try {
    # Directories to exclude
    $excludePatterns = @(
        '\\obj\\',
        '\\bin\\',
        '\\.git\\',
        '\\.vs\\',
        '\\packages\\',
        '\\TestResults\\',
        '\\node_modules\\'
    )

    # Get all files
    $files = Get-ChildItem -Recurse -File | Where-Object {
        $path = $_.FullName
        $exclude = $false
        foreach ($pattern in $excludePatterns) {
            if ($path -match [regex]::Escape($pattern).Replace('\\\\', '\\')) {
                $exclude = $true
                break
            }
        }
        -not $exclude
    }

    # Categorize files by extension
    $categories = @{
        'C# Source Files' = @('.cs')
        'Project Files' = @('.csproj', '.sln', '.props', '.targets')
        'Configuration' = @('.json', '.config', '.yml', '.yaml', '.xml')
        'Documentation' = @('.md', '.txt', '.rst')
        'Scripts' = @('.ps1', '.sh', '.bat', '.cmd')
        'Docker' = @('Dockerfile', '.dockerignore')
        'Web Assets' = @('.html', '.css', '.js', '.ts')
        'Images' = @('.png', '.jpg', '.jpeg', '.gif', '.ico', '.svg')
        'Other' = @()
    }

    # Build inventory
    $inventory = [System.Text.StringBuilder]::new()
    [void]$inventory.AppendLine("# File Inventory")
    [void]$inventory.AppendLine("")
    [void]$inventory.AppendLine("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss UTC' -AsUTC)")
    [void]$inventory.AppendLine("Root: $root")
    [void]$inventory.AppendLine("Total Files: $($files.Count)")
    [void]$inventory.AppendLine("")

    # Group by category
    $categorized = @{}
    foreach ($category in $categories.Keys) {
        $categorized[$category] = @()
    }

    foreach ($file in $files) {
        $ext = $file.Extension.ToLowerInvariant()
        $name = $file.Name
        $relativePath = $file.FullName.Substring($root.Length + 1)

        $found = $false
        foreach ($category in $categories.Keys) {
            if ($category -eq 'Other') { continue }

            $extensions = $categories[$category]
            if ($ext -in $extensions -or $name -in $extensions) {
                $categorized[$category] += $relativePath
                $found = $true
                break
            }
        }

        if (-not $found) {
            $categorized['Other'] += $relativePath
        }
    }

    # Output by category
    foreach ($category in $categories.Keys | Sort-Object) {
        $categoryFiles = $categorized[$category] | Sort-Object
        if ($categoryFiles.Count -gt 0) {
            [void]$inventory.AppendLine("## $category ($($categoryFiles.Count))")
            [void]$inventory.AppendLine("")
            [void]$inventory.AppendLine('```')
            foreach ($file in $categoryFiles) {
                [void]$inventory.AppendLine($file)
            }
            [void]$inventory.AppendLine('```')
            [void]$inventory.AppendLine("")
        }
    }

    # Summary table
    [void]$inventory.AppendLine("## Summary")
    [void]$inventory.AppendLine("")
    [void]$inventory.AppendLine("| Category | Count |")
    [void]$inventory.AppendLine("|----------|-------|")
    foreach ($category in $categories.Keys | Sort-Object) {
        $count = $categorized[$category].Count
        if ($count -gt 0) {
            [void]$inventory.AppendLine("| $category | $count |")
        }
    }
    [void]$inventory.AppendLine("| **Total** | **$($files.Count)** |")

    $output = $inventory.ToString()

    # Determine output destination
    if ($SaveToAI) {
        $timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
        $OutputPath = Join-Path $scriptDir "FILE-INVENTORY-$timestamp.md"
    }

    if ($OutputPath) {
        $output | Out-File -FilePath $OutputPath -Encoding UTF8
        Write-Host "Inventory saved to: $OutputPath" -ForegroundColor Green
    } else {
        Write-Output $output
    }
}
finally {
    Pop-Location
}
