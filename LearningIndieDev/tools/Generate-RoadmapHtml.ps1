[CmdletBinding()]
param([switch] $Check)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$sourcePath = Join-Path $PSScriptRoot '..\ROADMAP.md'
$outputPath = Join-Path $PSScriptRoot '..\ROADMAP.html'
$utf8 = [System.Text.UTF8Encoding]::new($false)
$lf = [string][char]10
$fencePattern = '^' + ([string][char]96 * 3)

function Html([string] $Text) {
    [System.Net.WebUtility]::HtmlEncode($Text)
}

function Inline([string] $Text) {
    $values = [System.Collections.Generic.List[string]]::new()
    $tokens = [System.Collections.Generic.List[string]]::new()
    $tick = [string][char]96
    $codePattern = [regex]::Escape($tick) + '([^' + [regex]::Escape($tick) + ']+)' + [regex]::Escape($tick)
    $Text = [regex]::Replace($Text, $codePattern, {
        param($m)
        $token = 'TOK' + $tokens.Count + 'END'
        $tokens.Add($token)
        $values.Add('<code>' + (Html $m.Groups[1].Value) + '</code>')
        $token
    })
    $Text = [regex]::Replace($Text, '\[(?<label>[^\]]+)\]\((?<url>[^)]+)\)', {
        param($m)
        $token = 'TOK' + $tokens.Count + 'END'
        $tokens.Add($token)
        $label = Html $m.Groups['label'].Value
        $url = Html $m.Groups['url'].Value
        $values.Add('<a href="' + $url + '">' + $label + '</a>')
        $token
    })
    $Text = Html $Text
    $Text = [regex]::Replace($Text, '\*\*(.+?)\*\*', '<strong>$1</strong>')
    for ($n = 0; $n -lt $tokens.Count; $n++) {
        $Text = $Text.Replace($tokens[$n], $values[$n])
    }
    $Text
}

function IsTable([string[]] $Lines, [int] $At) {
    if ($At + 1 -ge $Lines.Length -or $Lines[$At] -notmatch '\|') { return $false }
    $cells = $Lines[$At + 1].Trim().Trim('|').Split('|')
    if ($cells.Count -lt 2) { return $false }
    foreach ($cell in $cells) {
        if ($cell.Trim() -notmatch '^:?-{3,}:?$') { return $false }
    }
    $true
}

function Render([string] $Markdown) {
    $lines = $Markdown -split $lf
    $out = [System.Text.StringBuilder]::new()
    $i = 0
    $headingId = 0
    while ($i -lt $lines.Length) {
        $line = $lines[$i]
        if ([string]::IsNullOrWhiteSpace($line)) { $i++; continue }

        if ($line -match $fencePattern) {
            $i++
            $code = [System.Collections.Generic.List[string]]::new()
            while ($i -lt $lines.Length -and $lines[$i] -notmatch $fencePattern) { $code.Add($lines[$i]); $i++ }
            if ($i -lt $lines.Length) { $i++ }
            [void]$out.Append('<pre><code>').Append((Html ($code -join $lf))).Append('</code></pre>').Append($lf)
            continue
        }
        if ($line -match '^(#{1,6})\s+(.+?)\s*$') {
            $level = $Matches[1].Length
            $headingId++
            [void]$out.Append('<h').Append($level).Append(' id="section-').Append($headingId).Append('">').Append((Inline $Matches[2])).Append('</h').Append($level).Append('>').Append($lf)
            $i++
            continue
        }
        if ($line -match '^\s*>\s?(.*)$') {
            $parts = [System.Collections.Generic.List[string]]::new()
            while ($i -lt $lines.Length -and $lines[$i] -match '^\s*>\s?(.*)$') { $parts.Add($Matches[1].Trim()); $i++ }
            [void]$out.Append('<blockquote><p>').Append((Inline ($parts -join ' '))).Append('</p></blockquote>').Append($lf)
            continue
        }
        if (IsTable $lines $i) {
            $headers = @($lines[$i].Trim().Trim('|').Split('|') | ForEach-Object { $_.Trim() })
            $align = @($lines[$i + 1].Trim().Trim('|').Split('|') | ForEach-Object {
                $cell = $_.Trim()
                if ($cell.StartsWith(':') -and $cell.EndsWith(':')) { 'center' }
                elseif ($cell.EndsWith(':')) { 'right' }
                else { 'left' }
            })
            $class = if ($headers.Count -ge 8 -and $headers[0] -eq 'ID') { 'feature-ledger' } else { 'roadmap-table' }
            [void]$out.Append('<div class="table-wrap"><table class="').Append($class).Append('"><thead><tr>')
            for ($col = 0; $col -lt $headers.Count; $col++) {
                [void]$out.Append('<th scope="col" style="text-align:').Append($align[$col]).Append('">').Append((Inline $headers[$col])).Append('</th>')
            }
            [void]$out.Append('</tr></thead><tbody>')
            $i += 2
            while ($i -lt $lines.Length -and $lines[$i].Trim().StartsWith('|')) {
                $cells = @($lines[$i].Trim().Trim('|').Split('|') | ForEach-Object { $_.Trim() })
                [void]$out.Append('<tr>')
                for ($col = 0; $col -lt $headers.Count; $col++) {
                    $value = if ($col -lt $cells.Count) { $cells[$col] } else { '' }
                    [void]$out.Append('<td style="text-align:').Append($align[$col]).Append('">').Append((Inline $value)).Append('</td>')
                }
                [void]$out.Append('</tr>')
                $i++
            }
            [void]$out.Append('</tbody></table></div>').Append($lf)
            continue
        }
        if ($line -match '^\s*[-*]\s+' -or $line -match '^\s*\d+\.\s+') {
            $ordered = $line -match '^\s*\d+\.\s+'
            $tag = if ($ordered) { 'ol' } else { 'ul' }
            [void]$out.Append('<').Append($tag).Append('>').Append($lf)
            while ($i -lt $lines.Length) {
                $isItem = if ($ordered) { $lines[$i] -match '^\s*\d+\.\s+(.*)$' } else { $lines[$i] -match '^\s*[-*]\s+(.*)$' }
                if (-not $isItem) { break }
                $item = [System.Collections.Generic.List[string]]::new()
                $item.Add($Matches[1].Trim())
                $i++
                while ($i -lt $lines.Length -and $lines[$i] -match '^\s{2,}\S') { $item.Add($lines[$i].Trim()); $i++ }
                [void]$out.Append('<li>').Append((Inline ($item -join ' '))).Append('</li>').Append($lf)
            }
            [void]$out.Append('</').Append($tag).Append('>').Append($lf)
            continue
        }

        $para = [System.Collections.Generic.List[string]]::new()
        while ($i -lt $lines.Length -and -not [string]::IsNullOrWhiteSpace($lines[$i])) {
            if ($para.Count -gt 0 -and (
                $lines[$i] -match '^(#{1,6})\s+' -or $lines[$i] -match '^\s*>\s?' -or
                $lines[$i] -match $fencePattern -or $lines[$i] -match '^\s*[-*]\s+' -or
                $lines[$i] -match '^\s*\d+\.\s+' -or (IsTable $lines $i)
            )) { break }
            $para.Add($lines[$i].Trim())
            $i++
        }
        [void]$out.Append('<p>').Append((Inline ($para -join ' '))).Append('</p>').Append($lf)
    }
    $out.ToString()
}

$sourceText = [System.IO.File]::ReadAllText((Resolve-Path $sourcePath).Path)
$sourceText = $sourceText.Replace(([string][char]13 + [string][char]10), $lf).Replace([string][char]13, $lf)
$hash = [Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData($utf8.GetBytes($sourceText))).ToLowerInvariant()
$titleMatch = [regex]::Match($sourceText, '(?m)^#\s+(.+?)\s*$')
$title = if ($titleMatch.Success) { $titleMatch.Groups[1].Value } else { 'Product Roadmap' }
$safeTitle = Html $title
$body = Render $sourceText

$html = [System.Text.StringBuilder]::new()
$shell = @(
    '<!doctype html>', '<html lang="en">', '<head>', '<meta charset="utf-8">',
    '<meta name="viewport" content="width=device-width, initial-scale=1">',
    ('<meta name="description" content="' + $safeTitle + '">'), ('<title>' + $safeTitle + '</title>'),
    '<style>',
    ':root { color-scheme: light; --ink:#202a26; --muted:#5a6861; --forest:#294d43; --border:#d6dfda; --stripe:#f4f8f6; --paper:#fff; --ground:#edf2ef; }',
    '* { box-sizing:border-box; } body { margin:0; background:var(--ground); color:var(--ink); font:16px/1.55 "Segoe UI",Aptos,Arial,sans-serif; }',
    'article { max-width:1320px; margin:32px auto; padding:48px 56px; background:var(--paper); box-shadow:0 10px 34px rgba(32,58,47,.09); }',
    '.generated-note { margin:0 0 24px; padding:10px 14px; border-left:3px solid var(--forest); background:#f4f8f6; color:var(--muted); font-size:.88rem; }',
    'h1,h2,h3,h4,h5,h6 { color:#17211d; line-height:1.22; } h1 { margin:.2em 0 .6em; font-size:clamp(2rem,4vw,2.7rem); letter-spacing:-.025em; }',
    'h2 { margin:2.2em 0 .6em; padding-bottom:.28em; border-bottom:1px solid var(--border); font-size:1.55rem; } h3 { margin:1.65em 0 .45em; font-size:1.18rem; } p { margin:.6em 0 .95em; }',
    'a { color:#1c5c4a; text-decoration-thickness:.08em; text-underline-offset:.15em; } a:hover { color:#103b30; }',
    'blockquote { margin:1em 0; padding:.65em 1em; border-left:3px solid #789387; background:#f4f8f6; color:#44554d; } blockquote p { margin:.2em 0; }',
    'ul,ol { padding-left:1.6em; margin:.55em 0 1em; } li { margin:.3em 0; padding-left:.12em; }',
    'code { padding:.06em .28em; border-radius:3px; background:#f0f4f2; color:#284c40; font:.91em Consolas,"Cascadia Code",monospace; }',
    'pre { overflow:auto; margin:.8em 0 1.2em; padding:15px 18px; border:1px solid var(--border); background:#f7faf8; color:#284c40; font:.88rem/1.5 Consolas,"Cascadia Code",monospace; } pre code { padding:0; background:transparent; }',
    '.table-wrap { max-width:100%; margin:18px 0 24px; overflow-x:auto; border:1px solid var(--border); } table { width:100%; border-collapse:collapse; font-size:.88rem; line-height:1.4; } table.feature-ledger { min-width:1340px; }',
    'th,td { padding:9px 11px; border:1px solid var(--border); text-align:left; vertical-align:top; } th { position:sticky; top:0; background:#e9f1ed; color:#203c32; font-weight:700; } tbody tr:nth-child(even) { background:var(--stripe); }',
    'footer { margin-top:40px; padding-top:14px; border-top:1px solid var(--border); color:var(--muted); font-size:.8rem; overflow-wrap:anywhere; }',
    '@media(max-width:760px) { article { margin:0; padding:26px 20px; } h1 { font-size:2rem; } } @media print { body { background:#fff; } article { max-width:none; margin:0; padding:0; box-shadow:none; } th { position:static; } }',
    '</style>', '</head>', '<body>', '<article>',
    '<p class="generated-note">Generated from ROADMAP.md. Update the Markdown source and regenerate this page; direct edits to this HTML file will be overwritten.</p>',
    '<main>'
)
foreach ($line in $shell) { [void]$html.Append($line).Append($lf) }
[void]$html.Append($body)
[void]$html.Append('<footer>Generated from ROADMAP.md · source SHA-256: <code>').Append($hash).Append('</code></footer>').Append($lf)
[void]$html.Append('</main>').Append($lf).Append('</article>').Append($lf).Append('</body>').Append($lf).Append('</html>').Append($lf)
$expected = $html.ToString()

if ($Check) {
    if (-not (Test-Path -LiteralPath $outputPath -PathType Leaf)) {
        Write-Error 'ROADMAP.html is missing. Run Generate-RoadmapHtml.ps1 to create it.'
        exit 1
    }
    $actual = [System.IO.File]::ReadAllText($outputPath).Replace(([string][char]13 + [string][char]10), $lf).Replace([string][char]13, $lf)
    if ($actual -cne $expected) {
        Write-Error 'ROADMAP.html is stale or was edited by hand. Run Generate-RoadmapHtml.ps1 and commit the refreshed output.'
        exit 1
    }
    Write-Output 'ROADMAP.html matches ROADMAP.md.'
    exit 0
}

[System.IO.File]::WriteAllText($outputPath, $expected, $utf8)
Write-Output ('Generated ' + $outputPath)
