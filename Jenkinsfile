node {
	stage 'Checkout'
		checkout scm

	stage 'Build'
		bat 'nuget restore Facepunch.UnityBatch.sln'
		bat "\"${tool 'MSBuild'}\" Facepunch.UnityBatch.csproj /p:Configuration=Release /p:ProductVersion=1.0.0.${env.BUILD_NUMBER}"
		bat "\"${tool 'MSBuild'}\" Facepunch.UnityBatch.csproj /p:Configuration=Debug /p:ProductVersion=1.0.0.${env.BUILD_NUMBER}"

	stage 'Archive'
		archiveArtifacts artifacts: 'bin/*/Facepunch.UnityBatch.*'
}