import {LIGHT,PIVOT,MODES,LAST,PLATES,GATES,initial,point,blocker,lit,doors,won,advance,solve} from './engine.mjs';
const $=id=>document.getElementById(id),colors={a:'#84ded4',b:'#f4ab6a'};
let state=initial(),selected='center',history=[];
const n=v=>Number(v.toFixed(2)),poly=points=>points.map(p=>`${n(p.x)},${n(p.y)}`).join(' ');
function hull(points){const pts=[...points].sort((a,b)=>a.x-b.x||a.y-b.y),cr=(a,b,c)=>(b.x-a.x)*(c.y-a.y)-(b.y-a.y)*(c.x-a.x);const half=a=>{let r=[];for(const p of a){while(r.length>1&&cr(r.at(-2),r.at(-1),p)<=0)r.pop();r.push(p)}return r.slice(0,-1)};return [...half(pts),...half([...pts].reverse())];}
function board(prediction){
 const shape=blocker(selected),shadow=hull([...shape,...shape.map(p=>({x:LIGHT.x+(p.x-LIGHT.x)*14,y:LIGHT.y+(p.y-LIGHT.y)*14}))]),open=doors(state);
 let out=`<path d="M${LIGHT.x} ${LIGHT.y}L-40 105H960Z" fill="url(#light-fill)"/><polygon points="${poly(shadow)}" fill="url(#shadow-lines)"/>`;
 // Each switch is connected to the opposite door, using the switch's colour.
 for(const lane of ['a','b']){const other=lane==='a'?'b':'a',p=point(lane,PLATES[lane]),g=point(other,GATES[other]),on=state[lane]===PLATES[lane];out+=`<path class="wires" d="M${p.x} ${p.y} Q460 ${Math.min(p.y,g.y)-95} ${g.x} ${g.y}" fill="none" stroke="${on?colors[lane]:'#50636d'}" stroke-width="${on?2.5:1.5}" stroke-dasharray="${on?'none':'5 7'}"/>`;}
 for(const lane of ['a','b']){
  const c=colors[lane],side=lane==='a'?-1:1;
  for(let i=0;i<=LAST;i++){
   const p=point(lane,i),isLit=lit(lane,i,selected),plate=i===PLATES[lane],on=plate&&state[lane]===i;
   if(i<LAST){const q=point(lane,i+1);out+=`<path d="M${p.x} ${p.y}L${q.x} ${q.y}" stroke="${lit(lane,i+1,selected)?'#8e998d':'#35434b'}" stroke-width="8" fill="none"/>`;}
   if(i===LAST)out+=`<rect x="${p.x-27}" y="${p.y-27}" width="54" height="54" rx="11" fill="${c}" fill-opacity=".12" stroke="${c}" stroke-width="2"/><text class="safe-label" x="${p.x}" y="${p.y-42}" fill="${c}" text-anchor="middle">안전지대 ${lane.toUpperCase()}</text>`;
   else out+=`<rect x="${p.x-17}" y="${p.y-17}" width="34" height="34" rx="6" fill="${isLit?'#d8d5b7':'#25343c'}" stroke="${isLit?'#e9e4c8':'#52616c'}" stroke-width="1"/>`;
   if(plate){out+=`<rect x="${p.x-13}" y="${p.y-13}" width="26" height="26" transform="rotate(45 ${p.x} ${p.y})" rx="2" fill="${on?c:'#192329'}" stroke="${c}" stroke-width="3"/><text class="plate-label" x="${p.x+side*38}" y="${p.y+5}" text-anchor="${side<0?'end':'start'}" fill="${c}">발판 ${lane.toUpperCase()}</text>`;}
   if(i===GATES[lane]){
    const owner=lane==='a'?'b':'a',gcolor=colors[owner],angle=lane==='a'?-36:36;
    out+=`<g transform="translate(${p.x} ${p.y}) rotate(${angle})"><path d="M-25 -15V15M25 -15V15" stroke="${gcolor}" stroke-width="5" fill="none"/>${open[lane]?'<path d="M-25 -12H-17M17 -12H25" stroke="#dbe9e7" stroke-width="3"/>':`<path d="M-23 0H23" stroke="${gcolor}" stroke-width="8"/>`}</g><text class="plate-label" x="${p.x+side*40}" y="${p.y+4}" text-anchor="${side<0?'end':'start'}" fill="${gcolor}">${lane.toUpperCase()} 문 ${open[lane]?'열림':'닫힘'}</text><text class="gate-status" x="${p.x+side*40}" y="${p.y+22}" text-anchor="${side<0?'end':'start'}">${owner.toUpperCase()} 발판과 연결</text>`;
   }
  }
 }
 out+=`<line x1="${LIGHT.x}" y1="${LIGHT.y}" x2="${PIVOT.x}" y2="${PIVOT.y}" stroke="#f7e7ac" stroke-opacity=".35" stroke-dasharray="4 7"/><polygon points="${poly(shape)}" fill="#b7c5cc" stroke="#f1f5f5" stroke-width="2"/><circle cx="${PIVOT.x}" cy="${PIVOT.y}" r="5" fill="#192329"/><circle cx="${LIGHT.x}" cy="${LIGHT.y}" r="16" fill="#f7e7ac"/><circle cx="${LIGHT.x}" cy="${LIGHT.y}" r="25" fill="none" stroke="#f7e7ac" stroke-opacity=".3"/><text class="light-label" x="${LIGHT.x}" y="${LIGHT.y+47}" text-anchor="middle">하나의 광원</text>`;
 $('scene').innerHTML=out;
 $('ghosts').innerHTML=prediction.moves.filter(m=>m.reason==='move').map(m=>{const p=point(m.lane,m.to);return `<g class="ghost" transform="translate(${p.x} ${p.y})">${m.lane==='a'?`<circle r="20" fill="none" stroke="${colors.a}" stroke-width="3" stroke-dasharray="4 4"/>`:`<rect x="-19" y="-19" width="38" height="38" rx="6" fill="none" stroke="${colors.b}" stroke-width="3" stroke-dasharray="4 4"/>`}</g>`}).join('');
 for(const lane of ['a','b']){const p=point(lane,state[lane]);$(`actor-${lane}`).style.transform=`translate(${p.x}px,${p.y}px)`;}
 $('board-desc').textContent=`${state.turn}턴. A는 ${state.a+1}번째 칸, B는 ${state.b+1}번째 칸. A 문 ${open.a?'열림':'닫힘'}, B 문 ${open.b?'열림':'닫힘'}.`;
}
const reason={safe:'안전지대 도착',shadow:'그림자 앞 대기',gate:'닫힌 문 앞 대기',move:'한 칸 전진'};
function render(){
 const p=advance(state,selected),solution=solve(state),dead=solution===null,done=won(state);
 $('turn').textContent=String(state.turn).padStart(2,'0');$('rescued').textContent=`${Number(state.a===LAST)+Number(state.b===LAST)} / 2 구조`;
 document.querySelectorAll('[data-mode]').forEach(b=>b.setAttribute('aria-pressed',String(b.dataset.mode===selected)));
 $('forecast').innerHTML=p.moves.map(m=>`<div class="forecast-row"><span class="person-name"><span class="token ${m.lane}">${m.lane.toUpperCase()}</span></span><span class="reason ${m.reason==='move'?'':'stay'}">${reason[m.reason]}</span></div>`).join('')||(done?'<p>두 사람 모두 도착했어요.</p>':'');
 $('advance').disabled=done||dead;$('undo').disabled=!history.length;
 $('result').hidden=!(done||dead);$('result').className=`result ${dead?'blocked':''}`;
 $('result').innerHTML=done?`<b>두 사람 모두 구조했습니다.</b>${state.turn}턴에 도착했어요. 처음부터 다시 다른 순서도 시험해 보세요.`:'<b>서로의 문 앞에서 막혔어요.</b>발판을 떠나면 문이 닫힙니다. 되돌리기로 이전 순서를 바꿔 보세요.';
 board(p);
}
function select(mode){if(!MODES.some(m=>m.id===mode))return;selected=mode;$('hint').hidden=true;render();}
function commit(){if(won(state)||solve(state)===null)return;history.push({...state});state=advance(state,selected).state;$('hint').hidden=true;render();}
function undo(){if(!history.length)return;state=history.pop();selected=state.mode;$('hint').hidden=true;render();}
function reset(){state=initial();selected='center';history=[];$('hint').hidden=true;render();}
function hint(){const path=solve(state);$('hint').textContent=path===null?'발판을 지나치기 전까지 되돌려 보세요. 한 사람은 상대가 문을 통과할 때까지 기다려야 해요.':!path.length?'두 사람 모두 안전합니다.':`다음 한 수: ${MODES.find(m=>m.id===path[0]).label}. 문은 턴 시작 시점의 발판 상태를 따릅니다.`;$('hint').hidden=false;}
document.querySelectorAll('[data-mode]').forEach(b=>b.addEventListener('click',()=>select(b.dataset.mode)));
$('advance').addEventListener('click',commit);$('undo').addEventListener('click',undo);$('reset').addEventListener('click',reset);$('hint-button').addEventListener('click',hint);
document.addEventListener('keydown',e=>{if(e.altKey||e.ctrlKey||e.metaKey||/INPUT|TEXTAREA|SELECT/.test(e.target.tagName))return;if(e.code==='ArrowLeft'||e.code==='ArrowRight'){e.preventDefault();const i=MODES.findIndex(m=>m.id===selected);select(MODES[(i+(e.code==='ArrowLeft'?2:1))%3].id)}else if(e.code==='KeyZ'){e.preventDefault();undo()}else if(e.code==='Space'&&(e.target.closest('[data-mode]')||(e.target.tagName!=='BUTTON'&&e.target.tagName!=='SUMMARY'))){e.preventDefault();commit()}});
render();
// Optional browser-native structured tools. The game works without WebMCP support.
const context=document.modelContext??navigator.modelContext;
if(context?.registerTool){
 const tools=[{name:'read_puzzle',description:'Read the current puzzle state and next-turn preview.',inputSchema:{type:'object',properties:{}},execute:async()=>({content:[{type:'text',text:JSON.stringify({state,selected,preview:advance(state,selected),won:won(state)})}]})},{name:'choose_shadow',description:'Preview a blocker direction without advancing the turn.',inputSchema:{type:'object',properties:{mode:{type:'string',enum:MODES.map(m=>m.id)}},required:['mode']},execute:async({mode})=>{select(mode);return{content:[{type:'text',text:JSON.stringify(advance(state,selected))}]}}},{name:'advance_turn',description:'Advance one turn. Both characters move automatically according to light and gates.',inputSchema:{type:'object',properties:{}},execute:async()=>{commit();return{content:[{type:'text',text:JSON.stringify(state)}]}}},{name:'undo_turn',description:'Undo the last puzzle turn.',inputSchema:{type:'object',properties:{}},execute:async()=>{undo();return{content:[{type:'text',text:JSON.stringify(state)}]}}}];
 const lifecycle=new AbortController();
 for(const t of tools){
  t.inputSchema.additionalProperties=false;
  t.annotations={readOnlyHint:t.name==='read_puzzle',untrustedContentHint:false};
  const run=t.execute;t.execute=async input=>{
   if(input===null||typeof input!=='object'||Array.isArray(input))throw Error('Expected an object');
   if(Object.keys(input).some(k=>!(k in t.inputSchema.properties)))throw Error('Unknown input');
   if(t.name==='choose_shadow'&&!MODES.some(m=>m.id===input.mode))throw Error('Choose left, center, or right');
   return run(input);
  };
  try{Promise.resolve(context.registerTool(t,{signal:lifecycle.signal})).catch(()=>{})}catch{}
 }
 window.addEventListener('pagehide',()=>lifecycle.abort(),{once:true});
}
