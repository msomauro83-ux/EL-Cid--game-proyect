$ErrorActionPreference = 'Stop'
[Console]::InputEncoding = [System.Text.UTF8Encoding]::new()
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new()

# Cada línea contiene situación, opción fiel al poema y dos alternativas.
$datos = @'
El rey Alfonso ha desterrado a Rodrigo. ¿Qué plazo tiene para salir de sus tierras?|Nueve días|Un año|Tres meses
Rodrigo reúne a sus parientes y vasallos. ¿Qué les comunica?|Que pueden acompañarlo al destierro|Que entregará Castilla a otro rey|Que renuncia para siempre a sus armas
¿Quién decide acompañar fielmente al Cid?|Álvar Fáñez, llamado Minaya|Los infantes de Carrión|El rey Búcar
Al salir de Vivar, el Cid mira su casa abandonada. ¿Cómo se siente?|Llora por lo que deja atrás|Celebra la pérdida de su casa|No reconoce el lugar
Rodrigo llega a Burgos. ¿Por qué la gente teme darle posada?|El rey ha prohibido ayudarlo|Una epidemia ha vaciado la ciudad|El Cid ha prohibido abrir las puertas
Una niña habla con el Cid en Burgos. ¿Qué le explica?|Que el castigo del rey amenaza a quien lo hospede|Que el rey ya le ha perdonado|Que Valencia ha sido conquistada
Sin posada en Burgos, ¿dónde acampa el Cid?|En la Glera, junto al río|En el palacio del rey|En el robledo de Corpes
¿Quién lleva pan y vino al Cid y a sus hombres?|Martín Antolínez|El conde de Barcelona|El rey Búcar
El Cid necesita dinero para sus hombres. ¿Qué propone usar como garantía?|Dos arcas llenas de arena|La corona del rey|Las murallas de Valencia
¿Con quiénes negocia Martín Antolínez el préstamo?|Raquel y Vidas|Los infantes de Navarra y Aragón|Hariz y Galve
¿Qué creen los prestamistas que contienen las arcas?|Riquezas del Cid|Armas de los infantes|Libros del monasterio
¿Qué condición se impone a los prestamistas sobre las arcas?|No deben abrirlas durante el plazo acordado|Deben abrirlas inmediatamente|Deben arrojarlas al río
Antes de partir, Rodrigo se encomienda a la Virgen. ¿Adónde se dirige después?|Al monasterio de San Pedro de Cardeña|A las cortes de Toledo|Al palacio de Carrión
¿Quién espera al Cid en Cardeña?|Su esposa Jimena y sus hijas|El conde de Barcelona y sus soldados|Los jueces de Toledo
¿A quién confía el Cid el cuidado de su familia?|Al abad de Cardeña|A los infantes de Carrión|Al rey Búcar
En la despedida, ¿qué espera Rodrigo para sus hijas?|Llegar a casarlas honradamente|Que lo acompañen a todas las batallas|Que gobiernen Burgos esa misma noche
La noticia del destierro atrae a nuevos hombres. ¿Qué hacen?|Se unen a la mesnada del Cid|Arrestan al abad|Cierran el camino de Cardeña
Antes de separarse de su familia, ¿qué hace Jimena?|Reza y pide protección para el Cid|Marcha con los infantes a Carrión|Exige que Rodrigo ataque al rey
Después de despedirse, ¿qué camino toma el Campeador?|Sale de Castilla con sus compañeros|Permanece oculto en Cardeña|Vuelve a pedir posada en Burgos
Al iniciar su campaña, ¿qué necesita conseguir para sostener a sus hombres?|Botín y recursos|Una flota para cruzar el océano|Un trono en Francia
El Cid se aproxima a Castejón. ¿Qué táctica emplea?|Prepara una emboscada y toma la población|Pide al rey que la destruya|Entrega sus armas y se rinde
Después de tomar Castejón, ¿qué hace el Cid con el botín?|Lo reparte entre sus hombres|Lo arroja al río|Lo entrega todo a sus enemigos
¿A quién envía el Cid con una partida a obtener ganancias por otras tierras?|A Minaya Álvar Fáñez|Al rey Alfonso|A Diego y Fernando González
Tras abandonar Castejón, ¿hacia qué zona continúa el Cid?|Hacia Alcocer, junto al Jalón|Hacia el robledo de Corpes para una boda|Hacia la corte de Navarra
¿Cómo logra entrar en Alcocer?|Finge retirarse y aprovecha la salida de sus defensores|Espera a que el rey Alfonso abra las puertas|La compra con las espadas Colada y Tizón
Una vez tomada Alcocer, ¿qué ocurre?|Sus enemigos reúnen fuerzas para cercarlo|Los infantes son juzgados en Toledo|Jimena es coronada reina de Aragón
¿Qué jefes enemigos cercan al Cid en Alcocer?|Hariz y Galve|Raquel y Vidas|Navarra y Aragón
Ante el cerco de Alcocer, ¿qué decide el Campeador?|Salir a combatir con sus hombres|Abandonar a sus soldados|Rendirse sin combatir
¿Quién lleva la enseña del Cid y se adelanta al combate?|Pero Bermúdez|El abad de Cardeña|El conde de Barcelona
En la batalla contra Hariz y Galve, ¿qué consiguen los hombres del Cid?|Rompen el cerco y vencen|Pierden Valencia|Son juzgados en Toledo
Tras la victoria, ¿qué hace el Cid con los bienes obtenidos?|Reparte el botín entre sus compañeros|Entrega todo a Hariz|Lo deja abandonado en Alcocer
El Campeador desea recuperar el favor real. ¿Qué envía al rey?|Caballos como presente|Las arcas llenas de arena|Una amenaza de guerra
¿Quién lleva al rey el primer presente del Cid?|Minaya Álvar Fáñez|El rey Búcar|El infante Fernando
¿Cómo recibe Alfonso el presente?|Lo acepta, aunque aún no levanta el destierro|Concede inmediatamente Valencia a los infantes|Encierra para siempre a Minaya
El Cid necesita continuar su camino. ¿Qué hace con Alcocer?|La vende antes de marcharse|La entrega al conde de Barcelona como dote|La convierte en sede de las cortes
La fama del Campeador crece. ¿Qué sucede con su mesnada?|Más caballeros se unen a él|Todos sus hombres lo abandonan|Se compone solo de los infantes
El Cid hace campañas por tierras próximas a Zaragoza. ¿Qué obtiene?|Tributos y botín|La corona de Aragón|Una sentencia contra sus hijas
¿Qué noble se enfrenta al Cid por estas campañas?|El conde de Barcelona|El abad de Cardeña|Raquel
¿Qué ocurre en la batalla contra el conde de Barcelona?|El Cid vence y captura al conde|El conde captura al rey Alfonso|El Cid pierde a Babieca
¿Qué espada gana el Cid en ese encuentro?|Colada|Excalibur|Durandarte
El conde cautivo rehúsa comer. ¿Cómo responde el Cid?|Le promete la libertad si come|Lo obliga a casarse con Jimena|Le entrega Valencia inmediatamente
Cuando el conde acepta comer, ¿qué hace el Cid?|Cumple su palabra y lo libera|Lo lleva como preso a Carrión|Lo nombra rey de Castilla
Tras estas campañas, ¿hacia qué importante ciudad dirige sus esfuerzos?|Valencia|París|Roma
¿Cómo consigue el Campeador apoderarse de Valencia?|La cerca hasta que se rinde|La recibe de los infantes de Carrión|La descubre abandonada desde hace siglos
Tras la conquista de Valencia, ¿qué hace el Cid con sus hombres?|Los establece y reparte los beneficios|Los despide sin recompensa|Los envía presos a Burgos
¿Qué religioso llega a Valencia y se convierte en obispo?|Don Jerónimo|El abad de Cardeña|El rey Yúcef
El Cid vuelve a enviar un presente al rey. ¿Cuál es su propósito?|Mantener el camino hacia la reconciliación|Exigir que Alfonso renuncie al trono|Anunciar la derrota en Valencia
¿A quién vuelve a enviar Rodrigo como mensajero al rey?|A Minaya|A Búcar|A Raquel
¿Qué solicita el Cid para su familia?|Que Jimena y sus hijas puedan reunirse con él|Que sus hijas sean llevadas a Carrión sin él|Que Jimena permanezca encerrada para siempre
¿Qué decisión toma el rey sobre la familia del Cid?|Permite que salga hacia Valencia|Ordena llevarla a Barcelona|Prohíbe que abandone Cardeña
¿Quién acompaña a Jimena y a sus hijas en el viaje?|Minaya y los hombres del Cid|Hariz y Galve|Los mensajeros del rey Búcar
Al acercarse su familia, ¿qué hace Rodrigo?|Sale a recibirla|Abandona Valencia en secreto|Envía a las damas de vuelta a Castilla
¿Qué muestra el Cid a Jimena y a sus hijas desde lo alto?|Valencia y sus fértiles alrededores|Las cortes reunidas en Toledo|El robledo de Corpes
Un ejército del rey Yúcef llega contra Valencia. ¿Qué hace el Cid?|Se prepara para defender la ciudad|Entrega Valencia sin luchar|Huye a Cardeña
¿Qué resultado tiene la batalla contra Yúcef?|El Cid y sus hombres vencen|Yúcef captura al Cid|Los infantes gobiernan Valencia
Después de vencer a Yúcef, ¿qué comparte el Campeador?|Las ganancias del combate|El reino de Francia|Las arcas originales sin abrir
Las riquezas del Cid despiertan el interés de unos nobles. ¿Quiénes son?|Los infantes de Carrión|Hariz y Galve|Raquel y Vidas
¿Qué desean los infantes de Carrión?|Casarse con las hijas del Cid|Retirarse a vivir en Cardeña|Devolver Colada al conde de Barcelona
¿A través de quién plantean los infantes sus deseos de matrimonio?|Del rey Alfonso|Del rey Búcar|Del conde de Barcelona
El rey pide encontrarse con el Cid. ¿Para qué sirve la reunión?|Para reconciliarse y tratar los matrimonios|Para condenar a Jimena|Para entregar Castilla a Yúcef
¿Dónde se encuentran el rey y el Campeador?|A orillas del Tajo|En el robledo de Corpes|En la Glera de Burgos
En el encuentro, ¿qué hace el Cid ante Alfonso?|Le muestra respeto y acata su autoridad|Lo desafía a un duelo por Valencia|Le exige que abandone Castilla
¿Qué concede el rey al Cid en este encuentro?|Su perdón y la recuperación de su favor|El trono de Francia|El mando sobre Navarra y Aragón
Aunque recela de los infantes, ¿qué responde el Cid a la petición del rey?|Acepta los matrimonios propuestos|Casa a sus hijas con Hariz y Galve|Envía a sus hijas de inmediato a un convento
¿Cómo se llaman las hijas del Cid en el poema?|Elvira y Sol|Jimena y Urraca|Raquel y Vidas
¿Quiénes se casan con Elvira y Sol?|Diego y Fernando González, infantes de Carrión|Hariz y Galve|Los reyes de Navarra y Aragón
¿Dónde se celebran las bodas de las hijas del Cid con los infantes?|En Valencia|En Burgos durante el destierro|En el robledo de Corpes
¿Cuánto duran las fiestas de las bodas según el relato?|Quince días|Una sola hora|Diez años
Tiempo después, mientras el Cid duerme, ¿qué animal se escapa?|Un león|Un águila|Un toro
Ante el león suelto, ¿cómo reaccionan los infantes de Carrión?|Se esconden llenos de miedo|Lo dominan juntos sin ayuda|Abandonan Valencia al frente del ejército
¿Qué hace el Cid cuando despierta y ve al león?|Lo sujeta y lo devuelve a su jaula|Huye bajo un banco|Lo entrega al rey Búcar
¿Qué efecto tiene el episodio del león en los infantes?|Se sienten avergonzados y ofendidos por las burlas|Reciben el perdón del rey|Son nombrados obispos
Otro rey llega con un ejército contra Valencia. ¿Quién es?|Búcar|Alfonso|El conde de Barcelona
¿Qué exige Búcar en su mensaje al Cid?|Que abandone Valencia|Que celebre nuevas bodas en Toledo|Que devuelva las arcas a Raquel y Vidas
Ante el ejército de Búcar, ¿qué hacen los hombres del Cid?|Salen a combatir|Entregan Valencia y huyen|Esperan en Cardeña la sentencia del rey
En la batalla, ¿qué evidencia Fernando, uno de los infantes?|Su cobardía ante el combate|Que es el obispo de Valencia|Que ha capturado al rey Alfonso
¿Qué caballero protege a Fernando y luego conoce la verdad de su actuación?|Pero Bermúdez|El rey Búcar|El abad de Cardeña
¿Qué obtiene el Cid tras vencer a Búcar?|La espada Tizón y un gran botín|Las arcas de arena de vuelta|El trono de Navarra
Los infantes planean vengar su vergüenza. ¿Qué piden al Cid?|Llevar a sus esposas a Carrión|Quedarse para siempre en Valencia|Que el león los acompañe a la corte
¿Qué hace el Cid ante la petición de sus yernos?|Consiente la partida y dota a sus hijas|Encierra a sus hijas en una torre|Envía a los infantes sin sus esposas y sin regalos
¿Qué entrega el Cid a sus yernos entre otros regalos?|Las espadas Colada y Tizón|La corona de Alfonso|Las llaves de Cardeña
¿A qué familiar envía Rodrigo para acompañar a sus hijas?|A Félez Muñoz|A Hariz|Al rey Yúcef
En Molina, ¿quién recibe hospitalariamente a los viajeros?|Abengalbón, amigo del Cid|El conde de Barcelona|El rey Búcar
¿Qué planean los infantes contra Abengalbón?|Matarlo para robar sus riquezas|Nombrarlo rey de Castilla|Entregarle a sus esposas
¿Qué sucede con el plan contra Abengalbón?|Es descubierto y Abengalbón les reprocha su traición|Abengalbón es coronado en Valencia|Los infantes reciben permiso del Cid para ejecutarlo
¿A qué lugar apartado llegan los viajeros?|Al robledo de Corpes|A las puertas de Burgos|Al puerto de Valencia
En Corpes, ¿qué ordenan los infantes a sus acompañantes?|Que se adelanten y los dejen con sus esposas|Que preparen las bodas de Navarra|Que traigan al rey Alfonso
¿Qué hacen los infantes a Elvira y Sol en el robledo?|Las golpean y abandonan malheridas|Las coronan reinas de Carrión|Las devuelven sanas a Valencia
¿Quién sospecha y vuelve para buscar a las hijas del Cid?|Félez Muñoz|El rey Búcar|Raquel
¿Qué hace Félez Muñoz al encontrarlas?|Las socorre y las lleva hacia San Esteban|Las deja solas en el robledo|Las entrega a los infantes
Al conocer la afrenta, ¿qué vía busca el Cid para obtener reparación?|Pide justicia al rey|Ataca al rey Alfonso|Renuncia a sus hijas
¿A quién envía Rodrigo para presentar su demanda al rey?|A Muño Gustioz|A Búcar|Al conde de Barcelona
¿Qué decide hacer Alfonso ante la demanda?|Convocar cortes en Toledo|Dar Valencia a los infantes|Prohibir al Cid reclamar
En las cortes, ¿qué reclama primero el Cid a sus yernos?|La devolución de Colada y Tizón|La corona de Navarra|El león de Valencia
Además de las espadas, ¿qué reclama el Cid?|Los bienes entregados a los infantes|La ciudad de Burgos|Las tierras del monasterio
Tras recuperar sus bienes, ¿qué exige por la afrenta a sus hijas?|Una reparación de honor mediante duelo|Que los infantes gobiernen Valencia|Que Jimena vuelva sola a Cardeña
¿Quiénes combaten por la causa del Cid en los duelos?|Pero Bermúdez, Martín Antolínez y Muño Gustioz|Hariz, Galve y Búcar|El rey Alfonso, el abad y Abengalbón
¿Qué ocurre en los duelos judiciales?|Los campeones del Cid vencen a los de Carrión|Los infantes vencen y conservan su honra|El rey Búcar conquista Toledo
Antes del desenlace llegan nuevas propuestas de matrimonio. ¿De quiénes?|De los infantes de Navarra y Aragón|De Hariz y Galve|De Raquel y Vidas
¿Cómo termina el cantar para las hijas del Cid?|Con sus nuevas bodas y la honra de su linaje acrecentada|Abandonadas en Corpes sin ser rescatadas|Casadas de nuevo con los infantes de Carrión
'@

$decisiones = @($datos.Trim() -split "\r?\n" | ForEach-Object { ,($_ -split '\|') })
if ($decisiones.Count -ne 100 -or @($decisiones | Where-Object { $_.Count -ne 4 }).Count -ne 0) { throw 'Los datos del juego no contienen 100 decisiones válidas.' }

function Leer-Opcion {
    while ($true) {
        $entrada = Read-Host 'Elige 1, 2 o 3 (S para salir)'
        if ($null -eq $entrada) { exit }
        $entrada = $entrada.Trim().ToUpperInvariant()
        if ($entrada -eq 'S') { exit }
        if ($entrada -in @('1','2','3')) { return [int]$entrada - 1 }
        Write-Host 'Introduce 1, 2, 3 o S.'
    }
}

Clear-Host
Write-Host 'EL CANTAR DE MÍO CID: 100 DECISIONES' -ForegroundColor Yellow
Write-Host 'Sigue los acontecimientos del poema desde el destierro hasta su desenlace.'
Write-Host 'Debes acertar las 100 decisiones en una misma partida. Un error te lleva al principio.'
Write-Host 'Escribe S en cualquier decisión para salir.'
Write-Host 'Esta aventura adapta el cantar literario, que mezcla historia y leyenda.'
[void](Read-Host 'Pulsa Enter para comenzar')

while ($true) {
    $perdio = $false
    for ($i = 0; $i -lt $decisiones.Count; $i++) {
        Clear-Host
        $capitulo = if ($i -lt 42) { 'I. EL DESTIERRO' } elseif ($i -lt 68) { 'II. LAS BODAS' } else { 'III. LA AFRENTA DE CORPES' }
        Write-Host "`n$capitulo — Decisión $($i + 1) de 100" -ForegroundColor Yellow
        Write-Host "`n$($decisiones[$i][0])`n"
        # La posición correcta cambia sin depender del azar ni repetirse siempre.
        $posicion = ($i * 7 + 1) % 3
        $opciones = [string[]]::new(3)
        $opciones[$posicion] = $decisiones[$i][1]
        $otros = @(0..2 | Where-Object { $_ -ne $posicion })
        $opciones[$otros[0]] = $decisiones[$i][2]
        $opciones[$otros[1]] = $decisiones[$i][3]
        for ($j = 0; $j -lt 3; $j++) { Write-Host "$($j + 1). $($opciones[$j])" }
        if ((Leer-Opcion) -ne $posicion) {
            Write-Host "`nLa historia ha tomado otro camino. Fin de la partida." -ForegroundColor Red
            Write-Host "En el cantar: $($decisiones[$i][1])."
            Write-Host "Decisiones correctas consecutivas: $i de 100."
            $perdio = $true
            break
        }
    }
    if (-not $perdio) {
        Write-Host "`n¡Has completado las 100 decisiones!" -ForegroundColor Green
        Write-Host 'El Cid ha recuperado su honra, se ha hecho justicia y sus hijas se unen a los linajes de Navarra y Aragón.'
        Write-Host 'Aquí termina la historia del Cantar de mío Cid.'
    }
    $respuesta = Read-Host '`nPulsa Enter para comenzar de nuevo, o escribe S para salir'
    if ($null -eq $respuesta -or $respuesta.Trim().ToUpperInvariant() -eq 'S') { break }
}
