import { useEffect, useState } from 'react'
import Api from '../../api/Api'
import type { LookupItem } from '../../api/models'
import AutocompleteField from './AutocompleteField'

type GardenerFieldProps = {
  error?: string
  onChange: (value: number | null) => void
  value: number | null
}

function GardenerField({ error, onChange, value }: GardenerFieldProps) {
  const [gardeners, setGardeners] = useState<LookupItem[]>([])
  const [loadError, setLoadError] = useState<string>()
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let ignore = false

    Api.getGardeners()
      .then((items) => {
        if (!ignore) setGardeners(items)
      })
      .catch((reason: unknown) => {
        if (!ignore) setLoadError(reason instanceof Error ? reason.message : 'Unable to load gardeners')
      })
      .finally(() => {
        if (!ignore) setLoading(false)
      })

    return () => {
      ignore = true
    }
  }, [])

  return (
    <AutocompleteField
      error={loadError ?? error}
      items={gardeners}
      label="Gardener"
      loading={loading}
      name="gardenerId"
      onChange={onChange}
      placeholder="Select or search"
      value={value}
    />
  )
}

export default GardenerField
