using System;

namespace Custom.InputHandling;

public class InputReader<T>
{
    public bool GetInput(string prompt, out T output)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;

        if (typeof(T) == typeof(int))
        {
            output = (T)(object)int.Parse(input);
            return true;
        }
        else if (typeof(T) == typeof(double))
        {
            output = (T)(object)double.Parse(input);
            return true;
        }
        else if (typeof(T) == typeof(string))
        {
            output = (T)(object)input;
            return true;
        }
        else
        {
            output = (T)(object)"";
            return false;
        }
    }

    public bool GetInputFromList(string prompt, out T output, T[] validAnswers)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;

        try
        {
            if (typeof(T) == typeof(int))
            {
                int parsedInput = int.Parse(input);
                if (validAnswers.Contains((T)(object)parsedInput))
                {
                    output = (T)(object)parsedInput;
                    return true;
                }
            }
            else if (typeof(T) == typeof(double))
            {
                double parsedInput = double.Parse(input);
                if (validAnswers.Contains((T)(object)parsedInput))
                {
                    output = (T)(object)parsedInput;
                    return true;
                }
            }
            else if (typeof(T) == typeof(string))
            {
                if (validAnswers.Contains((T)(object)input))
                {
                    output = (T)(object)input;
                    return true;
                }
            }
        }
        catch (Exception) { /*nope*/ }

        output = default!;
        return false;
    }

}
