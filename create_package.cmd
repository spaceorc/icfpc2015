if exist Pack rmdir /Q /S Pack
if not exist Pack mkdir Pack
copy SomeSecretProjectCore.sln Pack\SomeSecretProjectCore.sln
copy buildall.cmd Pack\buildall.cmd
copy "Readme for judjes.txt" Pack\README
mkdir Pack\packages\Newtonsoft.Json.7.0.1\lib\net45\
copy packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll Pack\packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll
copy packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.xml Pack\packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.xml
cp -R SomeSecretProject Pack\SomeSecretProject
rmdir /Q /S Pack\SomeSecretProject\bin
rmdir /Q /S Pack\SomeSecretProject\obj
del /F Pack\SomeSecretProject\*.suo
del /F Pack\SomeSecretProject\*.user
