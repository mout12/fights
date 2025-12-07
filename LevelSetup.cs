using System.Collections.Generic;

namespace fights;

public record LevelSetup(int Level, List<Fighter> Enemies, Boss Boss);
