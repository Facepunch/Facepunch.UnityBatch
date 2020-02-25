node ( 'vs2017' )
{
	stage 'Checkout'
		checkout scm

	stage 'Restore'
		bat 'dotnet restore Facepunch.UnityBatch.csproj'

	stage 'Build'
		bat "dotnet build Facepunch.UnityBatch.csproj --configuration Release /p:Version=1.0.0.${env.BUILD_NUMBER}"
		bat "dotnet build Facepunch.UnityBatch.csproj --configuration Debug /p:Version=1.0.0.${env.BUILD_NUMBER}"

	stage 'Archive'
		archiveArtifacts artifacts: 'bin/*/Facepunch.UnityBatch.*'
}