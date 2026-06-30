using LoggingApi.Contracts.Logs;

namespace LoggingApi.Blazor.Services;

/// <summary>
/// Provides methods for interacting with the log management endpoints of LoggingApi.
/// </summary>
public sealed class LogsClient(
    HttpClient httpClient)
{
    public async Task<HttpResponseMessage> AddLogAsync(
        AddLogRequest request,
        CancellationToken cancellationToken)
    {
        return await httpClient.PostAsJsonAsync(
            $"api/v1/Logs", 
            request,
            cancellationToken);
    }
    
    public async Task<LogResponse?> GetLogByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await httpClient.GetFromJsonAsync<LogResponse>(
            $"api/v1/Logs/{id}", 
            cancellationToken);
    }
    
    public async Task<GetLogsResponse?> GetLogsAsync(
        string url,
        CancellationToken cancellationToken)
    {
        return await httpClient.GetFromJsonAsync<GetLogsResponse>(
            url,
            cancellationToken);
    }
    
    public async Task<HttpResponseMessage> UpdateLogAsync(
        Guid id,
        UpdateLogRequest request,
        CancellationToken cancellationToken)
    {
        return await httpClient.PatchAsJsonAsync(
            $"api/v1/Logs/{id}",
            request,
            cancellationToken);
    }
    
    public async Task<HttpResponseMessage> DeleteLogAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await httpClient.DeleteAsync(
            $"api/v1/Logs/{id}",
            cancellationToken);
    }
}