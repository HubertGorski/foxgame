using FoxTales.Api.Helpers;
using FoxTales.Application.DTOs.Catalog;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces.Psych;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoxTales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PsychController(IPsychLibraryService psychLibraryService) : ControllerBase
{
    private readonly IPsychLibraryService _psychLibraryService = psychLibraryService;

    [HttpPost("addQuestion")]
    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequestDto request)
    {
        request.Question.OwnerId = User.GetUserId();
        int response = await _psychLibraryService.AddQuestion(request.Question);
        return Ok(response);
    }

    [HttpPost("removeQuestion")]
    public async Task<IActionResult> RemoveQuestion([FromBody] RemoveQuestionRequestDto request)
    {
        bool response = await _psychLibraryService.RemoveQuestion(request.QuestionId);
        return Ok(response);
    }

    [HttpPost("removeQuestions")]
    public async Task<IActionResult> RemoveQuestions([FromBody] RemoveQuestionsRequestDto request)
    {
        bool response = await _psychLibraryService.RemoveQuestions(request.QuestionsIds);
        return Ok(response);
    }

    [HttpPost("removeCatalog")]
    public async Task<IActionResult> RemoveCatalog([FromBody] RemoveCatalogRequestDto request)
    {
        bool response = await _psychLibraryService.RemoveCatalog(request.CatalogId);
        return Ok(response);
    }

    [HttpPost("addCatalog")]
    public async Task<IActionResult> AddCatalog([FromBody] CreateAndEditCatalogRequestDto request)
    {
        request.Catalog.OwnerId = User.GetUserId();
        int response = await _psychLibraryService.AddCatalog(request.Catalog);
        return Ok(response);
    }

    [HttpPost("editCatalog")]
    public async Task<IActionResult> EditCatalog([FromBody] CreateAndEditCatalogRequestDto request)
    {
        request.Catalog.OwnerId = User.GetUserId();
        bool response = await _psychLibraryService.EditCatalog(request.Catalog);
        return Ok(response);
    }

    [HttpPost("assignedQuestionsToCatalogs")]
    public async Task<IActionResult> AssignedQuestionsToCatalogs([FromBody] AssignedQuestionsToCatalogsRequestDto request)
    {
        bool response = await _psychLibraryService.AssignedQuestionsToCatalogs(request.QuestionsIds, request.CatalogsIds);
        return Ok(response);
    }
}
