using System;
using System.Collections.Generic;

namespace fights;

public class ConsoleInputSelectionService : IInputSelectionService
{
    public T SelectOption<T>(string prompt, IReadOnlyList<InputOption<T>> options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (options.Count == 0)
        {
            throw new ArgumentException("At least one option must be provided.", nameof(options));
        }

        while (true)
        {
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                Console.WriteLine(prompt);
            }

            for (var i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i].Description}");
            }

            Console.Write("> ");
            var key = Console.ReadKey(intercept: true);
            Console.WriteLine();

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Option selection canceled.");
                continue;
            }

            if (!char.IsDigit(key.KeyChar))
            {
                Console.WriteLine("Invalid choice. Please press a number from the list.");
                continue;
            }

            var choice = key.KeyChar - '0';
            if (choice < 1 || choice > options.Count)
            {
                Console.WriteLine("Invalid choice. Please press a number from the list.");
                continue;
            }

            Console.WriteLine();
            return options[choice - 1].Value;
        }
    }
}
