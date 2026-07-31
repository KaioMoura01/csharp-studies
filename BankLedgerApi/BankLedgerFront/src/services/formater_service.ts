export class FormaterServices{
  static FormatCurrency(value: number | string) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(Number(value))
  }

  static FormatDateOnly(date: Date): string {
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  static FormatDateTime(value: string): string {
    return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value))
  }

  static FormatStatus(status: 'Pending' | 'Completed' | 'Failed' | 'Reversed'): string {
    switch (status) {
      case 'Completed':
        return 'Concluída'
      case 'Pending':
        return 'Pendente'
      case 'Failed':
        return 'Falhou'
      case 'Reversed':
        return 'Estornada'
    }
  }
}
