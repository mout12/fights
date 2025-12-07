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

        var entries = BuildEntries(options);

        while (true)
        {
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                Console.WriteLine(prompt);
            }

            foreach (var entry in entries)
            {
                Console.WriteLine($"{entry.DisplayLabel}. {entry.Option.Description}");
            }

            Console.Write("> ");
            var key = Console.ReadKey(intercept: true);
            Console.WriteLine();

            var selected = FindMatch(entries, key.KeyChar);
            if (selected is null)
            {
                Console.WriteLine("Invalid choice. Please press the number or hotkey for an option.");
                continue;
            }

            Console.WriteLine();
            return selected.Value;
        }
    }

    private static List<SelectionEntry<T>> BuildEntries<T>(IReadOnlyList<InputOption<T>> options)
    {
        var entries = new List<SelectionEntry<T>>(options.Count);
        var number = 1;

        foreach (var option in options)
        {
            var label = number.ToString();
            entries.Add(new SelectionEntry<T>(option, label, number));
            number++;
        }

        return entries;
    }

    private static InputOption<T>? FindMatch<T>(IEnumerable<SelectionEntry<T>> entries, char input)
    {
        if (char.IsDigit(input))
        {
            var numericValue = input - '0';
            foreach (var entry in entries)
            {
                if (entry.NumberLabel == numericValue)
                {
                    return entry.Option;
                }
            }
        }

        var normalized = char.ToUpperInvariant(input);
        foreach (var entry in entries)
        {
            if (entry.Option.Hotkey.HasValue && char.ToUpperInvariant(entry.Option.Hotkey.Value) == normalized)
            {
                return entry.Option;
            }
        }

        return null;
    }

    private sealed record SelectionEntry<T>(InputOption<T> Option, string DisplayLabel, int NumberLabel);
}
