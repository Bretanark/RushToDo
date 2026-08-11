import { useState } from 'react'
import { WorkItemStatusId } from '../api/models'
import type { WorkItemModel } from '../api/models'
import PageFrame from '../components/PageFrame'
import Panel from '../components/Panel'
import DateField from '../components/fields/DateField'
import GardenerField from '../components/fields/GardenerField'
import TextAreaField from '../components/fields/TextAreaField'
import TextField from '../components/fields/TextField'
import WorkItemStatusField from '../components/fields/WorkItemStatusField'

function WorkItemEditPage() {
  const [workItem, setWorkItem] = useState<WorkItemModel>({
    title: '',
    description: null,
    statusId: WorkItemStatusId.New,
    address: '',
    gardenerId: null,
    scheduledDate: null,
    completionDate: null,
    cancellationDate: null,
    isDeleted: false,
  })

  return (
    <PageFrame title="Add Work Item">
      <Panel title="Details">
        <TextField
          autoComplete="off"
          className="field--full"
          label="Title"
          maxLength={255}
          name="title"
          onChange={(title) => setWorkItem((current) => ({ ...current, title }))}
          value={workItem.title}
        />

        <GardenerField
          onChange={(gardenerId) => setWorkItem((current) => ({ ...current, gardenerId }))}
          value={workItem.gardenerId}
        />

        <TextField
          autoComplete="street-address"
          className="field--full"
          label="Address"
          maxLength={255}
          name="address"
          onChange={(address) => setWorkItem((current) => ({ ...current, address }))}
          value={workItem.address}
        />

        <TextAreaField
          className="field--full"
          label="Description"
          name="description"
          onChange={(description) => setWorkItem((current) => ({ ...current, description }))}
          value={workItem.description ?? ''}
        />

        <WorkItemStatusField
          onChange={(statusId) => setWorkItem((current) => ({ ...current, statusId }))}
          value={workItem.statusId}
        />

        <DateField
          label="Scheduled date"
          name="scheduledDate"
          onChange={(scheduledDate) => setWorkItem((current) => ({ ...current, scheduledDate }))}
          value={workItem.scheduledDate}
        />
      </Panel>
    </PageFrame>
  )
}

export default WorkItemEditPage
