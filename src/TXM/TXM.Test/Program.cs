// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Text.Json;
using TXM.Core;

Player p = new Player("Becker", "Martin", "TKundNobody", "Pirates of Tatooine", "Germany", 3, 2, 1, 1, 100, 222, 111, 3.5, 123, "Rebels", false, false, false, 1233455, 1, 4, true, true, 2);
Result r = new Result(200, 111, 3, 200, 2, 3);
Enemy e = new Enemy(3, true);
p.Enemies.Add(e);
p.Results.Add(r);

Console.WriteLine($"Testplayer: {p.Nickname}");

Console.WriteLine($"Starting Serializing ...");

var options = new JsonSerializerOptions { WriteIndented = true };
string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM", "Test");
if (!Directory.Exists(SavePath))
    Directory.CreateDirectory(SavePath);

Print(p, "Player.json");

Player p1 = new Player("GreenViper");
Pairing pa = new Pairing(3) { Player1 = p, Player2 = p1 };

Print(pa, "pairings.json");

Player p2 = new Player("Backfire84");
Player p3 = new Player("Dodo");
Pairing pa2 = new Pairing() { Player1 = p2, Player2 = p3 };
var paL = new System.Collections.ObjectModel.ObservableCollection<Pairing>();
paL.Add(pa);
paL.Add(pa2);
var pL = new System.Collections.ObjectModel.ObservableCollection<Player>();
pL.Add(p);
pL.Add(p1);
pL.Add(p2);
pL.Add(p3);

Round ro = new Round(1, paL, pL, "");

Print(ro, "Round.json");

IO io = new IO(null, null);

Print(io, "IO.json");

//Console.Read();

Console.WriteLine($"Ready");

void Print(Object o, string fileName)
{
    using (StreamWriter f = new StreamWriter(Path.Combine(SavePath, fileName)))
    {
        f.WriteLine(JsonSerializer.Serialize(o, options));
    }
}