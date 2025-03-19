using System.Configuration;
using iamryanmacdonald.Console.CodingTracker;

var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

var database = new Database(connectionString);
var userInterface = new UserInterface(database);
userInterface.MainMenu();