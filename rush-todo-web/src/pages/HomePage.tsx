import { useEffect, useState } from 'react'
import Api from '../api/Api'
import { WorkItemStatusId } from '../api/models'
import type { LookupItem, WorkItemModel } from '../api/models'
import AppLink from '../components/AppLink'
import LinkButton from '../components/LinkButton'
import PageFrame from '../components/PageFrame'
import Panel from '../components/Panel'
import './HomePage.css'

const incompleteStatusIds = [WorkItemStatusId.New, WorkItemStatusId.Scheduled] as const

function HomePage() {
  const [workItems, setWorkItems] = useState<WorkItemModel[]>()
  const [gardeners, setGardeners] = useState<LookupItem[]>([])
  const [loadError, setLoadError] = useState<string>()

  useEffect(() => {
    let ignore = false

    Promise.all([
      Api.searchWorkItems({ statusIds: incompleteStatusIds }),
      Api.getGardeners(),
    ])
      .then(([loadedWorkItems, loadedGardeners]) => {
        if (ignore) return

        setWorkItems(loadedWorkItems)
        setGardeners(loadedGardeners)
      })
      .catch((reason: unknown) => {
        if (!ignore) {
          setLoadError(reason instanceof Error ? reason.message : 'Unable to load work items')
        }
      })

    return () => {
      ignore = true
    }
  }, [])

  const gardenerNames = new Map(gardeners.map((gardener) => [gardener.id, gardener.text]))

  return (
    <PageFrame
      actions={
        <LinkButton href="/work-item/new" primary>
          Add
        </LinkButton>
      }
      title="Rush To Do"
    >
      <Panel title="Work items">
        {loadError && (
          <p className="home-page__message home-page__message--error" role="alert">
            {loadError}
          </p>
        )}

        {!loadError && workItems === undefined && (
          <p className="home-page__message" role="status">
            Loading work items…
          </p>
        )}

        {workItems?.length === 0 && (
          <p className="home-page__message">There are no incomplete work items.</p>
        )}

        {workItems && workItems.length > 0 && (
          <div className="work-item-list">
            {workItems.map((workItem) =>
              workItem.workItemId === undefined ? null : (
                <AppLink
                  className="work-item-tile"
                  href={`/work-item/${workItem.workItemId}`}
                  key={workItem.workItemId}
                >
                  <strong className="work-item-tile__title">{workItem.title}</strong>
                  <span>{workItem.address}</span>
                  <span>
                    {workItem.gardenerId === null
                      ? 'Unassigned'
                      : (gardenerNames.get(workItem.gardenerId) ?? 'Unknown')}
                  </span>
                  <span>{formatDate(workItem.scheduledDate)}</span>
                  {workItem.description && (
                    <span className="work-item-tile__description">
                      {workItem.description}
                    </span>
                  )}
                </AppLink>
              ),
            )}
          </div>
        )}
      </Panel>
    </PageFrame>
  )
}

function formatDate(value: string | null): string {
  if (!value) return 'Not scheduled'

  const [year, month, day] = value.split('-')
  return `${day}/${month}/${year}`
}

export default HomePage
