// See https://aka.ms/new-console-template for more information
using ServicesAndDependencyInjection.Services;
using ServicesAndDependencyInjection.Models;
// Again--this is a bad way to inject an HttpClient
// Do not do this in production!  this is a simple example!
ApiService apiService = 
    new(new HttpClient()
    {
        BaseAddress = new("https://forms-dev.winsor.edu")
    });

Console.Write("Email:  ");
var email = Console.ReadLine()!;
Console.Write("Password:  ");
var password = Console.ReadLine()!;

apiService.Login(email, password);

