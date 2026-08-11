import type { LookupItem } from './models'

async function request<T>(url: string): Promise<T> {
  const response = await fetch(url, {
    headers: {
      Accept: 'application/json',
    },
  })

  if (!response.ok) throw new Error(await getErrorMessage(response))
  return (await response.json()) as T
}

async function getErrorMessage(response: Response): Promise<string> {
  const fallback = `${response.status} ${response.statusText}`.trim()
  if (!response.headers.get('content-type')?.includes('json')) return fallback

  const problem = (await response.json()) as Record<string, unknown>
  if (typeof problem.detail === 'string') return problem.detail
  if (typeof problem.title === 'string') return problem.title
  return fallback
}

const Api = {
  getGardeners: () => request<LookupItem[]>('/gardener'),
}

export default Api
