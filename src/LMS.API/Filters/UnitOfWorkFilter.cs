using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
namespace LMS.API.Filters;

/// <summary>
/// Unit-of-work per request: services/repositories only stage changes in the DbContext;
/// this filter commits them with ONE SaveChanges (= one DB transaction) after a
/// successful write action (POST/PUT/PATCH/DELETE). If the action throws or returns
/// a 4xx/5xx result, nothing is saved.
/// </summary>
public class UnitOfWorkFilter : IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkFilter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();

        var method = context.HttpContext.Request.Method;
        var isWrite = HttpMethods.IsPost(method) || HttpMethods.IsPut(method)
                      || HttpMethods.IsPatch(method) || HttpMethods.IsDelete(method);
        if (!isWrite || executed.Exception != null && !executed.ExceptionHandled)
            return;

        var status = executed.Result is IStatusCodeActionResult sc ? sc.StatusCode ?? 200 : 200;
        if (status >= 400)
            return;

        await _unitOfWork.SaveChangesAsync(context.HttpContext.RequestAborted);
    }
}
