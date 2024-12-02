using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Maui.Storage;

namespace CalculatorApp
{
    public partial class MainPage : ContentPage
    {
        private string _currentInput = "";
        private string _currentOperator = "";
        private double _previousValue;
        public ObservableCollection<string> History { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            LoadHistory();
            BindingContext = this;
        }

        private void OnNumberClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                _currentInput += button.Text;
                InputField.Text = _currentInput;
            }
        }

        private void OnOperatorClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                if (double.TryParse(_currentInput, out double value))
                {
                    _previousValue = value;
                    _currentInput = "";
                    _currentOperator = button.Text;
                }
            }
        }

        private void OnEqualsClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput) && double.TryParse(_currentInput, out double currentValue))
            {
                double result = 0;

                switch (_currentOperator)
                {
                    case "+":
                        result = _previousValue + currentValue;
                        break;
                    case "-":
                        result = _previousValue - currentValue;
                        break;
                    case "*":
                        result = _previousValue * currentValue;
                        break;
                    case "/":
                        result = currentValue != 0 ? _previousValue / currentValue : double.NaN;
                        break;
                }

                var calculation = $"{_previousValue} {_currentOperator} {currentValue} = {result}";
                History.Add(calculation);
                SaveHistory();

                InputField.Text = result.ToString();
                _currentInput = result.ToString();
                _currentOperator = "";
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            _currentInput = "";
            _currentOperator = "";
            _previousValue = 0;
            InputField.Text = "";
        }

        private void LoadHistory()
        {
            var savedHistory = Preferences.Get("CalculationHistory", "");
            if (!string.IsNullOrEmpty(savedHistory))
            {
                var historyItems = savedHistory.Split('|');
                foreach (var item in historyItems)
                {
                    if (!string.IsNullOrEmpty(item))
                        History.Add(item);
                }
            }
        }

        private void SaveHistory()
        {
            var historyString = string.Join("|", History);
            Preferences.Set("CalculationHistory", historyString);
        }
    }
}
