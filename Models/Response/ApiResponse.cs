using System.Diagnostics.CodeAnalysis;

namespace CodeChallenge.Models.Response
{
    public class ApiResponse<T>
    {
        public string message {get; set;}
        #nullable enable
        public object? errors {get; set;}
        #nullable disable
        [MaybeNull, AllowNull]
        public T data {get; set;}
    }
    public class ApiResponse : ApiResponse<object> { }
}