import type { ButtonHTMLAttributes, ReactNode } from 'react'
import Tooltip from './Tooltip'
import './Button.css'

type ButtonProps = {
  children: ReactNode
  isProcessing?: boolean
  primary?: boolean
  tooltipError?: boolean
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
}: ButtonProps) {
  const cssClass = primary ? 'button button--primary' : 'button'
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
