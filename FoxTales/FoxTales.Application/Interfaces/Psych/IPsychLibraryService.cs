using FoxTales.Application.DTOs.Catalog;
using FoxTales.Application.DTOs.User;
using FoxTales.Domain.Enums;

namespace FoxTales.Application.Interfaces.Psych;

public interface IPsychLibraryService
{
    Task<int> AddQuestion(QuestionDto request);
    Task<bool> RemoveQuestion(int questionId);
    Task<bool> RemoveCatalog(int catalogId);
    Task<bool> RemoveQuestions(List<int> questionIds);
    Task<int> AddCatalog(CreateAndEditCatalogDto request);
    Task<bool> EditCatalog(CreateAndEditCatalogDto request);
    Task<bool> AssignedQuestionsToCatalogs(List<int> questionsIds, List<int> catalogsIds);
    Task<ICollection<CatalogTypeDto>> GetCatalogTypesByPresetName(CatalogTypePresetName presetName);
    Task<ICollection<QuestionDto>> GetPublicQuestions();
    Task<ICollection<CatalogDto>> GetPublicCatalogsWithExampleQuestions();
}
