namespace myApp.Exceptions;

public sealed class FileValidationException(string message) : Exception(message);