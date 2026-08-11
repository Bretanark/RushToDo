import { useState } from 'react'
import { WorkItemStatusId } from '../api/models'
import type { WorkItemModel } from '../api/models'
import PageFrame from '../components/PageFrame'
import Panel from '../components/Panel'
import DateField from '../components/fields/DateField'
import GardenerField from '../components/fields/GardenerField'
import TextAreaField from '../components/fields/TextAreaField'
import TextField from '../components/fields/TextField'
import WorkItemValidator from '../validators/WorkItemValidator'

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
  const validator = new WorkItemValidator(workItem)

  return (
    <PageFrame title="Add Work Item">
      <Panel title="Details">
        
        <TextField
          autoComplete="off"
          className="field--full"
          name="title"
          onChange={(title) => setWorkItem((current) => ({ ...current, title }))}
          value={workItem.title}
          validator={validator.title}
        />

        <TextField
          autoComplete="street-address"
          className="field--full"
          name="address"
          onChange={(address) => setWorkItem((current) => ({ ...current, address }))}
          value={workItem.address}
          validator={validator.address}
        />

        <GardenerField
          onChange={(gardenerId) => setWorkItem((current) => ({ ...current, gardenerId }))}
          value={workItem.gardenerId}
        />

        <DateField
          name="scheduledDate"
          onChange={(scheduledDate) =>
            setWorkItem((current) => ({
              ...current,
              scheduledDate,
              statusId: scheduledDate ? WorkItemStatusId.Scheduled : WorkItemStatusId.New,
            }))
          }
          value={workItem.scheduledDate}
          validator={validator.scheduledDate}
        />

        <TextAreaField
          className="field--full"
          name="description"
          onChange={(description) => setWorkItem((current) => ({ ...current, description }))}
          value={workItem.description ?? ''}
          validator={validator.description}
        />

      </Panel>
    </PageFrame>
  )
}

export default WorkItemEditPage
