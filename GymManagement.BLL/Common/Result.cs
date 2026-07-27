using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Common
{
    public sealed record Result(bool success, string? error = null, Status status = Status.Ok )
    {
        // For services with Void return
        public static Result Ok() => new(success: true);
        public static Result Fail(string error, Status status = Status.Conflict) => new(false, error, status);
        public static Result NotFound(string error = "Not Found") => new(false, error, Status.NotFound);
        public static Result Validation(string error) => new(false, error, Status.ValidationFailed);
        
    }

    public sealed record Result<T>(bool success, T? value ,string? error = null, Status status = Status.Ok)
    {
        // For services with Value return
        public static Result<T> Ok(T value) => new(success: true, value);
        public static Result<T> Fail(string error, Status status = Status.Conflict) => new(false, default ,error, status);
        public static Result<T> NotFound(string error = "Not Found") => new(false, default ,error, Status.NotFound);

    }

}
