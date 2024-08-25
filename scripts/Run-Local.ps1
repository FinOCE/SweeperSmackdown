<#
.SYNOPSIS
  Starts all services to run the entire service locally.
.DESCRIPTION
  This script starts all services to run the entire service locally. All startup scripts are called to open new
  terminal tabs. Before this script can be used, the local environment must be appropriately configured with secrets
  files and necessary tools installed.
#>

Function Open-TerminalTab {
  Param(
    # The path to where the terminal tab should be opened.
    [Parameter(Mandatory=$false)]
    [string] $Path = ".",

    # The command to run in the terminal tab.
    [Parameter(Mandatory=$true)]
    [string] $Command
  )

  $ResolvedPath = Join-Path -Path $PSScriptRoot -ChildPath $Path -Resolve
  wt -w 0 -d $ResolvedPath -p "Windows PowerShell" cmd /k $Command
}

Function Start-Azurite {
  Param(
    # The path to run Azurite and therefore store data.
    [Parameter(Mandatory=$true)]
    [string] $Path
  )

  Open-TerminalTab -Path $Path -Command "azurite -s -l"
}

Function Start-Tunnel {
  Param(
    # The path to run the tunnel from..
    [Parameter(Mandatory=$true)]
    [string] $Path,

    # The ID of the tunnel to connect to.
    [Parameter(Mandatory=$true)]
    [string] $TunnelId
  )

  Open-TerminalTab -Path $Path -Command "cloudflared tunnel run $TunnelId"
}

Function Start-NodejsApp {
  Param(
    # The path to the Node.js app to start.
    [Parameter(Mandatory=$true)]
    [string] $Path
  )

  Open-TerminalTab -Path $Path -Command "npm start"
}

Function Start-FunctionApp {
  Param(
    # The path to the function app to start.
    [Parameter(Mandatory=$true)]
    [string] $Path
  )

  Open-TerminalTab -Path $Path -Command "func start"
}

Start-Azurite -Path "..\.azurite"
Start-Tunnel -Path "..\.cloudflared" -TunnelId "67534728-9f06-4528-ae86-7f93a2286929"
Start-FunctionApp -Path "..\api\SweeperSmackdown"
Start-FunctionApp -Path "..\bot\Bot"
Start-NodejsApp -Path "..\app"
