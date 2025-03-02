using System;

namespace BuchShop.Framework.Controls
{
    /// <summary>
    /// Schnittstelle für Validierungsregeln.
    /// </summary>
    public interface IValidationRule<T>
    {
        bool Validate(T value);
        string ErrorMessage { get; }
    }

    /// <summary>
    /// Abstrakte Basisklasse für Validierungsregeln.
    /// </summary>
    public abstract class ValidationRule<T> : IValidationRule<T>
    {
        public abstract bool Validate(T value);
        public abstract string ErrorMessage { get; }
    }

    /// <summary>
    /// Validierungsregel, die prüft, ob ein String nicht leer oder nur aus Leerzeichen besteht.
    /// </summary>
    public class NotEmptyRule : ValidationRule<string>
    {
        public override bool Validate(string value) => !string.IsNullOrWhiteSpace(value);
        public override string ErrorMessage => "Der Wert darf nicht leer sein.";
    }

    /// <summary>
    /// Validierungsregel, die einen ganzzahligen Wert innerhalb eines bestimmten Bereichs prüft.
    /// </summary>
    public class RangeRule : ValidationRule<int>
    {
        private readonly int _min;
        private readonly int _max;

        public RangeRule(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public override bool Validate(int value) => value >= _min && value <= _max;
        public override string ErrorMessage => $"Der Wert muss zwischen {_min} und {_max} liegen.";
    }

    /// <summary>
    /// Validierungsregel, die sicherstellt, dass ein Datum zwischen dem 01.01.2020 und dem aktuellen Datum liegt.
    /// </summary>
    public class DateRangeRule : ValidationRule<DateTime>
    {
        private readonly DateTime _min;
        private readonly DateTime _max;

        public DateRangeRule()
        {
            _min = new DateTime(2020, 1, 1);
            _max = DateTime.Now;
        }

        public override bool Validate(DateTime value)
        {
            return value >= _min && value <= _max;
        }

        public override string ErrorMessage => $"Das Datum muss zwischen {_min:dd.MM.yyyy} und {_max:dd.MM.yyyy} liegen.";
    }
}
