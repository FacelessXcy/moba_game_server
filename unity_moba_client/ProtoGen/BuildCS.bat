@echo off  
rem �����ļ�  
for /f "delims=" %%i in ('dir /b ".\*.proto"') do echo %%i  
rem תcpp  for /f "delims=" %%i in ('dir /b/a "*.proto"') do protoc -I=. --cpp_out=. %%i  
for /f "delims=" %%i in ('dir /b/a "*.proto"') do protogen -i:%%i -o:%%~ni.cs  -ns:gprotocol
pause  

