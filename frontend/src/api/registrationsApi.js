const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5205').replace(/\/$/, '')

async function request(path, options) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: {
      ...(options?.body ? { 'Content-Type': 'application/json' } : {}),
      ...(options?.headers || {}),
    },
  })

  if (!response.ok) {
    let message = `Request failed (${response.status})`
    try {
      const body = await response.json()
      message = body.detail || body.title || message
    } catch {
      // Keep the HTTP status message.
    }
    throw new Error(message)
  }

  return response.status === 204 ? null : response.json()
}

export async function getRegistrations({ participantName = '', type = '', status = '', page = 1, pageSize = 10 } = {}) {
  const params = new URLSearchParams()
  if (participantName) params.set('participantName', participantName)
  if (type) params.set('type', type)
  if (status) params.set('status', status)
  params.set('page', String(page))
  params.set('pageSize', String(pageSize))

  return request(`/api/registrations?${params.toString()}`)
}

export function getSummary() {
  return request('/api/registrations/summary')
}

export function createRegistration(registration) {
  return request('/api/registrations', {
    method: 'POST',
    body: JSON.stringify(registration),
  })
}

export function getRegistrationById(id) {
  return request(`/api/registrations/${encodeURIComponent(id)}`)
}
