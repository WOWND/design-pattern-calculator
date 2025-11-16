namespace DesignPatternCalculator.Memento;


/// 메멘토를 관리하는 Caretaker
/// Undo/Redo 기능 제공
public class CalculatorCaretaker
{
    private readonly Stack<CalculatorMemento> _undoStack;
    private readonly Stack<CalculatorMemento> _redoStack;
    private readonly int _maxHistorySize;

    public CalculatorCaretaker(int maxHistorySize = 100)
    {
        _undoStack = new Stack<CalculatorMemento>();
        _redoStack = new Stack<CalculatorMemento>();
        _maxHistorySize = maxHistorySize;
    }

    public void SaveState(CalculatorMemento memento)
    {
        _undoStack.Push(memento);
        _redoStack.Clear(); // 새로 저장 시 Redo 초기화

        // 최대 크기 초과 시 오래된 항목 제거
        if (_undoStack.Count > _maxHistorySize)
        {
            var tempStack = new Stack<CalculatorMemento>();
            for (int i = 0; i < _maxHistorySize; i++)
            {
                tempStack.Push(_undoStack.Pop());
            }
            _undoStack.Clear();
            while (tempStack.Count > 0)
            {
                _undoStack.Push(tempStack.Pop());
            }
        }
    }

    public CalculatorMemento? Undo()
    {
        if (_undoStack.Count == 0)
            return null;

        var memento = _undoStack.Pop();
        _redoStack.Push(memento);
        return _undoStack.Count > 0 ? _undoStack.Peek() : null;
    }

    public CalculatorMemento? Redo()
    {
        if (_redoStack.Count == 0)
            return null;

        var memento = _redoStack.Pop();
        _undoStack.Push(memento);
        return memento;
    }

    public bool CanUndo => _undoStack.Count > 0;

    public bool CanRedo => _redoStack.Count > 0;

    public CalculatorMemento? GetCurrentState()
    {
        return _undoStack.Count > 0 ? _undoStack.Peek() : null;
    }

    public IEnumerable<CalculatorMemento> GetHistory()
    {
        return _undoStack.Reverse().ToList();
    }

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }
}
