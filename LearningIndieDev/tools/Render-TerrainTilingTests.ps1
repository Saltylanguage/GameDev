[CmdletBinding()]
param(
    [string]$AtlasPath,
    [string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

if ([string]::IsNullOrWhiteSpace($AtlasPath)) {
    $AtlasPath = Join-Path $PSScriptRoot '..\Assets\Art\Terrain\Terrain_01_SpriteSheet.png'
}
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $PSScriptRoot '..\artifacts\terrain-tiling-tests'
}

$atlas = [System.Drawing.Bitmap]::new((Resolve-Path -LiteralPath $AtlasPath).Path)
$tileWidth = [int]($atlas.Width / 4)
$tileHeight = [int]($atlas.Height / 8)
$grassAtlasIndexByMask = @(5, 13, 12, 14, 1, 10, 3, 2, 4, 11, 9, 7, 8, 6, 15, 0)
$dark = [System.Drawing.Color]::FromArgb(30, 30, 30)
$line = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255, 214, 80), 3)
$gridLine = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(70, 70, 70), 1)
$font = [System.Drawing.Font]::new('Segoe UI', 12)
$smallFont = [System.Drawing.Font]::new('Segoe UI', 9)
$whiteBrush = [System.Drawing.Brushes]::White
$directory = [System.IO.Path]::GetFullPath($OutputDirectory)
[System.IO.Directory]::CreateDirectory($directory) | Out-Null

function Draw-AtlasTile {
    param(
        [System.Drawing.Graphics]$Graphics,
        [int]$AtlasIndex,
        [int]$X,
        [int]$Y
    )

    $sourceX = ($AtlasIndex % 4) * $tileWidth
    $sourceY = [math]::Floor($AtlasIndex / 4) * $tileHeight
    $source = [System.Drawing.Rectangle]::new($sourceX, $sourceY, $tileWidth, $tileHeight)
    $destination = [System.Drawing.Rectangle]::new($X, $Y, $tileWidth, $tileHeight)
    $Graphics.DrawImage($atlas, $destination, $source, [System.Drawing.GraphicsUnit]::Pixel)
}

function Render-Case {
    param(
        [string]$Name,
        [string]$Title,
        [int]$CenterAtlasIndex,
        [int]$Mask,
        [int]$OpenDiagonalMask,
        [string[]]$NonGrassCells
    )

    $canvasWidth = $tileWidth * 3
    $canvasHeight = $tileHeight * 3 + 42
    $bitmap = [System.Drawing.Bitmap]::new($canvasWidth, $canvasHeight)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.Clear($dark)

    $graphics.DrawString($Title, $font, $whiteBrush, 8, 5)
    for ($row = 0; $row -lt 3; $row++) {
        for ($column = 0; $column -lt 3; $column++) {
            $cellKey = "$column,$row"
            $isCenter = $column -eq 1 -and $row -eq 1
            $isNonGrass = $NonGrassCells -contains $cellKey
            $atlasIndex = if ($isCenter) { $CenterAtlasIndex } elseif ($isNonGrass) { 16 } else { 0 }
            $x = $column * $tileWidth
            $y = 38 + ($row * $tileHeight)
            Draw-AtlasTile -Graphics $graphics -AtlasIndex $atlasIndex -X $x -Y $y
            $graphics.DrawRectangle($gridLine, $x, $y, $tileWidth - 1, $tileHeight - 1)
            if ($isCenter) {
                $graphics.DrawRectangle($line, $x + 1, $y + 1, $tileWidth - 3, $tileHeight - 3)
            }
        }
    }

    $metadata = "cardinal fallback mask $Mask; open diagonal mask $OpenDiagonalMask; atlas index $CenterAtlasIndex"
    $graphics.DrawString($metadata, $smallFont, $whiteBrush, 8, $canvasHeight - 18)
    $path = Join-Path $directory "$Name.png"
    $bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose()
    $bitmap.Dispose()
    Write-Output $path
}

try {
    # The resolver uses only N/E/S/W neighbors. Diagonal cells intentionally do not affect the mask.
    Render-Case -Name '01-se-diagonal-non-grass' -Title '1. Non-grass at bottom right' -CenterAtlasIndex $grassAtlasIndexByMask[15] -Mask 15 -OpenDiagonalMask 32 -NonGrassCells @('2,2')
    Render-Case -Name '02-north-south-non-grass' -Title '2. Non-grass above and below' -CenterAtlasIndex $grassAtlasIndexByMask[10] -Mask 10 -OpenDiagonalMask 0 -NonGrassCells @('1,0', '1,2')
    Render-Case -Name '03-four-corner-non-grass' -Title '3. Non-grass at all four corners' -CenterAtlasIndex $grassAtlasIndexByMask[15] -Mask 15 -OpenDiagonalMask 240 -NonGrassCells @('0,0', '2,0', '0,2', '2,2')
}
finally {
    $line.Dispose()
    $gridLine.Dispose()
    $font.Dispose()
    $smallFont.Dispose()
    $atlas.Dispose()
}
