export type LookupItem = {
  id: number
  text: string
}

export const WorkItemStatusId = {
  New: 1,
  Scheduled: 2,
  Done: 3,
  Cancelled: 4,
} as const

export type WorkItemStatusId = (typeof WorkItemStatusId)[keyof typeof WorkItemStatusId]

export type WorkItemModel = {
  workItemId?: number
  updateDateTime?: string
  title: string
  description: string | null
  statusId: WorkItemStatusId
  address: string
  gardenerId: number | null
  scheduledDate: string | null
  completionDate: string | null
  cancellationDate: string | null
  isDeleted: boolean
}

export type WorkItemSearchParameters = {
  gardenerIds?: readonly number[]
  includeDeleted?: boolean
  scheduledFrom?: string
  scheduledTo?: string
  statusIds?: readonly WorkItemStatusId[]
}
