import { useState } from 'react'
import PageFrame from '../components/PageFrame'
import Panel from '../components/Panel'
import TextField from '../components/fields/TextField'

function WorkItemEditPage() {
  const [workItem, setWorkItem] = useState({
    title: '',
    address: '',
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
      </Panel>
    </PageFrame>
  )
}

export default WorkItemEditPage
