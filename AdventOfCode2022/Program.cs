using System.Text.RegularExpressions;
//day one
var elfNumber = 0;
var lines = File.ReadLines("day1.txt").Select(x => string.IsNullOrWhiteSpace(x) ? new {calories = 0, elfNumber = ++elfNumber} : new {calories = int.Parse(x), elfNumber});
var elfCalories = lines.GroupBy(x => x.elfNumber).Select(x => new {elfNumber = x.Key, calories = x.Sum(y => y.calories)}).OrderByDescending(x => x.calories);
var topElf = elfCalories.First();
Console.WriteLine($"the top elf {topElf.elfNumber} has {topElf.calories}");
var topThreeElves = elfCalories.Take(3);
Console.WriteLine($"the top three elves have {topThreeElves.Sum(x => x.calories)} calories");

//day2
int TranslateLetterToNumber(string letter) => letter == "A" ||letter == "X" ? 1 : letter == "B" || letter == "Y" ? 2 : 3;
int GetWinner(int play) => play == 3 ? 1 : play == 2 ? 3 : 2;
int GetLoser(int play) => play == 3 ? 2 : play == 2 ? 1 : 3;
var linesDay2 = File.ReadLines("day2.txt")
    .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
    .Select(x => new {opponent = TranslateLetterToNumber(x[0]), mine = TranslateLetterToNumber(x[1])})
    .Select(x => new { x.opponent,x.mine, score = (x.opponent == x.mine ? 3 : x.mine == GetWinner(x.opponent) ? 6 : 0) + x.mine });
Console.WriteLine($"{linesDay2.Sum(x => x.score)}");
var linesDay2Part2 = File.ReadLines("day2.txt")
    .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
    .Select(x => new {opponent = TranslateLetterToNumber(x[0]), mine = x[1]})
    .Select(x => new {x.opponent, mine = x.mine == "X" ? GetLoser(x.opponent) : x.mine == "Y" ? x.opponent : GetWinner(x.opponent) })
    .Select(x => new { x.opponent,x.mine, score = (x.opponent == x.mine ? 3 : x.mine == GetWinner(x.opponent) ? 6 : 0) + x.mine });
Console.WriteLine($"{linesDay2Part2.Sum(x => x.score)}");

//day 3
var day3Part1 = File.ReadLines("day3.txt")
    .Select(x => x.Skip(x.Length/2).Intersect(x.Take(x.Length/2)).First()) // repeated char
    .Select(x => (int)(x < 91 ? x - 38 : x - 96)) //priority
    .Sum();
Console.WriteLine(day3Part1);
var day3Part2 = File.ReadLines("day3.txt").Chunk(3)
    .Select(x => x[0].Intersect(x[1]).Intersect(x[2]).First()) // repeated char
    .Select(x => (int)(x < 91 ? x - 38 : x - 96)) //priority
    .Sum();
Console.WriteLine(day3Part2);

//day 4
var day4Part1 = File.ReadLines("day4.txt")
    .Select(x => x.Split(",").Select(y => y.Split("-").Select(int.Parse).ToArray()).ToArray())
    .Count(x => x[0][0] <= x[1][0] && x[0][1] >= x[1][1] || x[0][0] >= x[1][0] && x[0][1] <= x[1][1]);
Console.WriteLine(day4Part1);
var day4Part2 = File.ReadLines("day4.txt")
    .Select(x => x.Split(",").Select(y => y.Split("-").Select(int.Parse).ToArray()).Select(z =>Enumerable.Range(z[0],z[1]-z[0]+1)).ToArray())
    .Count(x => x[0].Intersect(x[1]).Any());
Console.WriteLine(day4Part2);

//day 5
var day5lines = File.ReadLines("day5.txt").ToList();
List<Stack<string>> StackCache = null;

//part 1 
List<Stack<string>> ToStacks(List<string[]> s) => Enumerable.Range(0, s.First().Length).Select(_ => new Stack<string>())
        .Select((x, i) => { s.ToList().ForEach(y => { if (!string.IsNullOrWhiteSpace(y[i])) x.Push(y[i]); }); return x;}).ToList();

day5lines.SkipWhile(x => !string.IsNullOrEmpty(x)).Skip(1) //skip up to the empty line and then skip again
    .Select(l => l.Replace("move ","").Replace(" from ", ",").Replace(" to ",",").Split(",").Select(int.Parse).ToArray())
    .Select(l => new {from = l[1], count = l[0], to = l[2]})
    .Select(c => new {command = c, stacks = StackCache ??= ToStacks(day5lines.TakeWhile(x => !string.IsNullOrEmpty(x)).SkipLast(1)
        .Select(x => Regex.Replace(x,"(.{4})", "$1,").Replace(" ","").Replace("[", "").Replace("]", ""))
        .Select(x => x.Split(",")).Reverse().ToList())}).ToList()
    .ForEach(x => Enumerable.Range(0,x.command.count).ToList().ForEach(_ => x.stacks[x.command.to-1].Push(x.stacks[x.command.from-1].Pop())));
    
Console.WriteLine(string.Join("",StackCache.Select(x => x.ToList().First())));
StackCache = null;

//part 2
day5lines.SkipWhile(x => !string.IsNullOrEmpty(x)).Skip(1) //skip up to the empty line and then skip again
    .Select(l => l.Replace("move ","").Replace(" from ", ",").Replace(" to ",",").Split(",").Select(int.Parse).ToArray())
    .Select(l => new {from = l[1]-1, count = l[0], to = l[2]-1})
    .Select(c => new {command = c, stacks = StackCache ??= ToStacks(day5lines.TakeWhile(x => !string.IsNullOrEmpty(x)).SkipLast(1)
        .Select(x => Regex.Replace(x,"(.{4})", "$1,").Replace(" ","").Replace("[", "").Replace("]", ""))
        .Select(x => x.Split(",")).Reverse().ToList()) }).ToList()
    .ForEach(x => { Enumerable.Range(0,x.command.count).Select(_=> x.stacks[x.command.from].Pop()).Reverse().ToList()
                .ForEach(i => x.stacks[x.command.to].Push(i)); 
    });
Console.WriteLine(string.Join("",StackCache.Select(x => x.ToList().First())));

//day 6 
var day6Text = File.ReadAllText("day6.txt");
var signalReceivedAfter = day6Text.Select((_,i) =>  day6Text.Skip(i).Take(4))
    .TakeWhile(x => x.Distinct().Count() != 4).Select(x => x.First())
    .Count() + 4;
Console.WriteLine(signalReceivedAfter);

var messageReceivedAfter = day6Text.Select((_,i) =>  day6Text.Skip(i).Take(14))
    .TakeWhile(x => x.Distinct().Count() != 14).Select(x => x.First())
    .Count() + 14;
Console.WriteLine(messageReceivedAfter);

//
// x => new {type =  x == "$ cd .." ? "up dir" : int.TryParse(x.First().ToString(),out _) ? "file" : "directory", text=x})
//     .Select(
//day 7
// var day7Lines = File.ReadLines("day7.txt");
// day7Lines.Select(x => new { content = x switch
//     {
//         "$ cd /" => new Day7Content("home", x.Replace("$ cd /",""), 0),
//         "$ cd .." => new Day7Content("up", x.Replace("$ cd ..",""), 0),
//         var t when t.StartsWith("$ cd ") => new Day7Content("navigate", x.Replace("$ cd ",""), 0),
//         var t when t.StartsWith("dir ") => new Day7Content("dir",x.Replace("dir ",""), 0),
//         {type: "file"} => new Day7Content(x.Split(" ")[1], int.Parse(x.text.Split(" ")[0])),
//         {type: "directory"} => new Day7Content(x.Split(" ")[1], 0)
//     }, x.type})
//     .Select();
//
// record Day7Content(string Command, string Text, int? Size);
