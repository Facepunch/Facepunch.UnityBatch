node {
	stage 'Checkout'
		checkout scm

	stage 'Build'
		bat 'dotnet restore Facepunch.UnityBatch.csproj'
		bat "dotnet build Facepunch.UnityBatch.csproj --configuration Release /p:Version=1.0.0.${env.BUILD_NUMBER}"
		bat "dotnet build Facepunch.UnityBatch.csproj --configuration Debug /p:Version=1.0.0.${env.BUILD_NUMBER}"

	stage 'Archive'
		archiveArtifacts artifacts: 'bin/*/Facepunch.UnityBatch.*'
}