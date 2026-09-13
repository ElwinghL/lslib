using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LSLib.LS;
using LSLib.LS.Enums;

namespace Divine.CLI;

internal class CommandLineActions
{
    public static string SourcePath;
    public static string DestinationPath;
    public static string PackagedFilePath;

    public static Game Game;
    public static LogLevel LogLevel;
    public static ResourceFormat InputFormat;
    public static ResourceFormat OutputFormat;
    public static PackageVersion PackageVersion;
    public static int PackagePriority;
    public static bool LegacyGuids;

    // BG3Tools fork (fix/LSLibForkBuildLinux): ConformPath, VTConfigPath, VTRootPath,
    // FastBuild, VTValidate and GR2Options removed — they only backed the now-removed
    // convert-model[s]/build-vt actions (GR2 mesh conversion, virtual-texture tileset
    // building), out of scope for this .pak-only fork. See CommandLineArguments.cs.

    // TODO: OSI support

    public static void Run(CommandLineArguments args)
    {
        SetUpAndValidate(args);
        Process(args);
    }

    private static void SetUpAndValidate(CommandLineArguments args)
    {
        // BG3Tools fork (fix/LSLibForkBuildLinux): upstream's "convert-models" was the
        // GR2 mesh batch action (disambiguated from the resource batch action via the
        // graphicsActions array below); it, "convert-model" and "build-vt" no longer
        // exist as valid --action values (see CommandLineArguments.cs), so the
        // graphicsActions array and the GR2Options/ConformPath/VTConfigPath/VTRootPath
        // setup below were removed. Only "convert-resources" (LSB/LSF/LSJ/LSX batch
        // conversion) remains a batch action here.
        string[] batchActions =
        {
            "extract-packages",
            "convert-resources"
        };

        string[] packageActions =
        {
            "create-package",
            "list-package",
            "extract-single-file",
            "extract-package",
            "extract-packages"
        };

        LogLevel = CommandLineArguments.GetLogLevelByString(args.LogLevel);
        CommandLineLogger.LogDebug($"Using log level: {LogLevel}");

        Game = CommandLineArguments.GetGameByString(args.Game);
        CommandLineLogger.LogDebug($"Using game: {Game}");

        LegacyGuids = args.LegacyGuids;

        if (batchActions.Any(args.Action.Contains))
        {
            if (args.InputFormat == null || args.OutputFormat == null)
            {
                if (args.InputFormat == null && args.Action != "extract-packages")
                {
                    CommandLineLogger.LogFatal("Cannot perform batch action without --input-format and --output-format arguments", 1);
                }
            }

            if (args.Action != "extract-packages")
            {
                // "extract-packages" (BatchExtract) filtre les fichiers sources par
                // extension brute (Args.InputFormat, ex: "pak") sans jamais passer par
                // l'enum ResourceFormat, qui ne connaît que les formats de conversion de
                // ressources (LSX/LSB/LSF/LSJ) — "pak" n'y figure pas et n'a donc jamais
                // pu être résolu ici, provoquant une ArgumentException non gérée (donc un
                // crash CLR complet) dès qu'on tentait un extract-packages sur des .pak.
                InputFormat = CommandLineArguments.GetResourceFormatByString(args.InputFormat);
                CommandLineLogger.LogDebug($"Using input format: {InputFormat}");

                OutputFormat = CommandLineArguments.GetResourceFormatByString(args.OutputFormat);
                CommandLineLogger.LogDebug($"Using output format: {OutputFormat}");
            }
        }

        if (args.Action == "create-package")
        {
            PackagePriority = args.PackagePriority;
            PackageVersion = Game.PAKVersion();
            CommandLineLogger.LogDebug($"Using package version: {PackageVersion}");
        }

        SourcePath = TryToValidatePath(args.Source);
        if (args.Action != "list-package")
        {
            DestinationPath = TryToValidatePath(args.Destination);
        }
        if (args.Action == "extract-single-file")
        {
            PackagedFilePath = args.PackagedPath;
        }
    }

    private static void Process(CommandLineArguments args)
    {
        Func<PackagedFileInfo, bool> filter;

        if (args.Expression != null)
        {
            Regex expression = null;
            if (args.UseRegex)
            {
                try
                {
                    expression = new Regex(args.Expression, RegexOptions.Singleline | RegexOptions.Compiled);
                }
                catch (ArgumentException)
                {
                    CommandLineLogger.LogFatal($"Cannot parse RegEx expression: {args.Expression}", -1);
                }
            }
            else
            {
                expression = new Regex("^" + Regex.Escape(args.Expression).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.Singleline | RegexOptions.Compiled);
            }

            filter = obj => obj.Name.Like(expression);
        }
        else
        {
            filter = obj => true;
        }
        
	        switch (args.Action)
        {
            case "create-package":
            {
                CommandLinePackageProcessor.Create();
                break;
            }

            case "extract-package":
            {
                CommandLinePackageProcessor.Extract(filter);
                break;
            }

            case "extract-single-file":
            {
                CommandLinePackageProcessor.ExtractSingleFile();
                break;
            }

            case "list-package":
            {
                CommandLinePackageProcessor.ListFiles(filter);
                break;
            }

            // "convert-model" (GR2 mesh conversion) removed — see CommandLineArguments.cs.

            case "convert-resource":
            {
                CommandLineDataProcessor.Convert();
                break;
                }

            case "convert-loca":
            {
                CommandLineDataProcessor.ConvertLoca();
                break;
            }

            case "extract-packages":
            {
                CommandLinePackageProcessor.BatchExtract(filter);
                break;
            }

            // "convert-models" (GR2 batch mesh conversion) and "build-vt" (virtual
            // texture tileset building) removed — see CommandLineArguments.cs.

            case "convert-resources":
            {
                CommandLineDataProcessor.BatchConvert();
                break;
            }

            default:
            {
                throw new ArgumentException($"Unhandled action: {args.Action}");
            }
        }
    }

    public static string TryToValidatePath(string path)
    {
        CommandLineLogger.LogDebug($"Using path: {path}");

        if (string.IsNullOrWhiteSpace(path))
        {
            CommandLineLogger.LogFatal($"Cannot parse path from input: {path}", 1);
        }

        // BG3Tools Linux patch: the original Uri-based check below assumed Windows path semantics
        // (Uri.TryCreate(path, RelativeOrAbsolute) only recognizes a Windows drive/UNC path or an
        // explicit "file://" URI as IsFile; a Unix absolute path like "/tmp/x" is parsed as a
        // RELATIVE Uri instead, so uri.IsFile then throws InvalidOperationException even for a
        // perfectly valid absolute path). Path.IsPathRooted(path) alone is the correct,
        // cross-platform test for "is this already an absolute path" and is what Path.GetFullPath
        // below actually relies on, so the Uri detour is dropped entirely rather than worked around.
        if (!Path.IsPathRooted(path))
        {
            CommandLineLogger.LogFatal($"Cannot proceed without absolute path [E2]: {path}", 1);
        }

        // ReSharper disable once AssignNullToNotNullAttribute
        path = Path.GetFullPath(path);

        return path;
    }
}
