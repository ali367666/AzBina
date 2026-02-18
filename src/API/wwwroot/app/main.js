const state = {
  token: localStorage.getItem('azbina_token') || '',
  module: 'city'
};

const el = {
  loginScreen: document.getElementById('loginScreen'),
  appShell: document.getElementById('appShell'),
  loginForm: document.getElementById('loginForm'),
  loginInput: document.getElementById('login'),
  passwordInput: document.getElementById('password'),
  loginError: document.getElementById('loginError'),
  welcomeText: document.getElementById('welcomeText'),
  moduleTitle: document.getElementById('moduleTitle'),
  refreshBtn: document.getElementById('refreshBtn'),
  logoutBtn: document.getElementById('logoutBtn'),
  log: document.getElementById('log'),
  cityModule: document.getElementById('cityModule'),
  districtModule: document.getElementById('districtModule'),
  listingModule: document.getElementById('listingModule'),
  cityForm: document.getElementById('cityForm'),
  cityName: document.getElementById('cityName'),
  cityList: document.getElementById('cityList'),
  districtForm: document.getElementById('districtForm'),
  districtName: document.getElementById('districtName'),
  districtCityId: document.getElementById('districtCityId'),
  districtList: document.getElementById('districtList'),
  listingForm: document.getElementById('listingForm'),
  listingList: document.getElementById('listingList')
};

boot();

function boot() {
  bindEvents();
  if (state.token) enterApp();
  else showLogin();
}

function bindEvents() {
  el.loginForm.addEventListener('submit', onLogin);
  el.logoutBtn.addEventListener('click', onLogout);
  el.refreshBtn.addEventListener('click', loadActiveModule);

  document.querySelectorAll('.menu-item').forEach((btn) => {
    btn.addEventListener('click', () => switchModule(btn.dataset.module));
  });

  el.cityForm.addEventListener('submit', createCity);
  el.districtForm.addEventListener('submit', createDistrict);
  el.listingForm.addEventListener('submit', createListing);
}

function showLogin() {
  el.loginScreen.classList.remove('hidden');
  el.appShell.classList.add('hidden');
}

function enterApp() {
  el.loginScreen.classList.add('hidden');
  el.appShell.classList.remove('hidden');
  el.welcomeText.textContent = 'Daxil oldunuz. Modulları istifadə edin.';
  switchModule(state.module);
}

function onLogout() {
  state.token = '';
  localStorage.removeItem('azbina_token');
  showLogin();
  log('Çıxış edildi.');
}

async function onLogin(e) {
  e.preventDefault();
  el.loginError.textContent = '';

  const login = el.loginInput.value.trim();
  const password = el.passwordInput.value;

  try {
    const res = await api('Auth/login', {
      method: 'POST',
      body: JSON.stringify({ login, password })
    });

    const token = res?.data?.accessToken || res?.accessToken;
    if (!token) throw new Error('Token gəlmədi.');

    state.token = token;
    localStorage.setItem('azbina_token', token);
    enterApp();
    log('Login uğurludur.');
  } catch (err) {
    el.loginError.textContent = normalizeError(err);
    log(normalizeError(err));
  }
}

function switchModule(name) {
  state.module = name;
  document.querySelectorAll('.menu-item').forEach((x) => x.classList.toggle('active', x.dataset.module === name));
  el.cityModule.classList.toggle('hidden', name !== 'city');
  el.districtModule.classList.toggle('hidden', name !== 'district');
  el.listingModule.classList.toggle('hidden', name !== 'listing');
  el.moduleTitle.textContent = `${capitalize(name)} Modulu`;
  loadActiveModule();
}

function capitalize(s) { return s.charAt(0).toUpperCase() + s.slice(1); }

async function loadActiveModule() {
  if (!state.token) return;
  if (state.module === 'city') await loadCities();
  if (state.module === 'district') await loadDistricts();
  if (state.module === 'listing') await loadListings();
}

async function loadCities() {
  try {
    const res = await api('City');
    const cities = res?.data || res || [];

    el.cityList.innerHTML = cities.map((c) => `
      <li>
        <span>#${c.id ?? '-'} - ${escapeHtml(c.name ?? '')}</span>
        <div class="row-actions">
          <button data-act="edit" data-id="${c.id}" data-name="${escapeAttr(c.name ?? '')}">Dəyiş</button>
          <button class="danger" data-act="delete" data-id="${c.id}">Sil</button>
        </div>
      </li>
    `).join('');

    bindCityRowActions();
    log(`City sayı: ${cities.length}`);
  } catch (err) {
    log(normalizeError(err));
  }
}

function bindCityRowActions() {
  el.cityList.querySelectorAll('button[data-act="delete"]').forEach((b) => b.addEventListener('click', () => deleteCity(b.dataset.id)));
  el.cityList.querySelectorAll('button[data-act="edit"]').forEach((b) => b.addEventListener('click', () => updateCityPrompt(b.dataset.id, b.dataset.name)));
}

async function createCity(e) {
  e.preventDefault();
  const name = el.cityName.value.trim();
  if (!name) return;

  try {
    await api('City', { method: 'POST', body: JSON.stringify({ name }) });
    el.cityName.value = '';
    await loadCities();
  } catch (err) {
    log(`City yaratma xətası: ${normalizeError(err)}`);
  }
}

async function updateCityPrompt(id, currentName) {
  const name = prompt('Yeni city adı:', currentName || '');
  if (!name || !name.trim()) return;
  try {
    await api(`City/${id}`, { method: 'PUT', body: JSON.stringify({ name: name.trim() }) });
    await loadCities();
  } catch (err) {
    log(`City update xətası: ${normalizeError(err)}`);
  }
}

async function deleteCity(id) {
  if (!confirm(`City #${id} silinsin?`)) return;
  try {
    await api(`City/${id}`, { method: 'DELETE' });
    await loadCities();
  } catch (err) {
    log(`City silmə xətası: ${normalizeError(err)}`);
  }
}

async function loadDistricts() {
  try {
    const res = await api('District');
    const districts = res?.data || res || [];

    el.districtList.innerHTML = districts.map((d) => `
      <li>
        <span>#${d.id ?? '-'} - ${escapeHtml(d.name ?? '')} (City: ${d.cityId ?? '-'})</span>
        <div class="row-actions">
          <button data-act="edit" data-id="${d.id}" data-name="${escapeAttr(d.name ?? '')}" data-cityid="${d.cityId ?? ''}">Dəyiş</button>
          <button class="danger" data-act="delete" data-id="${d.id}">Sil</button>
        </div>
      </li>
    `).join('');

    bindDistrictRowActions();
    log(`District sayı: ${districts.length}`);
  } catch (err) {
    log(normalizeError(err));
  }
}

function bindDistrictRowActions() {
  el.districtList.querySelectorAll('button[data-act="delete"]').forEach((b) => b.addEventListener('click', () => deleteDistrict(b.dataset.id)));
  el.districtList.querySelectorAll('button[data-act="edit"]').forEach((b) => b.addEventListener('click', () => updateDistrictPrompt(b.dataset.id, b.dataset.name, b.dataset.cityid)));
}

async function createDistrict(e) {
  e.preventDefault();
  const name = el.districtName.value.trim();
  const cityId = Number(el.districtCityId.value);
  if (!name || !cityId) return;

  try {
    await api('District', { method: 'POST', body: JSON.stringify({ name, cityId }) });
    el.districtName.value = '';
    el.districtCityId.value = '';
    await loadDistricts();
  } catch (err) {
    log(`District yaratma xətası: ${normalizeError(err)}`);
  }
}

async function updateDistrictPrompt(id, currentName, currentCityId) {
  const name = prompt('Yeni district adı:', currentName || '');
  if (!name || !name.trim()) return;

  const cityIdRaw = prompt('CityId:', currentCityId || '');
  const cityId = Number(cityIdRaw);
  if (!cityId) return;

  try {
    await api(`District/${id}`, { method: 'PUT', body: JSON.stringify({ name: name.trim(), cityId }) });
    await loadDistricts();
  } catch (err) {
    log(`District update xətası: ${normalizeError(err)}`);
  }
}

async function deleteDistrict(id) {
  if (!confirm(`District #${id} silinsin?`)) return;
  try {
    await api(`District/${id}`, { method: 'DELETE' });
    await loadDistricts();
  } catch (err) {
    log(`District silmə xətası: ${normalizeError(err)}`);
  }
}

async function loadListings() {
  try {
    const res = await api('PropertyListing');
    const listings = res?.data || res || [];

    el.listingList.innerHTML = listings.map((x) => {
      const canDelete = Number.isInteger(x.id) || /^\d+$/.test(String(x.id ?? ''));
      return `
      <li>
        <span>#${x.id ?? '-'} - ${escapeHtml(x.title ?? '')} (City:${x.cityId ?? '-'}, District:${x.districtId ?? '-'})</span>
        <div class="row-actions">${canDelete ? `<button class="danger" data-id="${x.id}" data-act="delete">Sil</button>` : ''}</div>
      </li>
    `;
    }).join('');

    el.listingList.querySelectorAll('button[data-act="delete"]').forEach((b) => b.addEventListener('click', () => deleteListing(b.dataset.id)));
    log(`Elan sayı: ${listings.length}`);
  } catch (err) {
    log(normalizeError(err));
  }
}

async function createListing(e) {
  e.preventDefault();
  const fd = new FormData();
  [
    'title', 'description', 'listingType', 'propertyType',
    'area', 'rooms', 'renovationStatus', 'cityId', 'districtId'
  ].forEach((id) => fd.append(id, document.getElementById(id).value));

  try {
    await api('PropertyListing', { method: 'POST', body: fd });
    await loadListings();
  } catch (err) {
    log(`Elan yaratma xətası: ${normalizeError(err)}`);
  }
}

async function deleteListing(id) {
  if (!confirm(`Elan #${id} silinsin?`)) return;
  try {
    await api(`PropertyListing/${id}`, { method: 'DELETE' });
    await loadListings();
  } catch (err) {
    log(`Elan silmə xətası: ${normalizeError(err)} (Delete üçün admin policy tələb oluna bilər)`);
  }
}

async function api(path, options = {}) {
  const headers = { ...(options.headers || {}) };
  if (state.token) headers.Authorization = `Bearer ${state.token}`;
  if (options.body && !(options.body instanceof FormData)) headers['Content-Type'] = 'application/json';

  const res = await fetch(`/api/${path}`, { ...options, headers });
  const isJson = (res.headers.get('content-type') || '').includes('application/json');
  const payload = isJson ? await res.json() : await res.text();

  if (!res.ok) {
    throw payload;
  }
  return payload;
}

function normalizeError(err) {
  if (!err) return 'Naməlum xəta.';
  if (typeof err === 'string') return err;
  if (err.message) return err.message;
  if (err.errors) return JSON.stringify(err.errors);
  return JSON.stringify(err);
}

function escapeHtml(v) {
  return String(v)
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;');
}

function escapeAttr(v) {
  return escapeHtml(v).replaceAll('`', '&#096;');
}

function log(msg) {
  const line = `[${new Date().toLocaleTimeString()}] ${typeof msg === 'string' ? msg : JSON.stringify(msg, null, 2)}`;
  el.log.textContent = `${line}\n${el.log.textContent}`;
}
