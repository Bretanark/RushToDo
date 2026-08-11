import { useId } from 'react'
import type { ReactNode } from 'react'
import './Panel.css'

type PanelProps = {
  children: ReactNode
  title?: string
}

function Panel({ children, title }: PanelProps) {
  const titleId = useId()

  return (
    <section className="panel" aria-labelledby={title ? titleId : undefined}>
      {title && (
        <header className="panel__header">
          <h2 id={titleId}>{title}</h2>
        </header>
      )}

      <div className="panel__body">{children}</div>
    </section>
  )
}

export default Panel
