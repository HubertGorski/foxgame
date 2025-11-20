using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Interfaces;

public interface IPsychLibraryRepository
{
    Task<int> AddQuestion(Question question);
    Task<bool> RemoveQuestion(int questionId);
    Task<bool> RemoveCatalog(int catalogId);
    Task<bool> RemoveQuestions(List<int> questionIds);
    Task<int> AddCatalog(Catalog catalog, List<int> newQuestionIds);
    Task<bool> EditCatalog(Catalog catalog);
    Task AddAvailableTypesToCatalog(int catalogId, List<int> typeIds);
    Task AddQuestionsToCatalogs(List<int> questionIds, List<int> catalogsIds);
    Task<ICollection<CatalogType>> GetCatalogTypesByPresetName(CatalogTypePresetName presetName);
    Task<ICollection<Question>> GetPublicQuestions();
    Task<ICollection<Catalog>> GetPublicCatalogsWithExampleQuestions();
}
