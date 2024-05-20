using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Zine.App.FileHelpers;

public static class FileSelector
{
    public static Task<string?> GetFilePathFromFileDialog()
    {
        return GetPathFromFileDialog([OpenDialogProperty.openFile]);
    }

    public static Task<string?> GetDirectoryPathFromFileDialog()
    {
        return GetPathFromFileDialog([OpenDialogProperty.openFile, OpenDialogProperty.openDirectory]);
    }

    private static async Task<string?> GetPathFromFileDialog(OpenDialogProperty[] properties)
    {
        var mainWindow = Electron.WindowManager.BrowserWindows.First();
        var options = new OpenDialogOptions
        {
            Properties = properties
        };

        //The ShowOpenDialogAsync can optionally return multiple file paths, but currently we only allow single file/directory selections
        string[] filesPaths = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);
        return filesPaths.Length != 0
            ? filesPaths.First()
            : null;
    }

}
