using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ResultExtensions
    {

        public static T GetValueOrDefault<T>(this Result<T> value, T defaultValue) => value.IsSuccess ? value.Value : defaultValue;
        public static Maybe<T> AsMaybeIfSuccessful<T>(this Result<T> value)
        {
            return value.IsSuccess ? value.Value : Maybe.None;
        }
    }
}
