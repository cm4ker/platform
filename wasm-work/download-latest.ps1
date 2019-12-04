Invoke-WebRequest -OutFile mono.wasm.zip https://jenkins.mono-project.com/job/test-mono-mainline-wasm/label=ubuntu-1804-amd64/lastSuccessfulBuild/Azure/processDownloadRequest/4312/ubuntu-1804-amd64/sdks/wasm/mono-wasm-42c5ccc298c.zip

Expand-Archive -LiteralPath mono.wasm.zip -DestinationPath .\mono

rm mono.wasm.zip
