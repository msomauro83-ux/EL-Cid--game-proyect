$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
[System.Windows.Forms.Application]::EnableVisualStyles()

$original = [IO.File]::ReadAllText((Join-Path $PSScriptRoot 'juego.ps1'), [Text.Encoding]::UTF8)
$encontrado = [regex]::Match($original, '(?s)\$datos = @''\r?\n(.*?)\r?\n''@')
if (-not $encontrado.Success) { throw 'No se encontraron las decisiones del juego.' }
$script:decisiones = @($encontrado.Groups[1].Value -split '\r?\n' | ForEach-Object { ,($_ -split '\|') })
if ($script:decisiones.Count -ne 100) { throw 'El juego necesita 100 decisiones.' }
$script:indice = 0
$script:correcta = 0

$ventana = [Windows.Forms.Form]::new()
$ventana.Text = 'El Cantar de Mío Cid — 100 decisiones'
$ventana.StartPosition = 'CenterScreen'
$ventana.ClientSize = [Drawing.Size]::new(1100, 820)
$ventana.MinimumSize = [Drawing.Size]::new(760, 650)
$ventana.BackColor = [Drawing.Color]::FromArgb(30, 26, 24)
$ventana.ForeColor = [Drawing.Color]::FromArgb(244, 232, 208)

$base = [Windows.Forms.TableLayoutPanel]::new()
$base.Dock = 'Fill'; $base.Padding = [Windows.Forms.Padding]::new(24, 12, 24, 16)
$base.ColumnCount = 1; $base.RowCount = 5
$base.RowStyles.Add([Windows.Forms.RowStyle]::new([Windows.Forms.SizeType]::Absolute, 36))
$base.RowStyles.Add([Windows.Forms.RowStyle]::new([Windows.Forms.SizeType]::Percent, 100))
$base.RowStyles.Add([Windows.Forms.RowStyle]::new([Windows.Forms.SizeType]::Absolute, 96))
$base.RowStyles.Add([Windows.Forms.RowStyle]::new([Windows.Forms.SizeType]::Absolute, 180))
$base.RowStyles.Add([Windows.Forms.RowStyle]::new([Windows.Forms.SizeType]::Absolute, 28))
$ventana.Controls.Add($base)

$cabecera = [Windows.Forms.Label]::new(); $cabecera.Dock = 'Fill'
$cabecera.Font = [Drawing.Font]::new('Georgia', 12, [Drawing.FontStyle]::Bold)
$cabecera.ForeColor = [Drawing.Color]::FromArgb(220, 175, 100)
$base.Controls.Add($cabecera, 0, 0)

$ilustracion = [Windows.Forms.PictureBox]::new(); $ilustracion.Dock = 'Fill'
$ilustracion.SizeMode = 'Zoom'; $ilustracion.BackColor = [Drawing.Color]::FromArgb(18, 17, 16)
$base.Controls.Add($ilustracion, 0, 1)

$pregunta = [Windows.Forms.Label]::new(); $pregunta.Dock = 'Fill'
$pregunta.Font = [Drawing.Font]::new('Georgia', 16, [Drawing.FontStyle]::Regular)
$pregunta.TextAlign = 'MiddleLeft'; $pregunta.Padding = [Windows.Forms.Padding]::new(0, 5, 0, 5)
$base.Controls.Add($pregunta, 0, 2)

$botonera = [Windows.Forms.TableLayoutPanel]::new(); $botonera.Dock = 'Fill'; $botonera.ColumnCount = 1; $botonera.RowCount = 3
for ($k=0; $k -lt 3; $k++) { $botonera.RowStyles.Add([Windows.Forms.RowStyle]::new([Windows.Forms.SizeType]::Percent, 33.333)) }
$base.Controls.Add($botonera, 0, 3)
$script:botones = @()
for ($k=0; $k -lt 3; $k++) {
    $boton = [Windows.Forms.Button]::new(); $boton.Dock = 'Fill'
    $boton.Margin = [Windows.Forms.Padding]::new(0, 3, 0, 3)
    $boton.FlatStyle = 'Flat'; $boton.FlatAppearance.BorderColor = [Drawing.Color]::FromArgb(139, 102, 58)
    $boton.BackColor = [Drawing.Color]::FromArgb(58, 45, 35)
    $boton.ForeColor = [Drawing.Color]::FromArgb(249, 238, 215)
    $boton.Font = [Drawing.Font]::new('Segoe UI', 11)
    $boton.TextAlign = 'MiddleLeft'; $boton.Padding = [Windows.Forms.Padding]::new(16,0,8,0)
    $boton.Tag = $k; $boton.Add_Click({ Elegir ([int]$this.Tag) })
    $botonera.Controls.Add($boton, 0, $k); $script:botones += $boton
}
$pie = [Windows.Forms.Label]::new(); $pie.Dock = 'Fill'; $pie.TextAlign = 'BottomRight'
$pie.Font = [Drawing.Font]::new('Segoe UI', 9); $pie.ForeColor = [Drawing.Color]::FromArgb(190, 174, 149)
$base.Controls.Add($pie, 0, 4)

function Imagen($nombre) {
    $ruta = Join-Path $PSScriptRoot "imagenes\$nombre.png"
    if (-not (Test-Path -LiteralPath $ruta)) { $ruta = Join-Path $PSScriptRoot 'imagenes\portada.png' }
    $anterior = $ilustracion.Image
    if (Test-Path -LiteralPath $ruta) {
        $stream = [IO.File]::OpenRead($ruta)
        try { $img = [Drawing.Image]::FromStream($stream); try { $ilustracion.Image = [Drawing.Bitmap]::new($img) } finally { $img.Dispose() } } finally { $stream.Dispose() }
    } else { $ilustracion.Image = $null }
    if ($anterior) { $anterior.Dispose() }
}
function Portada {
    $script:estado = 'portada'; $script:indice = 0
    $cabecera.Text = 'UNA AVENTURA BASADA EN EL CANTAR DE MÍO CID'
    $pregunta.Text = "EL CANTAR DE MÍO CID`n100 decisiones para recuperar la honra"
    Imagen 'portada'
    $script:botones[0].Text = 'COMENZAR LA AVENTURA'; $script:botones[0].Visible = $true
    $script:botones[1].Visible = $false; $script:botones[2].Visible = $false
    $pie.Text = 'Un error termina la partida. Debes comenzar de nuevo.'
}
function Mostrar-Pregunta {
    $script:estado = 'pregunta'
    $capitulo = if ($script:indice -lt 42) { 'I · EL DESTIERRO' } elseif ($script:indice -lt 68) { 'II · LAS BODAS' } else { 'III · LA AFRENTA DE CORPES' }
    $cabecera.Text = "$capitulo    /    DECISIÓN $($script:indice + 1) DE 100"
    $fila = $script:decisiones[$script:indice]
    $pregunta.Text = $fila[0]; Imagen ('{0:D3}' -f ($script:indice + 1))
    $script:correcta = ($script:indice * 7 + 1) % 3
    $orden = [System.Collections.Generic.List[string]]::new()
    for ($n=0; $n -lt 3; $n++) { $orden.Add('') }
    $orden[$script:correcta] = $fila[1]; $resto = @(0..2 | Where-Object { $_ -ne $script:correcta })
    $orden[$resto[0]] = $fila[2]; $orden[$resto[1]] = $fila[3]
    for ($n=0; $n -lt 3; $n++) { $script:botones[$n].Text = "$($n + 1).  $($orden[$n])"; $script:botones[$n].Visible = $true }
    $pie.Text = 'Una decisión incorrecta te devuelve al inicio de la aventura.'
}
function Elegir($opcion) {
    if ($script:estado -eq 'portada') { Mostrar-Pregunta; return }
    if ($script:estado -eq 'fin' -or $script:estado -eq 'derrota') { Portada; return }
    if ($opcion -ne $script:correcta) {
        $respuesta = $script:decisiones[$script:indice][1]
        $script:estado = 'derrota'
        $cabecera.Text = 'FIN DE LA PARTIDA'
        $pregunta.Text = "La historia tomó otro camino. Acertaste $($script:indice) de 100 decisiones.`nEn el cantar: $respuesta."
        $script:botones[0].Text = 'VOLVER AL PRINCIPIO'; $script:botones[1].Visible = $false; $script:botones[2].Visible = $false
        $pie.Text = 'La siguiente partida comienza desde la primera decisión.'
        $script:indice = 0
        return
    }
    $script:indice++
    if ($script:indice -lt 100) { Mostrar-Pregunta; return }
    $script:estado = 'fin'; Imagen 'final'
    $cabecera.Text = 'EL FINAL DEL CANTAR  /  100 DE 100'
    $pregunta.Text = "¡Has completado la historia!`nLa honra del Cid y de sus hijas ha sido reparada."
    $script:botones[0].Text = 'VOLVER A JUGAR'; $script:botones[1].Visible = $false; $script:botones[2].Visible = $false
    $pie.Text = 'Una adaptación del poema, entre historia y leyenda.'
}
Portada
[void][Windows.Forms.Application]::Run($ventana)
