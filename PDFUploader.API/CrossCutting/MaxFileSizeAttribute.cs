using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace PDFUploader.API.CrossCutting
{
    public class MaxFileSizeAttribute : ActionFilterAttribute
    {
        private string ERROR_MESSAGE => $"Maximum allowed file size is { _maxFileSize} bytes.";

        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var file = context.ActionArguments.Any(x => x.Key == "file") ? context.ActionArguments["file"] as IFormFile : null;
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    context.Result = new BadRequestObjectResult(ERROR_MESSAGE);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
