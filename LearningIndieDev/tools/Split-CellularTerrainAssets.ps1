$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

$projectRoot = Join-Path $PSScriptRoot '..'
$sourcePath = Join-Path $projectRoot 'Assets\Art\Terrain\Terrain_01_SpriteSheet.png'
$outputRoot = Join-Path $projectRoot 'Assets\Art\Terrain\Standardized'
$sizes = @(32, 64, 128)
$tileSize = 128
$columns = 4
$rows = 8

function Get-DeterministicGuid([string] $relativePath) {
    $sha1 = [System.Security.Cryptography.SHA1]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($relativePath.Replace('\', '/').ToLowerInvariant())
        return (($sha1.ComputeHash($bytes) | ForEach-Object { $_.ToString('x2') }) -join '').Substring(0, 32)
    }
    finally {
        $sha1.Dispose()
    }
}

function Write-FolderMeta([string] $folderPath, [string] $relativePath) {
    $metaPath = "$folderPath.meta"
    if (Test-Path -LiteralPath $metaPath) {
        return
    }

    @(
        'fileFormatVersion: 2'
        "guid: $(Get-DeterministicGuid $relativePath)"
        'folderAsset: yes'
        'DefaultImporter:'
        '  externalObjects: {}'
        '  userData:'
        '  assetBundleName:'
        '  assetBundleVariant:'
    ) | Set-Content -LiteralPath $metaPath -Encoding utf8NoBOM
}

function Write-SpriteMeta([string] $pngPath, [string] $relativePath, [int] $pixelsPerUnit) {
    $metaPath = "$pngPath.meta"
    if (Test-Path -LiteralPath $metaPath) {
        return
    }

    @(
        'fileFormatVersion: 2'
        "guid: $(Get-DeterministicGuid $relativePath)"
        'TextureImporter:'
        '  internalIDToNameTable: []'
        '  externalObjects: {}'
        '  serializedVersion: 13'
        '  mipmaps:'
        '    mipMapMode: 0'
        '    enableMipMap: 0'
        '    sRGBTexture: 1'
        '    linearTexture: 0'
        '  isReadable: 0'
        '  streamingMipmaps: 0'
        '  textureSettings:'
        '    serializedVersion: 2'
        '    filterMode: 0'
        '    aniso: 1'
        '    mipBias: 0'
        '    wrapU: 1'
        '    wrapV: 1'
        '    wrapW: 0'
        '  nPOTScale: 0'
        '  spriteMode: 1'
        '  spriteExtrude: 1'
        '  spriteMeshType: 1'
        '  alignment: 0'
        '  spritePivot: {x: 0.5, y: 0.5}'
        "  spritePixelsToUnits: $pixelsPerUnit"
        '  spriteBorder: {x: 0, y: 0, z: 0, w: 0}'
        '  alphaUsage: 1'
        '  alphaIsTransparency: 1'
        '  textureType: 8'
        '  textureShape: 1'
        '  spriteSheet:'
        '    serializedVersion: 2'
        '    sprites: []'
        '    outline: []'
        '    customData:'
        '    physicsShape: []'
        '    bones: []'
        '    spriteID: 5e97eb03825dee720800000000000000'
        '    internalID: 0'
        '    vertices: []'
        '    indices:'
        '    edges: []'
        '    weights: []'
        '    secondaryTextures: []'
        '    spriteCustomMetadata:'
        '      entries: []'
        '    nameFileIdTable: {}'
        '  userData:'
        '  assetBundleName:'
        '  assetBundleVariant:'
    ) | Set-Content -LiteralPath $metaPath -Encoding utf8NoBOM
}

function Save-NearestNeighbor([System.Drawing.Bitmap] $source, [string] $path, [int] $size) {
    $target = [System.Drawing.Bitmap]::new($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($target)
    $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
    $graphics.DrawImage($source, [System.Drawing.Rectangle]::new(0, 0, $size, $size), 0, 0, $source.Width, $source.Height, [System.Drawing.GraphicsUnit]::Pixel)
    $graphics.Dispose()
    $target.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $target.Dispose()
}

if (-not (Test-Path -LiteralPath $sourcePath)) {
    throw "Terrain source sheet not found: $sourcePath"
}

New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null
Write-FolderMeta $outputRoot 'Assets/Art/Terrain/Standardized'
foreach ($size in $sizes) {
    $sizeRoot = Join-Path $outputRoot "$size"
    New-Item -ItemType Directory -Force -Path $sizeRoot | Out-Null
    Write-FolderMeta $sizeRoot "Assets/Art/Terrain/Standardized/$size"
}

$source = [System.Drawing.Bitmap]::new($sourcePath)
try {
    if ($source.Width -ne $columns * $tileSize -or $source.Height -ne $rows * $tileSize) {
        throw "Expected a $($columns * $tileSize)x$($rows * $tileSize) source sheet; got $($source.Width)x$($source.Height)."
    }

    for ($row = 0; $row -lt $rows; $row++) {
        for ($column = 0; $column -lt $columns; $column++) {
            $family = if ($row -lt 4) { 'Grass' } else { 'Desert' }
            $variant = (($row % 4) * $columns) + $column
            $name = "Terrain_01_${family}_$('{0:D2}' -f $variant)"
            $crop = $source.Clone(
                [System.Drawing.Rectangle]::new($column * $tileSize, $row * $tileSize, $tileSize, $tileSize),
                [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
            try {
                $rootPath = Join-Path $outputRoot "$name.png"
                $crop.Save($rootPath, [System.Drawing.Imaging.ImageFormat]::Png)
                Write-SpriteMeta $rootPath "Assets/Art/Terrain/Standardized/$name.png" $tileSize

                foreach ($size in $sizes) {
                    $sizePath = Join-Path (Join-Path $outputRoot "$size") "$name.png"
                    Save-NearestNeighbor $crop $sizePath $size
                    Write-SpriteMeta $sizePath "Assets/Art/Terrain/Standardized/$size/$name.png" $size
                }
            }
            finally {
                $crop.Dispose()
            }
        }
    }
}
finally {
    $source.Dispose()
}

Write-Output "Wrote $($columns * $rows) terrain tiles at source, 32, 64, and 128 pixels to $outputRoot"
