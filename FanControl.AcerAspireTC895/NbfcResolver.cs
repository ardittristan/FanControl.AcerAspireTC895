using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SPath = System.IO.Path;

namespace FanControl.AcerAspireTC895;

internal static class NbfcResolver
{
    private static string? _path;

    public static string Path => _path ??=
        Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\Folders")
            ?.GetValueNames()
            .FirstOrDefault(p => p.EndsWith(@"\NoteBook FanControl\"))
        ?? DefaultPath;

    private const string DefaultPath = @"C:\Program Files (x86)\NoteBook FanControl";

    public static void Setup(AppDomain appDomain) =>
        appDomain.AssemblyResolve += NbfcLoader;

    private static Assembly? NbfcLoader(object sender, ResolveEventArgs args)
    {
        string assemblyPath = SPath.Combine(Path, new AssemblyName(args.Name).Name + ".dll");
        if (File.Exists(assemblyPath))
            return Assembly.LoadFrom(assemblyPath);

        assemblyPath = SPath.Combine(Path, "Plugins", new AssemblyName(args.Name).Name + ".dll");

        if (File.Exists(assemblyPath))
            return Assembly.LoadFrom(assemblyPath);

        return null;
    }
}