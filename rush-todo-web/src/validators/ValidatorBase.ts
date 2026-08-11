type ValueValidatorOptions<T> = {
  getValue: () => T
  isVisible?: boolean
  label: string
  validation?: (value: T) => string | undefined
}

abstract class ValidatorBase {
  readonly isVisible: boolean
  readonly label: string

  constructor(label: string, isVisible = true) {
    this.isVisible = isVisible
    this.label = label
  }

  abstract validate(): string | undefined
}

class ValueValidator<T> extends ValidatorBase {
  private readonly getValue: () => T
  private readonly validation?: (value: T) => string | undefined

  constructor(options: ValueValidatorOptions<T>) {
    super(options.label, options.isVisible)
    this.getValue = options.getValue
    this.validation = options.validation
  }

  override validate(): string | undefined {
    if (!this.isVisible) return undefined

    return this.validation?.(this.getValue())?.replace('[Label]', this.label)
  }
}

type TextValidatorOptions = ValueValidatorOptions<string | null> & {
  maxLength?: number
}

class TextValidator extends ValueValidator<string | null> {
  readonly maxLength?: number

  constructor(options: TextValidatorOptions) {
    super(options)
    this.maxLength = options.maxLength
  }
}

class DateValidator extends ValueValidator<string | null> {}

export { DateValidator, TextValidator, ValidatorBase, ValueValidator }
