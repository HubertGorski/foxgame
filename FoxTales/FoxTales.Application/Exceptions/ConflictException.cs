namespace FoxTales.Application.Exceptions;

public class ConflictException(string fieldName, string fieldValue) : Exception($"User with {fieldName} '{fieldValue}' already exists")
{
    public string FieldName { get; } = fieldName;
    public string FieldValue { get; } = fieldValue;
}