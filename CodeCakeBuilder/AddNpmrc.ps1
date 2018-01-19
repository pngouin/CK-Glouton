$pathToNpmrc = $env:APPVEYOR_BUILD_FOLDER + "/src/CK.Glouton.Web/app/.npmrc"

$strToPut = "registry=https://registry.npmjs.com/
always-auth=true
//invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/registry/:_authToken=$env:REGISTRY_AUTH_TOKEN
//invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/:_authToken=$env:REGISTRY_AUTH_TOKEN

@ck:registry=https://invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/registry
@ck-ac:registry=https://invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/registry
@invenietis:registry=https://invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/registry
@signature:registry=https://invenietis.pkgs.visualstudio.com/_packaging/InternalNPM/npm/registry"

$Npmrc = New-Item -type file $pathToNpmrc -Force
add-content $Npmrc $strToPut