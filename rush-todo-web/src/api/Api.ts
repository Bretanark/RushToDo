import type { LookupItem, WorkItemModel, WorkItemSearchParameters } from './models'

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

function getWorkItemSearchUrl(parameters: WorkItemSearchParameters): string {
  const query = new URLSearchParams()

  parameters.gardenerIds?.forEach((gardenerId) =>
    query.append('gardenerIds', String(gardenerId)),
  )
  parameters.statusIds?.forEach((statusId) => query.append('statusIds', String(statusId)))
  if (parameters.scheduledFrom) query.set('scheduledFrom', parameters.scheduledFrom)
  if (parameters.scheduledTo) query.set('scheduledTo', parameters.scheduledTo)
  if (parameters.includeDeleted) query.set('includeDeleted', 'true')

  const queryString = query.toString()
  return queryString ? `/work-item?${queryString}` : '/work-item'
}

const Api = {
  getGardeners: () => request<LookupItem[]>('/gardener'),
  getWorkItem: (workItemId: number) => request<WorkItemModel>(`/work-item/${workItemId}`),
  searchWorkItems: (parameters: WorkItemSearchParameters) =>
    request<WorkItemModel[]>(getWorkItemSearchUrl(parameters)),
  saveWorkItem: (workItem: WorkItemModel) =>
    workItem.workItemId === undefined
      ? request<WorkItemModel>('/work-item', { body: workItem, method: 'POST' })
      : request<WorkItemModel>(`/work-item/${workItem.workItemId}`, {
          body: workItem,
          method: 'PUT',
        }),
}

export default Api
