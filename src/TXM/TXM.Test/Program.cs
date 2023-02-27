// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Text.Json;
using TXM.Core.Models;
using TXM.Core.Logic;
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

Round ro = new Round(1, paL,  "");

Print(ro, "Round.json");

IO io = new IO(null, null);

Print(io, "IO.json");

Tournament t = new Tournament("Schlacht um Tatooine", 12345, "2.0", new XWing25Rules());
t.Participants = pL;
t.Rounds.Add(new Round(0));
t.Rounds.Add(new Round("Erste Runde", paL, "Chance Engagement"));

Print(t, "Tournament.json");

//Console.Read();

Console.WriteLine($"Ready");

void Print(Object o, string fileName)
{
    using (StreamWriter f = new StreamWriter(Path.Combine(SavePath, fileName)))
    {
        f.WriteLine(JsonSerializer.Serialize(o, options));
    }
}

// TestNumbers(10);
// TestNumbers(16);
// TestNumbers(1);
//
//
//
// string s = "12345678";
// char[] cs = s.ToCharArray();


//     for (int i = 0; i < (cs.Length / 2); i++)
//     {
//         Console.WriteLine($"{cs[i]}  {cs[^(i+1)]}");
//     }
//
//     void TestNumbers(int a)
// {
//     double log2 = Math.Log2(a);
//     int b = (int) Math.Ceiling(log2);
//     Console.WriteLine($"a {a}  log2 {log2}  b {b}");
// }