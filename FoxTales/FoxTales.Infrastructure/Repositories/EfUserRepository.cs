using System.Collections.Immutable;
using FoxTales.Application.Exceptions;
using FoxTales.Domain.Configurations;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Repositories;

public class EfUserRepository(FoxTalesDbContext db) : IUserRepository
{
    private readonly FoxTalesDbContext _db = db;

    public async Task<int> AddAsync(User user)
    {
        _db.Users.Add(user);
        return await _db.SaveChangesAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<ICollection<Question>> GetPublicQuestions()
    {
        return await _db.Questions.Where(q => q.IsPublic).ToListAsync();
    }

    public async Task<ICollection<User>> GetAllUsers()
    {
        return await _db.Users
            .Include(u => u.UserLimits)
            .ThenInclude(ul => ul.LimitDefinition)
            .ThenInclude(ut => ut.Thresholds)
            .ToListAsync();
    }

    public async Task<ICollection<Avatar>> GetAllAvatars()
    {
        return await _db.Avatars.ToListAsync();
    }

    public async Task<ICollection<CatalogType>> GetCatalogTypesByPresetName(CatalogTypePresetName presetName)
    {
        ImmutableList<int> selectedIds = CatalogTypePresets.GetPresetIds(presetName) ?? [];
        return await _db.CatalogTypes.Where(ct => selectedIds.Contains(ct.CatalogTypeId)).ToListAsync();
    }

    public async Task<User?> GetUserById(int userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .Include(u => u.Avatar)
            .Include(u => u.Role)
            .Include(u => u.Questions)
                .ThenInclude(u => u.Catalogs)
            .Include(u => u.Catalogs)
                .ThenInclude(u => u.Questions)
            .Include(u => u.Catalogs)
                .ThenInclude(u => u.CatalogType)
            .Include(u => u.Catalogs)
                .ThenInclude(u => u.AvailableTypes)
            .Include(u => u.UserLimits)
                .ThenInclude(ul => ul.LimitDefinition)
                .ThenInclude(ut => ut.Thresholds)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ICollection<User>> GetAllUsersWithCards()
    {
        return await _db.Users.Include(u => u.Cards).ToListAsync();
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<int> StoreRefreshToken(RefreshToken refreshToken)
    {
        _db.RefreshTokens.Add(refreshToken);
        return await _db.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetRefreshTokenWithUser(string refreshToken)
    {
        var tokenEntity = await _db.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken)
            ?? throw new UnauthorizedException("Refresh token not found");

        return tokenEntity;
    }

    public async Task RevokeRefreshToken(RefreshToken tokenEntity)
    {
        tokenEntity.IsRevoked = true;
        await _db.SaveChangesAsync();
    }

    public async Task ClearTokens()
    {
        _db.RefreshTokens.RemoveRange(_db.RefreshTokens);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> SetUsername(string username, int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        user.Username = username;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetAvatar(int avatarId, int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        user.AvatarId = avatarId;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<int> AddQuestion(Question question)
    {
        _db.Questions.Add(question);
        await _db.SaveChangesAsync();
        return question.Id ?? 0;
    }

    public async Task<bool> RemoveCatalog(int catalogId)
    {
        var catalog = await _db.Catalogs
            .Include(c => c.Questions)
            .FirstOrDefaultAsync(c => c.CatalogId == catalogId);

        if (catalog == null)
            return false;

        catalog.Questions.Clear();
        _db.Catalogs.Remove(catalog);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveQuestion(int questionId)
    {
        var question = await _db.Questions.FindAsync(questionId);
        if (question == null)
            return false;

        _db.Questions.Remove(question);
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> RemoveQuestions(List<int> questionIds)
    {
        var questionsToRemove = await _db.Questions
            .Where(q => questionIds.Contains(q.Id.Value))
            .ToListAsync();

        if (questionsToRemove.Count == 0)
            return false;

        _db.Questions.RemoveRange(questionsToRemove);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int> AddCatalog(Catalog catalog, List<int> newQuestionIds)
    {
        var newQuestions = await _db.Questions
            .Where(q => newQuestionIds.Contains(q.Id.Value))
            .ToListAsync();

        foreach (var q in newQuestions)
        {
            catalog.Questions.Add(q);
        }

        _db.Catalogs.Add(catalog);
        await _db.SaveChangesAsync();
        return catalog.CatalogId ?? 0;
    }

    public async Task AddAvailableTypesToCatalog(int catalogId, List<int> typeIds)
    {
        Catalog catalog = await _db.Catalogs
            .Include(c => c.AvailableTypes)
            .FirstAsync(c => c.CatalogId == catalogId);

        List<CatalogType> types = await _db.CatalogTypes
            .Where(ct => typeIds.Contains(ct.CatalogTypeId))
            .ToListAsync();

        types.ForEach(catalog.AvailableTypes.Add);

        await _db.SaveChangesAsync();
    }

    public async Task AddQuestionsToCatalogs(List<int> questionIds, List<int> catalogsIds)
    {
        var questions = await _db.Questions
            .Include(q => q.Catalogs)
            .Where(q => questionIds.Contains(q.Id!.Value))
            .ToListAsync();

        var catalogs = await _db.Catalogs
            .Where(ct => catalogsIds.Contains(ct.CatalogId!.Value))
            .ToListAsync();

        questions.ForEach(q =>
            catalogs.ForEach(c =>
            {
                if (!q.Catalogs.Contains(c))
                    q.Catalogs.Add(c);
            }));

        await _db.SaveChangesAsync();
    }

    public async Task<bool> EditCatalog(Catalog catalog)
    {
        var existingCatalog = await _db.Catalogs
            .Include(c => c.Questions)
            .FirstOrDefaultAsync(c => c.CatalogId == catalog.CatalogId)
            ?? throw new NotFoundException("Catalog not found");

        _db.Entry(existingCatalog).CurrentValues.SetValues(catalog);

        existingCatalog.Questions.Clear();

        if (catalog.Questions?.Any() == true)
        {
            var questionIds = catalog.Questions.Select(q => q.Id).ToList();

            var existingQuestions = await _db.Questions
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            foreach (var question in existingQuestions)
            {
                existingCatalog.Questions.Add(question);
            }
        }

        await _db.SaveChangesAsync();
        return true;
    }

}
