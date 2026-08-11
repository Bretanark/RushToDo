import type { ReactNode } from 'react'
import './FieldWrapper.css'

type FieldWrapperProps = {
  children: ReactNode
  className?: string
  error?: string
  errorId?: string
  htmlFor: string
  label: string
}

function FieldWrapper({ children, className, error, errorId, htmlFor, label }: FieldWrapperProps) {
  const cssClass = ['field', error ? 'field--invalid' : '', className ?? '']
    .filter(Boolean)
    .join(' ')

  return (
    <div className={cssClass}>
      <label className="field__label" htmlFor={htmlFor}>
        {label}
      </label>

      <div className="field__control">{children}</div>

      {error && (
        <span className="field__error" id={errorId}>
          {error}
        </span>
      )}
    </div>
  )
}

export default FieldWrapper
