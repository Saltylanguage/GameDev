param(
    [string]$ProjectRoot = (Join-Path $PSScriptRoot '..\LearningIndieDev')
)

Add-Type -AssemblyName System.Drawing

$size = 128
$canonicalMasks = @(0, 1, 5, 7, 17, 21, 23, 29, 31, 85, 87, 95, 119, 127, 255)
$families = @(
    @{ Name = 'Grass'; Source = 'Assets\Art\Terrain\Standardized\128\Grass_Full_255.png' },
    @{ Name = 'Desert'; Source = 'Assets\Art\Terrain\Standardized\128\Desert_Full.png' }
)
$outputRoot = Join-Path $ProjectRoot 'Assets\Art\Terrain\Blob\128'

function Get-RotatedMask([int]$mask) {
    return (($mask -shl 2) -bor ($mask -shr 6)) -band 255
}

function Get-MaskAlpha([int]$mask, [int]$x, [int]$y) {
    if ($mask -eq 0) { return 0 }
    if ($mask -eq 255) { return 255 }

    $px = ($x + 0.5) / $size
    $py = ($y + 0.5) / $size
    $waveX = ([Math]::Sin($py * 19.0 + $mask * 0.37) * 0.032) + ([Math]::Sin($py * 43.0 + $mask) * 0.012)
    $waveY = ([Math]::Sin($px * 17.0 + $mask * 0.53) * 0.032) + ([Math]::Sin($px * 41.0 + $mask) * 0.012)
    # A broad shared center plus rectangular edge corridors keeps every active
    # region connected. Small deterministic waves make exposed edges organic
    # without opening holes in the filled terrain.
    $inside = ($px -ge (0.22 + $waveX) -and $px -le (0.78 + $waveX) -and $py -ge (0.22 + $waveY) -and $py -le (0.78 + $waveY))
    $inside = $inside -or (($mask -band 1) -ne 0 -and $py -le (0.58 + $waveY) -and $px -ge (0.22 + $waveX) -and $px -le (0.78 + $waveX))
    $inside = $inside -or (($mask -band 4) -ne 0 -and $px -ge (0.42 + $waveX) -and $py -ge (0.22 + $waveY) -and $py -le (0.78 + $waveY))
    $inside = $inside -or (($mask -band 16) -ne 0 -and $py -ge (0.42 + $waveY) -and $px -ge (0.22 + $waveX) -and $px -le (0.78 + $waveX))
    $inside = $inside -or (($mask -band 64) -ne 0 -and $px -le (0.58 + $waveX) -and $py -ge (0.22 + $waveY) -and $py -le (0.78 + $waveY))

    # Valid corner bits add a connected quarter-region, not a detached circle.
    if (($mask -band 2) -ne 0 -and $px -ge 0.42 -and $py -le 0.42 -and $px + $py -ge 0.62) { $inside = $true }
    if (($mask -band 8) -ne 0 -and $px -ge 0.42 -and $py -ge 0.42 -and $px + (1.0 - $py) -ge 0.62) { $inside = $true }
    if (($mask -band 32) -ne 0 -and $px -le 0.58 -and $py -ge 0.42 -and (1.0 - $px) + (1.0 - $py) -ge 0.62) { $inside = $true }
    if (($mask -band 128) -ne 0 -and $px -le 0.58 -and $py -le 0.58 -and (1.0 - $px) + $py -ge 0.62) { $inside = $true }

    # Guarantee the shared edge center is filled whenever its bit is active.
    if (($mask -band 1) -ne 0 -and $y -le 2 -and $x -ge 28 -and $x -le 99) { $inside = $true }
    if (($mask -band 4) -ne 0 -and $x -ge 125 -and $y -ge 28 -and $y -le 99) { $inside = $true }
    if (($mask -band 16) -ne 0 -and $y -ge 125 -and $x -ge 28 -and $x -le 99) { $inside = $true }
    if (($mask -band 64) -ne 0 -and $x -le 2 -and $y -ge 28 -and $y -le 99) { $inside = $true }

    return $(if ($inside) { 255 } else { 0 })
}

function New-Tile([System.Drawing.Bitmap]$source, [int]$mask, [string]$path) {
    $tile = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    for ($y = 0; $y -lt $size; $y++) {
        for ($x = 0; $x -lt $size; $x++) {
            $alpha = Get-MaskAlpha $mask $x $y
            $pixel = $source.GetPixel($x, $y)
            $tile.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($alpha, $pixel.R, $pixel.G, $pixel.B))
        }
    }
    $tile.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $tile.Dispose()
}

foreach ($family in $families) {
    $sourcePath = Join-Path $ProjectRoot $family.Source
    if (-not (Test-Path $sourcePath)) {
        Write-Warning "Skipping $($family.Name): source not found at $sourcePath"
        continue
    }

    $familyRoot = Join-Path $outputRoot $family.Name
    New-Item -ItemType Directory -Force $familyRoot | Out-Null
    $source = [System.Drawing.Bitmap]::FromFile($sourcePath)
    $written = @{}

    foreach ($canonical in $canonicalMasks) {
        $mask = $canonical
        $rotation = 0
        do {
            if (-not $written.ContainsKey($mask)) {
                $path = Join-Path $familyRoot ("$($family.Name)_$('{0:D3}' -f $mask).png")
                New-Tile $source $mask $path
                $written[$mask] = $true
            }
            $mask = Get-RotatedMask $mask
            $rotation++
        } while ($mask -ne $canonical -and $rotation -lt 4)
    }

    $source.Dispose()
    Write-Host "$($family.Name): wrote $($written.Count) masks to $familyRoot"
}
