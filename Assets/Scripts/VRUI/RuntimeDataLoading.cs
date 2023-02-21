using System;
using System.IO;
using UnityEngine;

using UnityVolumeRendering;
public class RuntimeDataLoading : MonoBehaviour
{
    public string currentDirectory = "";
    private Vector2 scrollPos = Vector2.zero;
    private Vector2 dirScrollPos = Vector2.zero;

    public void onButtonBackClicked()
    {
        Debug.LogWarning("======back button clicked=====");
        
        DirectoryInfo parentDir = Directory.GetParent(currentDirectory);
        if (parentDir != null)
            currentDirectory = parentDir.FullName;
        else
            currentDirectory = "";
        scrollPos = Vector2.zero;

        Debug.LogWarning("======current directory:=====" + currentDirectory);
    }
    public void onButtonDocumentsClicked()
    {
        Debug.LogWarning("======onButtonDocumentsClicked clicked=====");

        currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        scrollPos = Vector2.zero;
        updateCurrentDir();
        Debug.LogWarning("======current directory:=====" + currentDirectory);
    }
    public void onButtonDesktopClicked()
    {
        Debug.LogWarning("======onButtonDesktopClicked clicked=====");

        currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        scrollPos = Vector2.zero;
        updateCurrentDir();
        Debug.LogWarning("======current directory:=====" + currentDirectory);
    }
    private void updateCurrentDir()
    {
        foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
        {
            if (GUILayout.Button(driveInfo.Name))
            {
                currentDirectory = driveInfo.Name;
                scrollPos = Vector2.zero;
            }
        }
    }
}
