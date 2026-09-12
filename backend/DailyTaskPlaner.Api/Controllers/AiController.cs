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
    private readonly ILLMService _llmService;
    public AiController(ILLMService llmService)
    {
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
}
