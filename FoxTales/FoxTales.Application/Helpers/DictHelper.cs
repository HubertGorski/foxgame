namespace FoxTales.Application.Helpers;

public static class DictHelper
{
    public static class Validation
    {
        public const string EmailCannotBeEmpty = "emailCannotBeEmpty";
        public const string EmailFormatIsIncorrect = "emailFormatIsIncorrect";
        public const string EmailIsAlreadyTaken = "emailIsAlreadyTaken";
        public const string UsernameCannotBeEmpty = "usernameCannotBeEmpty";
        public const string UsernameFormatIsIncorrect = "usernameFormatIsIncorrect";
        public const string UsernameIsAlreadyTaken = "usernameIsAlreadyTaken";
        public const string PasswordIsTooShort = "passwordIsTooShort";
        public const string PasswordsAreNotIdentical = "passwordsAreNotIdentical";
        public const string InvalidEmailOrPassword = "invalidEmailOrPassword";
        public const string UnexpectedError = "unexpectedError";
        public const string YouAreAlreadyAuthenticated = "youAreAlreadyAuthenticated";
    }
}
