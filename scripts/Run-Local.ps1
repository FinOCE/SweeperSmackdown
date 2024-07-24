<#
.SYNOPSIS
  Starts all services to run the entire service locally.
.DESCRIPTION
  This script starts all services to run the entire service locally. All startup
  scripts are called to open new terminal tabs. Before this script can be used,
  the local environment must be appropriately configured with secrets files and
  necessary tools installed.
#>

Function Open-TerminalTab {
  Param(
    # The path to where the terminal tab should be opened.
    [Parameter(Mandatory=$false)]
    [string] $Path = '.',

    # The command to run in the terminal tab.
    [Parameter(Mandatory=$true)]
    [string] $Command
  )

  wt -w 0 -d $Path -p "Windows PowerShell" cmd /k $Command
}

Function Start-Tunnel {
  Param(
    # The path to the tunnel executable and configuration.
    [Parameter(Mandatory=$true)]
    [string] $Path
  )

  Open-TerminalTab -Path $Path -Command "cloudflared tunnel run"
  Write-Host "Started tunnel at $Path"
}

Function Start-Azurite {
  Open-TerminalTab -Command "azurite -s -l"
  Write-Host "Started azurite"
}

Function Start-NodejsApp {
  Param(
    # The path to the Node.js app to start.
    [Parameter(Mandatory=$true)]
    [string] $Path
  )

  Open-TerminalTab -Path $Path -Command "npm start"
  Write-Host "Started node.js app at $Path"
}

Function Start-FunctionApp {
  Param(
    # The path to the function app to start.
    [Parameter(Mandatory=$true)]
    [string] $Path
  )

  Open-TerminalTab -Path $Path -Command "func start"
  Write-Host "Started function app at $Path"
}

Start-Tunnel -Path '..\.cloudflared'
Start-Azurite
Start-FunctionApp -Path '..\api\SweeperSmackdown'
Start-FunctionApp -Path '..\bot\Bot'
Start-NodejsApp -Path '..\app'
