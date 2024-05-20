using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Zine.App.FileHelpers;

public static class FileSelector
{
    public static async Task<string?> GetDirectoryPathFromFileDialog()
    {
        var mainWindow = Electron.WindowManager.BrowserWindows.First();
        var options = new OpenDialogOptions
        {
            Properties =
            [
                OpenDialogProperty.openFile,
                OpenDialogProperty.openDirectory,
            ]
        };

        string[] files = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);
        return files.Length != 0
            ? files.First()
            : null;
    }
}
