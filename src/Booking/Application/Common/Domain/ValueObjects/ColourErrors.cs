using ErrorOr;

namespace Booking.Booking.Application.Common.Domain.ValueObjects;

public static class ColourErrors
{
    public static Error UnsupportedColour(string code) =>
        Error.Validation(
            code: "ColourErrors.UnsupportedColour",
            description: $"The colour code '{code}' is not supported.");
}