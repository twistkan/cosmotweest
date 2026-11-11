$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$taskRoot = Split-Path $PSScriptRoot -Parent
$artFolder = Join-Path $taskRoot 'artwork'
New-Item -ItemType Directory -Force $artFolder | Out-Null
$image = New-Object System.Drawing.Bitmap 1024,1024
$g = [Drawing.Graphics]::FromImage($image)
$g.SmoothingMode = 'AntiAlias'
$g.ScaleTransform(4,4)
$badge = New-Object Drawing.Drawing2D.GraphicsPath
foreach($arc in @(@(8,8,60,60,180,90),@(188,8,60,60,270,90),@(188,188,60,60,0,90),@(8,188,60,60,90,90))){$badge.AddArc($arc[0],$arc[1],$arc[2],$arc[3],$arc[4],$arc[5])}
$badge.CloseFigure()
$gradient = New-Object Drawing.Drawing2D.LinearGradientBrush ([Drawing.Point]::new(0,0)),([Drawing.Point]::new(256,256)),([Drawing.Color]::FromArgb(36,37,88)),([Drawing.Color]::FromArgb(5,15,33))
$g.FillPath($gradient,$badge)
$outline = New-Object Drawing.Pen ([Drawing.Color]::FromArgb(63,83,142)),2
$g.DrawPath($outline,$badge)
$orbit = New-Object Drawing.Pen ([Drawing.Color]::FromArgb(70,163,203)),5
$g.DrawArc($orbit,35,50,180,156,23,280)
$orbit.Dispose()
$planet = New-Object Drawing.SolidBrush ([Drawing.Color]::FromArgb(66,53,113))
$g.FillEllipse($planet,46,143,67,67)
$planet.Dispose()
function Polygon($color,$coords){
    $points = [Drawing.PointF[]]@(for($i=0;$i -lt $coords.Length;$i+=2){[Drawing.PointF]::new($coords[$i],$coords[$i+1])})
    $brush = New-Object Drawing.SolidBrush $color
    $g.FillPolygon($brush,$points)
    $brush.Dispose()
}
# A rising silver rocket, its cyan canopy and bright exhaust readable at 16 px.
$state = $g.Save()
$g.TranslateTransform(132,124)
$g.RotateTransform(35)
Polygon ([Drawing.Color]::FromArgb(255,129,53)) @( -14,43, -17,69, 0,108, 17,69, 14,43 )
Polygon ([Drawing.Color]::FromArgb(255,222,127)) @( -7,44, -8,68, 0,88, 8,68, 7,44 )
Polygon ([Drawing.Color]::FromArgb(70,212,243)) @( -12,0, -42,49, -16,39, 0,50, 16,39, 42,49, 12,0 )
Polygon ([Drawing.Color]::FromArgb(219,236,250)) @( 0,-91, -19,-51, -18,30, -11,47, 11,47, 18,30, 19,-51 )
Polygon ([Drawing.Color]::FromArgb(121,160,195)) @( 0,-91, 0,44, 11,47, 18,30, 19,-51 )
Polygon ([Drawing.Color]::FromArgb(15,43,70)) @( 0,-54, -11,-31, -9,-2, 0,9, 9,-2, 11,-31 )
Polygon ([Drawing.Color]::FromArgb(81,238,255)) @( 0,-45, -7,-28, -6,-5, 0,2, 6,-5, 7,-28 )
$g.Restore($state)
$starPen = New-Object Drawing.Pen ([Drawing.Color]::FromArgb(179,236,255)),3
foreach($star in @(@(57,63,8),@(210,109,5),@(196,207,5))){$x=$star[0];$y=$star[1];$r=$star[2];$g.DrawLine($starPen,$x-$r,$y,$x+$r,$y);$g.DrawLine($starPen,$x,$y-$r,$x,$y+$r)}
$starPen.Dispose();$g.Dispose();$gradient.Dispose();$outline.Dispose();$badge.Dispose()
$sizes = @(16,24,32,48,64,128,256)
$blobs = @()
foreach($size in $sizes){
    $small = New-Object Drawing.Bitmap $size,$size
    $sg=[Drawing.Graphics]::FromImage($small)
    $sg.InterpolationMode='HighQualityBicubic'
    $sg.DrawImage($image,0,0,$size,$size)
    $sg.Dispose()
    if($size -eq 256){$small.Save((Join-Path $artFolder 'CosmoTweest-Icon.png'),[Drawing.Imaging.ImageFormat]::Png)}
    $memory = New-Object IO.MemoryStream
    $small.Save($memory,[Drawing.Imaging.ImageFormat]::Png)
    $blobs += ,$memory.ToArray()
    $memory.Dispose();$small.Dispose()
}
$iconPath = Join-Path $taskRoot 'src\CosmoTweest.ico'
$file = [IO.File]::Create($iconPath)
$writer = New-Object IO.BinaryWriter $file
$writer.Write([uint16]0);$writer.Write([uint16]1);$writer.Write([uint16]$sizes.Length)
$offset = 6 + 16*$sizes.Length
for($i=0;$i -lt $sizes.Length;$i++){
    $dimension = if($sizes[$i] -eq 256){0}else{$sizes[$i]}
    $writer.Write([byte]$dimension);$writer.Write([byte]$dimension);$writer.Write([byte]0);$writer.Write([byte]0)
    $writer.Write([uint16]1);$writer.Write([uint16]32);$writer.Write([uint32]$blobs[$i].Length);$writer.Write([uint32]$offset)
    $offset += $blobs[$i].Length
}
foreach($blob in $blobs){$writer.Write([byte[]]$blob)}
$writer.Dispose();$image.Dispose()
Write-Host 'Created seven-size Windows icon and PNG preview.'
