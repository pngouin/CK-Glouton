$pathToNpmrc = $env:APPVEYOR_BUILD_FOLDER + "/src/CK.Glouton.Web/app/.npmrc"

$strToPut = "registry=https://invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/registry/
always-auth=true
//invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/registry/:_authToken=$env:REGISTRY_AUTH_TOKEN
//invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/:_authToken=$env:REGISTRY_AUTH_TOKEN"

$Npmrc = New-Item -type file $pathToNpmrc -Force
add-content $Npmrc $strToPut