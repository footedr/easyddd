# Why not use dotnet dev-certs global tool?
# https://github.com/dotnet/aspnetcore/issues/7246
# The way the global tool generates certs does not work well for container to contiainer 
# SSL/TLS connections.

If (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{   
    $arguments = "& '" + $myinvocation.mycommand.definition + "'"
    Start-Process powershell -Verb runAs -ArgumentList $arguments
    Break
}

function Setup-Certificate {
    Param([string]$outputDirectory, [string]$hostname, [string]$password, [string]$description)

    Write-Host "Generating self-signed certificate for $hostname"
    $devCert = New-SelfSignedCertificate -DnsName $hostname, "localhost", "host.docker.internal" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1","2.5.29.19= {critical}{text}false") -KeyUsage CertSign, DigitalSignature, KeyEncipherment -FriendlyName $description -CertStoreLocation "Cert:\CurrentUser\My"

    Write-Host "Adding self-signed certificate to trusted root certificate store $hostname"
    $rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store -ArgumentList Root, LocalMachine
    $rootStore.Open("MaxAllowed")
    $rootStore.Add($devCert)
    $rootStore.Close()

    $pfxFile = [IO.Path]::Combine($outputDirectory , "$hostname.pfx")
    Write-Host "Exporting $pfxFile"
    $devCertPassword = ConvertTo-SecureString -String $password -Force -AsPlainText
    Export-PfxCertificate -Cert Cert:\CurrentUser\My\$($devCert.Thumbprint) -FilePath $pfxFile -Password $devCertPassword

    $crtFile = [IO.Path]::Combine($outputDirectory , "$hostname.crt")
    Write-Host "Exporting $crtFile"
    & 'C:\Program Files\Git\usr\bin\openssl.exe' pkcs12 -in $pfxFile -nokeys -out $crtFile -nodes -passin pass:$password
}

$certDirectory = Join-Path -Path $PSScriptRoot -ChildPath certs
Write-Host "Creating directory $certDirectory"
New-Item -ItemType Directory -Force -Path $certDirectory

if (!(Test-Path (Join-Path -Path $certDirectory -ChildPath localhost.crt))) {
    Setup-Certificate -outputDirectory $certDirectory -hostname localhost -password neversayneigh -description "Cert used for connections to localhost from host machine"
}

if (!(Test-Path (Join-Path -Path $certDirectory -ChildPath eventgrid.crt))) {
    Setup-Certificate -outputDirectory $certDirectory -hostname eventgrid -password neversayneigh -description "Cert used for connections to eventgrid from docker containers"
}

if (!(Test-Path (Join-Path -Path $certDirectory -ChildPath identityserver.crt))) {
    Setup-Certificate -outputDirectory $certDirectory -hostname identityserver -password neversayneigh -description "Cert used for connections to identity from docker containers"
}

Write-Host -NoNewLine 'All done, press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');