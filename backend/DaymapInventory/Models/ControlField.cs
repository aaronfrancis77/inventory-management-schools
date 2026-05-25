namespace DaymapInventory.Models
{
    public class TextField : Control
    {
        public int? MaxLength { get; set; }

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            if (MaxLength.HasValue && value.Length > MaxLength.Value) return false;
            return true;
        }
    }

    public class NumberField : Control
    {
        public double? Min { get; set; }
        public double? Max { get; set; }

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            if (!double.TryParse(value, out double parsed)) return false;
            if (Min.HasValue && parsed < Min.Value) return false;
            if (Max.HasValue && parsed > Max.Value) return false;
            return true;
        }
    }

    public class DateField : Control
    {
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            if (!DateTime.TryParse(value, out DateTime parsed)) return false;
            if (MinDate.HasValue && parsed < MinDate.Value) return false;
            if (MaxDate.HasValue && parsed > MaxDate.Value) return false;
            return true;
        }
    }

    public class DateTimeField : Control
    {
        public DateTime? MinDateTime { get; set; }
        public DateTime? MaxDateTime { get; set; }

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            if (!DateTime.TryParse(value, out DateTime parsed)) return false;
            if (MinDateTime.HasValue && parsed < MinDateTime.Value) return false;
            if (MaxDateTime.HasValue && parsed > MaxDateTime.Value) return false;
            return true;
        }
    }

    public class TimeField : Control
    {
        public TimeSpan? MinTime { get; set; }
        public TimeSpan? MaxTime { get; set; }

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            if (!TimeSpan.TryParse(value, out TimeSpan parsed)) return false;
            if (MinTime.HasValue && parsed < MinTime.Value) return false;
            if (MaxTime.HasValue && parsed > MaxTime.Value) return false;
            return true;
        }
    }

    public class DropdownField : Control
    {
        public List<string> Options { get; set; } = new();

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            return Options.Contains(value);
        }
    }

    public class AutocompleteField : Control
    {
        public List<string> Suggestions { get; set; } = new();

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            return true;
        }
    }

    public class RadioField : Control
    {
        public List<string> Options { get; set; } = new();

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            return Options.Contains(value);
        }
    }

    public class CheckboxField : Control
    {
        public override bool Validate(string? value)
        {
            if (value == null) return false;
            return value == "true" || value == "false";
        }
    }

    public class SliderField : Control
    {
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 100;
        public double Step { get; set; } = 1;

        public override bool Validate(string? value)
        {
            if (value == null) return false;
            if (!double.TryParse(value, out double parsed)) return false;
            if (parsed < Min || parsed > Max) return false;
            return true;
        }
    }

    public class LabelField : Control
    {
        public override bool Validate(string? value)
        {
            return true;
        }
    }
}