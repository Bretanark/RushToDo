import { WorkItemStatusId } from '../../api/models'
import type { LookupItem, WorkItemStatusId as WorkItemStatusIdType } from '../../api/models'
import DropdownField from './DropdownField'

const statuses: readonly LookupItem[] = [
  { id: WorkItemStatusId.New, text: 'New' },
  { id: WorkItemStatusId.Scheduled, text: 'Scheduled' },
  { id: WorkItemStatusId.Done, text: 'Done' },
  { id: WorkItemStatusId.Cancelled, text: 'Cancelled' },
]

type WorkItemStatusFieldProps = {
  error?: string
  onChange: (value: WorkItemStatusIdType) => void
  value: WorkItemStatusIdType
}

function WorkItemStatusField({ error, onChange, value }: WorkItemStatusFieldProps) {
  return (
    <DropdownField
      error={error}
      items={statuses}
      label="Status"
      name="statusId"
      onChange={(statusId) => {
        if (statusId !== null) onChange(statusId as WorkItemStatusIdType)
      }}
      value={value}
    />
  )
}

export default WorkItemStatusField
