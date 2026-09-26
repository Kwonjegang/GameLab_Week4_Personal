import assert from 'node:assert/strict';
import {initial,point,lit,MODES,advance,solve,won,doors} from './dist/engine.mjs';
for(let i=0;i<=7;i++){
 assert.equal(lit('a',i,'left'),false); assert.equal(lit('b',i,'left'),true);
 assert.equal(lit('a',i,'right'),true); assert.equal(lit('b',i,'right'),false);
 assert.equal(lit('a',i,'center'),true); assert.equal(lit('b',i,'center'),true);
}
const solution=solve(initial());assert.ok(solution);assert.equal(solution.length,8);assert.ok(solution.includes('left')&&solution.includes('right'));
let s=initial();for(const move of solution)s=advance(s,move).state;assert.ok(won(s));
let p=initial();for(let i=0;i<3;i++)p=advance(p,'center').state;assert.equal(solve(p),null);
const held={a:2,b:3,turn:3,mode:'left'},next=advance(held,'left');assert.equal(next.state.a,2);assert.equal(next.state.b,4);assert.equal(held.b,3);
assert.deepEqual(doors({a:2,b:5}),{a:true,b:true});
const blocked=advance({a:5,b:4,turn:0,mode:'center'},'center');assert.equal(blocked.state.a,5);assert.equal(blocked.state.b,5);
assert.ok(won(advance(s,'left').state));
console.log(JSON.stringify({result:'PASS',shadowRayChecks:48,minimumTurns:solution.length,solution,checks:['automatic simultaneous movement','plate / gate timing','unrecoverable state detection','pure preview / undo snapshots','terminal state']}));
