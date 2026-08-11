import type { WorkItemModel } from '../api/models'
import { DateValidator, TextValidator, ValidatorBase } from './ValidatorBase'

class WorkItemValidator extends ValidatorBase {
  readonly address: TextValidator
  readonly description: TextValidator
  readonly scheduledDate: DateValidator
  readonly title: TextValidator

  constructor(workItem: WorkItemModel) {
    super('Work item')

    this.title = new TextValidator({
      getValue: () => workItem.title,
      label: 'Title',
      maxLength: 255,
      validation: (value) => validateRequiredText(value, 255),
    })

    this.description = new TextValidator({
      getValue: () => workItem.description,
      label: 'Description',
    })

    this.address = new TextValidator({
      getValue: () => workItem.address,
      label: 'Address',
      maxLength: 255,
      validation: (value) => validateRequiredText(value, 255),
    })

    this.scheduledDate = new DateValidator({
      getValue: () => workItem.scheduledDate,
      label: 'Scheduled date',
    })
  }

  override validate(): string | undefined {
    return (
      this.title.validate() ?? this.address.validate()
    )
  }
}

function validateRequiredText(value: string | null, maxLength: number): string | undefined {
  if (!value?.trim()) return '[Label] is required'
  return value.length > maxLength
    ? `[Label] cannot exceed ${maxLength} characters`
    : undefined
}

export default WorkItemValidator
