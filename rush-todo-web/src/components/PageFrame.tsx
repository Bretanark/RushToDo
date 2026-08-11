import type { ReactNode } from 'react'
import './PageFrame.css'

type PageFrameProps = {
  children: ReactNode
  title: string
}

function PageFrame({ children, title }: PageFrameProps) {
  return (
    <main className="page-frame">
      <header className="page-frame__header">
        <h1>{title}</h1>
      </header>

      <div className="page-frame__body">{children}</div>
    </main>
  )
}

export default PageFrame
