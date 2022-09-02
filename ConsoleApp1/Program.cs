// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

DateTime dima = new DateTime(2021, 12, 13);
DateTime alina = new DateTime(2022, 3, 2);
DateTime currentTime = DateTime.Now;

var res = (currentTime - dima).Days / 30.0;
var res2 = (currentTime - alina).Days / 30.0;

Console.WriteLine(res);

Console.ReadLine();


