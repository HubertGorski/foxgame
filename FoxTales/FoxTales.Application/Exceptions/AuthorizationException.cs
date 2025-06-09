namespace FoxTales.Application.Exceptions;

public class AuthorizationException(string message) : Exception(message)
{
}