using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json.Serialization;

namespace DynamicRazorRender.Server.Filters
{
    public class ResponseResult<T>
    {
        public ResponseResult(bool success)
        {
            this.Success = success;
        }

        public ResponseResult(bool success, T data) : this(success)
        {
            this.Data = data;
        }

        [JsonPropertyName("success")]
        public bool Success { get; set; } = default!;

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    public class ApiResultWrapFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // base.OnResultExecuting(context);

            if (context.Result is ObjectResult _obj)
            {
                if (_obj.Value == null)
                {
                    context.Result = new ObjectResult(
                        new ResponseResult<object>(true)
                    );
                }
                else if (_obj.DeclaredType!.IsGenericType
                            && _obj.DeclaredType?.GetGenericTypeDefinition() == typeof(ResponseResult<>))
                {
                    return;
                }
                else
                {
                    context.Result = new ObjectResult(
                        new ResponseResult<object>(true, _obj.Value)
                    );
                }
            }
        }
    }
}