Invoke-WebRequest -OutFile mono.wasm.zip "https://jenkins.mono-project.com/job/test-mono-mainline-wasm/4319/label=ubuntu-1804-amd64/Azure/processDownloadRequest/4319/ubuntu-1804-amd64/sdks/wasm/mono-wasm-a3fbb644938.zip"

Expand-Archive -LiteralPath mono.wasm.zip -DestinationPath .\mono

rm mono.wasm.zip
