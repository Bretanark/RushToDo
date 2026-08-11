import type { ReactNode } from 'react'
import type { ValidatorBase } from '../../validators/ValidatorBase'
import './FieldWrapper.css'

type FieldWrapperProps = {
  children: ReactNode
  className?: string
  error?: string
  errorId?: string
  htmlFor: string
  label?: string
  validator?: ValidatorBase
}

function FieldWrapper({
  children,
  className,
  error,
  errorId,
  htmlFor,
  label,
  validator,
}: FieldWrapperProps) {
  if (validator?.isVisible === false) return null

  const validation = error ?? validator?.validate()
  const effectiveLabel = label ?? validator?.label
  const cssClass = ['field', validation ? 'field--invalid' : '', className ?? '']
    .filter(Boolean)
    .join(' ')

  return (
    <div className={cssClass}>
      <label className="field__label" htmlFor={htmlFor}>
        {effectiveLabel}
      </label>

      <div className="field__control">
        {children}

        {validation && (
          <span className="field__error" id={errorId} role="tooltip">
            {validation}
          </span>
        )}
      </div>
    </div>
  )
}

export default FieldWrapper
