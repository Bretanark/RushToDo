import type { AnchorHTMLAttributes, MouseEvent, ReactNode } from 'react'
import { navigate } from '../navigation'

type AppLinkProps = {
  children: ReactNode
} & Pick<
  AnchorHTMLAttributes<HTMLAnchorElement>,
  'className' | 'href' | 'title'
>

function AppLink({ children, className, href, title }: AppLinkProps) {
  function handleClick(event: MouseEvent<HTMLAnchorElement>) {
    if (
      !href?.startsWith('/') ||
      event.button !== 0 ||
      event.altKey ||
      event.ctrlKey ||
      event.metaKey ||
      event.shiftKey
    ) {
      return
    }

    event.preventDefault()
    navigate(href)
  }

  return (
    <a className={className} href={href} onClick={handleClick} title={title}>
      {children}
    </a>
  )
}

export default AppLink
