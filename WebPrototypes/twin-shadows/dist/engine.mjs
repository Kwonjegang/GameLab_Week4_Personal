export const LIGHT={x:460,y:615};
export const PIVOT={x:460,y:460};
export const MODES=[{id:'left',angle:-45,label:'A 길 가리기',short:'A 대기'},{id:'center',angle:0,label:'두 길 밝히기',short:'함께 이동'},{id:'right',angle:45,label:'B 길 가리기',short:'B 대기'}];
export const LAST=7, PLATES={a:2,b:5}, GATES={a:6,b:4};
export function initial(){return {a:0,b:0,turn:0,mode:'center'};}
export function point(lane,index){const angle=(lane==='a'?-36:36)*Math.PI/180,r=200+index*44;return{x:LIGHT.x+Math.sin(angle)*r,y:LIGHT.y-Math.cos(angle)*r};}
export function blocker(mode){const a=MODES.find(m=>m.id===mode).angle*Math.PI/180;return[[-95,-6],[95,-6],[95,6],[-95,6]].map(([x,y])=>({x:PIVOT.x+x*Math.cos(a)-y*Math.sin(a),y:PIVOT.y+x*Math.sin(a)+y*Math.cos(a)}));}
const cross=(a,b)=>a.x*b.y-a.y*b.x;
function intersects(p,q,a,b){const r={x:q.x-p.x,y:q.y-p.y},s={x:b.x-a.x,y:b.y-a.y},d=cross(r,s);if(Math.abs(d)<1e-9)return false;const ap={x:a.x-p.x,y:a.y-p.y},t=cross(ap,s)/d,u=cross(ap,r)/d;return t>0&&t<1&&u>=0&&u<=1;}
export function lit(lane,index,mode){const target=point(lane,index),poly=blocker(mode);return !poly.some((p,i)=>intersects(LIGHT,target,p,poly[(i+1)%poly.length]));}
export function doors(state){return {a:state.b===PLATES.b,b:state.a===PLATES.a};}
export function won(s){return s.a===LAST&&s.b===LAST;}
export function advance(state,mode){
 if(!MODES.some(m=>m.id===mode))throw Error('Invalid shadow position');
 if(won(state))return {state:{...state},moves:[],doors:doors(state)};
 const open=doors(state),next={...state,turn:state.turn+1,mode},moves=[];
 for(const lane of ['a','b']){
  const from=state[lane],to=Math.min(LAST,from+1);
  const reason=from===LAST?'safe':!lit(lane,to,mode)?'shadow':to===GATES[lane]&&!open[lane]?'gate':'move';
  if(reason==='move')next[lane]=to;
  moves.push({lane,from,to:next[lane],reason});
 }
 return {state:next,moves,doors:open};
}
export function solve(start){
 const q=[{state:start,path:[]}],seen=new Set([`${start.a},${start.b}`]);
 for(let i=0;i<q.length;i++){
  const {state,path}=q[i];if(won(state))return path;
  for(const m of MODES){const n=advance(state,m.id).state,key=`${n.a},${n.b}`;if(!seen.has(key)){seen.add(key);q.push({state:n,path:[...path,m.id]});}}
 }
 return null;
}
