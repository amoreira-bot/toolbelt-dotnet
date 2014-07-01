$currentPath = $MyInvocation.MyCommand.Definition
$currentVersion = ([regex]"vtex\.toolbelt\.(.+)\\tools").Match($currentPath).Groups[1].Value
Install-ChocolateyZipPackage `
  'vtex' `
  "http://vtex-io.s3.amazonaws.com/toolbelt/vtex-toolbelt-$currentVersion.zip" `
  "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"