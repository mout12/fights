using fights;

var fighterOne = new Fighter(name: "Jackson", health: 100, damage: 15);
var fighterTwo = new Fighter(name: "Nerd", health: 120, damage: 10);

var fight = new Fight(fighterOne, fighterTwo);
fight.Start();
