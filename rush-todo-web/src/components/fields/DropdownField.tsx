import { useId } from 'react'
import type { LookupItem } from '../../api/models'
import FieldWrapper from './FieldWrapper'

type DropdownFieldProps = {
  className?: string
  emptyText?: string
  error?: string
  items: readonly LookupItem[]
  label: string
  name?: string
  onChange: (value: number | null) => void
  value: number | null
}

function DropdownField({
  className,
  emptyText,
  error,
  items,
  label,
  name,
  onChange,
  value,
}: DropdownFieldProps) {
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
      <select
        aria-describedby={error ? errorId : undefined}
        aria-invalid={error ? true : undefined}
        className="field__input"
        id={inputId}
        name={name}
        onChange={(event) => onChange(event.target.value === '' ? null : Number(event.target.value))}
        value={value ?? ''}
      >
        {emptyText && <option value="">{emptyText}</option>}
        {items.map((item) => (
          <option key={item.id} value={item.id}>
            {item.text}
          </option>
        ))}
      </select>
    </FieldWrapper>
  )
}

export default DropdownField
