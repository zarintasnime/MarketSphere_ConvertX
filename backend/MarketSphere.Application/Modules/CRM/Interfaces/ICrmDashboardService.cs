using MarketSphere.Application.Modules.CRM.DTOs;
namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface ICrmDashboardService
{
    Task<CrmDashboardDto> GetAsync(CancellationToken cancellationToken = default);
}
