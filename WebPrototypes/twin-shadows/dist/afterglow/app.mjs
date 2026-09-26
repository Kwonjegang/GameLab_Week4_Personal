import { ZONES, PATH, GOAL, initial, clone, advance, forecast, solve, actionName } from './engine.mjs';

let state = initial();
let selected = 'A';
const history = [];
const $ = id => document.getElementById(id);
const xs = [78, 152, 226, 300, 374, 448, 522, 596, 670, 744, 818, 915];
const limits = { A: [122, 330], B: [418, 626], C: [640, 848] };
const label = s => s.pos === 0 ? '출발점' : s.pos === 4 ? '안전한 섬' : s.pos === GOAL ? '안전지대' : `${PATH[s.pos]} 다리 ${PATH.slice(0, s.pos + 1).filter(z => z === PATH[s.pos]).length}번째 칸`;
const text = (x, y, value, attrs = '') => `<text x="${x}" y="${y}" ${attrs}>${value}</text>`;
const energyLabel = (s, z) => s.shaded && z === 'B' ? (s.energy.B ? '고정 중' : '빈 다리 고정') : s.light === z && s.energy[z] && (z !== 'C' || s.charge === 3) ? '빛 공급 중' : s.energy[z] ? `잔광 ${s.energy[z]}턴` : z === 'C' && s.charge ? `충전 ${s.charge}/3` : '아직 없는 길';
const tileLabel = (s, z) => !s.energy[z] ? '—' : z === 'B' && s.shaded ? 'Ⅱ' : s.light === z && (z !== 'C' || s.charge === 3) ? '빛' : s.energy[z];

function renderBoard(next) {
  const [left, right] = limits[state.light];
  let ray = `<polygon points="464,454 ${left},190 ${right},190" fill="url(#beam)"/><path d="M464 454L${left} 190M464 454L${right} 190" stroke="#f3d49b" stroke-opacity=".16" fill="none"/>`;
  if (selected !== state.light && ZONES.includes(selected) && state.status === 'playing') {
    const [a, b] = limits[selected];
    ray += `<path d="M${a} 190L464 454L${b} 190" stroke="#f3d49b" stroke-opacity=".5" stroke-dasharray="5 7" fill="none"/>`;
  }
  $('rays').innerHTML = ray;
  let tiles = text(78, 181, '출발', 'class="small" text-anchor="middle"') + text(374, 181, '안전한 섬', 'class="small" text-anchor="middle"') + text(915, 181, '도착', 'class="small" text-anchor="middle"');
  for (const z of ZONES) {
    const [a, b] = limits[z];
    tiles += `<path d="M${a} 136V127H${b}V136" fill="none" stroke="#455766"/>`;
    tiles += text((a + b) / 2, 91, `${z} 다리`, 'class="zone-name" text-anchor="middle"');
    tiles += text((a + b) / 2, 113, z === 'C' ? '빛을 연속 3턴 모으기' : z === 'B' ? '그림자로 고정 가능' : '빛을 받으면 즉시 생성', 'class="small" text-anchor="middle"');
  }
  tiles += '<path d="M78 249H915" stroke="#738797" stroke-opacity=".45" stroke-dasharray="2 7"/>';
  PATH.forEach((z, i) => {
    const x = xs[i];
    if (!z) {
      tiles += `<rect x="${x - 32}" y="211" width="64" height="76" rx="6" fill="#263d48" stroke="${i === GOAL ? '#95e1d4' : '#57707c'}" stroke-width="1.5"/>`;
      tiles += `<path d="M${x - 25} 279H${x + 25}" stroke="#516a77"/>`;
      if (i === GOAL) tiles += text(x, 318, 'SAFE', 'fill="#95e1d4" font-size="13" text-anchor="middle" letter-spacing="2"');
    } else {
      const exists = state.energy[z] > 0, frozen = z === 'B' && state.shaded;
      const bright = state.light === z && !frozen && (z !== 'C' || state.charge === 3);
      const color = frozen && exists ? '#95e1d4' : bright && exists ? '#f3d49b' : exists ? '#ae9877' : '#536775';
      tiles += `<rect x="${x - 29}" y="221" width="58" height="58" rx="4" fill="${exists ? frozen ? '#294a43' : bright ? '#625a44' : '#393c36' : 'url(#dark)'}" stroke="${color}" stroke-width="${exists ? 2 : 1}" ${exists ? '' : 'stroke-dasharray="4 5"'}/>`;
      tiles += text(x, 313, tileLabel(state, z), `class="tile-value" fill="${color}"`);
      if (next && next.state.energy[z] !== state.energy[z]) tiles += text(x, 334, `다음 ${next.state.energy[z]}`, 'class="tile-caption"');
    }
  });
  for (let i = 0; i < xs.length - 1; i++) tiles += `<path d="M${(xs[i] + xs[i + 1]) / 2 - 2} 245l5 4-5 4" stroke="#a7b8c5" stroke-opacity=".5" fill="none"/>`;
  $('tiles').innerHTML = tiles;
  const covered = state.shaded;
  let machine = '<circle cx="464" cy="454" r="28" fill="#172832" stroke="#5d6b70"/><circle cx="464" cy="454" r="15" fill="#f3d49b"/><path d="M441 484H487" stroke="#526979" stroke-width="4"/>';
  machine += text(464, 510, `주 조명 → ${state.light}`, 'fill="#f3d49b" font-size="14" text-anchor="middle"');
  machine += `<path d="M494 454H${covered ? 562 : 658}" stroke="#f3d49b" stroke-width="3" stroke-opacity=".7"/>`;
  machine += text(523, 477, '감지광', 'class="small" text-anchor="middle"');
  if (covered) machine += '<path d="M568 442L704 425V485L568 466Z" fill="url(#dark)"/><path d="M568 431V477" stroke="#bdccd7" stroke-width="8"/>';
  else machine += '<path d="M568 392V424" stroke="#bdccd7" stroke-width="8"/><path d="M568 436V475" stroke="#5e7382" stroke-dasharray="3 6"/>';
  machine += `<rect x="658" y="440" width="29" height="29" rx="4" fill="${covered ? '#203a34' : '#635c42'}" stroke="${covered ? '#95e1d4' : '#f3d49b'}"/>`;
  machine += text(720, 451, covered ? '센서에 그림자' : '센서에 빛', `fill="${covered ? '#95e1d4' : '#c6d1d8'}" font-size="15"`);
  machine += text(720, 474, covered ? 'B 시간 고정' : 'B 시간 흐름', 'class="small"');
  machine += `<path d="M672 440V380H522V350" fill="none" stroke="${covered ? '#95e1d4' : '#59707e'}" stroke-opacity=".65" stroke-dasharray="4 5"/>`;
  if (selected === 'shade' && state.status === 'playing') machine += text(568, 370, covered ? '다음: 그림자 걷기' : '다음: 그림자 만들기', 'fill="#95e1d4" font-size="14" text-anchor="middle"');
  $('mechanism').innerHTML = machine;
  const actor = $('actor');
  actor.style.transform = `translate(${xs[state.pos]}px, ${state.status === 'lost' ? 278 : 249}px)`;
  actor.style.opacity = state.status === 'lost' ? '.3' : '1';
  let ghost = '';
  if (next && state.status === 'playing') {
    const nx = xs[next.state.pos];
    const color = next.state.status === 'lost' ? '#f19e92' : '#95e1d4';
    ghost += `<circle cx="${nx}" cy="249" r="25" fill="none" stroke="${color}" stroke-width="2" stroke-dasharray="5 5"/>`;
    if (next.state.status === 'lost') ghost += `<path d="M${nx - 9} 240l18 18m0-18-18 18" stroke="${color}" stroke-width="3"/>`;
    ghost += text(nx, 205, next.state.status === 'lost' ? '추락 위험' : next.moved ? '다음 위치' : '대기', `fill="${color}" text-anchor="middle" font-size="13"`);
  }
  $('preview').innerHTML = ghost;
  $('board-desc').textContent = `${state.turn}턴. 캐릭터: ${label(state)}. 조명: ${state.light}. ${ZONES.map(z => `${z} ${energyLabel(state, z)}`).join('. ')}. ${next ? `다음 턴: ${next.reason}` : ''}`;
}

function render() {
  const prediction = forecast(state, selected);
  const next = prediction[0];
  $('turn').textContent = String(state.turn).padStart(2, '0');
  $('state-label').textContent = state.status === 'won' ? '구조 완료' : state.status === 'lost' ? '잔광 소멸 · 되돌릴 수 있어요' : state.turn ? label(state) : '출발 준비';
  document.querySelectorAll('[data-action]').forEach(button => {
    button.setAttribute('aria-pressed', String(button.dataset.action === selected));
    button.disabled = state.status !== 'playing';
  });
  $('shade-label').textContent = state.shaded ? '센서 그림자 걷기' : '센서에 그림자 만들기';
  $('shade-help').textContent = state.shaded ? 'B 다리의 시간이 다시 흐릅니다' : 'B 다리의 남은 시간 고정';
  $('bridge-status').innerHTML = ZONES.map(z => `<div class="bridge-card"><div class="card-head"><b>${z}</b><span>${z === 'C' ? '느리게 만드는 길' : z === 'B' ? '붙잡아 둘 수 있는 길' : '첫 번째 길'}</span></div><div class="card-value ${z === 'B' && state.shaded ? 'frozen' : !state.energy[z] ? 'off' : ''}">${energyLabel(state, z)}</div><small>${next ? `다음 → ${energyLabel(next.state, z)}` : '이번 시도 종료'}</small></div>`).join('');
  $('forecast').innerHTML = prediction.map((result, i) => `<li class="${result.state.status === 'lost' ? 'danger' : result.state.status === 'won' ? 'safe' : ''}"><span class="when">+${i + 1}</span><div><b>${result.state.status === 'lost' ? '추락 · 길이 사라짐' : result.state.status === 'won' ? '안전지대 도착' : `${label(result.state)} ${result.moved ? '이동' : '대기'}`}</b><small>${result.state.status === 'lost' ? result.reason : result.state.light === 'C' && result.state.charge < 3 ? `C 충전 ${result.state.charge}/3 · ${energyLabel(result.state, 'B')}` : `A ${result.state.energy.A} / B ${result.state.shaded ? '고정' : result.state.energy.B} / C ${result.state.energy.C}`}</small></div></li>`).join('');
  if (!prediction.length) $('forecast').innerHTML = '<li><span class="when">—</span><div><b>이번 시도가 끝났습니다</b><small>되돌리거나 처음부터 다시 해보세요.</small></div></li>';
  $('advance').disabled = state.status !== 'playing';
  $('undo').disabled = history.length === 0;
  $('result').hidden = state.status === 'playing';
  $('result').className = state.status === 'lost' ? 'lost' : '';
  if (state.status === 'won') $('result').innerHTML = `<strong>빛이 길이 되었습니다.</strong>${state.turn}턴 만에 안전하게 도착했습니다. 잔광과 그림자로 이동할 시간을 만들었어요.`;
  if (state.status === 'lost') $('result').innerHTML = '<strong>발밑의 다리가 사라졌어요.</strong>‘한 턴 되돌리기’로 돌아가 빛을 다시 비추거나, 조금 더 앞서 그림자로 시간을 붙잡아 보세요.';
  renderBoard(next);
}

function clearHint() { $('hint').hidden = true; }
function choose(action) { if (state.status !== 'playing') return; selected = action; clearHint(); render(); }
function commit() {
  if (state.status !== 'playing') return;
  history.push({ state: clone(state), selected });
  state = advance(state, selected).state;
  selected = 'wait'; // A one-shot shutter action should not automatically toggle again.
  clearHint(); render();
}
function undo() {
  const previous = history.pop();
  if (!previous) return;
  state = previous.state; selected = previous.selected;
  clearHint(); render();
}
function reset() { state = initial(); selected = 'A'; history.length = 0; clearHint(); render(); }
document.querySelectorAll('[data-action]').forEach(button => button.addEventListener('click', () => choose(button.dataset.action)));
$('advance').addEventListener('click', commit);
$('undo').addEventListener('click', undo);
$('reset').addEventListener('click', reset);
$('hint-button').addEventListener('click', () => {
  const route = solve(state, ['wait', 'A', 'B', 'C', 'shade']);
  $('hint').hidden = false;
  $('hint').textContent = state.status === 'won' ? '이미 안전하게 도착했습니다.' : route?.length ? `다음 한 수: ${actionName(route[0], state)}. 이 상태에서 최소 ${route.length}턴으로 도착할 수 있습니다.` : '현재 상태에서는 도착할 수 없습니다. 한 턴씩 되돌려 다른 조작을 시도하세요.';
});
document.addEventListener('keydown', event => {
  if (event.repeat || event.ctrlKey || event.altKey || event.metaKey || /INPUT|TEXTAREA|SELECT/.test(event.target.tagName) || event.target.isContentEditable) return;
  const key = event.key.toLowerCase();
  const shortcuts = { '1': 'A', '2': 'B', '3': 'C', s: 'shade', w: 'wait' };
  if (shortcuts[key]) { event.preventDefault(); choose(shortcuts[key]); }
  else if (key === 'z') { event.preventDefault(); undo(); }
  else if (event.code === 'Space' && (!event.target.closest('button, a, summary') || event.target.closest('[data-action]'))) { event.preventDefault(); commit(); }
});
render();
