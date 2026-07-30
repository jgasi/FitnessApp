using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class BodyMeasurementService : IBodyMeasurementService
{
    private readonly IUnitOfWork _unitOfWork;

    public BodyMeasurementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BodyMeasurementReadDto>> GetAllAsync(string userId, bool isAdmin, DateTime? from = null, DateTime? to = null)
    {
        var query = _unitOfWork.BodyMeasurements.Query()
            .AsNoTracking()
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.MeasurementDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.MeasurementDate <= to.Value);
        }

        return await query
            .OrderByDescending(x => x.MeasurementDate)
            .Select(x => new BodyMeasurementReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                MeasurementDate = x.MeasurementDate,
                WeightKg = x.WeightKg,
                Bmi = x.Bmi,
                BodyFatPercentage = x.BodyFatPercentage,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<BodyMeasurementReadDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var query = _unitOfWork.BodyMeasurements.Query()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return await query
            .Select(x => new BodyMeasurementReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                MeasurementDate = x.MeasurementDate,
                WeightKg = x.WeightKg,
                Bmi = x.Bmi,
                BodyFatPercentage = x.BodyFatPercentage,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<BodyMeasurementReadDto> CreateAsync(string userId, BodyMeasurementCreateUpdateDto dto)
    {
        if (dto.MeasurementDate.Date > DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Datum mjerenja ne može biti u budućnosti.");
        }

        var profile = await _unitOfWork.UserProfiles.Query()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            throw new InvalidOperationException("Profil nije pronađen.");
        }

        decimal? bmi = null;
        if (profile.HeightCm.HasValue && profile.HeightCm.Value > 0)
        {
            var heightMeters = profile.HeightCm.Value / 100m;
            bmi = Math.Round(dto.WeightKg / (heightMeters * heightMeters), 2);
        }

        var entity = new BodyMeasurement
        {
            UserId = userId,
            MeasurementDate = dto.MeasurementDate.Date,
            WeightKg = dto.WeightKg,
            Bmi = bmi,
            BodyFatPercentage = dto.BodyFatPercentage,
            Notes = dto.Notes
        };

        await _unitOfWork.BodyMeasurements.AddAsync(entity);

        profile.CurrentWeightKg = dto.WeightKg;

        await _unitOfWork.SaveChangesAsync();

        return new BodyMeasurementReadDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            MeasurementDate = entity.MeasurementDate,
            WeightKg = entity.WeightKg,
            Bmi = entity.Bmi,
            BodyFatPercentage = entity.BodyFatPercentage,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, string userId, bool isAdmin, BodyMeasurementCreateUpdateDto dto)
    {
        if (dto.MeasurementDate.Date > DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Datum mjerenja ne može biti u budućnosti.");
        }

        var entity = await _unitOfWork.BodyMeasurements.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return false;
        }

        if (!isAdmin && entity.UserId != userId)
        {
            return false;
        }

        var profile = await _unitOfWork.UserProfiles.Query()
            .FirstOrDefaultAsync(x => x.UserId == entity.UserId);

        if (profile == null)
        {
            throw new InvalidOperationException("Profil nije pronađen.");
        }

        decimal? bmi = null;
        if (profile.HeightCm.HasValue && profile.HeightCm.Value > 0)
        {
            var heightMeters = profile.HeightCm.Value / 100m;
            bmi = Math.Round(dto.WeightKg / (heightMeters * heightMeters), 2);
        }

        entity.MeasurementDate = dto.MeasurementDate.Date;
        entity.WeightKg = dto.WeightKg;
        entity.Bmi = bmi;
        entity.BodyFatPercentage = dto.BodyFatPercentage;
        entity.Notes = dto.Notes;

        profile.CurrentWeightKg = dto.WeightKg;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var entity = await _unitOfWork.BodyMeasurements.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return false;
        }

        if (!isAdmin && entity.UserId != userId)
        {
            return false;
        }

        _unitOfWork.BodyMeasurements.Remove(entity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}