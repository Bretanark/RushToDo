import { useEffect, useState } from 'react'
import LinkButton from './components/LinkButton'
import PageFrame from './components/PageFrame'
import HomePage from './pages/HomePage'
import WorkItemEditPage from './pages/WorkItemEditPage'

function App() {
  const [path, setPath] = useState(getPath)

  useEffect(() => {
    function handleNavigation() {
      setPath(getPath())
    }

    window.addEventListener('popstate', handleNavigation)
    return () => window.removeEventListener('popstate', handleNavigation)
  }, [])

  if (path === '/') return <HomePage />
  if (path === '/work-item/new') return <WorkItemEditPage />

  const editMatch = path.match(/^\/work-item\/(\d+)$/)
  if (editMatch) return <WorkItemEditPage workItemId={Number(editMatch[1])} />

  return (
    <PageFrame
      actions={<LinkButton href="/">Home</LinkButton>}
      title="Page not found"
    >
      <p>The requested page does not exist.</p>
    </PageFrame>
  )
}

function getPath(): string {
  return window.location.pathname.replace(/\/$/, '') || '/'
}

export default App
