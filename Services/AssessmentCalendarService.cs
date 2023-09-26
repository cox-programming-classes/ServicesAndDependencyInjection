using ServicesAndDependencyInjection.Models;

namespace ServicesAndDependencyInjection.Services;

public class AssessmentCalendarService
{
    private readonly ApiService api;

    public AssessmentCalendarService(ApiService api)
    {
        this.api = api;
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