using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Data;
using DailyTaskPlaner.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace DailyTaskPlaner.Business.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;
    public AiService(HttpClient httpClient, AppDbContext dbContext)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _httpClient.BaseAddress = new Uri("http://localhost:11434");
    }

    public async Task<string> AskAboutTaskAsync(AiQueryDto dto)
    {
        try
        {
            var now = DateTime.UtcNow;
            var tasks = dto.Range switch
            {
                TimeRange.Day => await _dbContext.DailyTasks
                    .Where(t =>
                        t.UserId == dto.UserId &&
                        t.IsActive == true &&
                        t.StartDate >= now.Date &&
                        t.StartDate < now.Date.AddDays(1))
                    .ToListAsync(),

                TimeRange.Week => await _dbContext.DailyTasks
                    .Where(t =>
                        t.UserId == dto.UserId &&
                        t.IsActive == true &&
                        t.StartDate >= now.Date &&
                        t.StartDate < now.Date.AddDays(7))
                    .ToListAsync(),

                _ => await _dbContext.DailyTasks
                    .Where(t =>
                        t.UserId == dto.UserId &&
                        t.IsActive == true &&
                        t.StartDate >= now.Date &&
                        t.StartDate < now.Date.AddDays(31))
                    .ToListAsync()
            };

            var taskJson = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var prompt = $"Task:\n{taskJson}\n\nQuestion: {dto.Question}";
            var requestObj = new OllamaRequest
            {
                Model = "gemma:2b",
                Prompt = prompt,
                Stream = false
            };

            var requestJson = JsonSerializer.Serialize(requestObj);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var aiResponse = JsonSerializer.Deserialize<OllamaResponse>(responseJson);

            return aiResponse?.Response ?? "AI nije odgovorio.";
        }
        catch (HttpRequestException ex)
        {
            return $"Greška pri slanju zahteva: {ex.Message}";
        }
        catch (JsonException ex)
        {
            return $"Greška u parsiranju odgovora: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Neočekivana greška: {ex.Message}";
        }

    }
}
