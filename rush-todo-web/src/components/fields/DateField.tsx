import { useId } from 'react'
import type { DateValidator } from '../../validators/ValidatorBase'
import FieldWrapper from './FieldWrapper'
import './DateField.css'

type DateFieldProps = {
  className?: string
  error?: string
  label?: string
  max?: string
  min?: string
  name?: string
  onChange: (value: string | null) => void
  value: string | null
  validator?: DateValidator
}

function DateField({
  className,
  error,
  label,
  max,
  min,
  name,
  onChange,
  value,
  validator,
}: DateFieldProps) {
  const generatedId = useId()
  const inputId = name ?? generatedId
  const errorId = `${inputId}-error`
  const effectiveLabel = label ?? validator?.label ?? 'Date'

  return (
    <FieldWrapper
      className={className}
      error={error}
      errorId={errorId}
      htmlFor={inputId}
      label={label}
      validator={validator}
    >
      <div className={value === null ? 'date-field' : 'date-field date-field--clearable'}>
        <input
          aria-describedby={(error ?? validator?.validate()) ? errorId : undefined}
          aria-invalid={(error ?? validator?.validate()) ? true : undefined}
          className="field__input date-field__input"
          id={inputId}
          max={max}
          min={min}
          name={name}
          onInput={(event) => onChange(event.currentTarget.value || null)}
          type="date"
          value={value ?? ''}
        />

        {value !== null && (
          <button
            aria-label={`Clear ${effectiveLabel}`}
            className="field__button date-field__clear"
            onClick={() => onChange(null)}
            type="button"
          >
            <span aria-hidden="true">×</span>
          </button>
        )}
      </div>
    </FieldWrapper>
  )
}

export default DateField
