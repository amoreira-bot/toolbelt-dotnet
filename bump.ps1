#!/c/windows/system32/windowspowershell/v1.0/powershell.exe
param (
  [string]$increment = 'pre',
  [string]$name,
  [string]$file = 'src\ProductAssemblyInfo.cs'
)

###
# Setup
###
if ($name) {
  $defaultPrerelease = $name
} else {
  $defaultPrerelease = 'beta'
}

$assemblyInfo = get-content $file
$pattern = [regex]"AssemblyInformationalVersion\(""(.+?)""\)]"
$version = $pattern.Match($assemblyInfo).Groups[1].Value
$version = ([regex]'\+.+$').Replace($version, '')

$partGroups = ([regex]'^(\d+\.\d+\.\d+)(-([^+]+))?$').Match($version).Groups
$main = $partGroups[1].Value
$prerelease = $partGroups[3].Value

###
# Functions
###
function status($text) {
  write-host $text -foregroundcolor cyan
}

function incPrerelease($pre) {
  if(!$pre -or $name) {
    $newPre = $defaultPrerelease
  } else {
    $groups = ([regex]'^(.*?)(\d*)$').Match($pre).Groups
    $text = $groups[1].Value
    $number = $groups[2].Value
    if ($number) {
      $number = 1 + $number
    } else {
      $number = 1
    }
    $newPre = "$text$number"
  }

  return $newPre
}

function incMain($mainVersion, $inc) {
  $groups = ([regex]'^(\d+)\.(\d+)\.(\d+)$').Match($mainVersion).Groups
  $major = $groups[1].Value
  $minor = $groups[2].Value
  $patch = $groups[3].Value
  switch ($inc) {
    'major' { return "$(1+$minor).0.0" }
    'minor' { return "$major.$(1+$minor).0" }
    'patch' { return "$major.$minor.$(1+$patch)" }
    default { return 'invalid!' }
  }
}

function prompt($title) {
  $message = "Do you want to commit and tag updated version?"

  $yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", `
      "Updates assembly info, commits changes and creates git tag."

  $no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", `
      "Does nothing."

  $options = [System.Management.Automation.Host.ChoiceDescription[]]($yes, $no)

  $result = $host.ui.PromptForChoice($title, $message, $options, 1) 
  return $result -eq 0
}

function apply($onlyMain, $fullVersion) {
  status "writing new version $fullVersion to assembly info file"
  $assemblyInfo = get-content $file -raw

  $assemblyInfo = ([regex]'AssemblyVersion\("(.+?)"\)]').Replace(
    $assemblyInfo, "AssemblyVersion(""$onlyMain"")]")
  $assemblyInfo = ([regex]'AssemblyFileVersion\("(.+?)"\)]').Replace(
    $assemblyInfo, "AssemblyFileVersion(""$onlyMain"")]")
  $assemblyInfo = ([regex]'AssemblyInformationalVersion\("(.+?)"\)]').Replace(
    $assemblyInfo, "AssemblyInformationalVersion(""$fullVersion"")]")

  $assemblyInfo > $file
}

function commit($filePath, $newVersion) {
  status "committing assembly info file"
  git add $filePath
  git commit -m "Release $newVersion"
}

function tag($newVersion) {
  status "creating git tag v$newVersion"
  git tag "v$newVersion" -m "Release $newVersion"
}

###
# Begin script
###
$newMain = $main
$newPrerelease = $prerelease
if ($increment -eq 'pre') {
  $newPrerelease = incPrerelease $prerelease
} elseif ($increment -eq 'promote') {
  $newPrerelease = ''
} elseif (@('patch', 'minor', 'major') -contains $increment) {
  $newPrerelease = $defaultPrerelease
  $newMain = incMain $main $increment
}

if ($newPrerelease) {
  $newVersion = "$newMain-$newPrerelease"
} else {
  $newVersion = $newMain
}

if (prompt "$version   ->   $newVersion") {
  apply $newMain $newVersion
  commit $file $newVersion
  tag $newVersion
}
