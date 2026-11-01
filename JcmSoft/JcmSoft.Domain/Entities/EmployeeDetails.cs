using JcmSoft.Domain.Entities.Enums;

namespace JcmSoft.Domain.Entities;

public class EmployeeDetails
{
    public Guid Id { get; set; }
    public DateOnly Birthdate { get; set; }
    public GenreEnum Genre { get; set; }
    public EducationalBackgroundEnum EducationBackground { get; set; }
    public MaritalStatusEnum MaritalStatus { get; set; }
    public string? Phone { get; set; }
    public string? Photo { get; set; }
    public string? TaxDocument { get; set; }
    public string? Nationality { get; set; }
    public string? Address { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}