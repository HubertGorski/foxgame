using AutoMapper;
using FoxTales.Application.DTOs.Catalog;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Exceptions;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Services.Psych;

public class PsychLibraryService(IPsychLibraryRepository psychLibraryRepository, IMapper mapper) : IPsychLibraryService
{
    private readonly IPsychLibraryRepository _psychLibraryRepository = psychLibraryRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<int> AddQuestion(QuestionDto request)
    {
        Question question = _mapper.Map<Question>(request);
        question.CreatedDate = DateTime.UtcNow;
        int questionId = await _psychLibraryRepository.AddQuestion(question);
        await _psychLibraryRepository.AddQuestionsToCatalogs([questionId], request.CatalogIds ?? []);
        return questionId;
    }

    public async Task<bool> AssignedQuestionsToCatalogs(List<int> questionsIds, List<int> catalogsIds)
    {
        await _psychLibraryRepository.AddQuestionsToCatalogs(questionsIds, catalogsIds);
        return true;
    }

    public async Task<bool> RemoveQuestion(int questionId)
    {
        return await _psychLibraryRepository.RemoveQuestion(questionId);
    }

    public async Task<bool> RemoveCatalog(int catalogId)
    {
        return await _psychLibraryRepository.RemoveCatalog(catalogId);
    }

    public async Task<bool> RemoveQuestions(List<int> questionIds)
    {
        return await _psychLibraryRepository.RemoveQuestions(questionIds);
    }

    public async Task<int> AddCatalog(CreateAndEditCatalogDto request)
    {
        Catalog catalog = _mapper.Map<Catalog>(request);
        catalog.CreatedDate = DateTime.UtcNow;
        int catalogId = await _psychLibraryRepository.AddCatalog(catalog, request.QuestionsIds);
        await _psychLibraryRepository.AddAvailableTypesToCatalog(catalogId, request.AvailableTypeIds);

        return catalogId;
    }

    public async Task<bool> EditCatalog(CreateAndEditCatalogDto request)
    {
        if (request.CatalogId == null || request.CatalogId == 0)
            throw new NotFoundException("Catalog doesn't exist!");

        Catalog catalog = _mapper.Map<Catalog>(request);
        return await _psychLibraryRepository.EditCatalog(catalog);
    }

    public async Task<ICollection<CatalogTypeDto>> GetCatalogTypesByPresetName(CatalogTypePresetName presetName)
    {
        ICollection<CatalogType> availableCatalogTypes = await _psychLibraryRepository.GetCatalogTypesByPresetName(presetName);
        return _mapper.Map<ICollection<CatalogTypeDto>>(availableCatalogTypes);
    }

    public async Task<ICollection<QuestionDto>> GetPublicQuestions()
    {
        ICollection<Question> publicQuestions = await _psychLibraryRepository.GetPublicQuestions();
        return _mapper.Map<ICollection<QuestionDto>>(publicQuestions);
    }

}
