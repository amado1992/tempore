function Add-ReplicableServiceCreate([System.Text.StringBuilder] $commandBuilder, [string] $serviceName, [int] $replicas) {
    $commandBuilder.Append("docker service create --with-registry-auth --name ${serviceName} --detach --replicas ${replicas} --restart-condition on-failure ")
}

function Add-ContainerCreate()
{
    param(
        [Parameter(Mandatory=$true)][System.Text.StringBuilder] $commandBuilder,
        [Parameter(Mandatory=$true)][string] $containerName,
        [Parameter(Mandatory=$false)][int] $targetPort = -1,
        [Parameter(Mandatory=$false)][int] $publishedPort = -1
    )
    if ($targetPort -eq -1 -or $publishedPort -eq -1) 
    {
        $commandBuilder.Append("docker run --name ${containerName} --detach --restart always ")
    } 
    else 
    {
        $commandBuilder.Append("docker run --name ${containerName} -p ${targetPort}:${publishedPort} --detach --restart always ")
    }
}

function Add-HostServiceCreate([System.Text.StringBuilder] $commandBuilder, [string] $serviceName, [int] $targetPort, [int] $publishedPort) 
{
    $commandBuilder.Append("docker service create --with-registry-auth --name $serviceName --detach --restart-condition on-failure --mode global --publish mode=host,target=$targetPort,published=$publishedPort ");
}

$dockerRepositoryProxy = [System.Environment]::GetEnvironmentVariable("DOCKER_REPOSITORY_PROXY")

Write-Host "Installing Services ..."

Write-Host "=================================="
Write-Host "Loading Images ..."
Write-Host "=================================="
$dockerImagesPath = [System.IO.Path]::Combine($PSScriptRoot, "../docker")
if (Test-Path $dockerImagesPath)
{
	Get-ChildItem -Path $dockerImagesPath -Filter *.tar | ForEach-Object { docker load -i $_.FullName }
}
# PUSH into local repository?


Write-Host "=================================="
Write-Host "Installing Tempore Server ..."
Write-Host "=================================="

docker service rm tempore-server
$commandBuilder = [System.Text.StringBuilder]::new()
Add-HostServiceCreate $commandBuilder "tempore-server" 80 6000

$secrets = @("TMP_IDENTITYSERVER_PASSWORD_SECRET", "TMP_CONNECTIONSTRINGS_APPLICATIONDATABASE_PASSWORD_SECRET", "TMP_IDENTITYSERVER_APPPASSWORD_SECRET")
$environmentVariables = @("TMP_CONNECTIONSTRINGS_APPLICATIONDATABASE", "TMP_IDENTITYSERVER_AUTHORITY", "TMP_IDENTITYSERVER_USERNAME", "TMP_IDENTITYSERVER_PASSWORD", "TMP_IDENTITYSERVER_ALLOWUNTRUSTEDCERTIFICATES", "TMP_IDENTITYSERVER_APPINGRESS", "TMP_IDENTITYSERVER_APPUSERNAME", "TMP_IDENTITYSERVER_APPPASSWORD")
foreach($environmentVariable in $environmentVariables)
{
	$environmentVariableValue = [System.Environment]::GetEnvironmentVariable($environmentVariable)
	foreach($secret in $secrets)
	{
		$secretValue = [System.Environment]::GetEnvironmentVariable($secret)
		$environmentVariableValue = $environmentVariableValue.Replace("%$secret%", $secretValue)
	}

	$commandBuilder.Append("--env $environmentVariable=`"$environmentVariableValue`" ")
}

# required for service discovery.
$commandBuilder.Append("--env APP_INSTANCE=`"0`" ")

$commandBuilder.Append("$dockerRepositoryProxy/tempore-server:${VERSION_NUMBER}")
$command = $commandBuilder.ToString()

$scriptBlock = [ScriptBlock]::Create($command)
Invoke-Command $scriptBlock
if ($LASTEXITCODE -ne 0) {
	Write-Error 'Error creating docker swarm service tempore-server-host' -ErrorAction Stop
}

$skipIngress = [System.Environment]::GetEnvironmentVariable("TMP_DEPLOY_SKIP_TEMPORE_INGRESS")
if (-not $skipIngress)
{
	Write-Host "=================================="
	Write-Host "Installing Ingress ..."
	Write-Host "=================================="
	docker rm -f "tempore-ingress"
	$ingressConfigurationFileTemplate = [System.IO.Path]::Combine($PSScriptRoot, "config/Tempore.Ingress/nginx.conf")
	$ingressDataDirectoryName ="/data/tempore-ingress"
	
	New-Item -ItemType Directory $ingressDataDirectoryName -ErrorAction:SilentlyContinue
	$ingressConfigurationFile = [System.IO.Path]::Combine($ingressDataDirectoryName, "nginx.conf")
	Copy-Item -Path $ingressConfigurationFileTemplate -Destination $ingressConfigurationFile -Force
	
	$content = Get-Content -Raw $ingressConfigurationFile
	$regex = [regex] '(?im)%[^%]+%'
	$matches = $regex.Matches($content)
	
	foreach($match in $matches) {
	    $content = $content.Replace($match.Value, [System.Environment]::GetEnvironmentVariable($match.Value.Trim('%')))
	}
	
	Set-Content -Path $ingressConfigurationFile -Value $content
	
	$commandBuilder = [System.Text.StringBuilder]::new()
	Add-ContainerCreate $commandBuilder "tempore-ingress" -targetPort 443 -publishedPort 443
	$commandBuilder.Append("-p 80:80 ")
	$commandBuilder.Append("-v ${ingressConfigurationFile}:/etc/nginx/nginx.conf ")
	$commandBuilder.Append("-v /data/tempore-ingress/server.key:/etc/ssl/certs/server.key ")
	$commandBuilder.Append("-v /data/tempore-ingress/server.crt:/etc/ssl/certs/server.crt ")
	
	$commandBuilder.Append("nginx:latest")
	
	$command = $commandBuilder.ToString()
	
	$scriptBlock = [ScriptBlock]::Create($command)
	
	Invoke-Command $scriptBlock
	if ($LASTEXITCODE -ne 0) {
	    Write-Error "Error creating docker container tempore-ingress" -ErrorAction Stop
	}
}

