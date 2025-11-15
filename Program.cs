using fights;

var fighterOne = new Fighter(name: "Jackson", health: 100, damage: 15);
var fighterTwo = new Fighter(name: "Nerd", health: 120, damage: 10);

Console.WriteLine($"{fighterOne.Name} -> Health: {fighterOne.Health}, Damage: {fighterOne.Damage}");
Console.WriteLine($"{fighterTwo.Name} -> Health: {fighterTwo.Health}, Damage: {fighterTwo.Damage}");
