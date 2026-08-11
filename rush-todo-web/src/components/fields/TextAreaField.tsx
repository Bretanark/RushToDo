import { useId } from 'react'
import type { TextValidator } from '../../validators/ValidatorBase'
import FieldWrapper from './FieldWrapper'
import './TextAreaField.css'

type TextAreaFieldProps = {
  className?: string
  error?: string
  label?: string
  maxLength?: number
  name?: string
  onChange: (value: string) => void
  rows?: number
  value: string
  validator?: TextValidator
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
  validator,
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
      validator={validator}
    >
      <textarea
        aria-describedby={(error ?? validator?.validate()) ? errorId : undefined}
        aria-invalid={(error ?? validator?.validate()) ? true : undefined}
        className="field__input text-area-field__input"
        id={inputId}
        maxLength={maxLength ?? validator?.maxLength}
        name={name}
        onChange={(event) => onChange(event.target.value)}
        rows={rows}
        value={value}
      />
    </FieldWrapper>
  )
}

export default TextAreaField
