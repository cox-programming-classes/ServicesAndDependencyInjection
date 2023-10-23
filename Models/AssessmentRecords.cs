namespace ServicesAndDependencyInjection.Models;


public record struct TeacherRecord(string id, string firstName, string lastName, string email);

public record struct StudentRecord(string id, string displayName, string email,
    int gradYear, string className, string advisorName);

/// <summary>
/// Entry in my Academic Schedule
/// api/schedule/academics
/// </summary>
public record SectionRecord(string sectionId, string courseId, List<TeacherRecord> teachers,
    List<StudentRecord> students, string room, string block, string displayName)
{
    public override string ToString() =>
        $"[{block}] {displayName} ({room})";
}
    