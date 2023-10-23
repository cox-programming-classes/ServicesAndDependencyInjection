using System.Diagnostics;
using ServicesAndDependencyInjection.Models;

namespace ServicesAndDependencyInjection.Services;

public class AssessmentCalendarService
{
    private readonly ApiService api;

    public AssessmentCalendarService(ApiService api)
    {
        this.api = api;
    }

    public async Task<List<SectionRecord>> GetMyScheduleAsync()
    {
        var result = 
            await api.ApiCallAsync<object, List<SectionRecord>>(
                HttpMethod.Get,"api/schedule/academic");

        if (result is not null)
            return result;
        // if result is null, give me a non-null result...
        return Enumerable.Empty<SectionRecord>().ToList();
    }
    
    public List<SectionRecord> GetMySchedule()
    {
        if (api.TryApiCall(HttpMethod.Get, 
                "api/schedule/academic",
                out List<SectionRecord> records, 
                out ErrorRecord? error))
        {
            return records;
        }
        // do something with `error`
        return new();
    }
}