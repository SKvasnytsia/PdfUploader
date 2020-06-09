using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Linq;

namespace PDFUploader.API.CrossCutting
{
    public class AllowedExtensionsAttribute : ActionFilterAttribute
    {
        private const string ERROR_MESSAGE = "This extension is not allowed!";

        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var file = context.ActionArguments.Any(x => x.Key == "file") ? context.ActionArguments["file"] as IFormFile : null;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    context.Result = new BadRequestObjectResult(ERROR_MESSAGE);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
