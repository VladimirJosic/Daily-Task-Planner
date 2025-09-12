using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskPlaner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ILLMService _llmService;
    public AiController(IAiService aiService, ILLMService llmService)
    {
        _aiService = aiService;
        _llmService = llmService;
    }

    [HttpPost("query-llm")]
    public async Task<IActionResult> QueryLLM([FromBody] QueryLLMRequestDto request)
    {
        string response = await _llmService.ProcessQuery(request.UserId, request.Query, request.Date);
        return Ok(response);
    }

    [HttpPost("clear-chat-history")]
    public async Task<IActionResult> ClearChatHistory([FromBody] int userId)
    {
        await _llmService.ClearChatHistoryLogical(userId);
        return Ok("Chat history cleared");
    }

    [HttpPost("test")]
    public async Task<IActionResult> TestAI([FromBody] AiQueryDto dto)
    {
        var response = await _aiService.AskAboutTaskAsync(dto);
        return Ok(response);
    }
}
