import { useState } from 'react'
import Api from '../api/Api'
import { WorkItemStatusId } from '../api/models'
import type { WorkItemModel } from '../api/models'
import Button from '../components/Button'
import PageFrame from '../components/PageFrame'
import Panel from '../components/Panel'
import DateField from '../components/fields/DateField'
import GardenerField from '../components/fields/GardenerField'
import TextAreaField from '../components/fields/TextAreaField'
import TextField from '../components/fields/TextField'
import WorkItemValidator from '../validators/WorkItemValidator'
import './WorkItemEditPage.css'

type SaveMessage = {
  isError: boolean
  text: string
}

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
  const [persistedWorkItem, setPersistedWorkItem] = useState<WorkItemModel>()
  const [isSaving, setIsSaving] = useState(false)
  const [saveMessage, setSaveMessage] = useState<SaveMessage>()
  const validator = new WorkItemValidator(workItem)
  const validation = validator.validate()
  const isDirty = workItem !== persistedWorkItem

  function updateWorkItem(update: (current: WorkItemModel) => WorkItemModel) {
    setSaveMessage(undefined)
    setWorkItem(update)
  }

  async function handleSave() {
    if (validation || isSaving || !isDirty) return

    setIsSaving(true)
    setSaveMessage(undefined)

    try {
      const savedWorkItem = await Api.saveWorkItem(workItem)
      setWorkItem(savedWorkItem)
      setPersistedWorkItem(savedWorkItem)
      setSaveMessage({ isError: false, text: 'Work item saved.' })
    } catch (reason: unknown) {
      setSaveMessage({
        isError: true,
        text: reason instanceof Error ? reason.message : 'Unable to save work item',
      })
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <PageFrame title={workItem.workItemId === undefined ? 'Add Work Item' : 'Edit Work Item'}>
      <form
        className="work-item-edit-page__form"
        onSubmit={(event) => {
          event.preventDefault()
          void handleSave()
        }}
      >
        {saveMessage && (
          <p
            className={
              saveMessage.isError
                ? 'work-item-edit-page__message work-item-edit-page__message--error'
                : 'work-item-edit-page__message'
            }
            role={saveMessage.isError ? 'alert' : 'status'}
          >
            {saveMessage.text}
          </p>
        )}

        <Panel
          actions={
            <Button
              disabled={Boolean(validation) || !isDirty}
              isProcessing={isSaving}
              primary={!validation && isDirty}
              title={validation ?? (!isDirty ? 'No changes to save.' : undefined)}
              tooltipError={Boolean(validation)}
              type="submit"
            >
              Save
            </Button>
          }
          title="Details"
        >
          <TextField
            autoComplete="off"
            className="field--full"
            name="title"
            onChange={(title) => updateWorkItem((current) => ({ ...current, title }))}
            value={workItem.title}
            validator={validator.title}
          />

          <TextField
            autoComplete="street-address"
            className="field--full"
            name="address"
            onChange={(address) => updateWorkItem((current) => ({ ...current, address }))}
            value={workItem.address}
            validator={validator.address}
          />

          <GardenerField
            onChange={(gardenerId) =>
              updateWorkItem((current) => ({ ...current, gardenerId }))
            }
            value={workItem.gardenerId}
          />

          <DateField
            name="scheduledDate"
            onChange={(scheduledDate) =>
              updateWorkItem((current) => ({
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
            onChange={(description) =>
              updateWorkItem((current) => ({ ...current, description }))
            }
            value={workItem.description ?? ''}
            validator={validator.description}
          />
        </Panel>
      </form>
    </PageFrame>
  )
}

export default WorkItemEditPage
