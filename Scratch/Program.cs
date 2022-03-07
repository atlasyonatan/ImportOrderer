using Scratch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

var entities = new List<Entity>
{
    new(){Name="Baba",      Type="E1"},
    new(){Name="Yochai",    Type="E1"},

    new(){Name="Yoshi",     Type="E2"},
    new(){Name="Marcus",    Type="E2"},
    new(){Name="Sean",      Type="E2"},

    new(){Name="Abu",       Type="E3"},
    new(){Name="Dabur",     Type="E3"},

    new(){Name="Shimi",     Type="E4"},
    new(){Name="Abugalini", Type="E4"},
    new(){Name="Abigail",   Type="E4"},
    new(){Name="Tuvalk",    Type="E4"},

    new(){Name="Name1",     Type="E2"},
    new(){Name="Name2",     Type="E2"},
    new(){Name="Name3",     Type="E2"},
    new(){Name="Name4",     Type="E2"},
    new(){Name="Name5",     Type="E2"},
    new(){Name="Name6",     Type="E2"},
    new(){Name="Name7",     Type="E2"},
    new(){Name="Name8",     Type="E2"},

};

var testDependencies = new Dictionary<string, (string type, string name)[]>
{
    {"Baba", Array.Empty<(string type, string name)>() },
    {"Yochai", Array.Empty<(string type, string name)>() },

    {"Yoshi", new[]{("E2","Marcus")} },
    {"Marcus", new []{("E4", "Shimi"),("E3", "Abu") } },
    {"Sean", new []{("E1","Baba")} },

    {"Abu", new []{("E2", "Yoshi"),("E4","Abigail"),("E1","Baba") } },
    {"Dabur", new []{("E1", "Yochai"),("E2", "Sean") } },

    {"Shimi", new []{("E4", "Abigail"),("E1", "Yochai"),("E4", "Abugalini") } },
    {"Abugalini", new []{("E2", "Sean") } },
    {"Abigail", new []{ ("E3", "Dabur") } },

    {"Tuvalk", new []{ ("E2","Name1")} },
    {"Name1", new []{ ("E2","Name2")} },
    {"Name2", new []{ ("E2","Name3")} },
    {"Name3", new []{ ("E2","Name4")} },
    {"Name4", new []{ ("E2","Name5")} },
    {"Name5", new []{ ("E2","Name6")} },
    {"Name6", new []{ ("E2","Name7")} },
    {"Name7", new []{ ("E2","Name8")} },
    {"Name8", new []{ ("E3","Abu")} },
};

var dic = testDependencies.ToDictionary(
    pair => entities.First(e => e.Name == pair.Key),
    pair => pair.Value);

var orderer = new ImportOrderer(dic);
var sw = new Stopwatch();
sw.Start();
var order = orderer.GetImportOrder().ToArray();
sw.Stop();
Console.WriteLine($"{sw.Elapsed:c}");
foreach (var item in order)
{
    Import(item);
}


static void Import(Entity entity)
{
    Console.WriteLine($"{entity.Name}, {entity.Type}");
}
