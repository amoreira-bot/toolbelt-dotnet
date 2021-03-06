param (
  [string]$awsAccessKey = $env:AWS_ACCESS_KEY,
  [string]$awsSecretKey = $env:AWS_SECRET_KEY,
  [string]$chocApiKey = $env:CHOC_API_KEY
)


# Functions
function EnsureEmptyFolder($path) {
  if(test-path $path){
    rm $path\*
  } else {
    md $path
  }
}

function Status {
  Write-Host $args -ForegroundColor green -BackgroundColor black
}


# Identify current version tag
try {
  $currentTag = git describe --tags --exact-match
} catch {}
if (!($currentTag -match "^v\d+\.\d+\.\d+")) {
  echo "Nothing to do. Current tag ($currentTag) is not a version tag."
  exit
}
Status "Current version tag is $currentTag. Starting deployment."


# Update AppVeyor build version
$assemblyInfo = get-content src\ProductAssemblyInfo.cs
$pattern = [regex]"AssemblyInformationalVersion\(""(.+?)""\)]"
$version = $pattern.Match($assemblyInfo).Groups[1]

if ($env:APPVEYOR) {
  Status "Updating AppVeyor build version"
  $buildNumber = $env:APPVEYOR_BUILD_NUMBER
  Update-AppveyorBuild -Version "$version+$buildNumber"
}


# Create zip file
$buildPath = '.\build'
$artifactsPath = '.\artifacts'
$releasePath = '.\src\Toolbelt\bin\Release'

EnsureEmptyFolder $buildPath
EnsureEmptyFolder $artifactsPath
copy-item $releasePath\* -Destination $buildPath\ -exclude *.xml,*.pdb
$fileName = "vtex-toolbelt-$version.zip"
$fullFilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) $artifactsPath\$fileName))

$src = [System.IO.Path]::GetFullPath((Join-Path (pwd) $buildPath\))
[void][Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem')
[System.IO.Compression.ZipFile]::CreateFromDirectory($src, $fullFilePath)


# Upload zip file to S3
Status "Uploading $artifactsPath\$fileName to S3 bucket."

try {
  Initialize-AWSDefaults -AccessKey $awsAccessKey -SecretKey $awsSecretKey -Region "us-east-1"

  Write-S3Object -BucketName 'vtex-io' -Key "toolbelt/$fileName" -File $fullFilePath

  Clear-AWSCredentials
} catch {
  throw
  Write-Error $StackTrace
  exit 1
}


# Create and publish Chocolatey package
Status "Publishing Chocolatey package."
nuget pack .\chocolatey\vtex.toolbelt.nuspec -Version $version -OutputDirectory $artifactsPath
nuget push $artifactsPath\*.nupkg -Source http://chocolatey.org/ -ApiKey $chocApiKey
