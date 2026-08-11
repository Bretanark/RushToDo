import type { ReactNode } from 'react'
import './PageFrame.css'

type PageFrameProps = {
  actions?: ReactNode
  children: ReactNode
  title: string
}

function PageFrame({ actions, children, title }: PageFrameProps) {
  return (
    <main className="page-frame">
      <header className="page-frame__header">
        <h1>{title}</h1>
        {actions && <div className="page-frame__actions">{actions}</div>}
      </header>

      <div className="page-frame__body">{children}</div>
    </main>
  )
}

export default PageFrame
