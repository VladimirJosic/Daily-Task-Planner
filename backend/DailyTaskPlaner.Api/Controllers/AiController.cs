using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyTaskPlaner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ILLMService _llmService;
    public AiController(IAiService aiService, ILLMService llmService)
    {
        _aiService = aiService;
        _llmService = llmService;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("query-llm")]
    public async Task<IActionResult> QueryLLM([FromBody] QueryLLMRequestDto request)
    {
        string response = await _llmService.ProcessQuery(GetUserId(), request.Query, request.Date);
        return Ok(response);
    }

    [HttpPost("clear-chat-history")]
    public async Task<IActionResult> ClearChatHistory()
    {
        await _llmService.ClearChatHistoryLogical(GetUserId());
        return Ok("Chat history cleared");
    }

    [HttpPost("test")]
    public async Task<IActionResult> TestAI([FromBody] AiQueryDto dto)
    {
        var response = await _aiService.AskAboutTaskAsync(dto, GetUserId());
        return Ok(response);
    }
}
