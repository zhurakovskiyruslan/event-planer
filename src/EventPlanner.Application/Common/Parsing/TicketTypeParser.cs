using EventPlanner.Data.Enums;
using EventPlanner.Application.Common.Exceptions;

namespace EventPlanner.Application.Common.Parsing;

public class TicketTypeParser
{
    public static TicketType ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Ticket type is required");

        if (Enum.TryParse<TicketType>(value.Trim(), ignoreCase: true, out var parsed))
            return parsed;

        throw new ValidationException($"Invalid ticket type '{value}'. " +
                                      $"Allowed values: {string.Join(", ", 
                                          Enum.GetNames(typeof(TicketType)))}");
    }
}