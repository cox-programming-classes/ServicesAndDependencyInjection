// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ServicesAndDependencyInjection.Services;
using ServicesAndDependencyInjection.Models;
// Again--this is a bad way to inject an HttpClient
// Do not do this in production!  this is a simple example!
ApiService apiService = 
    new(new HttpClient()
    {
        BaseAddress = new("https://forms-dev.winsor.edu")
    });

AssessmentCalendarService assessmentCalendarService = 
    new AssessmentCalendarService(apiService);

Login:
Console.Write("Email:  ");
var email = Console.ReadLine()!;
Console.Write("Password:  ");
var password = Console.ReadLine()!;

bool loginComplete = false;
bool loginFailed = false;

Stopwatch sw = Stopwatch.StartNew();

var loginTask = apiService.LoginAsync(email, password);
loginTask.GetAwaiter().OnCompleted(() =>
{
    if(loginTask.Result)
        Console.WriteLine("Login Successful!");
    else
    {
        Console.WriteLine("Something Went Wrong!");
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
        loginFailed = false;
        goto Login;
    }
}

var getScheduleTask = assessmentCalendarService.GetMyScheduleAsync();
getScheduleTask.GetAwaiter().OnCompleted(() =>
{
    var schedule = getScheduleTask.Result;
    foreach (var thing in schedule)
        Console.WriteLine(thing);
});

if (getScheduleTask.Status == TaskStatus.WaitingToRun)
    getScheduleTask.Start();

Console.WriteLine("Getting my schedule...");
while (!getScheduleTask.IsCompleted)
{
    Console.WriteLine("waiting...");
    Thread.Sleep(500);
}


