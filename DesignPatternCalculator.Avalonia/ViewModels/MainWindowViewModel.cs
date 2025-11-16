using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignPatternCalculator.Calculator.Facade;

namespace DesignPatternCalculator.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly CalculatorFacade _calculator;

    [ObservableProperty]
    private string _expression = "";

    [ObservableProperty]
    private string _result = "0";

    [ObservableProperty]
    private bool _isLoggingEnabled = false;

    [ObservableProperty]
    private bool _canUndo = false;

    [ObservableProperty]
    private bool _canRedo = false;

    public ObservableCollection<string> History { get; } = new();

    public MainWindowViewModel()
    {
        _calculator = new CalculatorFacade();
    }

    partial void OnIsLoggingEnabledChanged(bool value)
    {
        _calculator.SetLogging(value);
    }

    [RelayCommand]
    private void NumberClick(string number)
    {
        Expression += number;
    }

    [RelayCommand]
    private void OperatorClick(string op)
    {
        Expression += op;
    }

    [RelayCommand]
    private void FunctionClick(string func)
    {
        Expression += func + "(";
    }

    [RelayCommand]
    private void ParenthesisClick(string p)
    {
        Expression += p;
    }

    [RelayCommand]
    private void DecimalClick()
    {
        Expression += ".";
    }

    [RelayCommand]
    private void Calculate()
    {
        if (string.IsNullOrEmpty(Expression))
            return;

        try
        {
            _calculator.Clear();
            double resultValue = _calculator.CalculateExpression(Expression);

            string historyEntry = $"{Expression} = {resultValue}";
            History.Insert(0, historyEntry);

            Result = resultValue.ToString();
            Expression = "";

            UpdateUndoRedoState();
        }
        catch (Exception ex)
        {
            Result = "Error";
            History.Insert(0, $"Error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Expression = "";
        Result = "0";
    }

    [RelayCommand]
    private void Backspace()
    {
        if (Expression.Length > 0)
        {
            Expression = Expression.Substring(0, Expression.Length - 1);
        }
    }

    [RelayCommand]
    private void ToggleSign()
    {
        if (string.IsNullOrEmpty(Expression) || Expression.EndsWith("(") ||
            Expression.EndsWith("+") || Expression.EndsWith("-") ||
            Expression.EndsWith("*") || Expression.EndsWith("/") ||
            Expression.EndsWith("^") || Expression.EndsWith("%"))
        {
            Expression += "-";
        }
        else
        {
            // Try to toggle the last number's sign
            int lastOpIndex = -1;
            for (int i = Expression.Length - 1; i >= 0; i--)
            {
                char c = Expression[i];
                if (c == '+' || c == '*' || c == '/' || c == '^' || c == '%' || c == '(')
                {
                    lastOpIndex = i;
                    break;
                }
                if (c == '-' && i > 0 && !char.IsDigit(Expression[i - 1]) && Expression[i - 1] != '.')
                {
                    lastOpIndex = i;
                    break;
                }
            }

            if (lastOpIndex == -1)
            {
                // Toggle at start
                if (Expression.StartsWith("-"))
                    Expression = Expression.Substring(1);
                else
                    Expression = "-" + Expression;
            }
            else if (Expression[lastOpIndex] == '-' && lastOpIndex > 0 &&
                     (Expression[lastOpIndex - 1] == '+' || Expression[lastOpIndex - 1] == '*' ||
                      Expression[lastOpIndex - 1] == '/' || Expression[lastOpIndex - 1] == '^' ||
                      Expression[lastOpIndex - 1] == '%' || Expression[lastOpIndex - 1] == '('))
            {
                // Remove the minus
                Expression = Expression.Remove(lastOpIndex, 1);
            }
            else
            {
                // Add minus after operator
                Expression = Expression.Insert(lastOpIndex + 1, "-");
            }
        }
    }

    [RelayCommand]
    private void ClearHistory()
    {
        History.Clear();
        _calculator.ClearHistory();
        UpdateUndoRedoState();
    }

    [RelayCommand]
    private void ToggleLogging()
    {
        IsLoggingEnabled = !IsLoggingEnabled;
        _calculator.SetLogging(IsLoggingEnabled);
    }

    [RelayCommand]
    private void Undo()
    {
        var memento = _calculator.Undo();
        if (memento != null)
        {
            Result = memento.Result.ToString();
        }
        UpdateUndoRedoState();
    }

    [RelayCommand]
    private void Redo()
    {
        var memento = _calculator.Redo();
        if (memento != null)
        {
            Result = memento.Result.ToString();
        }
        UpdateUndoRedoState();
    }

    [RelayCommand]
    private void UseHistoryItem(string item)
    {
        var parts = item.Split(" = ");
        if (parts.Length >= 1)
        {
            Expression = parts[0];
        }
    }

    private void UpdateUndoRedoState()
    {
        CanUndo = _calculator.CanUndo;
        CanRedo = _calculator.CanRedo;
    }
}
