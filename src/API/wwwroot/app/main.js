const state = {
  token: localStorage.getItem('azbina_token') || ''
};

const authState = document.getElementById('authState');
const moduleHint = document.getElementById('moduleHint');
const createSection = document.getElementById('createSection');
const logEl = document.getElementById('log');
const cityList = document.getElementById('cityList');
const districtList = document.getElementById('districtList');
const listingList = document.getElementById('listingList');

setAuthState();
toggleCreateSection();
loadListings();

function log(data) {
  const line = `[${new Date().toLocaleTimeString()}] ${typeof data === 'string' ? data : JSON.stringify(data, null, 2)}`;
  logEl.textContent = `${line}\n${logEl.textContent}`;
}

function setAuthState() {
  authState.textContent = state.token
    ? 'Login uğurludur. Token saxlanıldı.'
    : 'Token yoxdur.';
}

function toggleCreateSection() {
  if (state.token) {
    createSection.classList.remove('disabled-block');
    moduleHint.textContent = 'Login oldunuz. Elan yaratmaq mümkündür.';
  } else {
    createSection.classList.add('disabled-block');
    moduleHint.textContent = 'Login olmadan yalnız oxuma əməliyyatları mümkündür.';
  }
}

async function api(path, options = {}) {
  const headers = { ...(options.headers || {}) };

  if (state.token) headers.Authorization = `Bearer ${state.token}`;

  if (options.body && !(options.body instanceof FormData)) {
    headers['Content-Type'] = 'application/json';
  }

  const response = await fetch(`/api/${path}`, { ...options, headers });
  const isJson = (response.headers.get('content-type') || '').includes('application/json');
  const payload = isJson ? await response.json() : await response.text();

  if (!response.ok) {
    log(payload);
    throw new Error(`HTTP ${response.status}`);
  }

  return payload;
}

document.getElementById('loginForm').addEventListener('submit', async (e) => {
  e.preventDefault();

  const login = document.getElementById('login').value.trim();
  const password = document.getElementById('password').value;

  try {
    const data = await api('Auth/login', {
      method: 'POST',
      body: JSON.stringify({ login, password })
    });

    const token = data?.data?.accessToken || data?.accessToken || '';
    if (!token) throw new Error('Token alınmadı.');

    state.token = token;
    localStorage.setItem('azbina_token', token);

    setAuthState();
    toggleCreateSection();
    log('Login uğurlu oldu.');
  } catch {
    log('Login alınmadı. Login (username/email) və şifrəni yoxlayın.');
  }
});

document.getElementById('logoutBtn').addEventListener('click', () => {
  state.token = '';
  localStorage.removeItem('azbina_token');
  setAuthState();
  toggleCreateSection();
  log('Çıxış edildi.');
});

document.getElementById('loadLocations').addEventListener('click', async () => {
  try {
    const [citiesResp, districtsResp] = await Promise.all([api('City'), api('District')]);

    const cities = citiesResp?.data || citiesResp || [];
    const districts = districtsResp?.data || districtsResp || [];

    cityList.innerHTML = cities
      .map((x, i) => `<li>#${x.id ?? i + 1} - ${x.name ?? 'Adsız şəhər'}</li>`)
      .join('');

    districtList.innerHTML = districts
      .map((x, i) => `<li>#${x.id ?? i + 1} - ${x.name ?? 'Adsız rayon'} (City: ${x.cityId ?? '-'})</li>`)
      .join('');

    log({ cities: cities.length, districts: districts.length });
  } catch {
    log('Location dataları yüklənmədi.');
  }
});

document.getElementById('loadListings').addEventListener('click', loadListings);

async function loadListings() {
  try {
    const result = await api('PropertyListing');
    const items = result?.data || result || [];

    listingList.innerHTML = items
      .map(
        (x) => `<li>
          <strong>${x.title ?? 'Başlıqsız'}</strong>
          <div>${x.description ?? ''}</div>
          <small>CityId: ${x.cityId ?? '-'} | DistrictId: ${x.districtId ?? '-'}</small>
        </li>`
      )
      .join('');

    log(`Elan sayı: ${items.length}`);
  } catch {
    log('Elanlar yüklənmədi.');
  }
}

document.getElementById('listingForm').addEventListener('submit', async (e) => {
  e.preventDefault();

  if (!state.token) {
    log('Əvvəl login olmalısınız.');
    return;
  }

  const fd = new FormData();
  fd.append('title', document.getElementById('title').value);
  fd.append('description', document.getElementById('description').value);
  fd.append('listingType', document.getElementById('listingType').value);
  fd.append('propertyType', document.getElementById('propertyType').value);
  fd.append('area', document.getElementById('area').value);
  fd.append('rooms', document.getElementById('rooms').value);
  fd.append('renovationStatus', document.getElementById('renovationStatus').value);
  fd.append('cityId', document.getElementById('cityId').value);
  fd.append('districtId', document.getElementById('districtId').value);

  try {
    const payload = await api('PropertyListing', { method: 'POST', body: fd });
    log(payload);
    await loadListings();
  } catch {
    log('Elan yaratmaq alınmadı. Login token və daxil edilən dataları yoxlayın.');
  }
});
