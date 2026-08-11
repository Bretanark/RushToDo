import { useEffect, useState } from 'react'
import Api from '../api/Api'
import { WorkItemStatusId } from '../api/models'
import type { WorkItemModel } from '../api/models'
import Button from '../components/Button'
import LinkButton from '../components/LinkButton'
import PageFrame from '../components/PageFrame'
import Panel from '../components/Panel'
import DateField from '../components/fields/DateField'
import GardenerField from '../components/fields/GardenerField'
import TextAreaField from '../components/fields/TextAreaField'
import TextField from '../components/fields/TextField'
import { navigate } from '../navigation'
import WorkItemValidator from '../validators/WorkItemValidator'
import './WorkItemEditPage.css'

type WorkItemEditPageProps = {
  workItemId?: number
}

type ProcessingAction = 'save' | 'complete' | 'cancel'

function WorkItemEditPage({ workItemId }: WorkItemEditPageProps) {
  const [workItem, setWorkItem] = useState<WorkItemModel | undefined>(() =>
    workItemId === undefined ? newWorkItem() : undefined,
  )
  const [persistedWorkItem, setPersistedWorkItem] = useState<WorkItemModel | undefined>(
    () => (workItemId === undefined ? workItem : undefined),
  )
  const [loadError, setLoadError] = useState<string>()
  const [processingAction, setProcessingAction] = useState<ProcessingAction>()
  const [saveError, setSaveError] = useState<string>()

  useEffect(() => {
    if (workItemId === undefined) return

    let ignore = false

    Api.getWorkItem(workItemId)
      .then((loadedWorkItem) => {
        if (ignore) return

        setWorkItem(loadedWorkItem)
        setPersistedWorkItem(loadedWorkItem)
      })
      .catch((reason: unknown) => {
        if (!ignore) {
          setLoadError(reason instanceof Error ? reason.message : 'Unable to load work item')
        }
      })

    return () => {
      ignore = true
    }
  }, [workItemId])

  if (workItem === undefined) {
    return (
      <PageFrame actions={<LinkButton href="/">Back</LinkButton>} title="Edit Work Item">
        <p
          className={
            loadError
              ? 'work-item-edit-page__message work-item-edit-page__message--error'
              : 'work-item-edit-page__message'
          }
          role={loadError ? 'alert' : 'status'}
        >
          {loadError ?? 'Loading work item…'}
        </p>
      </PageFrame>
    )
  }

  const validator = new WorkItemValidator(workItem)
  const validation = validator.validate()
  const isDirty = workItem !== persistedWorkItem
  const isProcessing = processingAction !== undefined

  function updateWorkItem(update: (current: WorkItemModel) => WorkItemModel) {
    setSaveError(undefined)
    setWorkItem((current) => (current === undefined ? current : update(current)))
  }

  async function persist(updatedWorkItem: WorkItemModel, action: ProcessingAction) {
    if (validation || isProcessing) return

    setProcessingAction(action)
    setSaveError(undefined)

    try {
      await Api.saveWorkItem(updatedWorkItem)
      navigate('/')
    } catch (reason: unknown) {
      setSaveError(reason instanceof Error ? reason.message : 'Unable to save work item')
    } finally {
      setProcessingAction(undefined)
    }
  }

  function handleProcessAction(action: Exclude<ProcessingAction, 'save'>) {
    if (workItem === undefined) return

    const businessDate = getToday()
    const updatedWorkItem = action === 'complete'
      ? {
          ...workItem,
          statusId: WorkItemStatusId.Done,
          completionDate: businessDate,
          cancellationDate: null,
        }
      : {
          ...workItem,
          statusId: WorkItemStatusId.Cancelled,
          completionDate: null,
          cancellationDate: businessDate,
        }

    void persist(updatedWorkItem, action)
  }

  return (
    <PageFrame
      actions={
        <LinkButton href="/" warning={isDirty}>
          Back
        </LinkButton>
      }
      title={workItem.workItemId === undefined ? 'Add Work Item' : 'Edit Work Item'}
    >
      <form
        className="work-item-edit-page__form"
        onSubmit={(event) => {
          event.preventDefault()
          if (isDirty) void persist(workItem, 'save')
        }}
      >
        {saveError && (
          <p className="work-item-edit-page__message work-item-edit-page__message--error" role="alert">
            {saveError}
          </p>
        )}

        <Panel
          actions={
            <Button
              disabled={Boolean(validation) || !isDirty || isProcessing}
              isProcessing={processingAction === 'save'}
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

        {workItem.workItemId !== undefined &&
          !workItem.completionDate &&
          !workItem.cancellationDate && (
            <div className="work-item-edit-page__actions">
              <Button
                disabled={Boolean(validation) || isProcessing}
                isProcessing={processingAction === 'complete'}
                onClick={() => handleProcessAction('complete')}
                title={validation}
                tooltipError={Boolean(validation)}
              >
                Completed
              </Button>

              <Button
                disabled={Boolean(validation) || isProcessing}
                isProcessing={processingAction === 'cancel'}
                onClick={() => handleProcessAction('cancel')}
                title={validation}
                tooltipError={Boolean(validation)}
                warning
              >
                Cancel Job
              </Button>
            </div>
          )}
      </form>
    </PageFrame>
  )
}

function newWorkItem(): WorkItemModel {
  return {
    title: '',
    description: null,
    statusId: WorkItemStatusId.New,
    address: '',
    gardenerId: null,
    scheduledDate: null,
    completionDate: null,
    cancellationDate: null,
    isDeleted: false,
  }
}

function getToday(): string {
  // TODO: Get the current business date from an API controller backed by IDateTimeService.
  const today = new Date()
  const year = today.getFullYear()
  const month = String(today.getMonth() + 1).padStart(2, '0')
  const day = String(today.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

export default WorkItemEditPage
