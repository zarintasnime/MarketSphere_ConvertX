using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class CRMActivityParticipant : AuditableEntity
{
    public int CRMActivityParticipantID { get; set; }
    public int CRMActivityID { get; set; }
    public int? EmployeeID { get; set; }
    public int? ClientContactID { get; set; }
    public string? ExternalName { get; set; }
    public string? ExternalEmail { get; set; }
    public ParticipantRole? ParticipantRole { get; set; }
    public AttendanceStatus? AttendanceStatus { get; set; }

    public CRMActivity CRMActivity { get; set; } = null!;
    public Employee? Employee { get; set; }
    public ClientContact? ClientContact { get; set; }
}
