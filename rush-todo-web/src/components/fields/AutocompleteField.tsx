import { useId, useMemo, useRef, useState } from 'react'
import type { KeyboardEvent } from 'react'
import type { LookupItem } from '../../api/models'
import FieldWrapper from './FieldWrapper'
import './AutocompleteField.css'

const maxVisibleItems = 100

type AutocompleteFieldProps = {
  disabled?: boolean
  error?: string
  items: readonly LookupItem[]
  label: string
  loading?: boolean
  name?: string
  onChange: (value: number | null) => void
  placeholder?: string
  value: number | null
}

function AutocompleteField({
  disabled,
  error,
  items,
  label,
  loading,
  name,
  onChange,
  placeholder,
  value,
}: AutocompleteFieldProps) {
  const generatedId = useId()
  const inputId = name ?? generatedId
  const errorId = `${inputId}-error`
  const listId = `${inputId}-list`
  const inputRef = useRef<HTMLInputElement>(null)
  const [activeIndex, setActiveIndex] = useState(0)
  const [isOpen, setIsOpen] = useState(false)
  const [query, setQuery] = useState('')
  const selectedItem = items.find((item) => item.id === value)
  const filteredItems = useMemo(() => {
    const normalizedQuery = query.trim().toLocaleLowerCase()
    const matches = normalizedQuery
      ? items.filter((item) => item.text.toLocaleLowerCase().includes(normalizedQuery))
      : items

    return matches.slice(0, maxVisibleItems)
  }, [items, query])
  const visibleValue = isOpen ? query : (selectedItem?.text ?? '')
  const activeItem = filteredItems[activeIndex]

  function open(showAll = false) {
    setQuery(showAll ? '' : (selectedItem?.text ?? ''))
    setActiveIndex(0)
    setIsOpen(true)
  }

  function select(item: LookupItem) {
    onChange(item.id)
    setQuery(item.text)
    setIsOpen(false)
  }

  function handleKeyDown(event: KeyboardEvent<HTMLInputElement>) {
    if (event.key === 'ArrowDown') {
      event.preventDefault()
      if (!isOpen) {
        open()
        return
      }

      if (filteredItems.length === 0) return
      setActiveIndex((current) => Math.min(current + 1, filteredItems.length - 1))
    }

    if (event.key === 'ArrowUp') {
      event.preventDefault()
      setActiveIndex((current) => Math.max(current - 1, 0))
    }

    if (event.key === 'Enter' && isOpen && activeItem) {
      event.preventDefault()
      select(activeItem)
    }

    if (event.key === 'Escape') {
      event.preventDefault()
      setIsOpen(false)
    }
  }

  return (
    <FieldWrapper error={error} errorId={errorId} htmlFor={inputId} label={label}>
      <div
        className={value === null ? 'autocomplete' : 'autocomplete autocomplete--clearable'}
        onBlur={(event) => {
          if (!event.currentTarget.contains(event.relatedTarget)) setIsOpen(false)
        }}
      >
        <input
          aria-activedescendant={isOpen && activeItem ? `${listId}-${activeItem.id}` : undefined}
          aria-autocomplete="list"
          aria-controls={listId}
          aria-describedby={error ? errorId : undefined}
          aria-expanded={isOpen}
          aria-invalid={error ? true : undefined}
          autoComplete="off"
          className="field__input autocomplete__input"
          disabled={disabled || loading}
          id={inputId}
          name={name}
          onChange={(event) => {
            setQuery(event.target.value)
            setActiveIndex(0)
            setIsOpen(true)
            onChange(null)
          }}
          onFocus={(event) => {
            open()
            event.currentTarget.select()
          }}
          onKeyDown={handleKeyDown}
          placeholder={loading ? 'Loading…' : placeholder}
          ref={inputRef}
          role="combobox"
          type="text"
          value={visibleValue}
        />

        {value !== null && (
          <button
            aria-label={`Clear ${label}`}
            className="field__button autocomplete__button autocomplete__clear"
            disabled={disabled || loading}
            onClick={() => {
              onChange(null)
              setQuery('')
              setIsOpen(false)
            }}
            type="button"
          >
            <span aria-hidden="true">×</span>
          </button>
        )}

        <button
          aria-label={isOpen && query === '' ? 'Hide options' : 'Show options'}
          className="field__button autocomplete__button"
          disabled={disabled || loading}
          onClick={() => {
            if (isOpen && query === '') {
              setIsOpen(false)
              return
            }

            inputRef.current?.focus()
            open(true)
          }}
          type="button"
        >
          <span aria-hidden="true">⌄</span>
        </button>

        {isOpen && (
          <ul className="autocomplete__list" id={listId} role="listbox">
            {filteredItems.map((item, index) => (
              <li
                aria-selected={item.id === value}
                className={index === activeIndex ? 'autocomplete__option autocomplete__option--active' : 'autocomplete__option'}
                id={`${listId}-${item.id}`}
                key={item.id}
                onMouseDown={(event) => event.preventDefault()}
                onMouseEnter={() => setActiveIndex(index)}
                onClick={() => select(item)}
                role="option"
              >
                {item.text}
              </li>
            ))}

            {filteredItems.length === 0 && (
              <li className="autocomplete__empty">No matching options</li>
            )}
          </ul>
        )}
      </div>
    </FieldWrapper>
  )
}

export default AutocompleteField
