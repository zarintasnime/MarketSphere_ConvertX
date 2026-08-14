using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Exceptions;
using MarketSphere.Infrastructure.Persistence;

namespace MarketSphere.Infrastructure.Services;

public sealed class NumberSequenceService : INumberSequenceService
{
    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;

    public NumberSequenceService(MarketSphereDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task<string> NextAsync(string documentType, int? branchID, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(documentType)) throw new BusinessRuleException("Document type is required.");
        var type = documentType.Trim().ToUpperInvariant(); var year = _clock.UtcNow.Year;
        for (var attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                var sequence = await _db.NumberSequences.SingleOrDefaultAsync(x => x.DocumentType == type && x.YearValue == year && x.BranchID == branchID, cancellationToken);
                if (sequence is null)
                {
                    sequence = new NumberSequence { DocumentType = type, Prefix = type, YearValue = year, BranchID = branchID, LastNumber = 0, PaddingLength = 6, ResetPolicy = "YEARLY" };
                    await _db.NumberSequences.AddAsync(sequence, cancellationToken);
                }
                sequence.LastNumber++;
                await _db.SaveChangesAsync(cancellationToken);
                return $"{sequence.Prefix}-{year}-{sequence.LastNumber.ToString().PadLeft(sequence.PaddingLength, '0')}";
            }
            catch (DbUpdateConcurrencyException) when (attempt < 5)
            {
                foreach (var entry in _db.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged)) entry.State = EntityState.Detached;
            }
            catch (DbUpdateException) when (attempt < 5)
            {
                foreach (var entry in _db.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged)) entry.State = EntityState.Detached;
            }
        }
        throw new ConflictException("Document number could not be generated after multiple concurrency retries.");
    }
}
