// Turn order: one device action → charging → automatic movement → fading → fall check.
export const ZONES = ['A', 'B', 'C'];
export const ACTIONS = ['A', 'B', 'C', 'shade', 'wait'];
export const PATH = [null, 'A', 'A', 'A', null, 'B', 'B', 'B', 'C', 'C', 'C', null];
export const GOAL = PATH.length - 1;
export const initial = () => ({ turn: 0, pos: 0, light: 'A', shaded: false, energy: { A: 0, B: 0, C: 0 }, charge: 0, status: 'playing' });
export const clone = state => ({ ...state, energy: { ...state.energy } });
export const actionName = (action, state) => ({ A: 'A 다리 비추기', B: 'B 다리 비추기', C: 'C 다리 비추기', shade: state.shaded ? '센서 그림자 걷기' : '센서에 그림자 만들기', wait: '현재 조작 유지' })[action];

export function advance(state, action) {
  if (!ACTIONS.includes(action)) throw new Error('Unknown action');
  const next = clone(state);
  if (state.status !== 'playing') return { state: next, moved: false, reason: '이미 종료된 퍼즐입니다.', faded: [] };
  next.turn++;
  if (ZONES.includes(action)) next.light = action;
  if (action === 'shade') next.shaded = !next.shaded;
  const illuminated = { A: false, B: false, C: false };
  for (const zone of ['A', 'B']) {
    // The shadow latch preserves an existing bridge; it cannot create an empty one.
    if (next.light === zone && !(zone === 'B' && next.shaded)) {
      next.energy[zone] = 2;
      illuminated[zone] = true;
    }
  }
  if (next.light === 'C') {
    next.charge = Math.min(3, next.charge + 1);
    if (next.charge === 3) { next.energy.C = 2; illuminated.C = true; }
  } else {
    // Three uninterrupted turns are required, including after the light leaves C.
    next.charge = 0;
  }
  const destination = next.pos + 1;
  const targetZone = PATH[destination];
  const moved = targetZone === null || next.energy[targetZone] > 0;
  if (moved) next.pos = destination;
  let reason = moved ? '밝은 길을 따라 한 칸 이동합니다.' : `${targetZone} 다리가 없어 기다립니다.`;
  if (!moved && targetZone === 'C' && next.light === 'C') reason = `C 다리 충전 ${next.charge}/3 · 제자리에서 기다립니다.`;
  const faded = [];
  for (const zone of ZONES) {
    if (!illuminated[zone] && !(zone === 'B' && next.shaded)) next.energy[zone] = Math.max(0, next.energy[zone] - 1);
    if (state.energy[zone] > 0 && next.energy[zone] === 0) faded.push(zone);
  }
  const standingZone = PATH[next.pos];
  if (next.pos === GOAL) { next.status = 'won'; reason = '안전지대에 도착했습니다!'; }
  else if (standingZone && next.energy[standingZone] === 0) {
    next.status = 'lost';
    reason = `${standingZone} 다리의 잔광이 끝나 발밑이 사라집니다.`;
  }
  return { state: next, moved, reason, faded };
}

export function forecast(state, action, count = 3) {
  const steps = [];
  let cursor = state;
  for (let i = 0; i < count && cursor.status === 'playing'; i++) {
    const result = advance(cursor, i === 0 ? action : 'wait');
    steps.push(result);
    cursor = result.state;
  }
  return steps;
}

const key = s => [s.pos, s.light, +s.shaded, ...ZONES.map(z => s.energy[z]), s.charge].join('|');
export function solve(start, actions = ACTIONS) {
  if (start.status === 'won') return [];
  if (start.status !== 'playing') return null;
  const queue = [{ state: start, parent: -1, action: null }];
  const seen = new Set([key(start)]);
  for (let head = 0; head < queue.length; head++) {
    for (const action of actions) {
      const result = advance(queue[head].state, action).state;
      if (result.status === 'lost') continue;
      const id = key(result);
      if (seen.has(id)) continue;
      seen.add(id);
      const node = { state: result, parent: head, action };
      queue.push(node);
      if (result.status === 'won') {
        const path = [];
        let cursor = queue.length - 1;
        while (queue[cursor].parent >= 0) { path.unshift(queue[cursor].action); cursor = queue[cursor].parent; }
        return path;
      }
    }
  }
  return null;
}
