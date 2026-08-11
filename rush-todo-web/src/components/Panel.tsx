import { useId } from 'react'
import type { ReactNode } from 'react'
import './Panel.css'

type PanelProps = {
  actions?: ReactNode
  children: ReactNode
  title?: string
}

function Panel({ actions, children, title }: PanelProps) {
  const titleId = useId()

  return (
    <section className="panel" aria-labelledby={title ? titleId : undefined}>
      {(title || actions) && (
        <header className="panel__header">
          {title ? <h2 id={titleId}>{title}</h2> : <span />}
          {actions && <div className="panel__actions">{actions}</div>}
        </header>
      )}

      <div className="panel__body">{children}</div>
    </section>
  )
}

export default Panel
