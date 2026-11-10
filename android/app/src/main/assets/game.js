(function(){
  'use strict';
  const app=document.getElementById('app');
  const decisions=window.CID_DECISIONS;
  let stage='cover';
  let index=0;
  let locked=false;
  const chapters=['I · EL DESTIERRO','II · LAS BODAS','III · LA AFRENTA DE CORPES'];
  const chapter=i=>i<42?chapters[0]:i<68?chapters[1]:chapters[2];
  const asset=name=>'images/'+name+'.jpg';
  const escape=s=>String(s).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
  const pad=n=>String(n).padStart(3,'0');
  function shell(image,cover,eyebrow,title,progress){
    return `<header class="topbar"><img class="brand-icon" src="icon.png" alt=""><div class="brand"><div class="brand-title">EL CANTAR DE MÍO CID</div><div class="brand-sub">Una aventura narrativa</div></div><div class="count">${escape(progress)}</div></header><div class="track" aria-hidden="true"><div style="width:${stage==='end'?100:stage==='question'?index+1:0}%"></div></div><section class="hero${cover?' cover':''}"><img src="${asset(image)}" alt="Ilustración de ${escape(eyebrow.toLowerCase())}"><div class="hero-label"><div class="eyebrow">${escape(eyebrow)}</div><div class="hero-title">${escape(title)}</div></div></section>`;
  }
  function renderCover(){
    stage='cover';index=0;locked=false;
    app.innerHTML=shell('portada',true,'EL CANTAR DE MÍO CID','Toda una vida en cien decisiones','100 DECISIONES')+
      `<section class="card"><div class="eyebrow">LA AVENTURA COMIENZA</div><h1 class="question">La honra se gana con cada decisión.</h1><p class="intro">Recorre el destierro, la conquista de Valencia y la justicia de Corpes. Cada elección sigue el camino del poema.</p><button class="primary" id="start">Comenzar la historia <span aria-hidden="true">→</span></button><div class="rule-title">UNA SOLA OPORTUNIDAD POR PARTIDA</div><p class="rule">Si te equivocas, volverás al comienzo. Para llegar al final debes acertar las 100 decisiones seguidas.</p></section><p class="meta">Inspirado en el poema medieval · La historia mezcla hechos y leyenda</p>`;
    document.getElementById('start').addEventListener('click',renderQuestion);
    window.scrollTo(0,0);
  }
  function renderQuestion(){
    stage='question';locked=false;
    const d=decisions[index],correct=(index*7+1)%3;
    const options=[];options[correct]=d[1];
    let wrong=2;
    for(let i=0;i<3;i++)if(i!==correct)options[i]=d[wrong++];
    app.innerHTML=shell(pad(index+1),false,chapter(index),'DECISIÓN '+String(index+1).padStart(2,'0')+' DE 100',`${index+1} / 100`)+
      `<section class="card"><div class="eyebrow">ELIGE TU CAMINO</div><h1 class="question">${escape(d[0])}</h1><div class="choices">${options.map((option,i)=>`<button class="choice" data-choice="${i}"><span class="number">${i+1}</span><span>${escape(option)}</span></button>`).join('')}</div><p class="hint">Una respuesta incorrecta termina esta partida.</p></section><p class="meta">${escape(chapter(index))} · ${index+1} de 100</p>`;
    app.querySelectorAll('.choice').forEach(button=>button.addEventListener('click',()=>choose(Number(button.dataset.choice))));
    window.scrollTo(0,0);
  }
  function choose(choice){
    if(stage!=='question'||locked)return;
    locked=true;
    const correct=(index*7+1)%3;
    if(choice!==correct){renderLoss();return;}
    index++;
    if(index===100)renderEnd();else renderQuestion();
  }
  function renderLoss(){
    stage='loss';
    const d=decisions[index];
    app.innerHTML=shell(pad(index+1),false,'EL CAMINO SE INTERRUMPE','La historia tomó otro rumbo','PARTIDA TERMINADA')+
      `<section class="card loss"><div class="eyebrow">FIN DE LA PARTIDA</div><h1 class="question">Tendrás que empezar de nuevo.</h1><p class="intro">Acertaste <strong>${index} de 100</strong> decisiones.<br>En el cantar: ${escape(d[1])}.</p><button class="primary" id="restart">Volver al principio <span aria-hidden="true">↺</span></button></section>`;
    document.getElementById('restart').addEventListener('click',renderCover);
    window.scrollTo(0,0);
  }
  function renderEnd(){
    stage='end';
    app.innerHTML=shell('final',true,'EL FINAL DEL CANTAR','La honra ha sido restaurada','100 / 100')+
      `<section class="card end"><div class="eyebrow">HISTORIA COMPLETADA</div><h1 class="question">El Cid ha recuperado su honra.</h1><p class="intro">Has seguido las cien decisiones del poema. Se ha hecho justicia por Elvira y Sol, y sus nuevas bodas unen su linaje con Navarra y Aragón.</p><button class="primary" id="again">Jugar de nuevo <span aria-hidden="true">↺</span></button></section>`;
    document.getElementById('again').addEventListener('click',renderCover);
    window.scrollTo(0,0);
  }
  document.addEventListener('keydown',event=>{
    if(stage==='question'&&['1','2','3'].includes(event.key))choose(Number(event.key)-1);
    else if(stage==='cover'&&event.key==='Enter')renderQuestion();
  });
  if(!Array.isArray(decisions)||decisions.length!==100){app.textContent='No se pudieron cargar las 100 decisiones.';return;}
  renderCover();
})();
