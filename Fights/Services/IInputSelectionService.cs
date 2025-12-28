using System.Collections.Generic;

namespace fights;

public interface IInputSelectionService
{
    T SelectOption<T>(string prompt, IReadOnlyList<InputOption<T>> options);
}

public record InputOption<T>(string Description, T Value, char? Hotkey = null);
