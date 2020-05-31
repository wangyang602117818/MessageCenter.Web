rem 防止使用管理员运行%cd%路径发生变化
@setlocal enableextensions
@cd /d "%~dp0"
rem =======================
@set b=%cd%
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u "%b%\LogCenter.Service.exe"
@pause