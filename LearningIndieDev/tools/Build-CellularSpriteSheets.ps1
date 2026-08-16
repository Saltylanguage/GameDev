$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

$sourceRoot = Join-Path $PSScriptRoot '..\Assets\Art'
$outputRoot = Join-Path $PSScriptRoot '..\Assets\Resources\CellularArt'
New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null

function Test-BackgroundPixel([System.Drawing.Color] $color) {
    return $color.R -ge 205 -and $color.G -ge 190 -and $color.B -ge 180 -and
        ([Math]::Max($color.R, [Math]::Max($color.G, $color.B)) -
            [Math]::Min($color.R, [Math]::Min($color.G, $color.B))) -le 38
}

function Remove-ConnectedBackground([System.Drawing.Bitmap] $bitmap) {
    $queue = [System.Collections.Generic.Queue[System.Drawing.Point]]::new()
    $seen = New-Object 'bool[,]' $bitmap.Width, $bitmap.Height

    for ($x = 0; $x -lt $bitmap.Width; $x++) {
        $queue.Enqueue([System.Drawing.Point]::new($x, 0))
        $queue.Enqueue([System.Drawing.Point]::new($x, $bitmap.Height - 1))
    }
    for ($y = 1; $y -lt ($bitmap.Height - 1); $y++) {
        $queue.Enqueue([System.Drawing.Point]::new(0, $y))
        $queue.Enqueue([System.Drawing.Point]::new($bitmap.Width - 1, $y))
    }

    while ($queue.Count -gt 0) {
        $point = $queue.Dequeue()
        if ($point.X -lt 0 -or $point.X -ge $bitmap.Width -or
            $point.Y -lt 0 -or $point.Y -ge $bitmap.Height -or
            $seen[$point.X, $point.Y]) {
            continue
        }

        $seen[$point.X, $point.Y] = $true
        $color = $bitmap.GetPixel($point.X, $point.Y)
        if (-not (Test-BackgroundPixel $color)) {
            continue
        }

        $bitmap.SetPixel($point.X, $point.Y, [System.Drawing.Color]::Transparent)
        $queue.Enqueue([System.Drawing.Point]::new($point.X - 1, $point.Y))
        $queue.Enqueue([System.Drawing.Point]::new($point.X + 1, $point.Y))
        $queue.Enqueue([System.Drawing.Point]::new($point.X, $point.Y - 1))
        $queue.Enqueue([System.Drawing.Point]::new($point.X, $point.Y + 1))
    }
}

function Copy-Sprite([System.Drawing.Bitmap] $source, [System.Drawing.Rectangle] $sourceRect, [System.Drawing.Bitmap] $sheet, [int] $column, [int] $row) {
    $crop = [System.Drawing.Bitmap]::new($sourceRect.Width, $sourceRect.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $cropGraphics = [System.Drawing.Graphics]::FromImage($crop)
    $cropGraphics.Clear([System.Drawing.Color]::Transparent)
    $cropGraphics.DrawImage($source, [System.Drawing.Rectangle]::new(0, 0, $crop.Width, $crop.Height), $sourceRect, [System.Drawing.GraphicsUnit]::Pixel)
    $cropGraphics.Dispose()

    Remove-ConnectedBackground $crop
    $bounds = [System.Drawing.Rectangle]::Empty
    for ($x = 0; $x -lt $crop.Width; $x++) {
        for ($y = 0; $y -lt $crop.Height; $y++) {
            if ($crop.GetPixel($x, $y).A -gt 0) {
                $bounds = if ($bounds.IsEmpty) {
                    [System.Drawing.Rectangle]::new($x, $y, 1, 1)
                } else {
                    [System.Drawing.Rectangle]::Union($bounds, [System.Drawing.Rectangle]::new($x, $y, 1, 1))
                }
            }
        }
    }

    $destination = [System.Drawing.Rectangle]::new($column * 128, $row * 128, 128, 128)
    if ($bounds.IsEmpty) {
        $crop.Dispose()
        return
    }

    $padding = 8
    $scale = [Math]::Min(($destination.Width - $padding * 2) / [double]$bounds.Width, ($destination.Height - $padding * 2) / [double]$bounds.Height)
    $scaledWidth = [Math]::Max(1, [int][Math]::Round($bounds.Width * $scale))
    $scaledHeight = [Math]::Max(1, [int][Math]::Round($bounds.Height * $scale))
    $target = [System.Drawing.Rectangle]::new(
        $destination.X + [int](($destination.Width - $scaledWidth) / 2),
        $destination.Y + [int](($destination.Height - $scaledHeight) / 2),
        $scaledWidth,
        $scaledHeight)

    $graphics = [System.Drawing.Graphics]::FromImage($sheet)
    $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
    $graphics.DrawImage($crop, $target, $bounds, [System.Drawing.GraphicsUnit]::Pixel)
    $graphics.Dispose()
    $crop.Dispose()
}

function New-TransparentSheet([int] $columns, [int] $rows) {
    $sheet = [System.Drawing.Bitmap]::new($columns * 128, $rows * 128, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($sheet)
    $graphics.Clear([System.Drawing.Color]::Transparent)
    $graphics.Dispose()
    return $sheet
}

function Save-Sheet([System.Drawing.Bitmap] $sheet, [string] $outputPath) {
    $sheet.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $sheet.Dispose()
}

function Build-AnimalSheet([string] $sourcePath, [string] $outputPath) {
    $source = [System.Drawing.Bitmap]::new($sourcePath)
    $sheet = New-TransparentSheet 4 2
    Copy-Sprite $source ([System.Drawing.Rectangle]::new(35, 24, 420, 410)) $sheet 0 0
    Copy-Sprite $source ([System.Drawing.Rectangle]::new(466, 24, 420, 410)) $sheet 1 0
    Copy-Sprite $source ([System.Drawing.Rectangle]::new(887, 24, 420, 410)) $sheet 2 0
    Copy-Sprite $source ([System.Drawing.Rectangle]::new(1309, 24, 420, 410)) $sheet 3 0
    Copy-Sprite $source ([System.Drawing.Rectangle]::new(35, 445, 420, 410)) $sheet 0 1
    Copy-Sprite $source ([System.Drawing.Rectangle]::new(466, 445, 420, 410)) $sheet 1 1
    Copy-Sprite $source ([System.Drawing.Rectangle]::new(887, 445, 420, 410)) $sheet 2 1
    Copy-Sprite $source ([System.Drawing.Rectangle]::new(1309, 445, 420, 410)) $sheet 3 1
    Save-Sheet $sheet $outputPath
    $source.Dispose()
}

$terrainX = @(210, 430, 645, 865)
$terrainWidths = @(176, 178, 168, 178)
$grassY = @(63, 226, 389, 522)
$grassHeights = @(148, 148, 114, 125)
$desertY = @(715, 857, 998, 1116)
$desertHeights = @(125, 125, 100, 109)
Build-AnimalSheet (Join-Path $sourceRoot 'Species\Animals\Animals_01.png') (Join-Path $outputRoot 'Animals_01_SpriteSheet.png')
$terrainSource = [System.Drawing.Bitmap]::new((Join-Path $sourceRoot 'Terrain\Terrain_01.png'))
$terrainSheet = New-TransparentSheet 4 8
$tileIndex = 0
foreach ($yIndex in 0..3) {
    foreach ($xIndex in 0..3) {
        Copy-Sprite $terrainSource ([System.Drawing.Rectangle]::new(
            $terrainX[$xIndex], $grassY[$yIndex], $terrainWidths[$xIndex], $grassHeights[$yIndex])) $terrainSheet ($tileIndex % 4) ([int][Math]::Floor($tileIndex / 4))
        $tileIndex++
    }
}
foreach ($yIndex in 0..3) {
    foreach ($xIndex in 0..3) {
        Copy-Sprite $terrainSource ([System.Drawing.Rectangle]::new(
            $terrainX[$xIndex], $desertY[$yIndex], $terrainWidths[$xIndex], $desertHeights[$yIndex])) $terrainSheet ($tileIndex % 4) ([int][Math]::Floor($tileIndex / 4))
        $tileIndex++
    }
}
Save-Sheet $terrainSheet (Join-Path $outputRoot 'Terrain_01_SpriteSheet.png')
$terrainSource.Dispose()
Write-Output "Wrote cellular sprite sheets to $outputRoot"
