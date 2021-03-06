# Functions
function EnsureEmptyFolder($path) {
  if(test-path $path){
    rm $path\*
  } else {
    md $path
  }
}

$buildPath = '.\build'
$artifactsPath = '.\artifacts'
$releasePath = '.\src\Toolbelt\bin\Release'

$here = Split-Path -Parent $MyInvocation.MyCommand.Definition
$functions = Join-Path (Split-Path -Parent $here) 'appveyor/functions'
. "$functions\Get-VersionFromGitTag.ps1"

if ($env:APPVEYOR) {
    Push-Location $env:APPVEYOR_BUILD_FOLDER

    $version = Get-VersionFromGitTag
    if ($version) {

      EnsureEmptyFolder $buildPath
      EnsureEmptyFolder $artifactsPath

      copy-item $releasePath\* -Destination $buildPath\ -exclude *.xml,*.pdb

      $fileName = "vtex-toolbelt-$version.zip"
      $fullFilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) $artifactsPath\$fileName))

      $src = [System.IO.Path]::GetFullPath((Join-Path (pwd) $buildPath\))
      [void][Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem')
      [System.IO.Compression.ZipFile]::CreateFromDirectory($src, $fullFilePath)

      Push-AppveyorArtifact  $fullFilePath -DeploymentName to-publish
    }

    Pop-Location
}
