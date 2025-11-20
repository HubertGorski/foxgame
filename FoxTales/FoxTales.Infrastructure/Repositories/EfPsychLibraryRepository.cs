using System.Collections.Immutable;
using FoxTales.Application.Exceptions;
using FoxTales.Domain.Configurations;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Repositories;

public class EfPsychLibraryRepository(FoxTalesDbContext db) : IPsychLibraryRepository
{
    private readonly FoxTalesDbContext _db = db;

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

    public async Task<ICollection<CatalogType>> GetCatalogTypesByPresetName(CatalogTypePresetName presetName)
    {
        ImmutableList<int> selectedIds = CatalogTypePresets.GetPresetIds(presetName) ?? [];
        return await _db.CatalogTypes.Where(ct => selectedIds.Contains(ct.CatalogTypeId)).ToListAsync();
    }

    public async Task<ICollection<Question>> GetPublicQuestions()
    {
        return await _db.Questions.Where(q => q.IsPublic).ToListAsync();
    }

    public async Task<ICollection<Catalog>> GetPublicCatalogsWithExampleQuestions()
    {
        return await _db.Catalogs
            .Include(c => c.Questions)
            .Where(q => q.CatalogTypeId == (int)CatalogTypeName.Public).ToListAsync();
    }
}