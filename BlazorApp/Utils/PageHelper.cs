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
}