import { useId } from 'react'
import type { ReactNode } from 'react'
import './Tooltip.css'

type TooltipProps = {
  children: ReactNode
  disabledTarget?: boolean
  isError?: boolean
  text?: string
}

function Tooltip({ children, disabledTarget, isError, text }: TooltipProps) {
  const tooltipId = useId()
  const hasTooltip = Boolean(text?.trim())
  const cssClass = [
    'tooltip',
    disabledTarget ? 'tooltip--disabled-target' : '',
    isError ? 'tooltip--error' : '',
  ]
    .filter(Boolean)
    .join(' ')

  return (
    <span
      aria-describedby={hasTooltip ? tooltipId : undefined}
      className={cssClass}
      tabIndex={hasTooltip && disabledTarget ? 0 : undefined}
    >
      <span className="tooltip__body">{children}</span>

      {hasTooltip && (
        <span className="tooltip__content" id={tooltipId} role="tooltip">
          {text}
        </span>
      )}
    </span>
  )
}

export default Tooltip
