using System;

// Main program class that serves as the entry point
class Program
{
    static void Main(string[] args)
    {
        // Initialize the shipping quote processor with its dependencies
        var display = new ConsoleDisplay();
        var input = new ConsoleInput();
        var validator = new ShippingRulesValidator();
        var calculator = new ShippingCostCalculator();
        
        var processor = new ShippingQuoteProcessor(display, input, validator, calculator);
        processor.ProcessQuote();
    }
}

// Interface for displaying output
interface IDisplay
{
    void ShowMessage(string message);
    void ShowError(string error);
    void ShowQuote(double amount);
}

// Interface for handling user input
interface IInput
{
    double GetNumber(string prompt);
}

// Interface for validating shipping rules
interface IValidator
{
    (bool isValid, string error) ValidateWeight(double weight);
    (bool isValid, string error) ValidateDimensions(double width, double height, double length);
}

// Interface for calculating shipping costs
interface ICalculator
{
    double Calculate(double weight, double width, double height, double length);
}

// Implementation of display interface
class ConsoleDisplay : IDisplay
{
    public void ShowMessage(string message) => Console.WriteLine(message);
    public void ShowError(string error) => Console.WriteLine(error);
    public void ShowQuote(double amount) => Console.WriteLine($"Your estimated total for shipping this package is: ${amount:F2}");
}

// Implementation of input interface
class ConsoleInput : IInput
{
    public double GetNumber(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            if (double.TryParse(Console.ReadLine(), out double result))
            {
                return result;
            }
            Console.WriteLine("Invalid input. Please enter a numeric value.");
        }
    }
}

// Implementation of validator interface
class ShippingRulesValidator : IValidator
{
    private const double MaxWeight = 50;
    private const double MaxDimensionsSum = 50;

    public (bool isValid, string error) ValidateWeight(double weight)
    {
        if (weight > MaxWeight)
        {
            return (false, "Package too heavy to be shipped via Package Express. Have a good day.");
        }
        return (true, string.Empty);
    }

    public (bool isValid, string error) ValidateDimensions(double width, double height, double length)
    {
        var dimensionsSum = width + height + length;
        if (dimensionsSum > MaxDimensionsSum)
        {
            return (false, "Package too big to be shipped via Package Express.");
        }
        return (true, string.Empty);
    }
}

// Implementation of calculator interface
class ShippingCostCalculator : ICalculator
{
    public double Calculate(double weight, double width, double height, double length)
    {
        return (width * height * length * weight) / 100;
    }
}

// Main processor class that coordinates the shipping quote process
class ShippingQuoteProcessor
{
    private readonly IDisplay _display;
    private readonly IInput _input;
    private readonly IValidator _validator;
    private readonly ICalculator _calculator;

    public ShippingQuoteProcessor(IDisplay display, IInput input, IValidator validator, ICalculator calculator)
    {
        _display = display;
        _input = input;
        _validator = validator;
        _calculator = calculator;
    }

    public void ProcessQuote()
    {
        _display.ShowMessage("Welcome to Package Express. Please follow the instructions below.");

        // Get and validate weight
        var weight = _input.GetNumber("Please enter the package weight:");
        var (weightValid, weightError) = _validator.ValidateWeight(weight);
        if (!weightValid)
        {
            _display.ShowError(weightError);
            return;
        }

        // Get dimensions
        var width = _input.GetNumber("Please enter the package width:");
        var height = _input.GetNumber("Please enter the package height:");
        var length = _input.GetNumber("Please enter the package length:");

        // Validate dimensions
        var (dimensionsValid, dimensionsError) = _validator.ValidateDimensions(width, height, length);
        if (!dimensionsValid)
        {
            _display.ShowError(dimensionsError);
            return;
        }

        // Calculate and show quote
        var quote = _calculator.Calculate(weight, width, height, length);
        _display.ShowQuote(quote);
        _display.ShowMessage("Thank you!");
    }
}