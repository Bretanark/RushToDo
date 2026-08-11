import { useId } from 'react'
import FieldWrapper from './FieldWrapper'
import './TextAreaField.css'

type TextAreaFieldProps = {
  className?: string
  error?: string
  label: string
  maxLength?: number
  name?: string
  onChange: (value: string) => void
  rows?: number
  value: string
}

function TextAreaField({
  className,
  error,
  label,
  maxLength,
  name,
  onChange,
  rows = 4,
  value,
}: TextAreaFieldProps) {
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
      <textarea
        aria-describedby={error ? errorId : undefined}
        aria-invalid={error ? true : undefined}
        className="field__input text-area-field__input"
        id={inputId}
        maxLength={maxLength}
        name={name}
        onChange={(event) => onChange(event.target.value)}
        rows={rows}
        value={value}
      />
    </FieldWrapper>
  )
}

export default TextAreaField
