// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ServicesAndDependencyInjection.Services;
using ServicesAndDependencyInjection.Models;

#region Services
// Again--this is a bad way to inject an HttpClient
// Do not do this in production!  this is a simple example!
ApiService apiService = 
    new(new HttpClient()
    {
        BaseAddress = new("https://forms-dev.winsor.edu")
    });

AssessmentCalendarService assessmentCalendarService = 
    new AssessmentCalendarService(apiService);

#endregion

#region Global Variables, Cache, and Flags

bool loginComplete = false;
bool loginFailed = false;

bool quitRequested = false;

List<SectionRecord> mySchedule = Enumerable.Empty<SectionRecord>().ToList();
bool scheduleReady = false;

Dictionary<string, Action> menuChoices = new()
{
    {"Show my Schedule", PrintMySchedule},
    {"Quit", () => quitRequested = true}
};

#endregion

#region Login and Initialization.
Stopwatch sw = Stopwatch.StartNew();

var loginTask = apiService.LoginAsync();
loginTask.GetAwaiter().OnCompleted(() =>
{
    if(loginTask.Result)
        Console.WriteLine("Login Successful!");
    else
    {
        Console.WriteLine("Your saved credentials failed!");
        loginFailed = true;
    }
    
    loginComplete = true;
    
});
loginTask.Start();

while (!loginComplete)
{
    Thread.Sleep(500);
    Console.WriteLine($"Waiting {sw.Elapsed.TotalSeconds:N3} seconds");
    if (loginFailed)
    {
        return; // exit the program
    }
}

Console.WriteLine("Getting my schedule...");

var getScheduleTask = assessmentCalendarService.GetMyScheduleAsync();
getScheduleTask.GetAwaiter().OnCompleted(() =>
{
    mySchedule = getScheduleTask.Result;
    scheduleReady = true;
});

if (getScheduleTask.Status == TaskStatus.WaitingToRun)
    getScheduleTask.Start();

PrintMySchedule();

#endregion // Initialization

#region Program Loop!

while (!quitRequested)
{
    Console.WriteLine("___________________________________________\n\n");
    var choice = GetMenuChoice("What would you like to do?",
        menuChoices.Keys.ToList());
    
    Console.WriteLine("___________________________________________\n\n");
    menuChoices[choice](); // WHAT THE FU*(@$?
    
}

Console.WriteLine("Bye!");
// End of Program!

#endregion

#region HelperMethods

string GetMenuChoice(string prompt, List<string> options, string defaultOption = "")
{
    int choice = -1;
    while (choice == -1)
    {
        Console.WriteLine($"{prompt} [1-{options.Count}]");
        for (int i = 0; i < options.Count; i++)
            Console.WriteLine($"{i + 1}:\t{options[i]}");

        Console.Write(">> ");
        if (!int.TryParse(Console.ReadLine()!, out choice))
        {
            choice = -1;
            if (!string.IsNullOrEmpty(defaultOption))
                return defaultOption;
        }
    }

    return options[choice - 1];
}

void PrintMySchedule()
{
    if (!scheduleReady)
    {
        Console.WriteLine("I'm not done retrieving the schedule yet!");
        return;
    }

    foreach (var section in mySchedule)
        Console.WriteLine(section);
    
    Console.WriteLine("\nPress any key to continue.");
    Console.ReadKey();
}

#endregion