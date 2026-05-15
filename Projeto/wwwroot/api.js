const API_BASE = window.location.origin;

async function apiFetch(path, options = {}) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: { 'Content-Type': 'application/json', ...(options.headers || {}) },
    ...options
  });

  const text = await response.text();
  const data = text ? JSON.parse(text) : null;

  if (!response.ok) {
    throw new Error(data?.mensagem || data?.message || 'Erro ao acessar a API.');
  }

  return data;
}

function moeda(valor) {
  return Number(valor || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

function getUsuarioLogado() {
  return JSON.parse(localStorage.getItem('usuarioLogado') || 'null');
}

function exigirLogin() {
  const usuario = getUsuarioLogado();
  if (!usuario) {
    window.location.href = 'Telalogin.html';
    return null;
  }
  return usuario;
}

function sair() {
  localStorage.removeItem('usuarioLogado');
  window.location.href = 'Telalogin.html';
}
