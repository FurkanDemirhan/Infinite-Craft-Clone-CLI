using OllamaSharp;
using System;
using System.Drawing;
using System.Collections.Frozen;
using System.IO;
using System.Net.Mime;
using System.Text.RegularExpressions;
using OllamaSharp.Models;
using OllamaSharp.Models.Exceptions;
using Console = Colorful.Console;

Console.Clear();
Console.Title = "Infinite Craft Clone CLI";
Console.WriteLine("16 GB Ram Required\n", Color.Red);
Console.WriteLine("Preparation:", Color.Cyan);
Console.WriteLine("Ensure Ollama is Installed On Your System", Color.Red);
Console.Write("FOR FIRST TIME:  Run '");
Console.Write("ollama run qwen2.5:14b", Color.Yellow);
Console.WriteLine("' on your system using your Terminal(or for windows users PowerShell)");
Console.Write("Run '");
Console.Write("ollama serve", Color.Yellow);
Console.WriteLine("' on your system using your Terminal(or for windows users PowerShell)");
Console.WriteLine("If The Following Steps Are Done Press Any Key To Continue...", Color.Green);
Console.ReadKey();
Console.Clear();
Console.WriteLine("Loading...");

string Result = "";
string Object1 = "";
string Object2 = "";
string Cache_Path = "Save/Cache.txt";
string Save_Path = "Save/Save.txt";
bool Found_Cache = false;
bool Found_Object1 = false;
bool Found_Object2 = false;

var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri);

var chat = new Chat(ollama);

ollama.SelectedModel = "qwen2.5:14b";

Directory.CreateDirectory("Save");


if (!File.Exists(Cache_Path))
{
    using (File.Create(Cache_Path)) { }
}

if (!File.Exists(Save_Path))
{
    using (File.Create(Save_Path)) { }
    using (StreamWriter writer = new StreamWriter(Save_Path, true))
    {
        writer.Write("Earth\nWater\nFire\nWind");
    }
}
string[] Cache_P = File.ReadAllLines(Cache_Path);
string Recipes_P =
    "\nWater + Fire = Steam\nWater + Wind = Wave\nWater + Water = Lake\nFire + Wind = Smoke\nFire + Earth = Lava\nFire + Fire = Volcano\nEarth + Earth = Mountain\nEarth + Wind = Dust\nEarth + Water = Plant\nWind + Wind = Tornado\n";

for (int i = 0; i < Cache_P.Length; i+=3)
{
    string obj1 = Cache_P[i];
    string obj2 = Cache_P[i + 1];
    string result = Cache_P[i + 2];
    Recipes_P += obj1 + "," + obj2 + "," + result + "\n";
}

string Start_Prompt = 
    "You are an AI designed to generate outputs based on combinations of objects in the game 'Infinite Craft'. Follow these rules:\n\nKnown Combinations:\n"
    +
    Recipes_P
    +
    "\nUnknown Combinations:\n\nFor any combination not listed above, create a plausible or creative output based on the properties and interactions of the objects involved.\nResponse Format:\n\nOnly respond with the output and Single object; no explanations or additional text.\n You're Allowed To Respond Offensively.\nWhen given a combination, refer to the known rules if applicable. For unknown combinations, use creativity based on object properties. Output only the result. for now return Initialized and dont do anything";


try
{
    await foreach (var answerToken in chat.SendAsync(Start_Prompt))
    {
        Console.Write(answerToken);
    }
}
catch (Exception)
{
    Console.Clear();
    Console.Write("You Didn't Run '",Color.Red);
    Console.Write("ollama serve", Color.Yellow);
    Console.Write("' or '", Color.Red);
    Console.Write("ollama run qwen2.5:14b",Color.Yellow);
    Console.WriteLine("' (You Need to run this once per system)", Color.Red);
    Console.WriteLine("Press any key to Exit...");
    Console.ReadKey();
    Environment.Exit(1);
}

while (true)
{
    Console.Clear();
    Console.WriteLine("______Credits______");
    Console.WriteLine("Made by Furkan Demirhan", Color.Blue);
    Console.Write("Github Page: ");
    Console.WriteLine("https://github.com/FurkanDemirhan/InfiniteCraft", Color.Yellow);
    Console.WriteLine("Inspired by Infinite Craft", Color.Magenta);
    Console.WriteLine("Original Game: Infinite Craft", Color.Magenta);
    Console.Write("Original Game Created by:" , Color.Magenta);
    Console.WriteLine(" Neal Agarwal", Color.Blue);
    Console.Write("Play the original game at: ", Color.Blue);
    Console.WriteLine("https://neal.fun/infinite-craft/", Color.Yellow);
    Console.WriteLine("___________________");
    Found_Object1 = false;
    Found_Object2 = false;
    Console.WriteLine("______Inventory______");
    string[] Save = File.ReadAllLines(Save_Path);
    int item_count = 0;
    foreach (var line in Save)
    {
        item_count++;
        Console.WriteLine(item_count.ToString()+".  " + line.Trim());
    }
    Console.WriteLine("_____________________");
    
    
    Console.WriteLine("Enter First Object:");
    
    Object1 = Console.ReadLine();

    for (int i = 0; i < Save.Length; i++)
    {
        string line = Save[i].Trim();
        if (line == Object1)
        {
            Found_Object1 = true;
            break;
        }
        else if(i == Save.Length - 1)
        {
            Console.WriteLine("Wrong Object Name");
            Found_Object1 = false;
        }
    }

    if (Found_Object1 == true)
    {
        Console.WriteLine("Enter Second Object:");

        Object2 = Console.ReadLine();

        for (int i = 0; i < Save.Length; i++)
        {
            string line = Save[i].Trim();
            if (line == Object2)
            {
                Found_Object2 = true;
                break;
            }
            else if(i == Save.Length - 1)
            {
                Console.WriteLine("Wrong Object Name");
                Found_Object2 = false;
            }
        }
        

        if (Found_Object2 == true && Found_Object1 == true)
        {
            Console.WriteLine("Result:");

            string[] Cache = File.ReadAllLines(Cache_Path);

            for (int i = 0; i < Cache.Length; i += 3)
            {
                Result = "";
                Found_Cache = false;
                string C_Object1 = Cache[i].Trim();
                string C_Object2 = Cache[i + 1].Trim();
                string C_Result = Cache[i + 2].Trim();

                if ((C_Object1 == Object1 && C_Object2 == Object2) || (C_Object1 == Object2 && C_Object2 == Object1))
                {
                    Result = C_Result;
                    Found_Cache = true;
                    break;
                }
            }

            if (!Found_Cache)
            {
                await foreach (var answerToken in chat.SendAsync(Object1 + " + " + Object2))
                {
                    Result += answerToken;

                }

                using (StreamWriter writer = new StreamWriter(Cache_Path, true))
                {
                    writer.WriteLine(Object1);
                    writer.WriteLine(Object2);
                    writer.WriteLine(Result.Trim());
                }
            }

            Console.WriteLine(Result.Trim());

            for (int i = 0; i < Save.Length; i++)
            {
                string line = Save[i];
                if (line != Result.Trim() && i == Save.Length - 1)
                {
                    using (StreamWriter writer = new StreamWriter(Save_Path, true))
                    {
                        writer.Write("\n" + Result.Trim());
                    }
                }
            }
        }
    }

    
    
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
    Console.Clear();
}
