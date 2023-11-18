# LlamaSharpApiServer Build and Publish Script
# Copyright (C) Gianni Rosa Gallina.
# Licensed under the Apache License, Version 2.0.

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 1
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1

# Build for Windows
dotnet publish .\src\LlamaSharpApiServer.sln -c Release -f net8.0 -r win-x64 --self-contained

echo "Package available at: .\src\LlamaSharpApiServer\bin\Release\net8.0\win-x64\publish"