import type { AnchorHTMLAttributes, ReactNode } from 'react'
import AppLink from './AppLink'
import './Button.css'

type LinkButtonProps = {
  children: ReactNode
  primary?: boolean
  warning?: boolean
} & Pick<AnchorHTMLAttributes<HTMLAnchorElement>, 'href' | 'title'>

function LinkButton({ children, href, primary, title, warning }: LinkButtonProps) {
  const cssClass = [
    'button',
    primary ? 'button--primary' : '',
    warning ? 'button--warning' : '',
  ]
    .filter(Boolean)
    .join(' ')

  return (
    <AppLink className={cssClass} href={href} title={title}>
      {children}
    </AppLink>
  )
}

export default LinkButton
