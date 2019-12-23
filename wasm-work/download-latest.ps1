

Invoke-WebRequest -OutFile mono.wasm.zip "https://jenkins.mono-project.com/job/test-mono-mainline-wasm/label=ubuntu-1804-amd64/4399/Azure/processDownloadRequest/4399/ubuntu-1804-amd64/sdks/wasm/mono-wasm-1587c49f5f5.zip"

rm -r .\mono\*

Expand-Archive -LiteralPath mono.wasm.zip -DestinationPath .\mono\

rm mono.wasm.zip
