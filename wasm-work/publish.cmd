SET TOP=.

SET DRIVER_CONF=--debugrt

SET DOM_CONF=Debug

SET WASM_SDK=%TOP%\mono
SET WASM_SDK_FRAMEWORK=%TOP%\mono\framework
SET WASM_SDK_PACKAGER=%TOP%\mono\

SET APP_SOURCES=".\Hello.cs"

SET ASSETS=--asset %TOP%\client\index.html
SET PREF_FOLDER=%TOP%\client\bin\%DOM_CONF%

%WASM_SDK_PACKAGER%\packager.exe %DRIVER_CONF% --copy=ifnewer --out=publish --prefix=%PREF_FOLDER% %ASSETS% client.dll




