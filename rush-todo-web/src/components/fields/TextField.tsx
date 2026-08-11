import { useId } from 'react'
import type { TextValidator } from '../../validators/ValidatorBase'
import FieldWrapper from './FieldWrapper'

type TextFieldProps = {
  autoComplete?: string
  autoFocus?: boolean
  className?: string
  error?: string
  label?: string
  maxLength?: number
  name?: string
  onChange: (value: string) => void
  value: string
  validator?: TextValidator
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
  validator,
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
      validator={validator}
    >
      <input
        aria-describedby={(error ?? validator?.validate()) ? errorId : undefined}
        aria-invalid={(error ?? validator?.validate()) ? true : undefined}
        autoComplete={autoComplete}
        autoFocus={autoFocus}
        className="field__input"
        id={inputId}
        maxLength={maxLength ?? validator?.maxLength}
        name={name}
        onChange={(event) => onChange(event.target.value)}
        type="text"
        value={value}
      />
    </FieldWrapper>
  )
}

export default TextField
