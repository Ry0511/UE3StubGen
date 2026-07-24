# UE3 Stub Generator

Generates Python type stubs (`.pyi`) from decompressed Unreal Engine 3 script packages for use
with [`pyunrealsdk`](https://github.com/bl-sdk/pyunrealsdk)

# Creating a release

I am not a C# person but google told me this

```powershell
dotnet publish ./UE3StubGenCli/UE3StubGenCli.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o ./publish/win-x64
dotnet publish ./UE3StubGenCli/UE3StubGenCli.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o ./publish/linux-x64
```

# Usage

## Requirements

1. An input directory containing decompressed `.u` package files
2. An output directory to write the generated stub files
3. (optional) ruff to format the generated stub files

You can generate the stub files with the following command:

```powershell
UE3StubGenCli.exe --input "path/to/packages" --output "output/path" --import-root "bl1"
```

> It is recommended that you decompress all `*.u` packages into a single directory for the game you are generating stubs for

> Note that the `--import-root` option is optional and defaults to "" and is used to prefix imports i.e., from `bl1.WillowGame` as opposed to `from WillowGame`
