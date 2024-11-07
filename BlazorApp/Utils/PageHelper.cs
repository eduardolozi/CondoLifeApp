using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorApp.Utils;

public class PageHelper
{
    private readonly IDialogService _dialogService;
    public PageHelper(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public Task ShowBusy(ref bool isLoading, Func<Task> action)
    {
        try
        {
            isLoading = true;
            return action();
        }
        finally
        {
            isLoading = false;
        }
    }
}