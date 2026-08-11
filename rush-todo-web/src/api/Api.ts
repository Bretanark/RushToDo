import type { LookupItem, WorkItemModel } from './models'

type RequestOptions = {
  body?: unknown
  method?: 'GET' | 'POST' | 'PUT'
}

async function request<T>(url: string, options?: RequestOptions): Promise<T> {
  const response = await fetch(url, {
    headers: {
      Accept: 'application/json',
      ...(options?.body === undefined ? {} : { 'Content-Type': 'application/json' }),
    },
    body: options?.body === undefined ? undefined : JSON.stringify(options.body),
    method: options?.method ?? 'GET',
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
  saveWorkItem: (workItem: WorkItemModel) =>
    workItem.workItemId === undefined
      ? request<WorkItemModel>('/work-item', { body: workItem, method: 'POST' })
      : request<WorkItemModel>(`/work-item/${workItem.workItemId}`, {
          body: workItem,
          method: 'PUT',
        }),
}

export default Api
