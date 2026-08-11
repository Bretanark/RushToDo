import { useId } from 'react'
import FieldWrapper from './FieldWrapper'

type DateFieldProps = {
  className?: string
  error?: string
  label: string
  max?: string
  min?: string
  name?: string
  onChange: (value: string | null) => void
  value: string | null
}

function DateField({ className, error, label, max, min, name, onChange, value }: DateFieldProps) {
  const generatedId = useId()
  const inputId = name ?? generatedId
  const errorId = `${inputId}-error`

  return (
    <FieldWrapper
      className={className}
      error={error}
      errorId={errorId}
      htmlFor={inputId}
      label={label}
    >
      <input
        aria-describedby={error ? errorId : undefined}
        aria-invalid={error ? true : undefined}
        className="field__input"
        id={inputId}
        max={max}
        min={min}
        name={name}
        onChange={(event) => onChange(event.target.value || null)}
        type="date"
        value={value ?? ''}
      />
    </FieldWrapper>
  )
}

export default DateField
