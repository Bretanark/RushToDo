import { useState } from 'react'
import PageFrame from '../components/PageFrame'
import Panel from '../components/Panel'
import GardenerField from '../components/fields/GardenerField'
import TextField from '../components/fields/TextField'

type WorkItemDraft = {
  address: string
  gardenerId: number | null
  title: string
}

function WorkItemEditPage() {
  const [workItem, setWorkItem] = useState<WorkItemDraft>({
    title: '',
    address: '',
    gardenerId: null,
  })

  return (
    <PageFrame title="Add Work Item">
      <Panel title="Details">
        <TextField
          autoComplete="off"
          label="Title"
          maxLength={255}
          name="title"
          onChange={(title) => setWorkItem((current) => ({ ...current, title }))}
          value={workItem.title}
        />

        <TextField
          autoComplete="street-address"
          label="Address"
          maxLength={255}
          name="address"
          onChange={(address) => setWorkItem((current) => ({ ...current, address }))}
          value={workItem.address}
        />

        <GardenerField
          onChange={(gardenerId) => setWorkItem((current) => ({ ...current, gardenerId }))}
          value={workItem.gardenerId}
        />
      </Panel>
    </PageFrame>
  )
}

export default WorkItemEditPage
