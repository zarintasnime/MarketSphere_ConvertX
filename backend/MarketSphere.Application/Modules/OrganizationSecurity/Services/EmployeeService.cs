using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IApplicationDbContext _db;

    public EmployeeService(IApplicationDbContext db)
    {
        _db = db;
    }

    public Task<PagedResult<EmployeeListItemDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        PaginationValidator.Validate(request);

        var query =
            from employee in _db.Employees
            join designation in _db.Designations
                on employee.DesignationID equals designation.DesignationID
            join branch in _db.Branches
                on employee.BranchID equals branch.BranchID
            join user in _db.Users
                on employee.UserID equals (int?)user.UserID into userJoin
            from user in userJoin.DefaultIfEmpty()
            select new
            {
                Employee = employee,
                DesignationName = designation.DesignationName,
                BranchName = branch.BranchName,
                UserFullName = user == null ? null : user.FullName
            };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();

            query = query.Where(
                x => x.Employee.EmployeeCode.ToLower().Contains(search) ||
                     (x.UserFullName != null &&
                      x.UserFullName.ToLower().Contains(search)) ||
                     x.DesignationName.ToLower().Contains(search) ||
                     x.BranchName.ToLower().Contains(search));
        }

        var totalCount = query.Count();

        var items = query
            .OrderBy(x => x.Employee.EmployeeCode)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new EmployeeListItemDto(
                x.Employee.EmployeeID,
                x.Employee.EmployeeCode,
                x.Employee.UserID,
                x.UserFullName,
                x.Employee.DesignationID,
                x.DesignationName,
                x.Employee.BranchID,
                x.BranchName,
                x.Employee.Status))
            .ToArray();

        return Task.FromResult(
            PagedResult<EmployeeListItemDto>.Create(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize));
    }

    public Task<EmployeeDetailsDto> GetByIDAsync(
        int employeeID,
        CancellationToken cancellationToken = default)
    {
        var employee = RequireEmployee(employeeID);

        return Task.FromResult(
            new EmployeeDetailsDto(
                employee.EmployeeID,
                employee.EmployeeCode,
                employee.UserID,
                employee.DesignationID,
                employee.ManagerEmployeeID,
                employee.BranchID,
                employee.RegionID,
                employee.AreaID,
                employee.TerritoryID,
                employee.JoiningDate,
                employee.EndDate,
                employee.Status,
                employee.Phone,
                employee.Email));
    }

    public async Task<int> CreateAsync(
        CreateEmployeeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidationHelper.RequireNotBlank(
            request.EmployeeCode,
            nameof(request.EmployeeCode),
            50);

        ValidateDates(request.JoiningDate, request.EndDate);

        var employeeCode = request.EmployeeCode.NormalizeCode();

        if (_db.Employees.Any(x => x.EmployeeCode == employeeCode))
        {
            throw new ConflictException(
                $"Employee code '{employeeCode}' already exists.");
        }

        ValidateReferences(
            request.UserID,
            request.DesignationID,
            request.ManagerEmployeeID,
            request.BranchID,
            request.RegionID,
            request.AreaID,
            request.TerritoryID,
            null);

        var employee = new Employee
        {
            EmployeeCode = employeeCode,
            UserID = request.UserID,
            DesignationID = request.DesignationID,
            ManagerEmployeeID = request.ManagerEmployeeID,
            BranchID = request.BranchID,
            RegionID = request.RegionID,
            AreaID = request.AreaID,
            TerritoryID = request.TerritoryID,
            JoiningDate = request.JoiningDate,
            EndDate = request.EndDate,
            Status = request.Status,
            Phone = request.Phone.NullIfWhiteSpace(),
            Email = request.Email.NullIfWhiteSpace()?.NormalizeEmail()
        };

        await _db.AddAsync(employee, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return employee.EmployeeID;
    }

    public async Task UpdateAsync(
        int employeeID,
        UpdateEmployeeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var employee = RequireEmployee(employeeID);

        ValidateDates(request.JoiningDate, request.EndDate);

        ValidateReferences(
            request.UserID,
            request.DesignationID,
            request.ManagerEmployeeID,
            request.BranchID,
            request.RegionID,
            request.AreaID,
            request.TerritoryID,
            employeeID);

        employee.UserID = request.UserID;
        employee.DesignationID = request.DesignationID;
        employee.ManagerEmployeeID = request.ManagerEmployeeID;
        employee.BranchID = request.BranchID;
        employee.RegionID = request.RegionID;
        employee.AreaID = request.AreaID;
        employee.TerritoryID = request.TerritoryID;
        employee.JoiningDate = request.JoiningDate;
        employee.EndDate = request.EndDate;
        employee.Status = request.Status;
        employee.Phone = request.Phone.NullIfWhiteSpace();
        employee.Email = request.Email.NullIfWhiteSpace()?.NormalizeEmail();

        await _db.SaveChangesAsync(cancellationToken);
    }

    private Employee RequireEmployee(int employeeID) =>
        _db.Employees.FirstOrDefault(x => x.EmployeeID == employeeID)
        ?? throw new NotFoundException(
            $"Employee with ID {employeeID} was not found.");

    private void ValidateReferences(
        int? userID,
        int designationID,
        int? managerEmployeeID,
        int branchID,
        int? regionID,
        int? areaID,
        int? territoryID,
        int? currentEmployeeID)
    {
        if (!_db.Designations.Any(
                x => x.DesignationID == designationID &&
                     x.IsActive))
        {
            throw new NotFoundException(
                $"Designation with ID {designationID} was not found or is inactive.");
        }

        if (!_db.Branches.Any(
                x => x.BranchID == branchID &&
                     x.IsActive))
        {
            throw new NotFoundException(
                $"Branch with ID {branchID} was not found or is inactive.");
        }

        if (userID.HasValue)
        {
            if (!_db.Users.Any(x => x.UserID == userID.Value))
            {
                throw new NotFoundException(
                    $"User with ID {userID.Value} was not found.");
            }

            if (_db.Employees.Any(
                    x => x.UserID == userID.Value &&
                         x.EmployeeID != currentEmployeeID))
            {
                throw new ConflictException(
                    "The selected user is already linked to another employee.");
            }
        }

        if (managerEmployeeID.HasValue)
        {
            if (managerEmployeeID == currentEmployeeID)
            {
                throw new BusinessRuleException(
                    "An employee cannot be their own manager.");
            }

            if (!_db.Employees.Any(
                    x => x.EmployeeID == managerEmployeeID.Value &&
                         x.Status == EmployeeStatus.Active))
            {
                throw new NotFoundException(
                    $"Manager employee with ID {managerEmployeeID.Value} was not found or is inactive.");
            }
        }

        ValidateGeography(regionID, areaID, territoryID);
    }

    private void ValidateGeography(
        int? regionID,
        int? areaID,
        int? territoryID)
    {
        if (regionID.HasValue &&
            !_db.Regions.Any(x => x.RegionID == regionID.Value))
        {
            throw new NotFoundException(
                $"Region with ID {regionID.Value} was not found.");
        }

        if (areaID.HasValue)
        {
            var area = _db.Areas.FirstOrDefault(
                x => x.AreaID == areaID.Value)
                ?? throw new NotFoundException(
                    $"Area with ID {areaID.Value} was not found.");

            if (regionID.HasValue &&
                area.RegionID != regionID.Value)
            {
                throw new BusinessRuleException(
                    "The selected area does not belong to the selected region.");
            }
        }

        if (territoryID.HasValue)
        {
            var territory = _db.Territories.FirstOrDefault(
                x => x.TerritoryID == territoryID.Value)
                ?? throw new NotFoundException(
                    $"Territory with ID {territoryID.Value} was not found.");

            var territoryArea = _db.Areas.First(
                x => x.AreaID == territory.AreaID);

            if (areaID.HasValue &&
                territory.AreaID != areaID.Value)
            {
                throw new BusinessRuleException(
                    "The selected territory does not belong to the selected area.");
            }

            if (regionID.HasValue &&
                territoryArea.RegionID != regionID.Value)
            {
                throw new BusinessRuleException(
                    "The selected territory does not belong to the selected region.");
            }
        }
    }

    private static void ValidateDates(
        DateOnly joiningDate,
        DateOnly? endDate)
    {
        if (endDate.HasValue &&
            endDate.Value < joiningDate)
        {
            throw new BusinessRuleException(
                "End date cannot be earlier than joining date.");
        }
    }
}
