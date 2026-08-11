import { useId } from 'react'
import FieldWrapper from './FieldWrapper'
import './TextField.css'

type TextFieldProps = {
  autoComplete?: string
  autoFocus?: boolean
  className?: string
  error?: string
  label: string
  maxLength?: number
  name?: string
  onChange: (value: string) => void
  value: string
}

function TextField({
  autoComplete,
  autoFocus,
  className,
  error,
  label,
  maxLength,
  name,
  onChange,
  value,
}: TextFieldProps) {
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
        autoComplete={autoComplete}
        autoFocus={autoFocus}
        className="text-field__input"
        id={inputId}
        maxLength={maxLength}
        name={name}
        onChange={(event) => onChange(event.target.value)}
        type="text"
        value={value}
      />
    </FieldWrapper>
  )
}

export default TextField
