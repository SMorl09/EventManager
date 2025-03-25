using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Filters
{
    public class EventExceptionFilter:ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is NotFoundException || (context.Exception is Exception ex && ex.Message == "Event not found."))
            {
                context.Result = new NotFoundObjectResult(context.Exception.Message);
            }
            else
            {
                context.Result = new ObjectResult(new { Error = "An unexpected error occurred." })
                {
                    StatusCode = 500
                };
            }

            context.ExceptionHandled = true;
        }


    }
}
