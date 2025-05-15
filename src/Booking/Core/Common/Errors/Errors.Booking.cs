using ErrorOr;

namespace Booking.Common.Errors;

public static partial class Errors
{
    public static class Booking
    {
        public static Error NotFound => Error.NotFound(
            code: "Booking.NotFound",
            description: "La reserva especificada no fue encontrada.");

        public static Error AlreadyExists => Error.Conflict(
            code: "Booking.AlreadyExists",
            description: "Ya existe una reserva con los detalles especificados.");

        public static Error InvalidStatus => Error.Validation(
            code: "Booking.InvalidStatus",
            description: "El estado de la reserva especificada no es válido.");

        public static Error PaymentRequired => Error.Failure(
            code: "Booking.PaymentRequired",
            description: "Se requiere un pago para confirmar la reserva.");

        public static Error NotAuthorized => Error.Unauthorized(
            code: "Booking.NotAuthorized",
            description: "No estás autorizado para realizar esta acción sobre la reserva.");
    }
}
