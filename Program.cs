using fights;

var stick = new Weapon(name: "Stick", damage: 1);
var dagger = new Weapon(name: "Dagger", damage: 5);

var fighterOne = new Fighter(name: "Jackson", health: 100, weapon: dagger);
var fighterTwo = new Fighter(name: "Nerd", health: 120, weapon: stick);

var fight = new Fight(fighterOne, fighterTwo);
fight.Start();
