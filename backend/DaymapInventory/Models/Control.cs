using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public enum ControlType
    {
        Label,
        Text,
        Number,
        Date,
        DateAndTime,
        Time,
        Dropdown,
        Autocomplete,
        Radio,
        Checkbox,
        Slider
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "controlType")]
    [JsonDerivedType(typeof(TextField), "Text")]
    [JsonDerivedType(typeof(NumberField), "Number")]
    [JsonDerivedType(typeof(DateField), "Date")]
    [JsonDerivedType(typeof(DateTimeField), "DateAndTime")]
    [JsonDerivedType(typeof(TimeField), "Time")]
    [JsonDerivedType(typeof(DropdownField), "Dropdown")]
    [JsonDerivedType(typeof(AutocompleteField), "Autocomplete")]
    [JsonDerivedType(typeof(RadioField), "Radio")]
    [JsonDerivedType(typeof(CheckboxField), "Checkbox")]
    [JsonDerivedType(typeof(SliderField), "Slider")]
    [JsonDerivedType(typeof(LabelField), "Label")]
    public abstract class Control
    {
        public ControlType ControlType { get; set; }
        public string? Value { get; set; }

        public abstract bool Validate(string? value);
    }
}