namespace KShop.ProductApi.Application.Abstractions;

public sealed class UnrecognizedUserException(string? sub) : Exception($"User '{sub}' is not recognized by the user service");
