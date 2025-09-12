using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Data;
using DailyTaskPlaner.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Text.Json;

namespace DailyTaskPlaner.Business.Services;

public class LLMService(AppDbContext _context, IDailyTaskService dailyTaskService) : ILLMService
{
    private IChatClient _chatClient = new OllamaApiClient(new Uri("http://localhost:11434/"), "gemma:2b");
    private IDailyTaskService _dailyTaskService = dailyTaskService;
    
    public async Task<string> ProcessQuery(int userId, string query, DateOnly? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(query))
            return "Please provide a valid query";

        if (query == "Hi" || query == "Hello")
        {
            return "Hello, I'm your personal tasks assistant. How can I help?";
        }

        List<DailyTask> tasks = endDate is null
            ? await _dailyTaskService.GetAllTasksByUserId(userId)
            : await _dailyTaskService.GetTasksByUserIdAndEndDate(userId, endDate.Value);

        if (tasks.Count == 0)
        {
            return "You currently have no tasks";
        }

        string tasksJson = BuildTasksJson(tasks);
        var history = await _context.AIChatHistory
            .Where(ai => ai.UserId == userId && ai.IsDeleted == false)
            .OrderBy(ai => ai.Timestamp)
            .ToListAsync();

        List<ChatMessage> chatHistory = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, AddLLMCommands(tasksJson))
        };

        foreach (var item in history)
        {
            var chatRole = item.Role == "user" ? ChatRole.User : ChatRole.Assistant;
            chatHistory.Add(new ChatMessage(chatRole, item.Content));
        }

        chatHistory.Add(new ChatMessage(ChatRole.User, query));

        _context.AIChatHistory.Add(new AIChatHistory
        {
            Role = "user",
            Content = query,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        });

        try
        {
            string response = "";
            await foreach (ChatResponseUpdate item in _chatClient.GetStreamingResponseAsync(chatHistory))
            {
                response += item.Text;
            }

            _context.AIChatHistory.Add(new AIChatHistory
            {
                Role = "assistant",
                Content = response,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return response;
        } 
        catch (Exception ex)
        {
            return "Error in communication with LLM";
        }
    }
    public async Task ClearChatHistoryLogical(int userId)
    {

        var history = await _context.AIChatHistory
            .Where(ai => ai.UserId == userId)
            .ToListAsync();

        foreach (var item in history)
        {
            item.IsDeleted = true;
        }
    }

    public async Task ClearChatHistoryPhysical(int userId)
    {
        await _context.DailyTasks
            .Where(t => t.UserId == userId)
            .ExecuteDeleteAsync();
    }

    string BuildTasksJson(List<DailyTask> tasks)
    {
        return JsonSerializer.Serialize(tasks.Select(t => new
        {
            t.Title,
            t.Description,
            StartDate = t.StartDate.ToString("dd/MM/yyyy"),
            EndDate = t.EndDate.ToString("dd/MM/yyyy"),
            t.IsUrgent,
            t.IsActive
        }));
    }

    private string AddLLMCommands(string tasksJson)
    {
        return $@"USER TASKS (JSON FORMAT): {tasksJson}

            STRICT RESPONSE RULES:
            1. TASK REFERENCE:
               - ALWAYS check tasks JSON before responding
               - NEVER invent tasks not in the JSON data
               - NEVER respond with raw JSON
            
            2. TASK LOOKUP:
               - When asked about specific tasks (like 'football'):
                 a) Perform CASE-INSENSITIVE search in Title
                 b) Check BOTH Title and Description fields
                 c) ALWAYS respond with ACTUAL tasks from JSON

            3. RESPONSE FORMAT:
               - Specific task query response:
                 ✓ Found: 'You have: [Task Title] (Due: {{dd/MM/yyyy}}) - [Urgency]'
                 ✓ Not found: 'You don't have any tasks containing ""[search term]""'
               - Full list response:
                 • [Task Title] (Due: {{dd/MM/yyyy}}) - [Urgency]
                 • Sorted by due date (nearest first)

            4. SPECIAL INSTRUCTIONS:
               - For 'do I have' questions:
                 - Search ALL task fields
                 - NEVER answer 'no' unless absolutely certain
                 - If unsure, respond with full task list
                 - If no tasks are found, respond with 'You currently have no tasks'
               - Urgency MUST match IsUrgent field exactly
               - NEVER use phrases like 'in the list' or 'members of'
               - NEVER say 'current user', instead say 'you'
               - If user says 'Hello' respond with 'Hello, I'm your personal tasks assistant. How can I help?'
   
            5. CONTENT REQUIREMENTS:
               - ALWAYS sort tasks chronologically (nearest due first)
               - ALWAYS address user as 'you/your' (second person)
               - NEVER respond with:
                 - Just numbers (e.g., '3')
                 - Single words (e.g., 'Yes')
                 - Unformatted lists
               - MUST include ALL matching tasks

            6. SPECIAL CASES:
               - No tasks: 'You have no tasks matching this criteria'
               - No description: 'No additional details available for [Task Title]'
               - Urgency clarification:
                 - 'Urgent' = IsUrgent=true
                 - 'Not Urgent' = IsUrgent=false

            7. CHARACTER ENCODING:
               - ALWAYS use apostrophes ('') instead of unicode escapes (\u0027)
               - ALWAYS use proper dashes (–) for date ranges

            Additional context: User is asking about their task list

            EXAMPLE INTERACTIONS:
            User: Do I have a football task?
            Assistant: You have: Football practice (Due: 28/07/2025) - Urgent

            User: What tasks do I have?
            Assistant: Your current tasks:
            • Football practice (Due: 28/07/2025) - Urgent
            • Homework (Due: 03/07/2025) - Not Urgent
            • Flat tire on bike (Due: 04/07/2025) - Not Urgent

            User: Is there anything about sports?
            Assistant: You have: Football practice (Due: 28/07/2025) - Urgent";
    }

}
