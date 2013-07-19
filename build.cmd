"\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" Manhole.sln /p:Configuration=Release
@cmd /c .nuget\nuget pack Manhole\Manhole.nuspec
@cmd /c .nuget\nuget pack Manhole.Web\Manhole.Web.nuspec	