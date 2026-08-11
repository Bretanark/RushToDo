import type { ButtonHTMLAttributes, ReactNode } from 'react'
import Tooltip from './Tooltip'
import './Button.css'

type ButtonProps = {
  children: ReactNode
  isProcessing?: boolean
  primary?: boolean
  tooltipError?: boolean
  warning?: boolean
} & Pick<ButtonHTMLAttributes<HTMLButtonElement>, 'disabled' | 'onClick' | 'title' | 'type'>

function Button({
  children,
  disabled,
  isProcessing,
  onClick,
  primary,
  title,
  tooltipError,
  type = 'button',
  warning,
}: ButtonProps) {
  const cssClass = [
    'button',
    primary ? 'button--primary' : '',
    warning ? 'button--warning' : '',
  ]
    .filter(Boolean)
    .join(' ')
  const isDisabled = Boolean(disabled || isProcessing)

  return (
    <Tooltip
      disabledTarget={isDisabled}
      isError={tooltipError}
      text={title}
    >
      <button
        aria-busy={isProcessing || undefined}
        className={cssClass}
        disabled={isDisabled}
        onClick={onClick}
        type={type}
      >
        {isProcessing && <span aria-hidden="true" className="button__spinner" />}
        {children}
      </button>
    </Tooltip>
  )
}

export default Button
