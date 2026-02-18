const state = {
  token: localStorage.getItem('azbina_token') || ''
};

const authState = document.getElementById('authState');
const logEl = document.getElementById('log');
const cityList = document.getElementById('cityList');
const districtList = document.getElementById('districtList');
const listingList = document.getElementById('listingList');

setAuthState();
loadListings();

function log(data) {
  const line = `[${new Date().toLocaleTimeString()}] ${typeof data === 'string' ? data : JSON.stringify(data, null, 2)}`;
  logEl.textContent = `${line}\n${logEl.textContent}`;
}

function setAuthState() {
  authState.textContent = state.token ? 'Token hazırdır (localStorage).' : 'Token yoxdur.';
}

async function api(path, options = {}) {
  const headers = { ...(options.headers || {}) };

  if (state.token) {
    headers.Authorization = `Bearer ${state.token}`;
  }

  if (options.body && !(options.body instanceof FormData)) {
    headers['Content-Type'] = 'application/json';
  }

  const response = await fetch(`/api/${path}`, {
    ...options,
    headers
  });

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

  const email = document.getElementById('email').value;
  const password = document.getElementById('password').value;

  try {
    const data = await api('Auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password })
    });

    state.token = data?.data?.accessToken || '';
    localStorage.setItem('azbina_token', state.token);
    setAuthState();
    log('Login uğurlu oldu.');
  } catch {
    log('Login alınmadı. Email/şifrə və ya role policy-ni yoxlayın.');
  }
});

document.getElementById('loadLocations').addEventListener('click', async () => {
  try {
    const [cities, districts] = await Promise.all([
      api('City'),
      api('District')
    ]);

    const cityItems = (cities?.data || []);
    const districtItems = (districts?.data || []);

    cityList.innerHTML = cityItems.map((x) => `<li>#${x.id} - ${x.name}</li>`).join('');
    districtList.innerHTML = districtItems.map((x) => `<li>#${x.id} - ${x.name} (City: ${x.cityId})</li>`).join('');

    log({ cities: cityItems.length, districts: districtItems.length });
  } catch {
    log('Location dataları yüklənmədi.');
  }
});

document.getElementById('loadListings').addEventListener('click', loadListings);

async function loadListings() {
  try {
    const result = await api('PropertyListing');
    const items = result?.data || [];

    listingList.innerHTML = items.map((x) => `
      <li>
        <strong>${x.title}</strong>
        <div>${x.description}</div>
        <small>CityId: ${x.cityId} | DistrictId: ${x.districtId}</small>
      </li>`).join('');

    log(`Elan sayı: ${items.length}`);
  } catch {
    log('Elanlar yüklənmədi.');
  }
}

document.getElementById('listingForm').addEventListener('submit', async (e) => {
  e.preventDefault();

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
    const payload = await api('PropertyListing', {
      method: 'POST',
      body: fd
    });

    log(payload);
    await loadListings();
  } catch {
    log('Elan yaratmaq alınmadı. Login token və policy tələb olunur.');
  }
});
