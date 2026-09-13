using System;
using System.Collections.Generic;
using CommandLineParser.Arguments;
using LSLib.LS;
using LSLib.LS.Enums;

namespace Divine.CLI;

public class CommandLineArguments
{
    // @formatter:off
    [EnumeratedValueArgument(typeof(string), 'l', "loglevel",
        Description = "Set verbosity level of log output",
        DefaultValue = "info",
        AllowedValues = "off;fatal;error;warn;info;debug;trace;all",
        ValueOptional = false,
        Optional = true
    )]
    public string LogLevel;

    // @formatter:off
    [EnumeratedValueArgument(typeof(string), 'g', "game",
        Description = "Set target game when generating output",
        DefaultValue = null,
        AllowedValues = "dos;dosee;dos2;dos2de;bg3",
        ValueOptional = false,
        Optional = false
    )]
    public string Game;

    // @formatter:off
    [ValueArgument(typeof(string), 's', "source",
        Description = "Set source file path or directory",
        DefaultValue = null,
        ValueOptional = false,
        Optional = false
    )]
    public string Source;

    // @formatter:off
    [ValueArgument(typeof(string), 'd', "destination",
        Description = "Set destination file path or directory",
        DefaultValue = null,
        ValueOptional = true,
        Optional = true
    )]
    public string Destination;

    // @formatter:off
    [ValueArgument(typeof(string), 'f', "packaged-path",
        Description = "File to extract from package",
        DefaultValue = null,
        ValueOptional = true,
        Optional = true
    )]
    public string PackagedPath;

    // @formatter:off
    // BG3Tools fork (fix/LSLibForkBuildLinux): dropped "dae;glb;gltf;gr2" (3D
    // model formats, convert-model[s] only) and "lsv" (savegames) from the
    // allowed values below — both are out of scope for a .pak-only fork and
    // their backing code (Granny/GR2, LS/Save) is excluded from LSLib.csproj.
    [EnumeratedValueArgument(typeof(string), 'i', "input-format",
        Description = "Set input format for batch operations",
        DefaultValue = null,
        AllowedValues = "pak;lsj;lsx;lsb;lsf",
        ValueOptional = false,
        Optional = true
    )]
    public string InputFormat;

    // @formatter:off
    [EnumeratedValueArgument(typeof(string), 'o', "output-format",
        Description = "Set output format for batch operations",
        DefaultValue = null,
        AllowedValues = "pak;lsj;lsx;lsb;lsf",
        ValueOptional = false,
        Optional = true
    )]
    public string OutputFormat;

    // BG3Tools fork (fix/LSLibForkBuildLinux): dropped "convert-model",
    // "convert-models" (GR2 mesh/animation conversion — needs the native,
    // MSVC-only LSLibNative) and "build-vt" (virtual texture tileset
    // building — unrelated to .pak handling) from the allowed actions.
    [EnumeratedValueArgument(typeof(string), 'a', "action",
        Description = "Set action to execute",
        DefaultValue = "extract-package",
        AllowedValues = "create-package;list-package;extract-single-file;extract-package;extract-packages;convert-resource;convert-resources;convert-loca",
        ValueOptional = false,
        Optional = false
    )]
    public string Action;

    // @formatter:off
    [EnumeratedValueArgument(typeof(string), 'c', "compression-method",
        Description = "Set compression method",
        DefaultValue = "lz4hc",
        AllowedValues = "zlib;zlibfast;lz4;lz4hc;none",
        ValueOptional = false,
        Optional = true
    )]
    public string PakCompressionMethod;

		// @formatter:off
		[ValueArgument(typeof(string), 'x', "expression",
        Description = "Set glob expression for extract and list actions",
        DefaultValue = "*",
        ValueOptional = false,
        Optional = true
    )]
    public string Expression;

    // BG3Tools fork (fix/LSLibForkBuildLinux): "gr2-options" (-e), "conform-path",
    // "fast-build", "vt-validate" and "vt-root" arguments removed below — they only
    // configured the now-removed convert-model[s]/build-vt actions (GR2 mesh
    // conversion and virtual-texture tileset building), out of scope for this
    // .pak-only fork. See CommandLineActions.cs and CommandLineDataProcessor.cs.

    // @formatter:off
    [ValueArgument(typeof(int), "package-priority",
        Description = "Set a custom package priority",
        DefaultValue = 0,
        ValueOptional = true,
        Optional = true
    )]
    public int PackagePriority;

    // @formatter:off
    [SwitchArgument("legacy-guids", false,
        Description = "Use legacy GUID serialization format when serializing LSX/LSJ files",
        Optional = true
    )]
    public bool LegacyGuids;

    // @formatter:off
    [SwitchArgument("use-package-name", false,
        Description = "Use package name for destination folder",
        Optional = true
    )]
    public bool UsePackageName;

	// @formatter:off
    [SwitchArgument("use-regex", false,
        Description = "Use Regular Expressions for expression type",
        Optional = true
    )]
    public bool UseRegex;

    // @formatter:on
    public static LogLevel GetLogLevelByString(string logLevel)
    {
        switch (logLevel)
        {
            case "off":
            {
                return LSLib.LS.Enums.LogLevel.OFF;
            }
            case "fatal":
            {
                return LSLib.LS.Enums.LogLevel.FATAL;
            }
            case "error":
            {
                return LSLib.LS.Enums.LogLevel.ERROR;
            }
            case "warn":
            {
                return LSLib.LS.Enums.LogLevel.WARN;
            }
            case "info":
            {
                return LSLib.LS.Enums.LogLevel.INFO;
            }
            case "debug":
            {
                return LSLib.LS.Enums.LogLevel.DEBUG;
            }
            case "trace":
            {
                return LSLib.LS.Enums.LogLevel.TRACE;
            }
            case "all":
            {
                return LSLib.LS.Enums.LogLevel.ALL;
            }
            default:
            {
                return LSLib.LS.Enums.LogLevel.INFO;
            }
        }
    }

    // ReSharper disable once RedundantCaseLabel
    public static Game GetGameByString(string game)
    {
        switch (game)
        {
            case "bg3":
            {
                return LSLib.LS.Enums.Game.BaldursGate3;
            }
            case "dos":
            {
                return LSLib.LS.Enums.Game.DivinityOriginalSin;
            }
            case "dosee":
            {
                return LSLib.LS.Enums.Game.DivinityOriginalSinEE;
            }
            case "dos2":
            {
                return LSLib.LS.Enums.Game.DivinityOriginalSin2;
            }
            case "dos2de":
            {
                return LSLib.LS.Enums.Game.DivinityOriginalSin2DE;
             }
            case "unset":
            {
                return LSLib.LS.Enums.Game.Unset;
            }
            default:
            {
                throw new ArgumentException($"Unknown game: \"{game}\"");
            }
        }
    }

    // ReSharper disable once RedundantCaseLabel
    public static ResourceFormat GetResourceFormatByString(string resourceFormat)
    {
        switch (resourceFormat)
        {
            case "lsb":
            {
                return ResourceFormat.LSB;
            }
            case "lsf":
            {
                return ResourceFormat.LSF;
            }
            case "lsj":
            {
                return ResourceFormat.LSJ;
            }
            case "lsx":
            {
                return ResourceFormat.LSX;
            }
            default:
            {
                throw new ArgumentException($"Unknown resource format: \"{resourceFormat}\"");
            }
        }
    }

    public static Dictionary<string, object> GetCompressionOptions(string compressionOption, PackageVersion packageVersion)
    {
        CompressionMethod compression;
        LSCompressionLevel level;

        switch (compressionOption)
        {
            case "zlibfast":
            {
                compression = CompressionMethod.Zlib;
                level = LSCompressionLevel.Fast;
                break;
            }

            case "zlib":
            {
                compression = CompressionMethod.Zlib;
                level = LSCompressionLevel.Default;
                break;
            }

            case "lz4":
            {
                compression = CompressionMethod.LZ4;
                level = LSCompressionLevel.Fast;
                break;
            }

            case "lz4hc":
            {
                compression = CompressionMethod.LZ4;
                level = LSCompressionLevel.Default;
                break;
            }

            // ReSharper disable once RedundantCaseLabel
            case "none":
            default:
            {
                compression = CompressionMethod.None;
                level = LSCompressionLevel.Default;
                break;
            }
        }

        // fallback to zlib, if the package version doesn't support lz4
        if (compression == CompressionMethod.LZ4 && packageVersion <= PackageVersion.V9)
        {
            compression = CompressionMethod.Zlib;
            level = LSCompressionLevel.Default;
        }

        var compressionOptions = new Dictionary<string, object>
        {
            { "Compression", compression },
            { "CompressionLevel", level }
        };

        return compressionOptions;
    }

    // GetGR2Options() removed (BG3Tools fork, fix/LSLibForkBuildLinux): only used to
    // configure the removed convert-model[s] action's GR2 mesh conversion. See the
    // removed "gr2-options" argument above and CommandLineGR2Processor.cs (excluded
    // from Divine.csproj).
}
