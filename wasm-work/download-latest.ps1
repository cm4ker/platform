

Invoke-WebRequest -OutFile mono.wasm.zip "https://jenkins.mono-project.com/job/test-mono-mainline-wasm/label=ubuntu-1804-amd64/4417/Azure/processDownloadRequest/4417/ubuntu-1804-amd64/sdks/wasm/mono-wasm-59a1eade7ce.zip"

rm -r .\mono\*

Expand-Archive -LiteralPath mono.wasm.zip -DestinationPath .\mono\

rm mono.wasm.zip
